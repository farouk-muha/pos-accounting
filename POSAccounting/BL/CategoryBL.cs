using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.BL
{
    public class CategoryBL
    {
        private CropAccountingAppEntities db;
        private CategoryUtils utils = new CategoryUtils();

        public CategoryBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public List<Category> Get(Expression<Func<Category, bool>> filter)
        {
            var query = from m in db.Categories select m;
            if (filter != null)
                query = query.Where(filter);
            var entities = query.ToList();
            return entities;
        }

        public PaginatedList<CategoryM> Get(int? sortOrder, Expression<Func<Category, bool>> filter, int page, int pageSize)
        {
            page = page == 0 ? 1 : page;
            var query = from m in db.Categories select m;
            if (filter != null)
                query = query.Where(filter);

            var entities = query.OrderBy(m => m.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PaginatedList<CategoryM> model = new PaginatedList<CategoryM>();
            model.Count = query.Count();
            model.Models = new ObservableCollection<CategoryM>();
            foreach (var v in entities)
            {
                var temp = utils.FromEntity(v);
                temp.ItemsCount = db.Products.Where(m => m.CategoryId == temp.Id).Count();
                model.Models.Add(temp);
                model.Amount += (decimal)temp.ItemsCount;
            }
            return model;
        }



        public CategoryM GetById(Guid id)
        {
            var entity = db.Categories.AsNoTracking().Where(m => m.Id == id).FirstOrDefault();
            return utils.FromEntity(entity);
        }

        public Guid Insert(CategoryM model)
        {
            var entity = utils.FromModel(model);
            db.Categories.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public void Delete(Guid id)
        {
            var entity = db.Categories.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(Category entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.Categories.Attach(entityToDelete);
            }
            db.Categories.Remove(entityToDelete);
            db.SaveChanges();
        }

        public void Update(CategoryM model)
        {
            var entity = utils.FromModel(model);
            var e = db.Categories.Where(m => m.Id == model.Id)
                .AsQueryable().FirstOrDefault();

            db.Entry(e).CurrentValues.SetValues(entity);
            db.SaveChanges();
        }

        public int GetNewNum()
        {
            var num = db.Categories.OrderByDescending(m => m.Num).Select(m => m.Num).FirstOrDefault();
            if (num > 0)
            {
                ++num;
            }
            else
            {
                num = 1;
            }
            return num;
        }
    }

}
