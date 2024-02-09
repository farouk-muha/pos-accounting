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
    public class ProductBL
    {
        private CropAccountingAppEntities db;
        private ProductUnitUtils unitUtils = new ProductUnitUtils();

        public ProductBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public ProductM GetById(Guid id)
        {
            var entity = db.Products.Where(m => m.Id == id).Include(m => m.ProductUnits).FirstOrDefault();

            var product = ProductUtils.FromEntity(entity);
            product.ProductUnits = new ObservableCollection<ProductUnitM>();

            foreach (var v in entity.ProductUnits)
            {
                var u = ProductUnitUtils.FromEntity(v);
                u.UnitName = db.Units.Where(m => m.Id == u.UnitId).Select(m => m.Name).FirstOrDefault();
                product.ProductUnits.Add(u);
            }

            return product;
        }

        public IEnumerable<Product> Get(Expression<Func<Product, bool>> filter)
        {
            var query = db.Products.Include(m => m.ProductUnits).AsNoTracking();

            if (filter != null)
                query = query.Where(filter);
            return query;
        }

        public PaginatedList<ProductM> Get(Expression<Func<Product, bool>> filter, int? sortOrder, int page, 
            int pageSize)
        {
            var query = Get(filter);

            PaginatedList<ProductM> model = new PaginatedList<ProductM>();
            model.Models = new ObservableCollection<ProductM>();

            //var q = (from m in db.Products
            //         join m2 in db.ProductUnits on m.Id equals m2.ProductId
            //         where m.DefaultUnitId == m2.UnitId
            //         group new { m.QTY, m2.PriceSale } by m.Id into g
            //         select new
            //         {
            //             qty = g.FirstOrDefault().QTY,
            //             price = g.FirstOrDefault().PriceSale,
            //         }).AsEnumerable()
            //          .Select(m =>
            //          new
            //          {
            //              amount = (decimal)m.qty * m.price,
            //          }).ToList();
            //model.Count = q.Count();
            //model.Amount = q.Select(m => m.amount).DefaultIfEmpty(0).Sum();

            model.Count = query.Count();
            var entities = query.OrderBy(m => m.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            foreach (var v in entities)
            {
                var product = ProductUtils.FromEntity(v);
                product.ProductUnit = new ProductUnitM();
                product.ProductUnits = new ObservableCollection<ProductUnitM>();
                var units = db.Units.ToList();
                foreach (var vv in v.ProductUnits)
                {
                    var u = ProductUnitUtils.FromEntity(vv);
                    u.UnitName = units.Where(m => m.Id == u.UnitId).Select(m => m.Name).FirstOrDefault();
                    if (u.UnitId == v.DefaultUnitId)
                    {
                        product.ProductUnit = u.Copy();
                        product.TotalPrice = (decimal)product.QTY * vv.PriceBuy;
                        model.Amount += product.TotalPrice;
                    }
                    product.ProductUnits.Add(u);
                }
                
                model.Models.Add(product);
            }

            return model;
        }

        public PaginatedList<ProductM> Get(int? sortOrder, string keyWord, DateTime? startDate, DateTime? endDate,
            bool? status, int? page, int itemsForPage)
        {
            if (page == 0)
                page = 1;
            if (status == null)
                status = true;

            Expression<Func<Product, bool>> filter = null;

            if (keyWord != null)
                filter = m => m.Status == status && (m.Code.ToUpper().StartsWith(keyWord.Trim().ToUpper()) || m.Name.ToUpper().StartsWith(keyWord.Trim().ToUpper()));
            else
                filter = m => m.Status == status;


            return Get(filter, sortOrder, (int)page, itemsForPage);
        }

        public ObservableCollection<ProductM> GetModels(Expression<Func<Product, bool>> filter)
        {
            var models = new ObservableCollection<ProductM>();

            var model = Get(filter).ToList();
            foreach (var v in model)
            {
                var product = ProductUtils.FromEntity(v);
                product.ProductUnit = new ProductUnitM();
                product.ProductUnits = new ObservableCollection<ProductUnitM>();
                var units = db.Units.ToList();
                foreach (var vv in v.ProductUnits)
                {
                    var u = ProductUnitUtils.FromEntity(vv);
                    u.UnitName = units.Where(m => m.Id == u.UnitId).Select(m => m.Name).FirstOrDefault();
                    product.ProductUnits.Add(u);
                }
                models.Add(product);
            }
            return models;
        }

        public Guid Insert(ProductM model)
        {
            var entity = ProductUtils.FromModel(model);
            db.Products.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        //public void Update(ProductM model)
        //{
        //    Product entity = db.Products.Where(m => m.Id == model.Id).FirstOrDefault();
        //    entity.Status = model.Status;
        //    db.SaveChanges();
        //}

        public void Update(ProductM model)
        {
            var entityToUpdate = ProductUtils.FromModel(model);
            var entity = db.Products.Where(m => m.Id == model.Id).AsQueryable().FirstOrDefault();
            db.Entry(entity).CurrentValues.SetValues(entityToUpdate);
            db.SaveChanges();
        }

        public void UpdateQTY(Guid id, double qty)
        {
            Product entity = db.Products.Where(m => m.Id == id).FirstOrDefault();
            var q = entity.QTY + qty;

            if (q < 0)
                return;

            entity.QTY = q;
            db.SaveChanges();
        }


        public Guid InsertUnitConversion(ProductUnitM model)
        {
            var entity = ProductUtils.FromModel(model);
            entity.Id = Guid.NewGuid();
            db.UnitConversions.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public void Delete(Guid id)
        {
            var entity = db.Products.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(Product entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.Products.Attach(entityToDelete);
            }
            db.Products.Remove(entityToDelete);
            db.SaveChanges();
        }

    }

    public class ProductFun
    {
        public decimal GetPrice(double countTo, double count, decimal price)
        {
            decimal priceTo = price;
            if (countTo == 0 || count == 0 || price == 0)
                return 0;

            if (countTo < count)
                priceTo = price * (decimal)(count / countTo);
            else if (countTo > count)
                priceTo = price / (decimal)(countTo / count);
            return priceTo;
        }

        public double GetQTY(double countTo, double count, double qty)
        {
            double c = qty;
            if (countTo == 0 || count == 0 || qty == 0)
                return 0;

            if (countTo < count)
                c = qty / (count / countTo);
            else if (countTo > count)
                c = qty * (countTo / count);
            return c;
        }
    }
}
