# ChangeNotifications
Base class providing automatic change notification chaining for INotifyPropertyChanged and INotifyCollectionChanged enabled objects.

## NuGet package
* [SDolha.ChangeNotifications](https://www.nuget.org/packages/SDolha.ChangeNotifications)

## Usage
1. Define data object classes inheriting from Changeable. Define properties with Change setters. You can aggregate other Changeable objects, including within ObservableCollection instances:

		public class Project : Changeable, INotifyPropertyChanged
		{
			public string Title { get => title; set => Change(() => { title = value; }); }
			public Schedule Schedule { get => schedule; set => Change(() => { schedule = value; }); }
			public ObservableCollection<Task> Tasks { get => tasks; set => Change(() => { tasks = value; }); }
		}

2. Use the data objects and receive centralized PropertyChanged events in your application, receiving full property paths as e.PropertyName:

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
            }
        };
        project.PropertyChanged += (sender, e) => { Console.WriteLine($"{e.PropertyName} changed."); };
        project.Title = "Changed project";
        project.Schedule.DayDuration = TimeSpan.Parse("10:00:00");
        project.Tasks[0].Title = "Changed task";
        project.Tasks[1].Duration += TimeSpan.Parse("01:00:00");
        resource1.LastName = "Jones";
        project.Tasks.Add(new Task
        {
            Title = "New task",
            Start = DateTime.Today,
            Duration = TimeSpan.FromHours(24),
            Assignments = new ObservableCollection<Resource> { resource1 }
        });
        project.Tasks[2].Title = "Changed new task";

3. Build and run your application:

		Title changed.
		Schedule.DayDuration changed.
		Tasks.Item[0].Title changed.
		Tasks.Item[1].Duration changed.
		Tasks.Item[0].Assignments.Item[0].LastName changed.
		Tasks.Item[1].Assignments.Item[2].LastName changed.
		Tasks.Count changed.
		Tasks.Item[] changed.
		Tasks.Item[2].Title changed.
