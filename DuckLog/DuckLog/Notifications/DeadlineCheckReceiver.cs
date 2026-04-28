using Android.App;
using Android.Content;
using DuckLog.Notifications;

namespace DuckLog.Notifications
{
    /// <summary>
    /// BroadcastReceiver déclenché par l'AlarmManager pour vérifier les deadlines des tâches.
    /// Se déclenche périodiquement en arrière-plan, même si l'app est fermée.
    /// </summary>
    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { "com.companyname.DuckLog.CHECK_DEADLINES" })]
    public class DeadlineCheckReceiver : BroadcastReceiver
    {
        /// <summary>
        /// Méthode appelée par le système lorsque l'alarme se déclenche.
        /// Lance la vérification des deadlines et envoie les notifications.
        /// </summary>
        /// <param name="context">Contexte Android fourni par le système.</param>
        /// <param name="intent">Intent reçu contenant l'action déclenchée.</param>
        public override void OnReceive(Context? context, Intent? intent)
        {
            
            if (context == null)
            {
                return;
            }

            NotificationHelper.CheckAndNotifyAll(context);
        }
    }
}
