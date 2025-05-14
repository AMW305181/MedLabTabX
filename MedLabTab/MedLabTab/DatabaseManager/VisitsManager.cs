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
        public bool DeactivateVisit(MedLabContext db,Visit visit)
        {
            TransactionOptions specialOptions = new TransactionOptions
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
                        visit.IsActive = false;
                        db.SaveChanges();
                        scope.Complete();
                        return true;
                    }
                    return false;
                }
                catch { return false; }
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
        public List<Visit> GetAllVisits(MedLabContext db)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    List<Visit> AllVisits = db.Visits.ToList();
                    scope.Complete();
                    return AllVisits;
                }
                catch { return null; }
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
                    oldVisit.Cost = newVisit.Cost;
                    oldVisit.PaymentStatus = newVisit.PaymentStatus;
                    oldVisit.IsActive = newVisit.IsActive;
                    oldVisit.PatientId = newVisit.PatientId;
                    oldVisit.TimeSlotId = newVisit.TimeSlotId;
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

            return newVisit;
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
