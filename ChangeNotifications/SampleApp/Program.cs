using System;
using System.Collections.ObjectModel;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define a project inheriting Changeable type and aggregating related Changeable objects such as schedule, tasks, and optionally common resources.
            var resource1 = new Resource { FirstName = "Diane", LastName = "Smith" };
            var resource2 = new Resource { FirstName = "John", LastName = "Doe" };
            var project = new Project
            {
                Title = "Test project",
                Schedule = new Schedule
                {
                    WeekStart = DayOfWeek.Monday,
                    WeekFinish = DayOfWeek.Friday,
                    DayStart = TimeSpan.Parse("08:00:00"),
                    DayDuration = TimeSpan.Parse("08:00:00")
                },
                Tasks = new ObservableCollection<Task>
                {
                    new Task
                    {
                        Title = "Task 1",
                        Start = DateTime.Today.Add(TimeSpan.Parse("08:00:00")),
                        Duration = TimeSpan.Parse("04:00:00"),
                        Assignments = new ObservableCollection<Resource> { resource1 }
                    },
                    new Task
                    {
                        Title = "Task 2",
                        Start = DateTime.Today.AddDays(1).Add(TimeSpan.Parse("08:00:00")),
                        Duration = TimeSpan.Parse("12:00:00"),
                        Assignments = new ObservableCollection<Resource> { resource2, new Resource { FirstName = "Mary", LastName = "Bing" }, resource1 }
                    }
                },
                DescriptionLines = new ObservableCollection<string> { "Description line 1", "Description line 2" }
            };
            Console.WriteLine("Project defined.");
           

            // Subscribe for property changed event.
            project.PropertyChanged += (sender, e) => { Console.WriteLine($"{e.PropertyName} changed."); };
            Console.WriteLine("Subscribed for project property changes.");

            // Change simple property of project. Simple event occurs with changed property name (Title).
            project.Title = "Changed project";

            // Change property of a related object of the project. Event occurs with property path (Schedule.DayDuration).
            project.Schedule.DayDuration = TimeSpan.Parse("10:00:00");

            // Change properties of items of a related collection of the project. Events occur with indexed property path (Tasks.Item[0].Title and Tasks.Item[1].Duration).
            project.Tasks[0].Title = "Changed task";
            project.Tasks[1].Duration += TimeSpan.Parse("01:00:00");

            // Change a property of multiply related item for the project. Multiple events occur with different property paths (Tasks.Item[0].Assignments.Item[0].LastName and Tasks.Item[1].Assignments.Item[2].LastName).
            resource1.LastName = "Jones";

            // Add an item into a related collection of the project. Events occur with property path (Tasks.Count and Tasks.Item[]).
            project.Tasks.Add(new Task
            {
                Title = "New task",
                Start = DateTime.Today,
                Duration = TimeSpan.FromHours(24),
                Assignments = new ObservableCollection<Resource> { resource1 }
            });

            // Change property of a newly added item of a related collection of the project. Event occurs with property path (Tasks.Item[2].Title).
            project.Tasks[2].Title = "Changed new task";

            // Remove an item from a related collection of the project. Events occur with property path (Tasks.Count and Tasks.Item[]).
            var taskToRemove = project.Tasks[0];
            project.Tasks.Remove(taskToRemove);

            // Change property of a removed item from a related collection of the project. Event does not occurs.
            taskToRemove.Title = "Changed removed task";

            // Add items to a collection of non-changeable objects. Events occur with property path (DescriptionLines.Count and DescriptionLines.Item[]).
            project.DescriptionLines.Add("Added description line");
        }
    }
}
