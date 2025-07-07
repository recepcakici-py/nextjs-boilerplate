using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CalendarAppWPF.Models;
using CalendarAppWPF.Services;

namespace CalendarAppWPF.ViewModels
{
    public partial class CalendarViewModel : ObservableObject
    {
        private readonly FileService _fileService;
        private readonly NotificationService _notificationService;

        [ObservableProperty]
        private DateTime _currentDate = DateTime.Today;

        [ObservableProperty]
        private string _currentViewMode = "Aylık";

        [ObservableProperty]
        private ObservableCollection<Event> _events = new();

        [ObservableProperty]
        private ObservableCollection<Event> _currentViewEvents = new();

        [ObservableProperty]
        private bool _isDarkMode = false;

        [ObservableProperty]
        private Event? _selectedEvent;

        public CalendarViewModel()
        {
            _fileService = new FileService();
            _notificationService = NotificationService.Instance;
            
            // Initialize commands
            PreviousPeriodCommand = new RelayCommand(MoveToPreviousPeriod);
            NextPeriodCommand = new RelayCommand(MoveToNextPeriod);
            TodayCommand = new RelayCommand(MoveToToday);
            AddEventCommand = new RelayCommand(AddNewEvent);
            EditEventCommand = new RelayCommand<Event>(EditEvent);
            DeleteEventCommand = new RelayCommand<Event>(DeleteEvent);
            ToggleDarkModeCommand = new RelayCommand(ToggleDarkMode);
            ShowDayViewCommand = new RelayCommand(() => ChangeViewMode("Günlük"));
            ShowWeekViewCommand = new RelayCommand(() => ChangeViewMode("Haftalık"));
            ShowMonthViewCommand = new RelayCommand(() => ChangeViewMode("Aylık"));

            // Load events and start notification service
            _ = LoadEventsAsync();
            _notificationService.StartNotificationService();
        }

        public ICommand PreviousPeriodCommand { get; }
        public ICommand NextPeriodCommand { get; }
        public ICommand TodayCommand { get; }
        public ICommand AddEventCommand { get; }
        public ICommand EditEventCommand { get; }
        public ICommand DeleteEventCommand { get; }
        public ICommand ToggleDarkModeCommand { get; }
        public ICommand ShowDayViewCommand { get; }
        public ICommand ShowWeekViewCommand { get; }
        public ICommand ShowMonthViewCommand { get; }

        public string CurrentPeriodText
        {
            get
            {
                return CurrentViewMode switch
                {
                    "Günlük" => CurrentDate.ToString("dd MMMM yyyy, dddd"),
                    "Haftalık" => GetWeekRangeText(),
                    "Aylık" => CurrentDate.ToString("MMMM yyyy"),
                    _ => CurrentDate.ToString("MMMM yyyy")
                };
            }
        }

        private string GetWeekRangeText()
        {
            var startOfWeek = CurrentDate.AddDays(-(int)CurrentDate.DayOfWeek + 1);
            var endOfWeek = startOfWeek.AddDays(6);
            return $"{startOfWeek:dd MMM} - {endOfWeek:dd MMM yyyy}";
        }

        private void MoveToPreviousPeriod()
        {
            CurrentDate = CurrentViewMode switch
            {
                "Günlük" => CurrentDate.AddDays(-1),
                "Haftalık" => CurrentDate.AddDays(-7),
                "Aylık" => CurrentDate.AddMonths(-1),
                _ => CurrentDate.AddMonths(-1)
            };
            UpdateCurrentViewEvents();
            OnPropertyChanged(nameof(CurrentPeriodText));
        }

        private void MoveToNextPeriod()
        {
            CurrentDate = CurrentViewMode switch
            {
                "Günlük" => CurrentDate.AddDays(1),
                "Haftalık" => CurrentDate.AddDays(7),
                "Aylık" => CurrentDate.AddMonths(1),
                _ => CurrentDate.AddMonths(1)
            };
            UpdateCurrentViewEvents();
            OnPropertyChanged(nameof(CurrentPeriodText));
        }

        private void MoveToToday()
        {
            CurrentDate = DateTime.Today;
            UpdateCurrentViewEvents();
            OnPropertyChanged(nameof(CurrentPeriodText));
        }

        private void ChangeViewMode(string viewMode)
        {
            CurrentViewMode = viewMode;
            UpdateCurrentViewEvents();
            OnPropertyChanged(nameof(CurrentPeriodText));
        }

        private void AddNewEvent()
        {
            // This will be handled by opening AddEventWindow
            var newEvent = new Event
            {
                StartDateTime = CurrentDate.Date.AddHours(DateTime.Now.Hour),
                EndDateTime = CurrentDate.Date.AddHours(DateTime.Now.Hour + 1)
            };
            
            // In a real implementation, this would open a dialog
            // For now, we'll add a sample event
            AddEventAsync(newEvent);
        }

