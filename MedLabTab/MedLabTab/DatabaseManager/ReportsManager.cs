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
    internal class ReportsManager
    {
        TransactionOptions options = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };
        public bool AddReport(MedLabContext db, Report report ) 
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    db.Reports.Add(report);
                    scope.Complete();
                    return true;
                }
                catch { return false; }
            }
        }

        public bool EditReport (MedLabContext db, Report ogReport, Report newData)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    ogReport.Results = newData.Results;
                    db.SaveChanges();
                    scope.Complete();
                    return true;
                }
                catch { return false; }
            }
        }
        public Report GetReport(MedLabContext db, int Id)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    var reports = db.Reports.Where(t => t.SampleId == Id).FirstOrDefault();
                    scope.Complete();
                    if (reports != null) { return reports; }
                    return null;
                }
                catch { return null; }
            }
        }
    }
}
