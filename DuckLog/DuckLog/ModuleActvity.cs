using System.Text.Json;
using System.Net.Http.Json; // REQUIRED for GetFromJsonAsync
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DuckLog;
using DuckLog.Models;

namespace DuckLog
{
    [Activity(MainLauncher = true)]
    public class ModuleActvity : Activity
    {
        // 1. Create the HttpClient for network requests
        private static readonly HttpClient _httpClient = new HttpClient();

        protected override void OnCreate(Android.OS.Bundle? savedInstanceState)
        {
            // 2. Point the client to your Docker network alias
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(Constants.BaseApiUrl);
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

        // 3. Make this async so it can wait for the Docker response
        protected override async void OnResume()
        {
            base.OnResume();

            LinearLayout moduleListContainer = FindViewById<LinearLayout>(Resource.Id.moduleListContainer);

            // Clear the existing UI cards (except the "Add" button if it's the first element)
            while (moduleListContainer.ChildCount > 1)
            {
                moduleListContainer.RemoveViewAt(0);
            }
            
            try
            {
                // 4. Fetch the real data from Docker!
                var fetchedModules = await _httpClient.GetFromJsonAsync<List<Module>>("/api/modules");

                if (fetchedModules != null)
                {
                    // 5. Loop through the backend data instead of the FakeModuleDatabase
                    foreach (var module in fetchedModules)
                    {
                        LoadModuleCardUI(moduleListContainer, module); 
                    }
                }
            }
            catch (Exception ex)
            {
                // If Docker is off or unreachable, show the error on screen
                Toast.MakeText(this, $"API Error: {ex.Message}", ToastLength.Long).Show();
            }
        }

        private void LoadModuleCardUI(LinearLayout container, Module module)
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