using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MedLabTab.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseManager
{
    internal class SchedulesManager
    {
        TransactionOptions options = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };
        public Schedule GetSchedule(MedLabContext db,int Id)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    var schedule = db.Schedules.Where(s => s.id == Id).FirstOrDefault();
                    scope.Complete();
                    if (schedule != null) { return schedule; }
                    return null;
                }
                catch { return null; }
            }
        }
        public List<Schedule> GetAllDates(MedLabContext db)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    List<Schedule> AvailableDates = db.Schedules.ToList();
                    scope.Complete();
                    return AvailableDates;
                }
                catch { return null; }
            }
        }

        public List<Schedule> GetAvailableSlotsForDate(MedLabContext db, DateOnly date)
        {
            TransactionOptions specialOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadUncommitted,
                Timeout = TransactionManager.DefaultTimeout
            };
            using (var scope = new TransactionScope(TransactionScopeOption.Required, specialOptions))
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
                    scope.Complete();
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

        public List<Schedule> GetAllSlotsForDate(MedLabContext db, DateOnly date)
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

                /*// Pobierz ID harmonogramów, które mają aktywne wizyty
                var bookedSlotIds = db.Visits
                    .Where(v => v.IsActive && v.TimeSlotId != null)
                    .Select(v => v.TimeSlotId.Value)
                    .Distinct() // Dodane, żeby upewnić się, że każde ID jest liczone tylko raz
                    .ToList();

                // Filtruj sloty, aby wykluczyć te, które już mają aktywne wizyty
                var availableSlots = allSlots
                    .Where(s => !bookedSlotIds.Contains(s.id))
                    .ToList();*/

                //Console.WriteLine($"Found {allSlots.Count} total slots, {availableSlots.Count} are available");

                return allSlots;
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Exception in GetAvailableSlotsForDate: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return new List<Schedule>();
            }
        }

        public bool AddScheduleSlots(MedLabContext db, List<Schedule> slots)
        {
            // Return early if no slots to add
            if (slots == null || slots.Count == 0)
                return false;

            try
            {
                // Start a transaction to ensure all slots are added together or none at all
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Check for existing slots before adding to avoid duplicates
                        foreach (var slot in slots)
                        {
                            bool slotExists = db.Schedules.Any(s =>
                                s.NurseId == slot.NurseId &&
                                s.Date == slot.Date &&
                                s.Time == slot.Time);

                            if (slotExists)
                            {
                                // Found a conflict, roll back transaction
                                transaction.Rollback();
                                return false;
                            }
                        }

                        // Add all slots to the context
                        db.Schedules.AddRange(slots);

                        // Save changes to the database
                        db.SaveChanges();

                        // Commit the transaction if everything succeeded
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        // Something went wrong, roll back the transaction
                        transaction.Rollback();
                        throw; // Re-throw the exception to be caught by the outer try-catch
                    }
                }
            }
            catch (Exception)
            {
                // Log the error if you have a logging system
                // logger.LogError($"Error adding schedule slots: {ex.Message}");
                return false;
            }
        }

    }
}
