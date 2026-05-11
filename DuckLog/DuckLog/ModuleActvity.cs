using System.Net.Http.Json;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Animation;
using Android.Util;
using DuckLog;
using DuckLog.Models;
using System.Text.Json;
using Android.OS;
using DuckLog.Notifications;

namespace DuckLog
{
    [Activity]
    //[Activity(MainLauncher = true)]
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
            
            if (User.CurrentUser == null)
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(intent);
                Finish();
                return;
            }
            
            SetContentView(Resource.Layout.activity_module);
            
            NotificationHelper.CreateNotificationChannel(this);
            AlarmScheduler.Schedule(this);
            
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                RequestPermissions(new string[] { Android.Manifest.Permission.PostNotifications }, 0);
            }
            
            NotificationHelper.CheckAndNotifyAll(this);

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
                    int id = User.CurrentUser.Id;
                    var response = await HttpClient.GetAsync($"/api/modules?userId={id}");
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
            
            // On récupère les deux éléments du FrameLayout pour pouvoir les manipuler lors du swipe
            FrameLayout frameLayout = moduleCard as FrameLayout;
            LinearLayout deleteBackground = null;
            LinearLayout moduleCardContent = null;
            
            if (frameLayout != null && frameLayout.ChildCount >= 2)
            {
                deleteBackground = frameLayout.GetChildAt(0) as LinearLayout;
                moduleCardContent = frameLayout.GetChildAt(1) as LinearLayout;
            }
            
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
            
            // Mettre en place le swipe pour la suppression
            if (moduleCardContent != null && deleteBackground != null)
            {
                // Avoir la largeur de l'écran pour limiter la translation
                DisplayMetrics displayMetrics = new DisplayMetrics();
                WindowManager?.DefaultDisplay?.GetMetrics(displayMetrics);
                int screenWidth = displayMetrics.WidthPixels;
                
                SwipeGestureListener swipeListener = new SwipeGestureListener(moduleCardContent, deleteBackground, screenWidth);
                
                swipeListener.OnSwipeToDelete += () =>
                {
                    ShowDeleteConfirmationDialog(module, moduleCard, container);
                };
                
                moduleCard.SetOnTouchListener(new SwipeTouchListener(swipeListener));
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
        
        private void ShowDeleteConfirmationDialog(Module module, View moduleCard, LinearLayout container)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Delete Module");
            builder.SetMessage($"Est-tu sûr de vouloir supprimer le module '{module.Name}'?");
            
            builder.SetPositiveButton("Oui", async (_, __) =>
            {
                Console.WriteLine("Deleting module id = " + module.Id);

                var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/modules/{module.Id}")
                {
                    Content = new StringContent("") // forces Content-Type
                };
                
                var response = await HttpClient.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"STATUS: {response.StatusCode}");
                Console.WriteLine($"BODY: {body}");

                if (!response.IsSuccessStatusCode)
                {
                    Toast.MakeText(this, $"Delete failed: {response.StatusCode}", ToastLength.Long).Show();
                    return;
                }

                moduleCard.Animate()
                    .Alpha(0f)
                    .SetDuration(300)
                    .SetListener(new RemoveViewAction(container, moduleCard))
                    .Start();

                Toast.MakeText(this, $"Module '{module.Name}' Supprimé", ToastLength.Short).Show();
            });
            
            builder.SetNegativeButton("Non", (_, __) =>
            {
                // Remettre le module à sa position initiale
                FrameLayout frameLayout = moduleCard as FrameLayout;
                LinearLayout moduleCardContent = null;
                LinearLayout deleteBackground = null;
                
                if (frameLayout != null && frameLayout.ChildCount >= 2)
                {
                    deleteBackground = frameLayout.GetChildAt(0) as LinearLayout;
                    moduleCardContent = frameLayout.GetChildAt(1) as LinearLayout;
                }
                
                if (moduleCardContent != null && deleteBackground != null)
                {
                    moduleCardContent.Animate()
                        .TranslationX(0)
                        .SetDuration(200)
                        .Start();
                    deleteBackground.Visibility = ViewStates.Gone;
                }
            });
            
            builder.Show();
        }
        
        // Classe interne, car garde encapsulation et permet d'accéder facilement au container et à la vue à supprimer.
        // Utile seulement pour la classe ModuleActivity, pas besoin de la rendre publique.
        private class RemoveViewAction : Java.Lang.Object, Animator.IAnimatorListener
        {
            private LinearLayout _container;
            private View _view;
            
            public RemoveViewAction(LinearLayout container, View view)
            {
                _container = container;
                _view = view;
            }
            
            public void OnAnimationCancel(Animator animation) { }
            public void OnAnimationEnd(Animator animation) 
            { 
                _container.RemoveView(_view);
            }
            public void OnAnimationRepeat(Animator animation) { }
            public void OnAnimationStart(Animator animation) { }
        }
    }
}