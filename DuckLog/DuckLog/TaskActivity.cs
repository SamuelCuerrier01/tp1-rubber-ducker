using System.Text.Json;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DuckLog;
using DuckLog.Api;
using DuckLog.Models;
using TaskStatus = DuckLog.Models.TaskStatus;

namespace DuckLog
{
    [Activity(MainLauncher = true)]
    //[Activity(Label = "Task Activity")]
    public class TaskActivity : Activity
    {
        public ModuleDemo module;
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            int moduleId = Intent.GetIntExtra("moduleId", 0);
            module = FakeModuleDatabase.GetModuleById( moduleId );
            if (module == null)
            {
                Toast.MakeText(this, "Error: module not found", ToastLength.Short).Show();
                Finish(); // go back
                return;
            }
            
            ActionBar?.Hide(); // hide the default action bar
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_task);

            //return button
            ImageButton btnReturn = FindViewById<ImageButton>(Resource.Id.btnReturn);
            ImageButton btnGenerate = FindViewById<ImageButton>(Resource.Id.btnGenerate);
            
            //list of created tasks
            LinearLayout taskListContainer = FindViewById<LinearLayout>(Resource.Id.taskListContainer);
            //add task button
            RelativeLayout btnAddTaskArea = FindViewById<RelativeLayout>(Resource.Id.btnAddTaskArea);

            btnAddTaskArea.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(AddTaskActivity));
                intent.PutExtra("moduleId", module.Id);
                StartActivity(intent);
            };
            btnReturn.Click += (sender, e) =>
            {
                Finish();
            };
            btnGenerate.Click += async (sender, e) =>
            {
                Toast.MakeText(this, "Generating task", ToastLength.Short).Show();
                btnGenerate.SetImageResource(Resource.Drawable.loading);

                await System.Threading.Tasks.Task.Run(() => ApiRequest.Run(module));
                btnGenerate.SetImageResource(Resource.Drawable.generate_btn);

                RunOnUiThread(() =>
                {
                    ReloadTasks();
                });
            };

            if (btnAddTaskArea != null)
            {
                btnAddTaskArea.Click += (sender, e) =>
                {
                    Android.Util.Log.Debug("MyApp", "créer tache");
                    
                };
            }
            
            TextView taskHeaderText = FindViewById<TextView>(Resource.Id.taskHeaderText);
            taskHeaderText.Text = $"{module.Name} Tasks";
        }

        protected override void OnResume()
        {
            base.OnResume();

            LinearLayout taskListContainer = FindViewById<LinearLayout>(Resource.Id.taskListContainer);

            ReloadTasks();
        }

        private void LoadTaskCardUI(LinearLayout container, ITask task)
        {
            LinearLayout taskListView = FindViewById<LinearLayout>(Resource.Id.taskListContainer);
            //Inflater for XML
            View taskCard = LayoutInflater.Inflate(Resource.Layout.item_task, container, false);

            switch (task.Status)
            {
                case TaskStatus.Done :
                    taskCard.SetBackgroundResource(Resource.Drawable.bg_task_done);
                    break;
                case TaskStatus.InProgress :
                    taskCard.SetBackgroundResource(Resource.Drawable.bg_task_in_progress);
                    break;
                default :
                    taskCard.SetBackgroundResource(Resource.Drawable.bg_task_todo);
                    break;
            }
            
            //Text for module
            TextView txtName = taskCard.FindViewById<TextView>(Resource.Id.txtTaskName);
            TextView txtStatus = taskCard.FindViewById<TextView>(Resource.Id.txtTaskStatus);
            TextView txtStart = taskCard.FindViewById<TextView>(Resource.Id.txtTaskStart);
            TextView txtEnd = taskCard.FindViewById<TextView>(Resource.Id.txtTaskEnd);

            txtName.Text = task.Name;
            txtStatus.Text = task.Status.ToString();
            txtStart.Text = "Start : " + task.StartDate.ToString("dd/MM/yyyy");
            txtEnd.Text = "End : " + task.EndDate.ToString("dd/MM/yyyy");

            //Insertion before add task button
            int insertIndex = container.ChildCount - 1;
            container.AddView(taskCard, insertIndex);
            
            taskCard.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(EditTaskActivity));
                intent.PutExtra("taskId", task.Id);
                StartActivity(intent);
            };
        }
        
        private void ReloadTasks()
        {
            LinearLayout taskListContainer = FindViewById<LinearLayout>(Resource.Id.taskListContainer);

            while (taskListContainer.ChildCount > 1)
            {
                taskListContainer.RemoveViewAt(0);
            }

            foreach (var task in module.GetTasks())
            {
                LoadTaskCardUI(taskListContainer, task);
            }
        }
    }
    
    
}