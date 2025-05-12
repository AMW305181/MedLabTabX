using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedLabTab.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseManager
{
    internal class SchedulesManager
    {
        public Schedule GetSchedule(MedLabContext db,int Id)
        {
            try
            {
                var schedule = db.Schedules.Where(s => s.id == Id).FirstOrDefault();
                if (schedule != null) { return schedule; }
                return null;
            }
            catch { return null; }
        }
        public List<Schedule> GetAllDates(MedLabContext db)
        {
            try
            {
                List<Schedule> AvailableDates = db.Schedules.ToList();
                return AvailableDates;
            }
            catch { return null; }
        }

        public List<Schedule> GetAvailableSlotsForDate(MedLabContext db, DateOnly date)
        {
            try
            {
                // First, get all schedules for the specified date
                var allSlots = db.Schedules
                    .Where(s => s.Date.Year == date.Year &&
                                 s.Date.Month == date.Month &&
                                 s.Date.Day == date.Day)
                    .ToList();

                if (allSlots == null || !allSlots.Any())
                {
                    // Log or debug output
                    Console.WriteLine($"No schedule slots found for date: {date}");
                    return new List<Schedule>();
                }

                // Pobierz ID harmonogramów, które mają aktywne wizyty
                var bookedSlotIds = db.Visits
                    .Where(v => v.IsActive && v.TimeSlotId != null)
                    .Select(v => v.TimeSlotId.Value)
                    .Distinct() // Dodane, żeby upewnić się, że każde ID jest liczone tylko raz
                    .ToList();

                // Filtruj sloty, aby wykluczyć te, które już mają aktywne wizyty
                var availableSlots = allSlots
                    .Where(s => !bookedSlotIds.Contains(s.id))
                    .ToList();

                Console.WriteLine($"Found {allSlots.Count} total slots, {availableSlots.Count} are available");

                return availableSlots;
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Exception in GetAvailableSlotsForDate: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return new List<Schedule>();
            }
        }
    }
}