        private void EditEvent(Event? eventToEdit)
        {
            if (eventToEdit != null)
            {
                SelectedEvent = eventToEdit;
                // This would open edit dialog
            }
        }

        private async void DeleteEvent(Event? eventToDelete)
        {
            if (eventToDelete != null)
            {
                await DeleteEventAsync(eventToDelete);
            }
        }

        private void ToggleDarkMode()
        {
            IsDarkMode = !IsDarkMode;
            // Apply theme changes - this would be handled by the main window
        }

        public async Task LoadEventsAsync()
        {
            try
            {
                var loadedEvents = await _fileService.LoadEventsAsync();
                Events.Clear();
                foreach (var evt in loadedEvents)
                {
                    Events.Add(evt);
                }
                UpdateCurrentViewEvents();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading events: {ex.Message}");
            }
        }

        public async Task AddEventAsync(Event newEvent)
        {
            try
            {
                var success = await _fileService.AddEventAsync(newEvent);
                if (success)
                {
                    Events.Add(newEvent);
                    UpdateCurrentViewEvents();
                    
                    // Schedule notification
                    _notificationService.ScheduleNotification(newEvent);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding event: {ex.Message}");
            }
        }

        public async Task UpdateEventAsync(Event updatedEvent)
        {
            try
            {
                var success = await _fileService.UpdateEventAsync(updatedEvent);
                if (success)
                {
                    var existingEvent = Events.FirstOrDefault(e => e.Id == updatedEvent.Id);
                    if (existingEvent != null)
                    {
                        var index = Events.IndexOf(existingEvent);
                        Events[index] = updatedEvent;
                        UpdateCurrentViewEvents();
                        
                        // Reschedule notification
                        _notificationService.ScheduleNotification(updatedEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating event: {ex.Message}");
            }
        }

        public async Task DeleteEventAsync(Event eventToDelete)
        {
            try
            {
                var success = await _fileService.DeleteEventAsync(eventToDelete.Id);
                if (success)
                {
                    Events.Remove(eventToDelete);
                    UpdateCurrentViewEvents();
                    
                    // Cancel notification
                    _notificationService.CancelNotification(eventToDelete.Id);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting event: {ex.Message}");
            }
        }

        private void UpdateCurrentViewEvents()
        {
            var (startDate, endDate) = GetCurrentViewDateRange();
            var filteredEvents = Events.Where(e => 
                e.StartDateTime.Date >= startDate && 
                e.StartDateTime.Date <= endDate).ToList();

            CurrentViewEvents.Clear();
            foreach (var evt in filteredEvents.OrderBy(e => e.StartDateTime))
            {
                CurrentViewEvents.Add(evt);
            }
        }

        private (DateTime startDate, DateTime endDate) GetCurrentViewDateRange()
        {
            return CurrentViewMode switch
            {
                "Günlük" => (CurrentDate.Date, CurrentDate.Date),
                "Haftalık" => GetWeekRange(CurrentDate),
                "Aylık" => GetMonthRange(CurrentDate),
                _ => GetMonthRange(CurrentDate)
            };
        }

        private (DateTime startDate, DateTime endDate) GetWeekRange(DateTime date)
        {
            var startOfWeek = date.AddDays(-(int)date.DayOfWeek + 1);
            var endOfWeek = startOfWeek.AddDays(6);
            return (startOfWeek, endOfWeek);
        }

        private (DateTime startDate, DateTime endDate) GetMonthRange(DateTime date)
        {
            var startOfMonth = new DateTime(date.Year, date.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            return (startOfMonth, endOfMonth);
        }

        public ObservableCollection<DateTime> GetCalendarDays()
        {
            var days = new ObservableCollection<DateTime>();
            var (startDate, endDate) = GetMonthRange(CurrentDate);
            
            // Start from the first day of the week containing the first day of the month
            var firstDayOfWeek = startDate.AddDays(-(int)startDate.DayOfWeek + 1);
            
            // Add days for 6 weeks (42 days) to fill the calendar grid
            for (int i = 0; i < 42; i++)
            {
                days.Add(firstDayOfWeek.AddDays(i));
            }
            
            return days;
        }

        public ObservableCollection<Event> GetEventsForDay(DateTime day)
        {
            var dayEvents = Events.Where(e => e.StartDateTime.Date == day.Date)
                                 .OrderBy(e => e.StartDateTime)
                                 .ToList();
            
            return new ObservableCollection<Event>(dayEvents);
        }
    }
}
