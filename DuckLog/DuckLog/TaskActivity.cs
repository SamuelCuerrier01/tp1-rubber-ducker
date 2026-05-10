using Android.Content;
using Android.Views;
using DuckLog.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

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
            
            SetContentView(Resource.Layout.activity_task);
            
            _moduleId = Intent.GetIntExtra("moduleId", 0);

            ImageButton btnReturn = FindViewById<ImageButton>(Resource.Id.btnReturn);
            
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
            txtEnd.Text = "End : " + task.EndDate.ToString("dd/MM/yyyy");

            int insertIndex = container.ChildCount - 1;
            container.AddView(taskCard, insertIndex);

            taskCard.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(EditTaskActivity));
                intent.PutExtra("taskId", task.Id);
                StartActivity(intent);
            };
        }
    }
}