using Android.App;
using Android.Content;
using Android.OS;


// Code basé de MicrosoftDoc : https://github.com/MicrosoftDocs/xamarin-docs/blob/live/docs/xamarin-forms/app-fundamentals/local-notifications.md
namespace DuckLog.Notifications
{
    /// <summary>
    /// Responsable de la planification périodique des vérifications de deadlines
    /// via l'AlarmManager Android. Le check se répète toutes les heures.
    /// </summary>
    public static class AlarmScheduler
    {
        private const string ActionCheckDeadlines = "com.companyname.DuckLog.CHECK_DEADLINES";
        /// <summary>
        /// Intervalle entre chaque vérification (1 heure en millisecondes).
        /// </summary>
        private const long IntervalMs = AlarmManager.IntervalHour;

        /// <summary>
        /// Démarre l'alarme répétitive si elle n'est pas déjà planifiée.
        /// Doit être appelé au démarrage de l'application.
        /// </summary>
        /// <param name="context">Contexte Android.</param>
        public static void Schedule(Context context)
        {
            var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService)!;
            var intent = new Intent(ActionCheckDeadlines);
            intent.SetPackage(context.PackageName);

            var pendingIntent = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            if (pendingIntent == null)
            {
                return;
            }

            long triggerAt = SystemClock.ElapsedRealtime() + IntervalMs;

            alarmManager.SetInexactRepeating(AlarmType.ElapsedRealtime, triggerAt, IntervalMs, pendingIntent);
        }

        /// <summary>
        /// Annule l'alarme répétitive (ex: si l'utilisateur désactive les notifications).
        /// </summary>
        /// <param name="context">Contexte Android.</param>
        public static void Cancel(Context context)
        {
            var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService)!;
            
            var intent = new Intent(ActionCheckDeadlines);
            
            intent.SetPackage(context.PackageName);

            var pendingIntent = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.NoCreate | PendingIntentFlags.Immutable);

            if (pendingIntent != null)
            {
                alarmManager.Cancel(pendingIntent);
            }
        }
    }
}
