using DuckLog.Data;
using DuckLog.Models;

namespace DuckLog;

[Activity(Label = "Connexion")]
public class ConnectionActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_connection);
        Button connectionButton = FindViewById<Button>(Resource.Id.connection_button);
        EditText emailInput = FindViewById<EditText>(Resource.Id.email_input);
        EditText passwordInput = FindViewById<EditText>(Resource.Id.password_input);
        string email = emailInput?.Text ?? string.Empty;
        string password = passwordInput?.Text ?? string.Empty;
        if (connectionButton != null)
        {
            connectionButton.Click += (sender, e) =>
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    Toast
                        .MakeText(this, "Veuillez remplir tous les champs", ToastLength.Short)
                        ?.Show();
                    return;
                }
                User authService = new User();
                authService.ConnectUser(email, password);

                if (User.CurrentUser != null)
                {
                    Toast
                        .MakeText(this, $"Succès : {User.CurrentUser.Name}", ToastLength.Short)
                        ?.Show();

                    Finish();
                }
                else
                {
                    Toast
                        .MakeText(this, "Email ou mot de passe incorrect", ToastLength.Long)
                        ?.Show();
                }
            };
        }
    }
}
