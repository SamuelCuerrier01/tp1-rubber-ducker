using DuckLog.Models;
using Android.App;
using Android.OS;
using Android.Widget;

namespace DuckLog;

[Activity(Label = "Add Module Activity")]
public class AddModuleActivity : Activity
{
    
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
        
        btnCreate.Click += (s, e) => 
        {
            string newName = editName?.Text ?? "";
            string newDesc = editDesc?.Text ?? "";
            int newId = Data.Data.GetIdModule;
        
            if (string.IsNullOrWhiteSpace(newName))
            {
                Toast.MakeText(this, "Please enter a name!", ToastLength.Short).Show();
                return;
            }
        
            //TODO: integration with database
            
            
            Module newModule = new Module(newId, newName, newDesc);
            FakeModuleDatabase.MyModules.Add(newModule);
    
            Finish();
        };
    }
}