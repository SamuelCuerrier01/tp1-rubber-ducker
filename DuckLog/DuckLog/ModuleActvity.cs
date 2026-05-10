using System.Net.Http.Json;
using Android.Content;
using Android.Views;
using DuckLog.Models;
using System.Text.Json;

namespace DuckLog
{
    [Activity(MainLauncher = true)]
    public class ModuleActvity : Activity
    {
        // Create http client for network requests
        private static readonly HttpClient HttpClient = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.Zero, 
            ConnectTimeout = TimeSpan.FromSeconds(10) 
        })
        {
            BaseAddress = new Uri(Constants.BaseApiUrl) 
        };

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            // Set up client
            if (HttpClient.BaseAddress == null)
            {
                HttpClient.BaseAddress = new Uri(Constants.BaseApiUrl);
            }
            
            HttpClient.DefaultRequestHeaders.ConnectionClose = true;
            
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
            
            System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    var response = await HttpClient.GetAsync("/api/modules");
                    string rawJson = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var fetchedModules = JsonSerializer.Deserialize<List<Module>>(rawJson, options);

                        if (fetchedModules != null)
                        {
                            RunOnUiThread(() =>
                            {
                                foreach (var module in fetchedModules)
                                {
                                    LoadModuleCardUi(moduleListContainer, module); 
                                }
                            });
                        }
                    }
                    else
                    {
                        RunOnUiThread(() => Toast.MakeText(this, $"API Error: {response.StatusCode}", ToastLength.Long).Show());
                    }
                }
                catch (JsonException jsonEx)
                {
                    Android.Util.Log.Error("DUCKLOG", $"JSON Parse Error: {jsonEx.Message}");
                    RunOnUiThread(() => Toast.MakeText(this, "Data Format Error. Check Logcat.", ToastLength.Long).Show());
                }
                catch (Exception ex)
                {
                    Android.Util.Log.Error("DUCKLOG", $"Network Error: {ex.Message}");
                    RunOnUiThread(() => Toast.MakeText(this, "Network Connection Failed.", ToastLength.Long).Show());
                }
            });
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