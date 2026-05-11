using Android.App;
using Android.Widget;
using DuckLog.Models;
using System.Net.Http.Json;

namespace DuckLog
{
    [Activity(Label = "Add User Activity")]
    public class AddUserActivity : Activity
    {
        private static readonly HttpClient HttpClient = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.Zero,
            ConnectTimeout = TimeSpan.FromSeconds(10)
        })
        {
            BaseAddress = new Uri(Constants.BaseApiUrl)
        };

        protected override void OnCreate(Android.OS.Bundle? savedInstanceState)
        {
            HttpClient.DefaultRequestHeaders.ConnectionClose = true;
            
            ActionBar?.Hide();
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_add_user);

            EditText editName = FindViewById<EditText>(Resource.Id.name_field);
            EditText editEmail = FindViewById<EditText>(Resource.Id.email_field);
            EditText editPassword = FindViewById<EditText>(Resource.Id.password_field);
            Android.Widget.Button btnRegister = FindViewById<Android.Widget.Button>(Resource.Id.register_button);
            Android.Widget.ImageButton btnReturn = FindViewById<Android.Widget.ImageButton>(Resource.Id.btnReturn);

            if (btnReturn != null)
            {
                btnReturn.Click += (s, e) => { Finish(); };
            }

            if (btnRegister != null)
            {
                btnRegister.Click += async (s, e) =>
                {
                    string newName = editName?.Text ?? "";
                    string newEmail = editEmail?.Text ?? "";
                    string newPassword = editPassword?.Text ?? "";

                    if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newPassword) ||
                        string.IsNullOrWhiteSpace(newEmail))
                    {
                        Toast.MakeText(this, "Please fill all the fields", ToastLength.Short).Show();
                        return;
                    }
                    User user = new User();
                    user.Name = newName;
                    user.Email = newEmail;
                    user.Password = newPassword;

                    try
                    {
                        var response = await HttpClient.PostAsJsonAsync("/api/users", user);

                        if (response.IsSuccessStatusCode)
                        {
                            var createdUser = await response.Content.ReadFromJsonAsync<User>();
                            if (createdUser != null)
                            {
                                User.CurrentUser = createdUser;
                            }
                            else
                            {
                                // fallback si l'API ne retourne pas l'user complet
                                User.CurrentUser = user;
                            }
                            StartActivity(typeof(ModuleActvity));
                        }
                        else
                        {
                            string errorDetails = await response.Content.ReadAsStringAsync();
                            Android.Util.Log.Error("API_ERROR",
                                $"Status: {response.StatusCode}, Details: {errorDetails}");
                            Toast.MakeText(this, $"Error {response.StatusCode}: Check Logcat", ToastLength.Long).Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        Android.Util.Log.Error("API_ERROR", $"Crash: {ex.Message}");
                        Toast.MakeText(this, "Network timeout. Check Logcat.", ToastLength.Long).Show();
                    }

                    Finish();
                };
            }
        }
    }
}