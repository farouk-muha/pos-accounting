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
    public class StoreBL
    {
        private CropAccountingAppEntities db;
        private StoreUtils utils = new StoreUtils();

        public StoreBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public StoreM GetById(Guid id)
        {
            var entity = db.Stores.AsNoTracking().Where(m => m.Id == id).FirstOrDefault();
            return utils.FromEntity(entity);
        }

        public PaginatedList<StoreM> Get(int? sortOrder, Expression<Func<Store, bool>> filter, int page, int pageSize)
        {
            page = page == 0 ? 1 : page;
            var query = from m in db.Stores select m;
            if (filter != null)
                query = query.Where(filter);

            var entities = query.AsNoTracking().OrderBy(m => m.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PaginatedList<StoreM> model = new PaginatedList<StoreM>();
            model.Count = query.Count();
            model.Models = new ObservableCollection<StoreM>();
            foreach (var v in entities)
            {
                var temp = utils.FromEntity(v);
                temp.ItemsCount = GetItemsCount(v.Id);
                model.Models.Add(temp);
                model.Amount += (decimal)temp.ItemsCount;
            }
            return model;
        }

        public List<Store> Get(Expression<Func<Store, bool>> filter)
        {
            var query = from m in db.Stores select m;
            if (filter != null)
                query = query.Where(filter);
            var entities = query.ToList();
            return entities;
        }

        public double GetItemsCount(Guid id)
        {
            //var v = (from m in db.Invoices
            //              join m2 in db.InvoiceLines on m.Id equals m2.InvoiceId
            //              join m3 in db.ProductUnits on m2.ProductUnitId equals m3.Id
            //              join m4 in db.Products on m3.ProductId equals m4.Id
            //              where m.StoreId == id
            //              select m4).Select(m => m.QTY).DefaultIfEmpty(0).Sum();

            var v = (from m in db.Products
                     join m2 in db.ProductUnits on m.Id equals m2.ProductId
                     join m3 in db.InvoiceLines on m2.Id equals m3.ProductUnitId
                     join m4 in db.Invoices on m3.InvoiceId equals m4.Id
                     where m4.StoreId == id
                     group m4 by m4.Id into g
                     select g.Key).Count();

            return v;
        }

        public Guid Insert(StoreM model)
        {
            var entity = utils.FromModel(model);
            db.Stores.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public void Delete(Guid id)
        {
            var entity = db.Stores.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(Store entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.Stores.Attach(entityToDelete);
            }
            db.Stores.Remove(entityToDelete);
            db.SaveChanges();
        }

        public void Update(StoreM model)
        {
            var entity = utils.FromModel(model);
            var e = db.Stores.Where(m => m.Id == model.Id)
                .AsQueryable().FirstOrDefault();

            db.Entry(e).CurrentValues.SetValues(entity);
            db.SaveChanges();
        }

        public int GetNewNum()
        {
            var num = db.Stores.OrderByDescending(m => m.Num).Select(m => m.Num).FirstOrDefault();
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
