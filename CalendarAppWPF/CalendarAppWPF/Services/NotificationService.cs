using System;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.Notifications;
using CalendarAppWPF.Models;

namespace CalendarAppWPF.Services
{
    public class NotificationService
    {
        private static NotificationService? _instance;
        private readonly Timer _checkTimer;
        private readonly FileService _fileService;
        private readonly HashSet<Guid> _notifiedEvents;

        public static NotificationService Instance => _instance ??= new NotificationService();

        private NotificationService()
        {
            _fileService = new FileService();
            _notifiedEvents = new HashSet<Guid>();
            
            // Check for notifications every minute
            _checkTimer = new Timer(60000); // 60 seconds
            _checkTimer.Elapsed += CheckForNotifications;
            _checkTimer.AutoReset = true;
        }

        public static void Initialize()
        {
            // Initialize the singleton instance
            _ = Instance;
        }

        public void StartNotificationService()
        {
            _checkTimer.Start();
        }

        public void StopNotificationService()
        {
            _checkTimer.Stop();
        }

        private async void CheckForNotifications(object? sender, ElapsedEventArgs e)
        {
            try
            {
                var events = await _fileService.LoadEventsAsync();
                var now = DateTime.Now;

                foreach (var eventItem in events)
                {
                    if (!eventItem.HasReminder || _notifiedEvents.Contains(eventItem.Id))
                        continue;

                    var reminderTime = eventItem.StartDateTime.AddMinutes(-eventItem.ReminderMinutes);
                    
                    // Check if it's time to show notification (within 1 minute window)
                    if (now >= reminderTime && now <= reminderTime.AddMinutes(1))
                    {
                        ShowNotification(eventItem);
                        _notifiedEvents.Add(eventItem.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking notifications: {ex.Message}");
            }
        }

        public void ShowNotification(Event eventItem)
        {
            try
            {
                var timeUntilEvent = eventItem.StartDateTime - DateTime.Now;
                var timeText = timeUntilEvent.TotalMinutes > 0 
                    ? $"{(int)timeUntilEvent.TotalMinutes} dakika sonra"
                    : "Şimdi başlıyor";

                new ToastContentBuilder()
                    .AddText("📅 Etkinlik Hatırlatması")
                    .AddText(eventItem.Title)
                    .AddText($"{timeText} - {eventItem.StartDateTime:HH:mm}")
                    .AddText(eventItem.Description)
                    .SetToastScenario(ToastScenario.Reminder)
                    .Show();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing notification: {ex.Message}");
            }
        }

        public void ShowTestNotification()
        {
            try
            {
                new ToastContentBuilder()
                    .AddText("📅 Takvim Uygulaması")
                    .AddText("Bildirimler aktif!")
                    .AddText("Etkinlik hatırlatmaları çalışıyor.")
                    .Show();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing test notification: {ex.Message}");
            }
        }

        public void ScheduleNotification(Event eventItem)
        {
            // Remove from notified list if it was previously notified
            // This allows rescheduling if event is modified
            _notifiedEvents.Remove(eventItem.Id);
        }

        public void CancelNotification(Guid eventId)
        {
            _notifiedEvents.Add(eventId);
        }

        public void Dispose()
        {
            _checkTimer?.Stop();
            _checkTimer?.Dispose();
        }
    }
}
