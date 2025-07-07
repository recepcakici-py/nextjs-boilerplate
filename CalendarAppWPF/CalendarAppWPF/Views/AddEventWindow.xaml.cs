using System;
using System.Windows;
using System.Windows.Controls;
using CalendarAppWPF.Models;

namespace CalendarAppWPF.Views
{
    public partial class AddEventWindow : Window
    {
        public Event EventData { get; private set; }
        public bool IsEventSaved { get; private set; } = false;

        public AddEventWindow(Event? existingEvent = null)
        {
            InitializeComponent();
            
            // Initialize with existing event or create new one
            EventData = existingEvent ?? new Event
            {
                StartDateTime = DateTime.Today.AddHours(DateTime.Now.Hour),
                EndDateTime = DateTime.Today.AddHours(DateTime.Now.Hour + 1),
                Color = "#E8F4FD" // Default pastel blue
            };

            DataContext = EventData;
            InitializeControls();
        }

        private void InitializeControls()
        {
            // Set date pickers
            StartDatePicker.SelectedDate = EventData.StartDateTime.Date;
            EndDatePicker.SelectedDate = EventData.EndDateTime.Date;

            // Set time controls
            StartHourComboBox.SelectedIndex = EventData.StartDateTime.Hour;
            StartMinuteComboBox.SelectedIndex = EventData.StartDateTime.Minute / 15;
            EndHourComboBox.SelectedIndex = EventData.EndDateTime.Hour;
            EndMinuteComboBox.SelectedIndex = EventData.EndDateTime.Minute / 15;

            // Set color selection
            SetColorSelection(EventData.Color);

            // Set reminder
            SetReminderSelection(EventData.ReminderMinutes);

            // Update UI based on all-day setting
            UpdateTimeControlsVisibility();
        }

        private void SetColorSelection(string color)
        {
            switch (color)
            {
                case "#E8F4FD": BlueColor.IsChecked = true; break;
                case "#FFE8E8": PinkColor.IsChecked = true; break;
                case "#E8F5E8": GreenColor.IsChecked = true; break;
                case "#FFF8E1": YellowColor.IsChecked = true; break;
                case "#F3E5F5": PurpleColor.IsChecked = true; break;
                case "#FFF3E0": OrangeColor.IsChecked = true; break;
                default: BlueColor.IsChecked = true; break;
            }
        }

        private void SetReminderSelection(int minutes)
        {
            foreach (ComboBoxItem item in ReminderComboBox.Items)
            {
                if (item.Tag != null && int.Parse(item.Tag.ToString()!) == minutes)
                {
                    ReminderComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void AllDayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EventData.IsAllDay = true;
            UpdateTimeControlsVisibility();
        }

        private void AllDayCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            EventData.IsAllDay = false;
            UpdateTimeControlsVisibility();
        }

        private void UpdateTimeControlsVisibility()
        {
            var isVisible = !EventData.IsAllDay;
            
            StartHourComboBox.IsEnabled = isVisible;
            StartMinuteComboBox.IsEnabled = isVisible;
            EndHourComboBox.IsEnabled = isVisible;
            EndMinuteComboBox.IsEnabled = isVisible;

            if (EventData.IsAllDay)
            {
                // Set to full day times
                StartHourComboBox.SelectedIndex = 0;
                StartMinuteComboBox.SelectedIndex = 0;
                EndHourComboBox.SelectedIndex = 23;
                EndMinuteComboBox.SelectedIndex = 3; // 45 minutes
            }
        }

        private void ColorRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                EventData.Color = radioButton.Name switch
                {
                    "BlueColor" => "#E8F4FD",
                    "PinkColor" => "#FFE8E8",
                    "GreenColor" => "#E8F5E8",
                    "YellowColor" => "#FFF8E1",
                    "PurpleColor" => "#F3E5F5",
                    "OrangeColor" => "#FFF3E0",
                    _ => "#E8F4FD"
                };
            }
        }

        private void ReminderCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            EventData.HasReminder = true;
            ReminderComboBox.IsEnabled = true;
        }

        private void ReminderCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            EventData.HasReminder = false;
            ReminderComboBox.IsEnabled = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                UpdateEventFromControls();
                IsEventSaved = true;
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsEventSaved = false;
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            // Validate title
            if (string.IsNullOrWhiteSpace(EventData.Title))
            {
                MessageBox.Show("Lütfen etkinlik başlığını girin.", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TitleTextBox.Focus();
                return false;
            }

            // Validate dates
            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Lütfen başlangıç ve bitiş tarihlerini seçin.", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate time logic
            var startDateTime = GetDateTimeFromControls(StartDatePicker, StartHourComboBox, StartMinuteComboBox);
            var endDateTime = GetDateTimeFromControls(EndDatePicker, EndHourComboBox, EndMinuteComboBox);

            if (endDateTime <= startDateTime)
            {
                MessageBox.Show("Bitiş zamanı başlangıç zamanından sonra olmalıdır.", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void UpdateEventFromControls()
        {
            // Update title and description
            EventData.Title = TitleTextBox.Text.Trim();
            EventData.Description = DescriptionTextBox.Text.Trim();

            // Update category
            if (CategoryComboBox.SelectedItem is ComboBoxItem selectedCategory)
            {
                EventData.Category = selectedCategory.Content.ToString() ?? "Genel";
            }

            // Update date and time
            EventData.StartDateTime = GetDateTimeFromControls(StartDatePicker, StartHourComboBox, StartMinuteComboBox);
            EventData.EndDateTime = GetDateTimeFromControls(EndDatePicker, EndHourComboBox, EndMinuteComboBox);

            // Update reminder
            if (ReminderComboBox.SelectedItem is ComboBoxItem selectedReminder && selectedReminder.Tag != null)
            {
                EventData.ReminderMinutes = int.Parse(selectedReminder.Tag.ToString()!);
            }

            // Set last modified time
            EventData.LastModified = DateTime.Now;
        }

        private DateTime GetDateTimeFromControls(DatePicker datePicker, ComboBox hourComboBox, ComboBox minuteComboBox)
        {
            var date = datePicker.SelectedDate ?? DateTime.Today;
            var hour = hourComboBox.SelectedIndex;
            var minute = minuteComboBox.SelectedIndex * 15;

            return new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
        }
    }
}
