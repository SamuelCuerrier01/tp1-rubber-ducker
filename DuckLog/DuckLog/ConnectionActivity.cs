using DuckLog.Models;

namespace DuckLog;

public class ConnectionActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_connection);
        Button connectionButton = FindViewById<Button>(Resource.Id.connection_button);
        if (connectionButton != null)
        {
            connectionButton.Click += (sender, e) =>
            {
                
                Finish();
            };
        }
    }
}