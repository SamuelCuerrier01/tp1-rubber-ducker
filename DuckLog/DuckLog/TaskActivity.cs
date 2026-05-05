using Android.Content;
using Android.Views;
using DuckLog.Models;
using System.Net.Http.Json;
using Task = DuckLog.Models.Task;

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
        public Module module;

        private static readonly HttpClient HttpClient = new HttpClient();
        
        /// <summary>
        /// Méthode appelée lors de la création de l'activité.
        /// Initialise les boutons, l'en-tête et la logique d'ajout de tâche.
        /// </summary>
        /// <param name="savedInstanceState">État précédent de l'activité, si disponible.</param>
        protected override async void OnCreate(Android.OS.Bundle? savedInstanceState)
        {
            int moduleId = Intent.GetIntExtra("moduleId", 0);
            try 
            {
                using var client = new HttpClient { BaseAddress = new Uri(Constants.BaseApiUrl) };
                module = await client.GetFromJsonAsync<Module>($"/api/modules/{moduleId}");
                List<Task> tasks = await client.GetFromJsonAsync<List<Task>>($"/api/tasks/{moduleId}");
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Module not found in database", ToastLength.Short).Show();
                Finish();
                return;
            }
            if (module == null)
            {
                Toast.MakeText(this, "Error: module not found", ToastLength.Short).Show();
                Finish();
                return;
            }

            ActionBar?.Hide();
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_task);

            ImageButton btnReturn = FindViewById<ImageButton>(Resource.Id.btnReturn);

            LinearLayout taskListContainer = FindViewById<LinearLayout>(Resource.Id.taskListContainer);

            RelativeLayout btnAddTaskArea = FindViewById<RelativeLayout>(Resource.Id.btnAddTaskArea);

            btnAddTaskArea.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(AddTaskActivity));
                intent.PutExtra("moduleId", module.Id);
                StartActivity(intent);
            };

            if (btnReturn != null)
            {
                btnReturn.Click += (sender, e) => Finish();
            }

            TextView taskHeaderText = FindViewById<TextView>(Resource.Id.taskHeaderText);
            taskHeaderText.Text = $"{module.Name} Tasks";
        }

        /// <summary>
        /// Méthode appelée lors de la reprise de l'activité.
        /// Rafraîchit la liste des tâches affichées.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();

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