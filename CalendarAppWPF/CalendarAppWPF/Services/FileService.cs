using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CalendarAppWPF.Models;

namespace CalendarAppWPF.Services
{
    public class FileService
    {
        private readonly string _dataDirectory;
        private readonly string _eventsFilePath;

        public FileService()
        {
            _dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CalendarAppWPF");
            _eventsFilePath = Path.Combine(_dataDirectory, "events.json");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }
        }

        public async Task<List<Event>> LoadEventsAsync()
        {
            try
            {
                if (!File.Exists(_eventsFilePath))
                {
                    return new List<Event>();
                }

                var json = await File.ReadAllTextAsync(_eventsFilePath);
                var events = JsonConvert.DeserializeObject<List<Event>>(json);
                return events ?? new List<Event>();
            }
            catch (Exception ex)
            {
                // Log error (in a real app, use proper logging)
                System.Diagnostics.Debug.WriteLine($"Error loading events: {ex.Message}");
                return new List<Event>();
            }
        }

        public async Task SaveEventsAsync(List<Event> events)
        {
            try
            {
                var json = JsonConvert.SerializeObject(events, Formatting.Indented);
                await File.WriteAllTextAsync(_eventsFilePath, json);
            }
            catch (Exception ex)
            {
                // Log error (in a real app, use proper logging)
                System.Diagnostics.Debug.WriteLine($"Error saving events: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> AddEventAsync(Event eventItem)
        {
            try
            {
                var events = await LoadEventsAsync();
                eventItem.CreatedAt = DateTime.Now;
                events.Add(eventItem);
                await SaveEventsAsync(events);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding event: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateEventAsync(Event eventItem)
        {
            try
            {
                var events = await LoadEventsAsync();
                var existingEvent = events.Find(e => e.Id == eventItem.Id);
                
                if (existingEvent != null)
                {
                    var index = events.IndexOf(existingEvent);
                    eventItem.LastModified = DateTime.Now;
                    events[index] = eventItem;
                    await SaveEventsAsync(events);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating event: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteEventAsync(Guid eventId)
        {
            try
            {
                var events = await LoadEventsAsync();
                var eventToRemove = events.Find(e => e.Id == eventId);
                
                if (eventToRemove != null)
                {
                    events.Remove(eventToRemove);
                    await SaveEventsAsync(events);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting event: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Event>> GetEventsForDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var allEvents = await LoadEventsAsync();
                return allEvents.FindAll(e => 
                    e.StartDateTime.Date >= startDate.Date && 
                    e.StartDateTime.Date <= endDate.Date);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting events for date range: {ex.Message}");
                return new List<Event>();
            }
        }
    }
}
