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
            try
            {
                if (visit != null)
                {
                    visit.IsActive = false;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch { return false; }
        }
        public List<Visit> GetMyVisits(MedLabContext db, int userId)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                return db.Visits
                .Include(v => v.TimeSlot)
                    .ThenInclude(ts => ts.Nurse)
                .Include(v => v.TestHistories)
                    .ThenInclude(th => th.Test)
                .Where(v => v.PatientId == userId)
                .AsNoTracking()
                .ToList();
            }
        }
        public List<Visit> GetAllVisits(MedLabContext db)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    List<Visit> AllVisits = db.Visits.ToList();
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
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    oldVisit.Cost = newVisit.Cost;
                    oldVisit.PaymentStatus = newVisit.PaymentStatus;
                    oldVisit.IsActive = newVisit.IsActive;
                    oldVisit.PatientId = newVisit.PatientId;
                    oldVisit.TimeSlotId = newVisit.TimeSlotId;
                    db.SaveChanges();
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

    }

    }
