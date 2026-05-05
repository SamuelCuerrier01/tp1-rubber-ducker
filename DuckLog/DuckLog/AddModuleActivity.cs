using System.Net.Http.Json;
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
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            ActionBar?.Hide();
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_add_module);

            EditText editName = FindViewById<EditText>(Resource.Id.editModuleName);
            EditText editDesc = FindViewById<EditText>(Resource.Id.editModuleDesc);
            Button btnCreate = FindViewById<Button>(Resource.Id.btnCreateModule);
            ImageButton btnReturn = FindViewById<ImageButton>(Resource.Id.btnReturn);

            if (btnReturn != null)
            {
                btnReturn.Click += (s, e) =>
                {
                    Finish();
                };
            }

            if (btnCreate != null)
            {
                btnCreate.Click += async (s, e) =>
                {
                    string newName = editName?.Text ?? "";
                    string newDesc = editDesc?.Text ?? "";

                    if (string.IsNullOrWhiteSpace(newName))
                    {
                        Toast.MakeText(this, "Please enter a name!", ToastLength.Short).Show();
                        return;
                    }


                    Module newModule = new Module(newName, newDesc);
                    try 
                    {
                        // Client set up
                        using var httpClient = new HttpClient();
                        httpClient.BaseAddress = new Uri(Constants.BaseApiUrl);

                        // Post module into api
                        var response = await httpClient.PostAsJsonAsync("/api/modules", newModule);

                        if (response.IsSuccessStatusCode)
                        {
                            Finish(); //Close the screen if the database saved it
                        }
                        else
                        {
                            Toast.MakeText(this, "Server Error: Could not save.", ToastLength.Long).Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, $"Connection failed: {ex.Message}", ToastLength.Long).Show();
                    }
                };
            }
        }
    }
}