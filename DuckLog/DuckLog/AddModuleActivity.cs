using Android.App;
using Android.Widget;
using DuckLog.Models;

namespace DuckLog
{
    /// <summary>
    /// Activité permettant d'ajouter un nouveau module dans l'application DuckLog.
    /// Fournit un formulaire avec un nom et une description pour le module.
    /// </summary>
    [Activity(Label = "Add Module Activity")]
    public class AddModuleActivity : Activity
    {
        /// <summary>
        /// Méthode appelée lors de la création de l'activité.
        /// Initialise les champs de saisie, les boutons et leurs événements.
        /// </summary>
        /// <param name="savedInstanceState">État précédent de l'activité, si disponible.</param>
        protected override void OnCreate(Android.OS.Bundle? savedInstanceState)
        {
            ActionBar?.Hide();
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_add_module);

            EditText editName = FindViewById<EditText>(Resource.Id.editModuleName);
            EditText editDesc = FindViewById<EditText>(Resource.Id.editModuleDesc);
            Android.Widget.Button btnCreate = FindViewById<Android.Widget.Button>(Resource.Id.btnCreateModule);
            Android.Widget.ImageButton btnReturn = FindViewById<Android.Widget.ImageButton>(Resource.Id.btnReturn);

            if (btnReturn != null)
            {
                btnReturn.Click += (s, e) =>
                {
                    Finish();
                };
            }

            if (btnCreate != null)
            {
                btnCreate.Click += (s, e) =>
                {
                    string newName = editName?.Text ?? "";
                    string newDesc = editDesc?.Text ?? "";

                    if (string.IsNullOrWhiteSpace(newName))
                    {
                        Toast.MakeText(this, "Please enter a name!", ToastLength.Short).Show();
                        return;
                    }


                    ModuleDemo newModule = new ModuleDemo(newName, newDesc);
                    FakeModuleDatabase.MyModules.Add(newModule);

                    Finish();
                };
            }
        }
    }
}