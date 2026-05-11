using System.Diagnostics;
using Android.App;
using Android.OS;
using Android.Widget;
using DuckLog.Services;
using DuckLog.Models;
using Activity = Android.App.Activity;

namespace DuckLog;

[Activity(Label = "Connexion")]
class ConnectionActivity : Activity
{
    private readonly AuthService _authService = new AuthService();

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        ActionBar?.Hide();
        base.OnCreate(savedInstanceState);
        
        SetContentView(Resource.Layout.activity_connection);

        Android.Widget.Button connectionButton = FindViewById<Android.Widget.Button>(Resource.Id.connection_button);
        EditText emailInput = FindViewById<EditText>(Resource.Id.email_field);
        EditText passwordInput = FindViewById<EditText>(Resource.Id.password_field);
        Android.Widget.ImageButton btnReturn = FindViewById<Android.Widget.ImageButton>(Resource.Id.btnReturn);

        if (btnReturn != null)
        {
            btnReturn.Click += (s, e) => { Finish(); };
        }

        if (connectionButton != null)
        {
            connectionButton.Click += async (sender, e) =>
            {
                    string email = emailInput?.Text?.ToString().Trim() ?? string.Empty;
                    string password = passwordInput?.Text?.ToString().Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    Toast.MakeText(this, "Please fill all the fields", ToastLength.Short)?.Show();
                    return;
                }

                try
                {
                    connectionButton.Enabled = false;
                    var user = await _authService.LoginAsync(email, password);

                    if (user != null)
                    {
                        User.CurrentUser = user;
                        Toast.MakeText(this, $"Success : {user.Name}", ToastLength.Short)?.Show();
                        StartActivity(typeof(ModuleActvity));
                    }
                    else
                    {
                        Toast.MakeText(this, "Email or password are incorrect", ToastLength.Long)?.Show();
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