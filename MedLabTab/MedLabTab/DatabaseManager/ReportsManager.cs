using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedLabTab.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseManager
{
    internal class ReportsManager
    {
        public bool AddReport(MedLabContext db, Report report ) 
        {
            try {
                db.Reports.Add( report );
                return true;
            }
            catch { return false; }
        }

        public bool EditReport (MedLabContext db, Report ogReport, Report newData)
        {
            try {
                ogReport.Results = newData.Results;
                db.SaveChanges();
                return true;
            }
            catch { return false; }
        }
    }
}
