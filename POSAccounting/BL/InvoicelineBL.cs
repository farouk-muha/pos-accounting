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
    public class InvoicelineBL
    {
        private CropAccountingAppEntities db;

        public InvoicelineBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public List<InvoiceLine> Get(Expression<Func<InvoiceLine, bool>> filter)
        {
            var query = db.InvoiceLines.Include(m => m.ProductUnit);

            if (filter != null)
              query = query.Where(filter);
            return query.ToList();

        }

        public PaginatedList<InvoiceLineM> Get(Expression<Func<InvoiceLine, bool>> filter, int? sortOrder, int page, int itemsForPage)
        {
            var query = db.InvoiceLines.Include(m => m.ProductUnit);

            if (filter != null)
                query = query.Where(filter);

            var entities = query.AsNoTracking().OrderBy(m => m.Id).Skip((page - 1) * itemsForPage).Take(itemsForPage).ToList();

            PaginatedList<InvoiceLineM> models = new PaginatedList<InvoiceLineM>();
            models.Count = query.Count();
            models.Models = new ObservableCollection<InvoiceLineM>();
            foreach (var v in entities)
            {
                var model = InvoiceLineUtils.FromEntity(v);
                model.ProductUnit = ProductUnitUtils.FromEntity(v.ProductUnit);                
                models.Models.Add(model);
            }
           
            return models;
        }

        public PaginatedList<InvoiceLineM> Get(int? sortOrder, Guid keyWord, int? page, int itemsForPage)
        {
            if (page == 0)
                page = 1;

            Expression<Func<InvoiceLine, bool>> filter = null;

            if (keyWord != null)
                filter = m => m.Id == keyWord;

            return Get(filter, sortOrder, (int)page, itemsForPage);
        }

        public void Insert(InvoiceLineM model)
        {
            var entity = InvoiceLineUtils.FromModel(model);
            db.InvoiceLines.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var entity = db.InvoiceLines.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(InvoiceLine entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.InvoiceLines.Attach(entityToDelete);
            }
            db.InvoiceLines.Remove(entityToDelete);
            db.SaveChanges();
        }

    }
}
