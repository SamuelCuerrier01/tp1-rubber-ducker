using TaskStatus = DuckLog.Models.TaskStatus;
using System.Net.Http.Json;

namespace DuckLog
{
    /// <summary>
    /// Activité permettant d'ajouter une nouvelle tâche à un module existant.
    /// Affiche un formulaire pour saisir le nom, la description, les dates de début et fin,
    /// ainsi que le statut de la tâche.
    /// </summary>
    [Activity(Label = "Add Task Activity")]
    public class AddTaskActivity : Activity
    {
        /// <summary>
        /// Module auquel la tâche sera ajoutée.
        /// </summary>
        private int _moduleId;
        
        private static readonly HttpClient HttpClient = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.Zero, 
            ConnectTimeout = TimeSpan.FromSeconds(10) 
        })
        {
            BaseAddress = new Uri(Constants.BaseApiUrl) 
        };
        
        /// <summary>
        /// Méthode appelée lors de la création de l'activité.
        /// Initialise les champs de formulaire, les boutons et le spinner de statut.
        /// </summary>
        /// <param name="savedInstanceState">État précédent de l'activité, si disponible.</param>
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            ActionBar?.Hide();
            base.OnCreate(savedInstanceState);
            
            _moduleId = Intent.GetIntExtra("moduleId", 0);

            SetContentView(Resource.Layout.activity_add_task);

            EditText editName = FindViewById<EditText>(Resource.Id.editTaskName);
            EditText editDesc = FindViewById<EditText>(Resource.Id.editTaskDesc);
            EditText editTaskStart = FindViewById<EditText>(Resource.Id.createTaskStart);
            EditText editTaskEnd = FindViewById<EditText>(Resource.Id.createTaskEnd);
            Spinner spinner = FindViewById<Spinner>(Resource.Id.createTaskStatus);
            Button btnCreate = FindViewById<Button>(Resource.Id.btnCreateTask);
            ImageButton btnReturn = FindViewById<ImageButton>(Resource.Id.btnReturn);

            TaskStatus newStatus = TaskStatus.Todo;
            DateTime newStart = DateTime.MinValue;
            DateTime newEnd = DateTime.MinValue;

            if (btnReturn != null)
            {
                btnReturn.Click += (s, e) =>
                {
                    Finish();
                };
            }

            editTaskStart.Click += (s, e) =>
            {
                DateTime date = DateTime.Now;

                var dialog = new DatePickerDialog(
                    this,
                    (sender, args) =>
                    {
                        editTaskStart.Text = $"{args.Date.Day:00}/{args.Date.Month:00}/{args.Date.Year}";
                        newStart = new DateTime(args.Date.Year, args.Date.Month, args.Date.Day);
                    },
                    date.Year,
                    date.Month - 1,
                    date.Day
                );

                dialog.Show();
            };

            editTaskEnd.Click += (s, e) =>
            {
                DateTime date = DateTime.Now;

                var dialog = new DatePickerDialog(
                    this,
                    (sender, args) =>
                    {
                        editTaskEnd.Text = $"{args.Date.Day:00}/{args.Date.Month:00}/{args.Date.Year}";
                        newEnd = new DateTime(args.Date.Year, args.Date.Month, args.Date.Day);
                    },
                    date.Year,
                    date.Month - 1,
                    date.Day
                );

                dialog.Show();
            };

            var items = new List<TaskStatus> { TaskStatus.Todo, TaskStatus.InProgress, TaskStatus.Done };
            var adapter = new ArrayAdapter<TaskStatus>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            spinner.ItemSelected += (sender, e) =>
            {
                newStatus = items[e.Position];
            };

            btnCreate.Click += async (s, e) =>
            {
                string newName = editName?.Text ?? "";
                string newDesc = editDesc?.Text ?? "";

                if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newDesc) ||
                    newStart == DateTime.MinValue || newEnd == DateTime.MinValue)
                {
                    Toast.MakeText(this, "Please fill in all the fields", ToastLength.Short).Show();
                    return;
                }
                
                if (newStart < DateTime.Today)
                {
                    Toast.MakeText(this, "Start date cannot be in the past", ToastLength.Short).Show();
                    return;
                }

                if (newEnd < DateTime.Today)
                {
                    Toast.MakeText(this, "End date cannot be in the past", ToastLength.Short).Show();
                    return;
                }

                if (newEnd < newStart)
                {
                    Toast.MakeText(this, "End date cannot be before start date", ToastLength.Short).Show();
                    return;
                }

                var newTask = new {
                    Name = newName,
                    Description = newDesc,
                    StartDate = newStart,
                    EndDate = newEnd,
                    Status = newStatus.ToString(),
                    ModuleId = _moduleId 
                };
                
                try 
                {
                    var response = await HttpClient.PostAsJsonAsync("/api/tasks", newTask);

                    if (response.IsSuccessStatusCode)
                    {
                        Finish();
                    }
                    else
                    {
                        string errorDetails = await response.Content.ReadAsStringAsync();
    
                        Android.Util.Log.Error("API_ERROR", $"Status: {response.StatusCode}, Details: {errorDetails}");
    
                        Toast.MakeText(this, $"Error {response.StatusCode}: Check Logcat", ToastLength.Long).Show();
                    }
                }
                catch (Exception)
                {
                    Toast.MakeText(this, "Network Error", ToastLength.Short).Show();
                }
            };
        }
    }
}