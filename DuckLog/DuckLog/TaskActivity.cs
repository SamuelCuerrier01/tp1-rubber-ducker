using Android.Content;
using Android.Views;
using DuckLog.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Android.Util;
using DuckLog.Api;
using Task = DuckLog.Models.Task;
using TaskStatus = DuckLog.Models.TaskStatus;

namespace DuckLog
{
    /// <summary>
    /// Activité affichant la liste des tâches d'un module.
    /// Permet de naviguer vers l'ajout ou l'édition de tâches.
    /// </summary>
    [Activity]
    public class TaskActivity : Activity
    {
        /// <summary>
        /// Module dont les tâches sont affichées.
        /// </summary>
        private Module module;
        private int _moduleId;
        
        private static readonly HttpClient HttpClient = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.Zero, 
            ConnectTimeout = TimeSpan.FromSeconds(10) 
        })
        {
            BaseAddress = new Uri(Constants.BaseApiUrl) 
        };
        
        /// <summary>
        /// Méthode appelée lors de la création de l'activité.
        /// Initialise les boutons, l'en-tête et la logique d'ajout de tâche.
        /// </summary>
        /// <param name="savedInstanceState">État précédent de l'activité, si disponible.</param>
        protected override async void OnCreate(Bundle? savedInstanceState)
        {
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
            
            SetContentView(Resource.Layout.activity_task);
            
            _moduleId = Intent.GetIntExtra("moduleId", 0);

            ImageButton btnReturn = FindViewById<ImageButton>(Resource.Id.btnReturn);
            ImageButton btnGenerate = FindViewById<ImageButton>(Resource.Id.btnGenerate);
            
            RelativeLayout btnAddTaskArea = FindViewById<RelativeLayout>(Resource.Id.btnAddTaskArea);

            btnAddTaskArea.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(AddTaskActivity));
                intent.PutExtra("moduleId", _moduleId);
                StartActivity(intent);
            };

            if (btnReturn != null)
            {
                btnReturn.Click += (sender, e) => Finish();
            }
            
            if (btnGenerate != null)
            {
                btnGenerate.Click += async (sender, e) =>
                {
                    Toast.MakeText(this, "Generating task...", ToastLength.Short).Show();
                    btnGenerate.SetImageResource(Resource.Drawable.loading);

                    try
                    {
                        await ApiRequest.Run(module);
                        await LoadModuleDataAsync();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, $"Generation failed: {ex.Message}", ToastLength.Long).Show();
                    }
                    finally
                    {
                        btnGenerate.SetImageResource(Resource.Drawable.generate_btn);
                    }
                };
            }
        }

        /// <summary>
        /// Méthode appelée lors de la reprise de l'activité.
        /// Rafraîchit la liste des tâches affichées.
        /// </summary>
        protected override async void OnResume()
        {
            base.OnResume();
            await LoadModuleDataAsync();
        }
        
        private async System.Threading.Tasks.Task LoadModuleDataAsync()
        {
            try 
            {
                var response = await HttpClient.GetAsync($"/api/modules/{_moduleId}");
                string rawJson = await response.Content.ReadAsStringAsync();
        
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    module = JsonSerializer.Deserialize<Module>(rawJson, options);

                    if (module != null)
                    {
                        TextView taskHeaderText = FindViewById<TextView>(Resource.Id.taskHeaderText);
                        taskHeaderText.Text = $"{module.Name} Tasks";
                        RefreshTaskList();
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                Android.Util.Log.Error("DUCKLOG", $"JSON Parse Error: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "An unexpected error occurred", ToastLength.Short).Show();
            }
        }
        
        private void RefreshTaskList()
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

        /// <summary>
        /// Crée et ajoute une carte représentant une tâche dans le conteneur donné.
        /// Permet de cliquer sur la carte pour éditer la tâche.
        /// </summary>
        /// <param name="container">Conteneur LinearLayout où ajouter la carte de tâche.</param>
        /// <param name="task">Tâche à afficher.</param>
        private void LoadTaskCardUI(LinearLayout container, ITask task)
        {
            // Inflate de la carte XML
            View taskCard = LayoutInflater.Inflate(Resource.Layout.item_task, container, false);

            TextView txtName = taskCard.FindViewById<TextView>(Resource.Id.txtTaskName);
            TextView txtStatus = taskCard.FindViewById<TextView>(Resource.Id.txtTaskStatus);
            TextView txtStart = taskCard.FindViewById<TextView>(Resource.Id.txtTaskStart);
            TextView txtEnd = taskCard.FindViewById<TextView>(Resource.Id.txtTaskEnd);

            txtName.Text = task.Name;
            txtStatus.Text = task.Status.ToString();
            txtStart.Text = "Start : " + task.StartDate.ToString("dd/MM/yyyy");
            
            switch (task.Status)
            {
                case TaskStatus.Todo:
                    taskCard.SetBackgroundResource(Resource.Drawable.bg_task_todo);
                    break;
                case TaskStatus.InProgress:
                    taskCard.SetBackgroundResource(Resource.Drawable.bg_task_in_progress);
                    break;
                case TaskStatus.Done:
                    taskCard.SetBackgroundResource(Resource.Drawable.bg_task_done);
                    break;
            }
            
            txtEnd.Text = "End : " + task.EndDate.ToString("dd/MM/yyyy");
            
            FrameLayout frameLayout = taskCard as FrameLayout; 
            LinearLayout deleteBackground = null;
            LinearLayout taskCardContent = null;

            if (frameLayout != null && frameLayout.ChildCount >= 2)
            {
                deleteBackground = frameLayout.GetChildAt(0) as LinearLayout;
                taskCardContent = frameLayout.GetChildAt(1) as LinearLayout;
            }

            if (taskCardContent != null && deleteBackground != null)
            {
                DisplayMetrics displayMetrics = new DisplayMetrics();
                WindowManager?.DefaultDisplay?.GetMetrics(displayMetrics);
                int screenWidth = displayMetrics.WidthPixels;

                SwipeGestureListener swipeListener = new SwipeGestureListener(taskCardContent, deleteBackground, screenWidth);

                swipeListener.OnSwipeToDelete += () =>
                {
                    ShowDeleteTaskConfirmationDialog((Task)task);
                };

                taskCard.SetOnTouchListener(new SwipeTouchListener(swipeListener));
            }

            int insertIndex = container.ChildCount - 1;
            container.AddView(taskCard, insertIndex);

            taskCard.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(EditTaskActivity));
                intent.PutExtra("taskId", task.Id);
                StartActivity(intent);
            };
        }
        
        private async System.Threading.Tasks.Task DeleteTask(Task task)
        {
            Console.WriteLine("Deleting task id = " + task.Id);

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/tasks/{task.Id}")
            {
                Content = new StringContent("")
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

            Toast.MakeText(this, $"Task '{task.Name}' has been deleted", ToastLength.Short).Show();

            await LoadModuleDataAsync();
        }
        
        private void ShowDeleteTaskConfirmationDialog(Task task)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Delete Task");
            builder.SetMessage($"Are you sure you want to delete task '{task.Name}' ?");

            builder.SetPositiveButton("Yes", async (_, __) =>
            {
                await DeleteTask(task);
            });

            builder.SetNegativeButton("No", (_, __) => { });

            builder.Show();
        }
    }
}