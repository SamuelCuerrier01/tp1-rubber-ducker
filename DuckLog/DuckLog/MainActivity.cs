using Android.App;
using Android.Content;

namespace DuckLog
{
    /// <summary>
    /// Activité principale de l'application DuckLog.
    /// Sert de point d'entrée initial et charge l'interface utilisateur principale.
    /// </summary>
    [Activity(MainLauncher = true)]
    public class MainActivity : Activity
    {
        /// <summary>
        /// Méthode appelée lors de la création de l'activité.
        /// Initialise l'interface utilisateur et les composants nécessaires.
        /// </summary>
        /// <param name="savedInstanceState">État précédent de l'activité, si disponible.</param>
        protected override void OnCreate(Android.OS.Bundle? savedInstanceState)
        {
            ActionBar?.Hide(); // hide the default action bar
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Widget.Button btnLogin = FindViewById<Android.Widget.Button>(Resource.Id.btnLogin);
            Android.Widget.Button btnRegister = FindViewById<Android.Widget.Button>(Resource.Id.btnRegister);

            if (btnLogin != null)
            {
                btnLogin.Click += (sender, e) =>
                {
                    Intent intent = new Intent(this, typeof(ConnectionActivity));
                    StartActivity(intent);
                };
            }

            if (btnRegister != null)
            {
                // todo add the register logic
            }
        }
    }
}