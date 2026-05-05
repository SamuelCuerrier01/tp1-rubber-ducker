using System.Net.Http.Json;
using Android.Content;
using Android.Views;
using DuckLog.Models;

namespace DuckLog
{
    [Activity(MainLauncher = true)]
    public class ModuleActvity : Activity
    {
        // Create http client for network requests
        private static readonly HttpClient HttpClient = new HttpClient();

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            // Set up client
            if (HttpClient.BaseAddress == null)
            {
                HttpClient.BaseAddress = new Uri(Constants.BaseApiUrl);
            }

            ActionBar?.Hide();
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_module);

            ImageButton btnReturn = FindViewById<ImageButton>(Resource.Id.btnReturn);
            RelativeLayout btnAddModuleArea = FindViewById<RelativeLayout>(Resource.Id.btnAddModuleArea);

            if (btnReturn != null)
            {
                btnReturn.Click += (sender, e) => Finish();
            }

            if (btnAddModuleArea != null)
            {
                btnAddModuleArea.Click += (sender, e) =>
                {
                    Intent intent = new Intent(this, typeof(AddModuleActivity));
                    StartActivity(intent);
                };
            }
        }

        // Async so it can wait for Docker 
        protected override async void OnResume()
        {
            base.OnResume();

            LinearLayout moduleListContainer = FindViewById<LinearLayout>(Resource.Id.moduleListContainer);
            
            while (moduleListContainer.ChildCount > 1)
            {
                moduleListContainer.RemoveViewAt(0);
            }
            
            try
            {
                //Fetch the data from Docker
                var fetchedModules = await HttpClient.GetFromJsonAsync<List<Module>>("/api/modules");

                if (fetchedModules != null)
                {
                    foreach (var module in fetchedModules)
                    {
                        LoadModuleCardUi(moduleListContainer, module); 
                    }
                }
            }
            catch (Exception ex)
            {
                // If Docker is off or unreachable
                Toast.MakeText(this, $"API Error: {ex.Message}", ToastLength.Long).Show();
            }
        }

        private void LoadModuleCardUi(LinearLayout container, Module module)
        {
            View moduleCard = LayoutInflater.Inflate(Resource.Layout.item_module, container, false);
            
            TextView txtName = moduleCard.FindViewById<TextView>(Resource.Id.txtModuleName);
            TextView txtTotalTasks = moduleCard.FindViewById<TextView>(Resource.Id.txtTotalTasks);
            ProgressBar progressBar = moduleCard.FindViewById<ProgressBar>(Resource.Id.progress_bar_horizontal);
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