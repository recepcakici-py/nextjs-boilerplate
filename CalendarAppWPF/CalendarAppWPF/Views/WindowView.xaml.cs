using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using CalendarAppWPF.ViewModels;
using CalendarAppWPF.Services;

namespace CalendarAppWPF.Views
{
    public partial class WindowView : MetroWindow
    {
        private CalendarViewModel? _calendarViewModel;

        public WindowView()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            
            this.DataContext = App.Services!.GetRequiredService<WindowViewModel>();
            
            // Get the calendar view model from the calendar view
            this.Loaded += WindowView_Loaded;
        }

        private void WindowView_Loaded(object sender, RoutedEventArgs e)
        {
            // Get the calendar view model
            if (MainCalendarView.DataContext is CalendarViewModel calendarVM)
            {
                _calendarViewModel = calendarVM;
            }
        }

        private void GoToSource(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://github.com/carsten-riedel/Coree.Template.Project") { UseShellExecute = true });
        }

        private void ShowCalendar(object sender, RoutedEventArgs e)
        {
            // Calendar is already shown as main content
            MainCalendarView.Visibility = Visibility.Visible;
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ayarlar penceresi henüz geliştirilmemiştir.", "Bilgi", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NewEvent_Click(object sender, RoutedEventArgs e)
        {
            var addEventWindow = new AddEventWindow()
            {
                Owner = this
            };

            if (addEventWindow.ShowDialog() == true && addEventWindow.IsEventSaved)
            {
                _calendarViewModel?.AddEventAsync(addEventWindow.EventData);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DayView_Click(object sender, RoutedEventArgs e)
        {
            _calendarViewModel?.ShowDayViewCommand.Execute(null);
        }

        private void WeekView_Click(object sender, RoutedEventArgs e)
        {
            _calendarViewModel?.ShowWeekViewCommand.Execute(null);
        }

        private void MonthView_Click(object sender, RoutedEventArgs e)
        {
            _calendarViewModel?.ShowMonthViewCommand.Execute(null);
        }

        private void ToggleDarkMode_Click(object sender, RoutedEventArgs e)
        {
            _calendarViewModel?.ToggleDarkModeCommand.Execute(null);
        }

        private void TestNotification_Click(object sender, RoutedEventArgs e)
        {
            NotificationService.Instance.ShowTestNotification();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Takvim Uygulaması v1.0\n\n" +
                "Modern, minimalist tasarımlı takvim uygulaması.\n" +
                "Pastel renkler ve karanlık mod desteği ile.\n\n" +
                "Özellikler:\n" +
                "• Günlük, haftalık, aylık görünümler\n" +
                "• Etkinlik ekleme ve düzenleme\n" +
                "• Windows bildirimleri\n" +
                "• JSON dosyasına kaydetme\n" +
                "• Karanlık/aydınlık mod\n\n" +
                "Geliştirici: Calendar Developer\n" +
                "© 2025 Tüm hakları saklıdır.",
                "Hakkında",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Stop notification service when closing
            NotificationService.Instance.StopNotificationService();
            base.OnClosing(e);
        }
    }
}
