using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using DuckLog.Models;
using TaskStatus = DuckLog.Models.TaskStatus;

namespace DuckLog.Notifications
{
    /// <summary>
    /// Classe utilitaire pour créer et envoyer des notifications de rappel de tâches.
    /// </summary>
    public static class NotificationHelper
    {
        private const string ChannelId = "ducklog_deadlines";
        private const string ChannelName = "DuckLog Deadlines";
        private const string ChannelDesc = "Notifications pour les tâches dues ou en retard";

        /// <summary>
        /// Crée le canal de notification nécessaire pour Android 8.0+.
        /// Doit être appelé au démarrage de l'application.
        /// </summary>
        /// <param name="context">Contexte Android.</param>
        [System.Runtime.Versioning.SupportedOSPlatform("android26.0")]
        public static void CreateNotificationChannel(Context context)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;

            var channel = new NotificationChannel(ChannelId, ChannelName, NotificationImportance.Default)
            {
                Description = ChannelDesc
            };

            var manager = (NotificationManager)context.GetSystemService(Context.NotificationService)!;
            manager.CreateNotificationChannel(channel);
        }

        /// <summary>
        /// Envoie une notification pour une tâche due bientôt (dans les 24h).
        /// </summary>
        /// <param name="context">Contexte Android.</param>
        /// <param name="task">Tâche concernée.</param>
        /// <param name="notificationId">Identifiant unique pour la notification.</param>
        public static void SendDueSoonNotification(Context context, ITask task, int notificationId)
        {
            var notification = new NotificationCompat.Builder(context, ChannelId)
                .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                ?.SetContentTitle("Tâche due bientôt !")
                ?.SetContentText($" {task.Name} est due le {task.EndDate:dd/MM/yyyy}")
                ?.SetPriority(NotificationCompat.PriorityDefault)
                ?.SetAutoCancel(true)
                ?.Build();

            var manager = NotificationManagerCompat.From(context);
            manager?.Notify(notificationId, notification);
        }

        /// <summary>
        /// Envoie une notification pour une tâche en retard (date de fin dépassée).
        /// </summary>
        /// <param name="context">Contexte Android.</param>
        /// <param name="task">Tâche concernée.</param>
        /// <param name="notificationId">Identifiant unique pour la notification.</param>
        public static void SendOverdueNotification(Context context, ITask task, int notificationId)
        {
            var notification = new NotificationCompat.Builder(context, ChannelId)
                ?.SetSmallIcon(Android.Resource.Drawable.IcDialogAlert)
                ?.SetContentTitle("Tâche en retard")
                ?.SetContentText($"{task.Name} était due le {task.EndDate:dd/MM/yyyy}")
                ?.SetPriority(NotificationCompat.PriorityHigh)
                ?.SetAutoCancel(true)
                ?.Build();

            var manager = NotificationManagerCompat.From(context);
            
            manager?.Notify(notificationId, notification);
        }

        /// <summary>
        /// Vérifie toutes les tâches dans la FakeModuleDatabase et envoie
        /// les notifications appropriées (due bientôt ou en retard).
        /// Ignore les tâches déjà complétées.
        /// </summary>
        /// <param name="context">Contexte Android.</param>
        public static void CheckAndNotifyAll(Context context)
        {
            DateTime now = DateTime.Now;
            
            DateTime inADay = now.AddHours(24);
            
            int notifId = 1000;
            foreach (var module in FakeModuleDatabase.MyModules)
            {
                foreach (var task in module.GetTasks())
                {
                    if (task.Status == TaskStatus.Done)
                    {
                        continue;

                    }
                    if (task.EndDate < now)
                    {
                        SendOverdueNotification(context, task, notifId++);
                    }
                    else if (task.EndDate <= inADay)
                    {
                        SendDueSoonNotification(context, task, notifId++);
                    }
                }
            }
        }
    }
}
