using Android.App;
using Android.Widget;
using Android.Content;
using System.Net.Http.Json;
using DuckLog.Models;
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

        /// <summary>
        /// Méthode appelée lors de la création de l'activité.
        /// Initialise les champs du formulaire et les événements des boutons et du spinner.
        /// </summary>
        /// <param name="savedInstanceState">État précédent de l'activité, si disponible.</param>
        protected override async void OnCreate(Android.OS.Bundle? savedInstanceState)
        {
            _taskId = Intent.GetIntExtra("taskId", 0);
            
            //get task from api
            _task = await FetchTaskFromApi(_taskId);

            ActionBar?.Hide();
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_edit_task);

            EditText editName = FindViewById<EditText>(Resource.Id.editTaskName);
            EditText editDesc = FindViewById<EditText>(Resource.Id.editTaskDesc);
            EditText editTaskStart = FindViewById<EditText>(Resource.Id.editTaskStart);
            EditText editTaskEnd = FindViewById<EditText>(Resource.Id.editTaskEnd);
            Spinner spinner = FindViewById<Spinner>(Resource.Id.editTaskStatus);
            Button btnCreate = FindViewById<Button>(Resource.Id.btnCreateTask);
            ImageButton btnReturn = FindViewById<ImageButton>(Resource.Id.btnReturn);

            TaskStatus newStatus = _task.Status;
            DateTime newStart = _task.StartDate;
            DateTime newEnd = _task.EndDate;

            editName.Text = _task.Name;
            editDesc.Text = _task.Description;
            editTaskStart.Text = _task.StartDate.ToString("dd/MM/yyyy");
            editTaskEnd.Text = _task.EndDate.ToString("dd/MM/yyyy");

            if (_task != null)
            {
                editName.Text = _task.Name;
                editDesc.Text = _task.Description;
                editTaskStart.Text = _task.StartDate.ToString("dd/MM/yyyy");
                editTaskEnd.Text = _task.EndDate.ToString("dd/MM/yyyy");
            }
            
            if (btnReturn != null)
            {
                btnReturn.Click += (s, e) => Finish();
            }

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
                string newName = editName?.Text ?? "";
                string newDesc = editDesc?.Text ?? "";

                if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newDesc) ||
                    newStart == DateTime.MinValue || newEnd == DateTime.MinValue)
                {
                    Toast.MakeText(this, "Please fill in all the fields", ToastLength.Short).Show();
                    return;
                }

                _task.Name = editName.Text;
                _task.Description = editDesc.Text;
                _task.StartDate = newStart;
                _task.EndDate = newEnd;
                _task.Status = newStatus;
                
                try 
                {
                    using var httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri(Constants.BaseApiUrl);

                    // Use PUT to update existing data
                    var response = await httpClient.PutAsJsonAsync($"/api/tasks/{_taskId}", _task);

                    if (response.IsSuccessStatusCode)
                    {
                        Toast.MakeText(this, "Task Updated!", ToastLength.Short).Show();
                        Finish();
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Update failed", ToastLength.Short).Show();
                }
            };
        }
        
        //To fetch task from api
        private async Task<Models.Task> FetchTaskFromApi(int id)
        {
            using var client = new HttpClient { BaseAddress = new Uri(Constants.BaseApiUrl) };
            return await client.GetFromJsonAsync<Models.Task>($"/api/tasks/{id}");
        }
    }
    
}