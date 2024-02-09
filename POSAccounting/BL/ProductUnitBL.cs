using POSAccounting.Models;
using POSAccounting.Utils;
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
    public class ProductUnitBL
    {
        private CropAccountingAppEntities db;
        private ProductUnitUtils unitUtils = new ProductUnitUtils();

        public ProductUnitBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public List<ProductUnit> Get(Expression<Func<ProductUnit, bool>> filter)
        {
            var query = from m in db.ProductUnits select m;
            if (filter != null)
                query = query.Where(filter);
            return query.ToList();
        }

        public ObservableCollection<ProductUnitM> GetModels(Expression<Func<ProductUnit, bool>> filter)
        {
            var query = from m in db.ProductUnits select m;
            if (filter != null)
                query = query.Where(filter);
            var entities = query.ToList();
            ObservableCollection<ProductUnitM> models = new ObservableCollection<ProductUnitM>();
            foreach (var v in entities)
            {
                var temp = ProductUnitUtils.FromEntity(v);
                temp.UnitName = db.Units.Where(m => m.Id == temp.UnitId).Select(m => m.Name).FirstOrDefault();
                models.Add(temp);
            }
            return models;
        }

        public Guid Insert(ProductUnitM model)
        {
            var entity = ProductUnitUtils.FromModel(model);
            db.ProductUnits.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }


        public void Delete(Guid id)
        {
            var entity = db.ProductUnits.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(ProductUnit entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.ProductUnits.Attach(entityToDelete);
            }
            db.ProductUnits.Remove(entityToDelete);
            db.SaveChanges();
        }
    }
}
