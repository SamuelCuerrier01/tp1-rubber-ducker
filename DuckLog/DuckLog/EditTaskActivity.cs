using System.Net.Http.Json;
using System.Text.Json;
using Android.Content;
using Android.Views;
using TaskStatus = DuckLog.Models.TaskStatus;

namespace DuckLog
{
    /// <summary>
    /// Activité permettant d'éditer une tâche existante.
    /// Affiche un formulaire pré-rempli avec le nom, la description, les dates et le statut de la tâche.
    /// </summary>
    [Activity(Label = "Edit Task Activity")]
    public class EditTaskActivity : Activity
    {
        /// <summary>
        /// Tâche en cours d'édition.
        /// </summary>
        private Models.Task _task;

        private int _taskId;

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
        /// Initialise les champs du formulaire et les événements des boutons et du spinner.
        /// </summary>
        /// <param name="savedInstanceState">État précédent de l'activité, si disponible.</param>
        protected override async void OnCreate(Android.OS.Bundle? savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            ActionBar?.Hide();
            SetContentView(Resource.Layout.activity_edit_task);

            //get task from api
            _taskId = Intent.GetIntExtra("taskId", 0);
            
            EditText editName = FindViewById<EditText>(Resource.Id.editTaskName);
            EditText editDesc = FindViewById<EditText>(Resource.Id.editTaskDesc);
            EditText editTaskStart = FindViewById<EditText>(Resource.Id.editTaskStart);
            EditText editTaskEnd = FindViewById<EditText>(Resource.Id.editTaskEnd);
            Spinner spinner = FindViewById<Spinner>(Resource.Id.editTaskStatus);
            Button btnCreate = FindViewById<Button>(Resource.Id.btnCreateTask);
            ImageButton btnReturn = FindViewById<ImageButton>(Resource.Id.btnReturn);

            if (btnReturn != null)
            {
                btnReturn.Click += (s, e) => Finish();
            }

            try
            {
                var response = await HttpClient.GetAsync($"/api/tasks/{_taskId}");
                string rawJson = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    _task = JsonSerializer.Deserialize<Models.Task>(rawJson, options);
                }
                else
                {
                    Toast.MakeText(this, "Task not found on server.", ToastLength.Short).Show();
                    Finish();
                    return;
                }
            }
            catch (Exception)
            {
                Toast.MakeText(this, "Network Error: Could not load task.", ToastLength.Short).Show();
                Finish();
                return;
            }

            if (_task == null)
            {
                Toast.MakeText(this, "Task not found.", ToastLength.Short).Show();
                Finish();
                return;
            }

            TaskStatus newStatus = _task.Status;
            DateTime newStart = _task.StartDate;
            DateTime newEnd = _task.EndDate;

            editName.Text = _task.Name;
            editDesc.Text = _task.Description;
            editTaskStart.Text = _task.StartDate.ToString("dd/MM/yyyy");
            editTaskEnd.Text = _task.EndDate.ToString("dd/MM/yyyy");

            editTaskStart.Click += (s, e) =>
            {
                DateTime date = _task.StartDate;
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
                DateTime date = _task.EndDate;
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
            spinner.SetSelection(items.IndexOf(_task.Status));

            spinner.ItemSelected += (sender, e) => newStatus = items[e.Position];

            btnCreate.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(editName.Text) || string.IsNullOrWhiteSpace(editDesc.Text))
                {
                    Toast.MakeText(this, "Please fill in all the fields", ToastLength.Short).Show();
                    return;
                }

                if (newEnd < newStart)
                {
                    Toast.MakeText(this, "End date cannot be before start date", ToastLength.Short).Show();
                    return;
                }

                _task.Name = editName.Text;
                _task.Description = editDesc.Text;
                _task.StartDate = newStart;
                _task.EndDate = newEnd;
                _task.Status = newStatus;

                try
                {
                    var response = await HttpClient.PutAsJsonAsync($"/api/tasks/{_taskId}", _task);

                    if (response.IsSuccessStatusCode)
                    {
                        Finish();
                    }
                    else
                    {
                        Toast.MakeText(this, "Failed to update task on server.", ToastLength.Short).Show();
                    }
                }
                catch (Exception)
                {
                    Toast.MakeText(this, "Network error during update.", ToastLength.Short).Show();
                }
            };
        }
    }
}