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
    internal class VisitsManager
    {
        TransactionOptions options = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };
        public bool DeactivateVisit(Visit visit)
        {
            using (var db = new MedLabContext()) 
            {
                TransactionOptions options = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.Serializable,
                    Timeout = TransactionManager.DefaultTimeout
                };

                using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    try
                    {
                        if (visit != null)
                        {
                            var visitToUpdate = db.Visits.Find(visit.id); 
                            if (visitToUpdate != null)
                            {
                                visitToUpdate.IsActive = false;
                                db.SaveChanges();
                                scope.Complete();
                                return true;
                            }
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
        }
        public List<Visit> GetMyVisits(MedLabContext db, int userId)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                List<Visit>  visits=db.Visits
                .Include(v => v.TimeSlot)
                    .ThenInclude(ts => ts.Nurse)
                .Include(v => v.TestHistories)
                    .ThenInclude(th => th.Test)
                .Where(v => v.PatientId == userId)
                .AsNoTracking()
                .ToList();
                scope.Complete();
                return visits;
            }
        }
public List<Visit> GetAllVisits()
{
    using (var db = new MedLabContext())
    using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
    {
        try
        {
            List<Visit> allVisits = db.Visits
                .Include(v => v.Patient)
                .Include(v => v.TimeSlot)
                    .ThenInclude(ts => ts.Nurse)
                .Include(v => v.TestHistories)
                    .ThenInclude(th => th.Test)
                .ToList();

            scope.Complete();
            return allVisits;
        }
        catch
        {
            return null;
        }
    }
}

        public bool EditVisit(MedLabContext db,Visit oldVisit, Visit newVisit)
        {
            TransactionOptions specialOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.Serializable,
                Timeout = TransactionManager.DefaultTimeout
            };
            using (var scope = new TransactionScope(TransactionScopeOption.Required, specialOptions))
            {
                try
                {
                    var existingVisit = db.Visits.Find(oldVisit.id);
                    if (existingVisit == null) return false;
                    existingVisit.Cost = newVisit.Cost;
                    existingVisit.PaymentStatus = newVisit.PaymentStatus;
                    existingVisit.IsActive = newVisit.IsActive;
                    existingVisit.PatientId = newVisit.PatientId;
                    existingVisit.TimeSlotId = newVisit.TimeSlotId;
                    db.SaveChanges();
                    scope.Complete();
                    return true;
                }
                catch { return false; }
            }
        }

        //NEW STUFF - Kamiś
        public bool AddVisit(MedLabContext db, float cost, bool paymentStatus, bool isActive, //currently not in use
                     int patientId, int? timeSlotId)
        {
            try
            {
                Visit newVisit = new Visit
                {
                    Cost = cost,
                    PaymentStatus = paymentStatus,
                    IsActive = isActive,
                    PatientId = patientId,
                    TimeSlotId = timeSlotId
                };

                db.Visits.Add(newVisit);
                db.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public Visit CreateVisit(MedLabContext db, float cost, bool paymentStatus, //the important one
                        bool isActive, int patientId, int? timeSlotId)
        {
            TransactionOptions specialOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.Serializable,
                Timeout = TransactionManager.DefaultTimeout
            };
           // using (var scope = new TransactionScope(TransactionScopeOption.Required, specialOptions))
           // {
                Visit newVisit = new Visit
                {
                    Cost = cost,
                    PaymentStatus = paymentStatus,
                    IsActive = isActive,
                    PatientId = patientId,
                    TimeSlotId = timeSlotId
                };
                //bool timeSlotTaken = db.Visits.Any(v => v.IsActive && v.TimeSlotId == timeSlotId);
                //if (timeSlotTaken) { return null; }
                db.Visits.Add(newVisit);
                db.SaveChanges();
                //scope.Complete();
                return newVisit;
            //}
        }

        public List<Visit> GetNurseVisits(MedLabContext db,int nurseId)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                List<Visit> nurseVisits = db.Visits
                .Include(v => v.TimeSlot)
                    .ThenInclude(ts => ts.Nurse)
                .Include(v => v.TestHistories)
                    .ThenInclude(th => th.Test)
                .Include(v => v.Patient)
                .Where(v => v.IsActive == true && v.TestHistories.Any(th => th.Status == 2) && v.TimeSlot != null && v.TimeSlot.NurseId == nurseId)
                .ToList();
                scope.Complete();
                return nurseVisits;
            }
        }

    }

    }
