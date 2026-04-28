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
    /// <summary>
    /// Activité principale affichant la liste des modules disponibles.
    /// Permet de naviguer vers l'ajout de module et de revenir à l'écran précédent.
    /// </summary>
    [Activity(MainLauncher = true)]
    public class ModuleActvity : Activity
    {
        /// <summary>
        /// Méthode appelée lors de la création de l'activité.
        /// Initialise les boutons et la vue principale.
        /// </summary>
        /// <param name="savedInstanceState">État précédent de l'activité, si disponible.</param>
        protected override void OnCreate(Android.OS.Bundle? savedInstanceState)
        {
            ActionBar?.Hide();
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_module);
            
            NotificationHelper.CreateNotificationChannel(this);
            AlarmScheduler.Schedule(this);
            NotificationHelper.CheckAndNotifyAll(this);

            ImageButton btnReturn = FindViewById<ImageButton>(Resource.Id.btnReturn);
            LinearLayout moduleListContainer = FindViewById<LinearLayout>(Resource.Id.moduleListContainer);
            RelativeLayout btnAddModuleArea = FindViewById<RelativeLayout>(Resource.Id.btnAddModuleArea);

            if (btnReturn != null)
            {
                btnReturn.Click += (sender, e) =>
                {
                    Finish();
                };
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

        /// <summary>
        /// Méthode appelée lors de la reprise de l'activité.
        /// Rafraîchit la liste des modules affichés.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();

            LinearLayout moduleListContainer = FindViewById<LinearLayout>(Resource.Id.moduleListContainer);

            while (moduleListContainer.ChildCount > 1)
            {
                moduleListContainer.RemoveViewAt(0);
            }
            
            foreach (var module in FakeModuleDatabase.MyModules)
            {
                LoadModuleCardUI(moduleListContainer, module); 
            }
        }

        /// <summary>
        /// Crée et ajoute une carte de module dans le conteneur donné.
        /// Affiche le nom, le nombre de tâches et la progression.
        /// </summary>
        /// <param name="container">Conteneur LinearLayout où ajouter la carte.</param>
        /// <param name="module">Module à afficher.</param>
        private void LoadModuleCardUI(LinearLayout container, Module module)
        {
            LinearLayout taskListView = FindViewById<LinearLayout>(Resource.Id.moduleListContainer);
            //Inflater for XML
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