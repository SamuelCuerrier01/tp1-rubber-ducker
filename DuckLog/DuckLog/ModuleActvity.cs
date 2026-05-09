using System.Text.Json;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DuckLog;
using DuckLog.Models;
using DuckLog.Notifications;
namespace DuckLog
{
    [Activity]
    //[Activity(Label = "Module Activity")]
    public class ModuleActvity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            ActionBar?.Hide(); // hide the default action bar
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_module);
            
            NotificationHelper.CreateNotificationChannel(this);
            AlarmScheduler.Schedule(this);
            NotificationHelper.CheckAndNotifyAll(this);

            //return button
            Android.Widget.ImageButton btnReturn = FindViewById<Android.Widget.ImageButton>(Resource.Id.btnReturn);
            
            //list of created modules
            LinearLayout moduleListContainer = FindViewById<LinearLayout>(Resource.Id.moduleListContainer);
            //add module button
            RelativeLayout btnAddModuleArea = FindViewById<RelativeLayout>(Resource.Id.btnAddModuleArea);

            if (btnReturn != null)
            {
                btnReturn.Click += (sender, e) =>
                {
                    Finish(); //if module window is always after MainActivity, it will just go back to MainActivity
                };
            }

            if (btnAddModuleArea != null)
            {
                btnAddModuleArea.Click += (sender, e) =>
                {
                    Intent intent = new Intent(this, typeof(AddModuleActivity));
                    StartActivity(intent);
                    //TODO: go to a new page of creation of module
                    
                };
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            LinearLayout moduleListContainer = FindViewById<LinearLayout>(Resource.Id.moduleListContainer);

            while (moduleListContainer.ChildCount > 1)
            {
                moduleListContainer.RemoveViewAt(0);
            }
            
            //TODO: get modules form database here
            //List<ModuleDemo> userModules = Database.GetModules();
            
            foreach (var module in FakeModuleDatabase.MyModules)
            {
                // Notice we are just passing 'module' now!
                LoadModuleCardUI(moduleListContainer, module); 
            }
        }

        private void LoadModuleCardUI(LinearLayout container, ModuleDemo module)
        {
            LinearLayout taskListView = FindViewById<LinearLayout>(Resource.Id.moduleListContainer);
            //Inflater for XML
            Android.Views.View moduleCard = LayoutInflater.Inflate(Resource.Layout.item_module, container, false);
            
            //Text for module
            TextView txtName = moduleCard.FindViewById<TextView>(Resource.Id.txtModuleName);
            TextView txtTotalTasks = moduleCard.FindViewById<TextView>(Resource.Id.txtTotalTasks);
            Android.Widget.ProgressBar progressBar = moduleCard.FindViewById<Android.Widget.ProgressBar>(Resource.Id.progress_bar_horizontal);
            TextView progressText = moduleCard.FindViewById<TextView>(Resource.Id.progressText);
            
            if (txtName != null) txtName.Text = module.Name;
            if (txtTotalTasks != null) txtTotalTasks.Text = $"Tasks: {module.TotalTasks}";

            if (progressBar != null && progressText != null)
            {
                int percentage = 0;
                if (module.TotalTasks > 0)
                {
                    percentage = (module.CompletedTasks * 100) / module.TotalTasks;
                }

                progressBar.Progress = percentage;
                progressText.Text = $"{percentage}%";
            }

            //Insertion before add module button
            int insertIndex = container.ChildCount - 1;
            container.AddView(moduleCard, insertIndex);
            
            moduleCard.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(TaskActivity));
                intent.PutExtra("moduleId", module.Id);
                StartActivity(intent);
            };
        }
    }
}