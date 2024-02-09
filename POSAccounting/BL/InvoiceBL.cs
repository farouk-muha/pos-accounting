using POSAccounting.Models;
using POSAccounting.Utils;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace POSAccounting.BL
{
    public class InvoiceBL
    {
        private CropAccountingAppEntities db;
        private InvoiceUtils utils = new InvoiceUtils();
        public InvoiceBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public InvoiceM GetById(Guid id)
        {
            var entity = db.Invoices.Where(m => m.Id == id).Include(m => m.InvoiceLines)
                .AsNoTracking().FirstOrDefault();

            var model = utils.FromEntity(entity);
            if (model == null)
                return model;

            model.InvoiceLines = new ObservableCollection<InvoiceLineM>();
            foreach (var vv in entity.InvoiceLines)
            {
                model.InvoiceLines.Add(InvoiceLineUtils.FromEntity(vv));
            }

            model.TotalQty = model.InvoiceLines.Sum(m => m.QTY);
            if (model.AccountId != null)
            {
                model.Client =
                    new ClientUtils().FromEntity(db.Clients.Where(m => m.AccountId == model.AccountId).FirstOrDefault());
                model.ClientName = model.Client.Name;
                model.ClientPhone = model.Client.Phone;
            }

            model.Debt = model.Amount - model.Payed;

            model.InvoiceLines = new ObservableCollection<InvoiceLineM>();
            ProductBL pBL = new ProductBL(db);
            foreach (var v in entity.InvoiceLines)
            {
                var temp = InvoiceLineUtils.FromEntity(v);
                temp.ProductUnit = ProductUnitUtils.FromEntity(db.ProductUnits.Where(m => m.Id == temp.ProductUnitId).FirstOrDefault());
                temp.Product = pBL.GetById(temp.ProductUnit.ProductId);
                temp.TotalPrice = temp.QTY * temp.Price;
                model.InvoiceLines.Add(temp);
            }
            return model;
        }

        public List<InvoiceM> Get(Expression<Func<Invoice, bool>> filter)
        {
           
            var query = db.Invoices.Include(m => m.InvoiceLines).AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            var q = query.AsNoTracking().ToList();
            List<InvoiceM> models = new List<InvoiceM>();
            foreach (var v in q)
            {
                var temp = utils.FromEntity(v);
                temp.InvoiceLines = new ObservableCollection<InvoiceLineM>();
                foreach (var vv in v.InvoiceLines)
                {
                    temp.InvoiceLines.Add(InvoiceLineUtils.FromEntity(vv));
                }
                models.Add(temp);
            }

            return models;
        }

        public PaginatedList<InvoiceM> Get(Expression<Func<Invoice, bool>> filter, int? sortOrder, int page, 
            int pageSize)
        {
            db.Configuration.LazyLoadingEnabled = true;
            var query = db.Invoices.AsNoTracking().AsQueryable();


            if (filter != null)
                query =  query.Where(filter);

            PaginatedList<InvoiceM> model = new PaginatedList<InvoiceM>();
            model.Models = new ObservableCollection<InvoiceM>();

            var q = query.GroupBy(m => 1).Select(g => new {
                sum = g.Select(m => m.Amount).DefaultIfEmpty(0).Sum(),
                paid = g.Select(m => m.Payed).DefaultIfEmpty(0).Sum(),
                paidByRec = 
                g.Where(m => !m.IsPayed && !m.IsRemind).Select(m => (m.Amount - m.Payed)).DefaultIfEmpty(0).Sum(),
                count = g.Count(),
            }).FirstOrDefault();
            if (q != null)
            {
                model.Count = q.count;
                model.Amount = q.sum;
                var p = q.paid != null ? (decimal)q.paid : 0;
                var d = q.paidByRec != null ? (decimal)q.paidByRec : 0;
                model.Credit = p;
                model.Debt = q.sum - p - d;
            }

            List<Invoice> entities ;
            if(pageSize > 0)
                entities = query.OrderBy(m => m.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            else
                entities = query.OrderBy(m => m.Id).ToList();

            foreach (var v in entities)
            {
                var temp = utils.FromEntity(v);
                temp.InvoiceLines = new ObservableCollection<InvoiceLineM>();
                foreach(var vv in v.InvoiceLines)
                {
                    temp.InvoiceLines.Add(InvoiceLineUtils.FromEntity(vv));
                }

                temp.TotalQty = temp.InvoiceLines.Sum(m => m.QTY);
                if (temp.AccountId != null)
                {
                    var lang = AppSettings.GetLang();
                    if (lang == ConstLang.En)
                        temp.ClientName = db.Accounts.Where(m => m.Id == temp.AccountId).Select(m => m.EnName).FirstOrDefault();
                    else
                        temp.ClientName = db.Accounts.Where(m => m.Id == temp.AccountId).Select(m => m.ArName).FirstOrDefault();

                }
                else if (string.IsNullOrWhiteSpace(temp.ClientName))
                {
                    if (temp.InvoiceTypeId == ConstInvoiceType.Purchases || temp.InvoiceTypeId == ConstInvoiceType.PurchasesReturns)
                        temp.ClientName = Properties.Resources.Supplier;
                    else
                        temp.ClientName = Properties.Resources.Customer;
                }

                temp.Debt = temp.Amount - (decimal)temp.Payed;
                model.Models.Add(temp);

            }
            db.Configuration.LazyLoadingEnabled = false;
            return model;
        }

        public PaginatedList<InvoiceM> Get(int? sortOrder, Nullable<Guid> accId, byte typeId,
            string kw, DateTime? startDate, DateTime? endDate, bool status, int page, int pageSize, bool? isaccClientDebt)
        {
            if (page == 0)
                page = 1;

            Expression<Func<Invoice, bool>> filter = null;
            Expression<Func<Invoice, bool>> filter2 = null;
            Expression<Func<Invoice, bool>> filter3 = null;
            Expression<Func<Invoice, bool>> filter4 = null;

            if(isaccClientDebt != null)
            {
                if ((bool)isaccClientDebt)
                    filter = m => m.InvoiceTypeId == ConstInvoiceType.Sales || m.InvoiceTypeId == ConstInvoiceType.PurchasesReturns;
                else
                    filter = m => m.InvoiceTypeId == ConstInvoiceType.Purchases || m.InvoiceTypeId == ConstInvoiceType.SalesReturns;
            }
            else if (typeId > 0)
                filter = m => m.InvoiceTypeId == typeId;

            if (accId != null)
                filter2 = m => m.AccountId == accId;
            if (kw != null)
            {
                int i = 0; int.TryParse(kw, out i);
                if (i > 0)
                    filter3 = m => m.Num == i;
                else
                    filter3 = m => m.Account.EnName.Equals(kw);
            }

            if (startDate != null && endDate != null)
            {
                if(status)
                filter4 = m => m.IsRemind && m.Date >= startDate && m.Date < endDate;
                else
                    filter4 = m => m.Date >= startDate && m.Date < endDate;
            }
            else
            {
                var year = DateTime.Now.Year;
                if (status)
                    filter4 = m => m.IsRemind && m.Date.Year == year;
                else
                    filter4 = m => m.Date.Year == year;
            }


            if (filter2 != null)
            {
                if (filter == null)
                    filter = filter2;
                else
                    filter = ReplaceExpressionVisitor.AndAlso(filter, filter2);
            }
            if (filter3 != null)
            {
                if (filter == null)
                    filter = filter3;
                else
                    filter = ReplaceExpressionVisitor.AndAlso(filter, filter3);
            }
            if (filter4 != null)
            {
                if (filter == null)
                    filter = filter4;
                else
                    filter = ReplaceExpressionVisitor.AndAlso(filter, filter4);
            }

            return Get(filter, sortOrder, page, pageSize);
        }

        public ObservableCollection<InvoiceM> GetNotPayed(JournalBL journallBL, Nullable<Guid> accClientId,
            bool isaccClientDebt, bool isDate)
        {
            IQueryable<Invoice> query;

            if(!isDate)
            {
                query = from m in db.Invoices
                        where m.IsRemind && m.AccountId == accClientId
                        select m;
            }
            else
            {
                var date = DateTime.Now.AddDays(-AppSettings.GetInvoiceReminder());
                if(accClientId != null)
                {
                    query = from m in db.Invoices
                            where  m.IsRemind && m.AccountId == accClientId && m.Date <= date
                            select m;
                }else
                {
                    query = from m in db.Invoices
                            where m.IsRemind && m.Date <= date
                            select m;
                }
            }

            if (isaccClientDebt)
                query = query.Where(m => m.InvoiceTypeId == ConstInvoiceType.Sales || m.InvoiceTypeId == ConstInvoiceType.PurchasesReturns);
            else
                query = query.Where(m => m.InvoiceTypeId == ConstInvoiceType.Purchases || m.InvoiceTypeId == ConstInvoiceType.SalesReturns);

            ObservableCollection<InvoiceM> models = new ObservableCollection<InvoiceM>();
            foreach (var v in query)
            {
                var temp = utils.FromEntity(v);

                decimal a = temp.Amount;
                if (temp.IsPayed)
                {
                    if (temp.Discount > 0)
                    {
                        a = temp.Amount - (decimal)temp.Discount;
                        temp.Payed = a;
                    }
                    else
                        temp.Payed = a;
                }

                temp.Debt = a - (decimal)temp.Payed;
                models.Add(temp);
            }
            return models;
        }

        public List<SeriesCollection> GetChart(int year)
        {
            var q = from m in db.Invoices
                    where m.Date.Year == year
                    group m by m.InvoiceTypeId into g
                    select new
                    {
                        Id = g.Key,
                        Sum = g.Select(m => m.Amount).DefaultIfEmpty(0).Sum(),
                        Date = g.FirstOrDefault().Date,
                   };

            var entities = q.ToList();

            List<SeriesCollection> models = new List<SeriesCollection>();

            var sales = entities.Where(m => m.Date == DateTime.Today && m.Id == ConstInvoiceType.Sales).FirstOrDefault();
            var pur = entities.Where(m => m.Date == DateTime.Today && m.Id == ConstInvoiceType.Purchases).FirstOrDefault();
            var retSales = entities.Where(m => m.Date == DateTime.Today && m.Id == ConstInvoiceType.SalesReturns).FirstOrDefault();
            var retPur = entities.Where(m => m.Date == DateTime.Today && m.Id == ConstInvoiceType.PurchasesReturns).FirstOrDefault();

            for (int i = 0; i < 3; i++)
            {

                 if (i == 1)
                {
                    DateTime date = DateTime.Today;
                    DateTime start = date.Date.AddDays(-(int)date.DayOfWeek),
                        end = start.AddDays(7);
                    sales = entities.Where(m => m.Date >= start && m.Date < end && m.Id == ConstInvoiceType.Sales).FirstOrDefault();
                    pur = entities.Where(m => m.Date >= start && m.Date < end && m.Id == ConstInvoiceType.Purchases).FirstOrDefault();
                    retSales = entities.Where(m => m.Date >= start && m.Date < end && m.Id == ConstInvoiceType.SalesReturns).FirstOrDefault();
                    retPur = entities.Where(m => m.Date >= start && m.Date < end && m.Id == ConstInvoiceType.PurchasesReturns).FirstOrDefault();
                }
                else if (i == 2)
                {
                    DateTime date = DateTime.Now;
                    sales = entities.Where(m => m.Date.Month == date.Month && m.Date.Year == date.Year && m.Id == ConstInvoiceType.Sales).FirstOrDefault();
                    pur = entities.Where(m => m.Date.Month == date.Month && m.Date.Year == date.Year && m.Id == ConstInvoiceType.Purchases).FirstOrDefault();
                    retSales = entities.Where(m => m.Date.Month == date.Month && m.Date.Year == date.Year && m.Id == ConstInvoiceType.SalesReturns).FirstOrDefault();
                    retPur = entities.Where(m => m.Date.Month == date.Month && m.Date.Year == date.Year && m.Id == ConstInvoiceType.PurchasesReturns).FirstOrDefault();
                }

                SeriesCollection model = new SeriesCollection();

                if (sales != null)
                {
                    model.Add(new PieSeries
                    {
                        Title = ConstInvoiceType.GetList().Where(m => m.Id == sales.Id).Select(m => m.Name).FirstOrDefault(),
                        Values = new ChartValues<ObservableValue> { new ObservableValue((double)sales.Sum) },
                        Fill = Application.Current.Resources["GoogleBlueBrush"] as SolidColorBrush,
                    });
                }

                if (pur != null)
                {
                    model.Add(new PieSeries
                    {
                        Title = ConstInvoiceType.GetList().Where(m => m.Id == pur.Id).Select(m => m.Name).FirstOrDefault(),
                        Values = new ChartValues<ObservableValue> { new ObservableValue((double)pur.Sum) },
                        Fill = Application.Current.Resources["GoogleRedBrush"] as SolidColorBrush,
                    });
                }

                if (retSales != null)
                {
                    model.Add(new PieSeries
                    {
                        Title = ConstInvoiceType.GetList().Where(m => m.Id == retSales.Id).Select(m => m.Name).FirstOrDefault(),
                        Values = new ChartValues<ObservableValue> { new ObservableValue((double)retSales.Sum) },
                        Fill = Application.Current.Resources["GoogleOrangeBrush"] as SolidColorBrush,
                    });
                }

                if (retPur != null)
                {
                    model.Add(new PieSeries
                    {
                        Title = ConstInvoiceType.GetList().Where(m => m.Id == retPur.Id).Select(m => m.Name).FirstOrDefault(),
                        Values = new ChartValues<ObservableValue> { new ObservableValue((double)retPur.Sum) },
                        Fill = Application.Current.Resources["GoogleNavyBrush"] as SolidColorBrush,
                    });
                }
                models.Add(model);
            }

            return models;
        }
        public SeriesCollection GetSeries(DateTime date)
        {
            var q = from m in db.Invoices
                    where m.Date >= date
                    select new
                    {
                        Id = m.InvoiceTypeId,
                        Value = m.Amount,
                        Date = m.Date,
                    };

            var entities = q.ToList();
            var sales = entities.Where(m => m.Id == ConstInvoiceType.Sales).OrderBy(m => m.Date).ToList();
            //var pur = entities.Where(m => m.Id == ConstInvoiceType.Purchases).OrderBy(m => m.Date).ToList();
            //var retSales = entities.Where(m => m.Id == ConstInvoiceType.SalesReturns).OrderBy(m => m.Date).ToList();
            //var retPur = entities.Where(m => m.Id == ConstInvoiceType.PurchasesReturns).OrderBy(m => m.Date).ToList();

            var dayConfig = Mappers.Xy<DateModel>()
             .X(dayModel => dayModel.DateTime.Ticks)
                           .Y(dayModel => dayModel.Value);

            SeriesCollection models = new SeriesCollection(dayConfig);
            ChartValues<DateModel> line = new ChartValues<DateModel>();
            foreach (var v in sales)
            {
                line.Add(new DateModel(v.Date, (double)v.Value ));
            }
            models.Add(new LineSeries
            {
                Title = Properties.Resources.Sales,
                Values = line,
            });

            //line = new ChartValues<DateModel>();
            //foreach (var v in pur)
            //{
            //    line.Add(new DateModel(v.Date, (double)v.Value));
            //}
            //models.Add(new LineSeries
            //{
            //    Title = "Purchases",
            //    Values = line,
            //});

            //line = new ChartValues<DateModel>();
            //foreach (var v in retSales)
            //{
            //    line.Add(new DateModel(v.Date, (double)v.Value));
            //}
            //models.Add(new LineSeries
            //{
            //    Title = "Return Sales",
            //    Values = line,
            //});

            //line = new ChartValues<DateModel>();
            //foreach (var v in retPur)
            //{
            //    line.Add(new DateModel(v.Date, (double)v.Value));
            //}
            //models.Add(new LineSeries
            //{
            //    Title = "Return Purchases",
            //    Values = line,
            //});



            return models;
        }

        //public decimal GetInvoiceAmountPayed(Guid id, Guid clientAccId, bool debt)
        //{
        //    var query = from m in db.Journals
        //                join jd in db.JournalDets on m.Id equals jd.JournalId
        //                where m.RefId == id
        //                group m by m.Id into g
        //                select g.FirstOrDefault();

        //    if (query == null || query.Count() == 0)
        //        return 0;

        //    decimal d = 0;
        //    if (debt)
        //        d = query.FirstOrDefault().JournalDets.Where(m => m.AccountID == clientAccId).Select(m => m.Debt).DefaultIfEmpty(0).Sum();
        //    else
        //        d = query.FirstOrDefault().JournalDets.Where(m => m.AccountID == clientAccId).Select(m => m.Credit).DefaultIfEmpty(0).Sum();
        //    return d;
        //}

        public int GetCount(Expression<Func<Invoice, bool>> filter)
        {
            var query = from m in db.Invoices select m;

            if (filter != null)
                query = query.Where(filter);

            return query.Count();
        }

        public void Insert(InvoiceM model)
        {
            var entity = utils.FromModel(model);
            db.Invoices.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var entity = db.Invoices.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(Invoice entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.Invoices.Attach(entityToDelete);
            }
            db.Invoices.Remove(entityToDelete);
            db.SaveChanges();
        }

        public void Update(InvoiceM model)
        {
            var entityToUpdate = utils.FromModel(model);
            var entity = db.Invoices.Where(m => m.Id == model.Id).AsQueryable().FirstOrDefault();
            db.Entry(entity).CurrentValues.SetValues(entityToUpdate);
            db.SaveChanges();
        }

        public void UpdatePayStatus(Guid id, bool status)
        {
            var entity = db.Invoices.Where(m => m.Id == id).FirstOrDefault();
            entity.IsRemind = status;
            db.SaveChanges();
        }

        public int GetNewNum()
        {
            var num = db.Invoices.OrderByDescending(m => m.Num).Select(m => m.Num).FirstOrDefault();
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

    public class DateModel
    {
        public DateModel()
        {

        }

        public DateModel(DateTime datetime, double value)
        {
            this.DateTime = datetime;
            this.Value = value;
        }

        public System.DateTime DateTime { get; set; }
        public double Value { get; set; }
    }
}
