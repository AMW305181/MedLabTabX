using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MedLabTab.DatabaseModels;

namespace MedLabTab.DatabaseManager
{
    internal class CategoriesManager
    {
        TransactionOptions options = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };
        public List<CategoryDictionary> GetCategories(MedLabContext db)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try {
                    var categories = db.CategoryDictionaries.ToList();
                    scope.Complete();
                    return categories;
                }
                catch { return null; }
            }
        }
        public Dictionary<int, string> GetCategoriesDictionary(MedLabContext db)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    Dictionary<int, string> categories = new Dictionary<int, string>();
                    List<CategoryDictionary> categoryDictionaries = db.CategoryDictionaries.ToList();
                    scope.Complete();
                    foreach (var category in categoryDictionaries)
                    {
                        categories.Add(category.id, category.CategoryName);
                    }
                    return categories;

                }

                catch { return null; }
            }
        }
    }

}
