using Android.App;
using Android.OS;
using Android.Widget;
using DuckLog.Models;
namespace DuckLog;

[Activity(Label = "Connexion")]
class ConnectionActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_connection);

        
        Android.Widget.Button connectionButton = FindViewById<Android.Widget.Button>(Resource.Id.connection_button);
        EditText emailInput = FindViewById<EditText>(Resource.Id.email_field); 
        EditText passwordInput = FindViewById<EditText>(Resource.Id.password_field);

        if (connectionButton != null)
        {
            connectionButton.Click += (sender, e) =>
            {
                string email = emailInput?.Text ?? string.Empty;
                string password = passwordInput?.Text ?? string.Empty;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    Toast.MakeText(this, "Veuillez remplir tous les champs", ToastLength.Short)?.Show();
                    return;
                }
                User authService = new User();
                authService.ConnectUser(email, password);

                if (User.CurrentUser != null)
                {
                    Toast.MakeText(this, $"Succès : {User.CurrentUser.Name}", ToastLength.Short)?.Show();
                    
                    Finish(); 
                }
                else
                {
                    Toast.MakeText(this, "Email ou mot de passe incorrect", ToastLength.Long)?.Show();
                }
            };
        }
    }
}
