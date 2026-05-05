using DuckLog.Models;
using Java.Sql;
using TaskStatus = DuckLog.Models.TaskStatus;
using System.Net.Http.Json;

namespace DuckLog;

[Activity(Label = "Add Task Activity")]
public class AddTaskActivity : Activity
{
    private ModuleDemo _module;
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        int moduleId = Intent.GetIntExtra("moduleId", 0);
        _module = FakeModuleDatabase.GetModuleById( moduleId );
        ActionBar?.Hide();
        base.OnCreate(savedInstanceState);
        
        SetContentView(Resource.Layout.activity_add_task);
        
        EditText editName = FindViewById<EditText>(Resource.Id.editTaskName);
        EditText editDesc = FindViewById<EditText>(Resource.Id.editTaskDesc);
        EditText editTaskStart = FindViewById<EditText>(Resource.Id.createTaskStart);
        EditText editTaskEnd = FindViewById<EditText>(Resource.Id.createTaskEnd);
        Spinner spinner = FindViewById<Spinner>(Resource.Id.createTaskStatus);
        TaskStatus newStatus = TaskStatus.Todo;
        DateTime newStart =  DateTime.MinValue;
        DateTime newEnd =  DateTime.MinValue;
        Button btnCreate = FindViewById<Button>(Resource.Id.btnCreateTask);
        
        ImageButton btnReturn = FindViewById<ImageButton>(Resource.Id.btnReturn);
        
        if (btnReturn != null)
        {
            btnReturn.Click += (s, e) =>
            {
                Finish();
            };
        }
        
        editTaskStart.Click += (s, e) =>
        {
            DateTime date = DateTime.Now;

            var dialog = new DatePickerDialog(
                this,
                (sender, args) =>
                {
                    editTaskStart.Text = $"{args.Date.Day:00}/{args.Date.Month:00}/{args.Date.Year}";
                    newStart = new DateTime(args.Date.Year, args.Date.Month, args.Date.Day);
                },
                date.Year,
                date.Month - 1,
                date.Day
            );
            

            dialog.Show();
        };
        editTaskEnd.Click += (s, e) =>
        {
            DateTime date = DateTime.Now;

            var dialog = new DatePickerDialog(
                this,
                (sender, args) =>
                {
                    editTaskEnd.Text = $"{args.Date.Day:00}/{args.Date.Month:00}/{args.Date.Year}";
                    newEnd = new DateTime(args.Date.Year, args.Date.Month, args.Date.Day);
                },
                date.Year,
                date.Month - 1,
                date.Day
            );
            

            dialog.Show();
        };
        

        var items = new List<TaskStatus>
        {
            TaskStatus.Todo,
            TaskStatus.InProgress,
            TaskStatus.Done
        };

        var adapter = new ArrayAdapter<TaskStatus>(
            this,
            Android.Resource.Layout.SimpleSpinnerItem,
            items
        );

        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

        spinner.Adapter = adapter;
        
        spinner.ItemSelected += (sender, e) =>
        {
            newStatus = items[e.Position];
        };
        
        btnCreate.Click += (s, e) => 
        {
            string newName = editName?.Text ?? "";
            string newDesc = editDesc?.Text ?? "";
        
            if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newDesc) ||  newStart == DateTime.MinValue || newEnd == DateTime.MinValue)
            {
                Toast.MakeText(this, "Please fill in all the fields", ToastLength.Short).Show();
                return;
            }
        
            //TODO: integration with database
            
            ITask task = new Models.Task(newName, newDesc, newEnd, newStart, newStatus);
            _module.AddTask(task);
    
            Finish();
        };
        
        
    }
}