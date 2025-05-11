using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedLabTab.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseManager
{
    internal class VisitsManager
    {
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
            return db.Visits
                .Include(v => v.TimeSlot)
                    .ThenInclude(ts => ts.Nurse)
                .Include(v => v.TestHistories)
                    .ThenInclude(th => th.Test)
                .Where(v => v.PatientId == userId)
                .AsNoTracking()
                .ToList();
        }
        public List<Visit> GetAllVisits(MedLabContext db)
        {
            try
            {
                List<Visit> AllVisits = db.Visits.ToList();
                return AllVisits;
            }
            catch { return null; }
        }
        public bool EditVisit(MedLabContext db,Visit oldVisit, Visit newVisit)
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

    }
