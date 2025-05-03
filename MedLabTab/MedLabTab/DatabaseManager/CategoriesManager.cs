using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedLabTab.DatabaseModels;

namespace MedLabTab.DatabaseManager
{
    internal class CategoriesManager
    {
        public List<CategoryDictionary> GetCategories(MedLabContext db)
        {
            try { return db.CategoryDictionaries.ToList(); }
            catch { return null; }
        }
        public Dictionary<int, string> GetCategoriesDictionary(MedLabContext db)
        {
            try
            {
                Dictionary<int, string> categories = new Dictionary<int, string>();
                List<CategoryDictionary> categoryDictionaries = db.CategoryDictionaries.ToList();
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
