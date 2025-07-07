using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CalendarAppWPF.Models;
using CalendarAppWPF.ViewModels;

namespace CalendarAppWPF.Views
{
    public partial class CalendarView : UserControl
    {
        private CalendarViewModel? _viewModel;

        public CalendarView()
        {
            InitializeComponent();
            _viewModel = new CalendarViewModel();
            DataContext = _viewModel;

            // Subscribe to view model property changes
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            
            // Initialize calendar display
            UpdateCalendarDisplay();
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CalendarViewModel.CurrentViewMode))
            {
                UpdateViewModeDisplay();
            }
            else if (e.PropertyName == nameof(CalendarViewModel.CurrentDate) || 
                     e.PropertyName == nameof(CalendarViewModel.Events))
            {
                UpdateCalendarDisplay();
            }
            else if (e.PropertyName == nameof(CalendarViewModel.IsDarkMode))
            {
                ApplyTheme();
            }
        }

        private void UpdateViewModeDisplay()
        {
            if (_viewModel == null) return;

            // Hide all views first
            MonthViewGrid.Visibility = Visibility.Collapsed;
            WeekViewGrid.Visibility = Visibility.Collapsed;
            DayViewGrid.Visibility = Visibility.Collapsed;

            // Show the appropriate view
            switch (_viewModel.CurrentViewMode)
            {
                case "Günlük":
                    DayViewGrid.Visibility = Visibility.Visible;
                    break;
                case "Haftalık":
                    WeekViewGrid.Visibility = Visibility.Visible;
                    UpdateWeekView();
                    break;
                case "Aylık":
                default:
                    MonthViewGrid.Visibility = Visibility.Visible;
                    UpdateMonthView();
                    break;
            }
        }

        private void UpdateCalendarDisplay()
        {
            if (_viewModel == null) return;

            switch (_viewModel.CurrentViewMode)
            {
                case "Günlük":
                    // Day view is handled by data binding
                    break;
                case "Haftalık":
                    UpdateWeekView();
                    break;
                case "Aylık":
                default:
                    UpdateMonthView();
                    break;
            }
        }

        private void UpdateMonthView()
        {
            if (_viewModel == null) return;

            var calendarDays = _viewModel.GetCalendarDays();
            var calendarData = new List<CalendarDayData>();

            foreach (var day in calendarDays)
            {
                var eventsForDay = _viewModel.GetEventsForDay(day);
                calendarData.Add(new CalendarDayData
                {
                    Date = day,
                    Day = day.Day,
                    Events = eventsForDay,
                    IsCurrentMonth = day.Month == _viewModel.CurrentDate.Month,
                    IsToday = day.Date == DateTime.Today
                });
            }

            CalendarDaysControl.ItemsSource = calendarData;
        }

        private void UpdateWeekView()
        {
            if (_viewModel == null) return;

            WeekScheduleGrid.Children.Clear();
            WeekScheduleGrid.RowDefinitions.Clear();
            WeekScheduleGrid.ColumnDefinitions.Clear();

            // Create column definitions (Time + 7 days)
            WeekScheduleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
            for (int i = 0; i < 7; i++)
            {
                WeekScheduleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Create row definitions for 24 hours
            for (int hour = 0; hour < 24; hour++)
            {
                WeekScheduleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
            }

            // Add time labels
            for (int hour = 0; hour < 24; hour++)
            {
                var timeLabel = new TextBlock
                {
                    Text = $"{hour:00}:00",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Style = (Style)FindResource("BodyTextStyle")
                };
                Grid.SetRow(timeLabel, hour);
                Grid.SetColumn(timeLabel, 0);
                WeekScheduleGrid.Children.Add(timeLabel);
            }

            // Add grid lines and events
            var startOfWeek = _viewModel.CurrentDate.AddDays(-(int)_viewModel.CurrentDate.DayOfWeek + 1);
            
            for (int day = 0; day < 7; day++)
            {
                var currentDay = startOfWeek.AddDays(day);
                var eventsForDay = _viewModel.GetEventsForDay(currentDay);

                // Add vertical grid lines
                for (int hour = 0; hour < 24; hour++)
                {
                    var cell = new Border
                    {
                        BorderBrush = (System.Windows.Media.Brush)FindResource("AppBorder"),
                        BorderThickness = new Thickness(0.5),
                        Background = System.Windows.Media.Brushes.Transparent
                    };
                    Grid.SetRow(cell, hour);
                    Grid.SetColumn(cell, day + 1);
                    WeekScheduleGrid.Children.Add(cell);
                }

                // Add events
                foreach (var evt in eventsForDay)
                {
                    var eventBlock = CreateEventBlock(evt);
                    var startHour = evt.StartDateTime.Hour;
                    var duration = (evt.EndDateTime - evt.StartDateTime).TotalHours;
                    
                    Grid.SetRow(eventBlock, startHour);
                    Grid.SetColumn(eventBlock, day + 1);
                    Grid.SetRowSpan(eventBlock, Math.Max(1, (int)Math.Ceiling(duration)));
                    
                    WeekScheduleGrid.Children.Add(eventBlock);
                }
            }
        }

        private Border CreateEventBlock(Event evt)
        {
            var eventBorder = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(evt.Color)),
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(2),
                Padding = new Thickness(4),
                Cursor = Cursors.Hand
            };

            var stackPanel = new StackPanel();
            
            var titleBlock = new TextBlock
            {
                Text = evt.Title,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            
            var timeBlock = new TextBlock
            {
                Text = $"{evt.StartDateTime:HH:mm} - {evt.EndDateTime:HH:mm}",
                FontSize = 10,
                Opacity = 0.8
            };

            stackPanel.Children.Add(titleBlock);
            stackPanel.Children.Add(timeBlock);
            eventBorder.Child = stackPanel;

            // Add click handler
            eventBorder.MouseLeftButtonUp += (s, e) => EventItem_Click(s, e, evt);

            return eventBorder;
        }

        private void EventItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Event evt)
            {
                EventItem_Click(sender, e, evt);
            }
        }

        private void EventItem_Click(object sender, MouseButtonEventArgs e, Event evt)
        {
            // Open edit event window
            var editWindow = new AddEventWindow(evt)
            {
                Owner = Window.GetWindow(this),
                Title = "Etkinliği Düzenle"
            };

            if (editWindow.ShowDialog() == true && editWindow.IsEventSaved)
            {
                _viewModel?.UpdateEventAsync(editWindow.EventData);
            }
        }

        private void ApplyTheme()
        {
            if (_viewModel == null) return;

            // Apply dark/light theme
            var app = Application.Current;
            var resources = app.Resources;

            if (_viewModel.IsDarkMode)
            {
                resources["AppBackground"] = resources["DarkBackground"];
                resources["AppSurface"] = resources["DarkSurface"];
                resources["AppPrimary"] = resources["DarkPrimary"];
                resources["AppSecondary"] = resources["DarkSecondary"];
                resources["AppOnBackground"] = resources["DarkOnBackground"];
                resources["AppOnSurface"] = resources["DarkOnSurface"];
                resources["AppBorder"] = resources["DarkBorder"];
            }
            else
            {
                resources["AppBackground"] = resources["LightBackground"];
                resources["AppSurface"] = resources["LightSurface"];
                resources["AppPrimary"] = resources["LightPrimary"];
                resources["AppSecondary"] = resources["LightSecondary"];
                resources["AppOnBackground"] = resources["LightOnBackground"];
                resources["AppOnSurface"] = resources["LightOnSurface"];
                resources["AppBorder"] = resources["LightBorder"];
            }
        }
    }

    // Helper class for calendar day data
    public class CalendarDayData
    {
        public DateTime Date { get; set; }
        public int Day { get; set; }
        public System.Collections.ObjectModel.ObservableCollection<Event> Events { get; set; } = new();
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
    }
}
