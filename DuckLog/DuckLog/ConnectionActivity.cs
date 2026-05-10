using System.Diagnostics;
using Android.App;
using Android.OS;
using Android.Widget;
using DuckLog.Api;
using DuckLog.Models;
using Activity = Android.App.Activity;

namespace DuckLog;

[Activity(Label = "Connexion")]
class ConnectionActivity : Activity
{
    private readonly AuthService _authService = new AuthService();

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_connection);

        Android.Widget.Button connectionButton = FindViewById<Android.Widget.Button>(Resource.Id.connection_button);
        EditText emailInput = FindViewById<EditText>(Resource.Id.email_field);
        EditText passwordInput = FindViewById<EditText>(Resource.Id.password_field);

        if (connectionButton != null)
        {
            connectionButton.Click += async (sender, e) =>
            {
                string email = emailInput?.Text ?? string.Empty;
                string password = passwordInput?.Text ?? string.Empty;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    Toast.MakeText(this, "Veuillez remplir tous les champs", ToastLength.Short)?.Show();
                    return;
                }

                try
                {
                    connectionButton.Enabled = false;
                    var user = await _authService.LoginAsync(email, password);

                    if (user != null)
                    {
                        User.CurrentUser = user;
                        Toast.MakeText(this, $"Succès : {user.Name}", ToastLength.Short)?.Show();
                        Finish();
                    }
                    else
                    {
                        Toast.MakeText(this, "Email ou mot de passe incorrect", ToastLength.Long)?.Show();
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long)?.Show();
                }
                finally
                {
                    connectionButton.Enabled = true;
                }
            };
        }
    }
}