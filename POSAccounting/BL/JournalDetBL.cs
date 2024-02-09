using POSAccounting.Models;
using POSAccounting.Utils;
using LiveCharts;
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
    public class JournalDetBL
    {
        private CropAccountingAppEntities db;
        private AccountUtils accUtils = new AccountUtils();

        public JournalDetBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public List<JournalDet> Get(Expression<Func<JournalDet, bool>> filter)
        {
            var query = db.JournalDets.AsNoTracking().AsQueryable();

            if (filter != null)
               query =  query.Where(filter);
            var v = query.ToList();
            return v;

        }


        public PaginatedList<JournalM> Get(Nullable<Guid> id = null, DateTime? startDate = null, DateTime? endDate = null
            , int page = 0, int pageSize = 0)
        {
            if (page == 0)
                page = 1;

            var list = ConstJournalTypes.GetList().ToList();
            PaginatedList<JournalM> model = new PaginatedList<JournalM>();

            var query = (from m in db.JournalDets
                         join m2 in db.Journals on m.JournalId equals m2.Id
                         select new
                         {
                            m = m,
                            m2 =m2,
                         }).AsQueryable();

            if (startDate != null && endDate != null)
            {
                if (id == null)
                    query = query.Where(m => m.m2.Date >= startDate && m.m2.Date < endDate);
                else
                    query = query.Where(m => m.m.AccountID == id &&
                    m.m2.Date >= startDate && m.m2.Date < endDate);
            }
            else
            {
                var year = DateTime.Now.Year;
                query = query.Where(m => m.m2.Date.Year == year);

                if (id == null)
                    query = query.Where(m => m.m2.Date.Year == year);
                else
                    query = query.Where(m => m.m.AccountID == id &&
                    m.m2.Date.Year == year);
            }

            model.Models = new ObservableCollection<JournalM>();
            model.Count = query.Count();
            List<JournalM> entities;

            if (pageSize > 0)
                entities = query.OrderBy(m => m.m.Id).Skip((page - 1) * pageSize).Take(pageSize).
                          Select( m=> new JournalM
                          {
                              Date = m.m2.Date,
                              Num = m.m2.Num,
                              Credit = m.m.Credit,
                              Debt = m.m.Debt,
                              Details = m.m2.Details,
                              JournalTypeId = m.m2.JournalTypeId,
                          }).ToList();
            else
                entities = query.OrderBy(m => m.m.Id).Select(m => new JournalM
                {
                    Date = m.m2.Date,
                    Num = m.m2.Num,
                    Credit = m.m.Credit,
                    Debt = m.m.Debt,
                    Details = m.m2.Details,
                    JournalTypeId = m.m2.JournalTypeId,
                }).ToList(); ;

            foreach (var v in entities)
            {
                model.Credit += v.Credit;
                model.Debt += v.Debt;
                v.Details = v.Details != null ? v.Details : list.Where(m => m.Id == v.JournalTypeId)
                    .Select(m => m.Name).FirstOrDefault();
                model.Models.Add(v);
            }
            model.Amount = model.Credit - model.Debt;
            return model;
        }


    public List<SeriesCollection> GetChart()
        {
            var p = from m in db.Accounts
                    where m.AccountParentId == ConstAccounts.Bank
                    select m;

            var q = from ac in db.Accounts
                    join m2 in db.JournalDets on ac.Id equals m2.AccountID
                    join m3 in db.Journals on m2.JournalId equals m3.Id
                    where ac.Id == ConstAccounts.Bank || ac.Id == ConstAccounts.BoxSafe
                    || p.Any(x => x.Id == ac.Id) || p.Any(x => x.Id == ac.AccountParentId)
                    group new { ac, m2, m3 } by m2.AccountID into g
                    select new
                    {
                        Id = g.Key,
                        Credit = g.Select(m => m.m2.Credit).DefaultIfEmpty(0).Sum(),
                        Debt = g.Select(m => m.m2.Debt).DefaultIfEmpty(0).Sum(),
                        Date = g.FirstOrDefault().m3.Date,
                    };


            var entities = q.AsNoTracking().ToList();
            List<SeriesCollection> models = new List<SeriesCollection>();
            decimal income = 0;
            decimal exp = 0;
            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                {
                    income = entities.Where(m => m.Date == DateTime.Today).Select(m => m.Debt).DefaultIfEmpty(0).Sum();
                    exp = entities.Where(m => m.Date == DateTime.Today).Select(m => m.Credit).DefaultIfEmpty(0).Sum();
                }
                else if (i == 1)
                {
                    DateTime start = DateTime.Now;
                    while (start.DayOfWeek != DayOfWeek.Saturday)
                        start = start.AddDays(-1);
                     DateTime end = start.AddDays(7);

                    var v = entities.Where(m => m.Date >= start && m.Date < end).ToList();
                    income = entities.Where(m => m.Date >= start && m.Date < end).Select(m => m.Debt).DefaultIfEmpty(0).Sum();
                    exp = entities.Where(m => m.Date >= start && m.Date < end).Select(m => m.Credit).DefaultIfEmpty(0).Sum();
                }
                else if (i == 2)
                {
                    DateTime now = DateTime.Now;
                    income = entities.Where(m => m.Date.Month == now.Month && m.Date.Year == now.Year).Select(m => m.Debt).DefaultIfEmpty(0).Sum();
                    exp = entities.Where(m => m.Date.Month == now.Month && m.Date.Year == now.Year).Select(m => m.Credit).DefaultIfEmpty(0).Sum();
                }


                SeriesCollection s = new SeriesCollection { new PieSeries()
            {
                Title = "Income",
                Values = new ChartValues<ObservableValue> { new ObservableValue((double)income)},
                Fill = Application.Current.Resources["GoogleBlueBrush"] as SolidColorBrush,

            }, new PieSeries()
            {
                Title = "Expenses",
                Values = new ChartValues<ObservableValue> { new ObservableValue((double)exp)},
                 Fill = Application.Current.Resources["GoogleRedBrush"] as SolidColorBrush,
            } };

                models.Add(s);

            }
            return models;
        }


        public void Insert(JournalDetM model)
        {
            var entity = JournalDettUtils.FromModel(model);
            db.JournalDets.Add(entity);
            db.SaveChanges();
        }

        public void UpdateCrAndDe(Guid id, decimal creidt = 0, decimal debit = 0)
        {
            var entity = db.JournalDets.Where(m => m.Id == id).FirstOrDefault();
            if (creidt > 0)
                entity.Credit = creidt;
            else
                entity.Debt = debit;
            db.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var entity = db.JournalDets.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(JournalDet entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.JournalDets.Attach(entityToDelete);
            }
            db.JournalDets.Remove(entityToDelete);
            db.SaveChanges();
        }

        public int GetLastNum()
        {
            var num = db.JournalDets.OrderByDescending(m => m.Num).Select(m => m.Num).FirstOrDefault();
            return num;
        }
        public int GetNewNum(int num)
        {
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
