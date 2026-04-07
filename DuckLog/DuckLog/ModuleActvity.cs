using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Animation;
using Android.Util;
using DuckLog;
using DuckLog.Models;

namespace DuckLog
{
    [Activity(MainLauncher = true)]
    //[Activity(Label = "Module Activity")]
    public class ModuleActvity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            ActionBar?.Hide(); // hide the default action bar
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_module);

            //return button
            ImageButton btnReturn = FindViewById<ImageButton>(Resource.Id.btnReturn);
            
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
            //Inflater for XML
            View moduleCard = LayoutInflater.Inflate(Resource.Layout.item_module, container, false);
            
            //Text for module
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

            //Insertion before add module button
            int insertIndex = container.ChildCount - 1;
            container.AddView(moduleCard, insertIndex);
        }
        
        private void ShowDeleteConfirmationDialog(ModuleDemo module, View moduleCard, LinearLayout container)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Delete Module");
            builder.SetMessage($"Est-tu sûr de vouloir supprimer le module '{module.Name}'?");
            
            builder.SetPositiveButton("Oui", (_, __) =>
            {
                FakeModuleDatabase.MyModules.Remove(module);
                
                // Animation de disparition du module
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