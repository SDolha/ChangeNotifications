using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ChangeNotifications
{
    public class Changeable : INotifyPropertyChanged
    {
        private readonly Dictionary<object, string> relatedChangeableNames = new Dictionary<object, string>();
        private readonly Dictionary<object, IEnumerable<object>> managedEnumerations = new Dictionary<object, IEnumerable<object>>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null && propertyName != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void Change(Action setter, [CallerMemberName]string propertyName = null)
        {
            Change(() => { setter?.Invoke(); return true; }, propertyName);
        }

        protected void Change(Func<bool> setter, [CallerMemberName]string propertyName = null)
        {
            RemoveChangeHandlers(propertyName);
            if (setter != null && setter.Invoke())
                OnPropertyChanged(propertyName);
            AddChangeHandlers(propertyName);
        }

        protected void AddChangeHandlers([CallerMemberName]string propertyName = null)
        {
            var propertyDefinition = GetType().GetRuntimeProperty(propertyName);
            var relatedObject = propertyDefinition?.GetValue(this);
            var relatedItem = relatedObject as INotifyPropertyChanged;
            var relatedCollection = relatedObject as INotifyCollectionChanged;
            if (relatedItem != null || relatedCollection != null)
                relatedChangeableNames.Add(relatedItem, propertyName);
            if (relatedItem != null)
                relatedItem.PropertyChanged += RelatedItem_PropertyChanged;
            if (relatedCollection != null)
            {
                var relatedEnumeration = relatedCollection as IEnumerable<object>;
                AddChildrenChangeHandlers(propertyName, relatedEnumeration);
                relatedCollection.CollectionChanged += RelatedCollection_CollectionChanged;
            }
        }

        protected void RemoveChangeHandlers([CallerMemberName]string propertyName = null)
        {
            var propertyDefinition = GetType().GetRuntimeProperty(propertyName);
            var relatedObject = propertyDefinition?.GetValue(this);
            var relatedItem = relatedObject as INotifyPropertyChanged;
            var relatedCollection = relatedObject as INotifyCollectionChanged;
            if (relatedItem != null)
                relatedItem.PropertyChanged -= RelatedItem_PropertyChanged;
            if (relatedCollection != null)
            {
                relatedCollection.CollectionChanged -= RelatedCollection_CollectionChanged;
                var relatedEnumeration = relatedCollection as IEnumerable<object>;
                RemoveChildrenChangeHandlers(relatedEnumeration);
            }
            if (relatedItem != null || relatedCollection != null)
                relatedChangeableNames.Remove(relatedItem);
        }

        private void AddChildrenChangeHandlers(string propertyName, IEnumerable<object> addedItems)
        {
            AddChildrenChangeHandlers(propertyName, addedItems, referenceEnumeration: addedItems);
        }

        private void AddChildrenChangeHandlers(string propertyName, IEnumerable<object> addedItems, IEnumerable<object> referenceEnumeration)
        {
            if (addedItems == null)
                return;
            var relatedChildren = addedItems.Where(i => i is INotifyPropertyChanged).Cast<INotifyPropertyChanged>().ToArray();
            foreach (var relatedChildItem in relatedChildren)
            {
                managedEnumerations.Add(relatedChildItem, referenceEnumeration);
                relatedChangeableNames.Add(relatedChildItem, $"{propertyName}.Item[]");
                relatedChildItem.PropertyChanged += RelatedChildItem_PropertyChanged;
            }
        }

        private void RemoveChildrenChangeHandlers(IEnumerable<object> removedItems)
        {
            if (removedItems == null)
                return;
            var relatedChildren = removedItems.Where(i => i is INotifyPropertyChanged).Cast<INotifyPropertyChanged>().ToArray();
            foreach (var relatedChildItem in relatedChildren)
            {
                relatedChildItem.PropertyChanged -= RelatedChildItem_PropertyChanged;
                relatedChangeableNames.Remove(relatedChildItem);
                managedEnumerations.Remove(relatedChildItem);
            }
        }

        private void RelatedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged($"{relatedChangeableNames[sender]}.{e.PropertyName}");
        }

        private void RelatedChildItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged($"{relatedChangeableNames[sender].Replace("[]", $"[{managedEnumerations[sender].ToList().IndexOf(sender)}]")}.{e.PropertyName}");
        }

        private void RelatedCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddChildrenChangeHandlers(relatedChangeableNames[sender], e.NewItems.Cast<object>(), sender as IEnumerable<object>);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveChildrenChangeHandlers(e.OldItems.Cast<object>());
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
