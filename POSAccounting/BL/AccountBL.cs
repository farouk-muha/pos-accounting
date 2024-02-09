using POSAccounting.Models;
using POSAccounting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.BL
{
    public class AccountBL
    {
        private CropAccountingAppEntities db;
        private AccountUtils utils = new AccountUtils();

        public AccountBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public Account GetTrackEntityById(Guid id)
        {            
           return db.Accounts.Where(m => m.Id == id).FirstOrDefault();
        }

        public AccountM GetById(Guid id)
        {
            var entity = db.Accounts.AsNoTracking().Where(m => m.Id == id).FirstOrDefault();
            var model = utils.FromEntity(entity);           
            return model;
        }

        public ObservableCollection<AccountM> GetModels(Expression<Func<Account, bool>> filter)
        {
            var query = from m in db.Accounts select m;
            if (filter != null)
                query = query.Where(filter);

            var entities = query.ToList();
            ObservableCollection<AccountM> models = new ObservableCollection<AccountM>();
            foreach (var v in entities)
                models.Add( utils.FromEntity(v));
            return models;
        }

        public IEnumerable<Account> Get(Expression<Func<Account, bool>> filter)
        {
            var query = from m in db.Accounts select m;
            if (filter != null)
                query = query.Where(filter);
            return query.ToList();
        }

        public AccountM GetModel(Guid id, DateTime startDate, DateTime endDate)
        {
            var list = ConstJournalTypes.GetList().ToList();
            var model = GetById(id);
            if (model == null)
                return new AccountM();

            var v = (from m in db.JournalDets
                     join m2 in db.Journals on m.JournalId equals m2.Id
                     where m.AccountID == id && m2.Date >= startDate && m2.Date < endDate
                     group m by m.AccountID into g
                     select new
                     {
                         Credit = g.Select(m => m.Credit).DefaultIfEmpty(0).Sum(),
                         Debt = g.Select(m => m.Debt).DefaultIfEmpty(0).Sum(),
                     }).FirstOrDefault();

            if (v != null)
                model.Balance = v.Credit - v.Debt;

            return model;
        }

        public List<AccountM> GetModels(DateTime? startDate, DateTime? endDate)
        {
            var year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);
            DateTime lastDay = new DateTime(year, 12, 31);
            if (startDate != null && endDate != null)
            {
                firstDay = (DateTime)startDate;
                lastDay = (DateTime)endDate;
            }

            var list = ConstJournalTypes.GetList().ToList();
            var lang = AppSettings.GetLang();

            var models = (from m in db.Accounts
                          join m2 in db.JournalDets on m.Id equals m2.AccountID into j1
                          from j2 in j1.DefaultIfEmpty()
                          join m3 in db.Journals on j2.JournalId equals m3.Id into j3
                          from j4 in j3.DefaultIfEmpty()
                          where j4.Date == null || (j4.Date >= firstDay && j4.Date < lastDay)
                          group new { m, j2 } by m.Id into g
                          select new AccountM
                          {
                              Id = g.Select(m => m.m.Id).FirstOrDefault(),
                              Num = g.Select(m => m.m.Num).FirstOrDefault(),
                              EnName = lang == ConstLang.En ? g.Select(m => m.m.EnName).FirstOrDefault() :
                              g.Select(m => m.m.ArName).FirstOrDefault(),
                              ArName = g.Select(m => m.m.ArName).FirstOrDefault(),
                              AccountEndId = g.Select(m => m.m.AccountEndId).FirstOrDefault() != null ?
                              (int)g.Select(m => m.m.AccountEndId).FirstOrDefault() : 0,
                              AccountParentId = g.Select(m => m.m.AccountParentId).FirstOrDefault(),
                              CropId = g.Select(m => m.m.CropId).FirstOrDefault(),
                              Deleteable = g.Select(m => m.m.Deleteable).FirstOrDefault(),
                              UserId = g.Select(m => m.m.UserId).FirstOrDefault(),

                              Credit = g.Where(m => m.j2 != null).Select(m => m.j2.Credit).DefaultIfEmpty(0).Sum(),
                              Debt = g.Where(m => m.j2 != null).Select(m => m.j2.Debt).DefaultIfEmpty(0).Sum(),
                          }).OrderBy(m => m.Num).ToList();

            return models;
        }

        public List<AccountM> GetModels(Guid[] ids, DateTime? startDate, DateTime? endDate)
        {
            var year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);
            DateTime lastDay = new DateTime(year, 12, 31);
            if (startDate != null && endDate != null)
            {
                firstDay = (DateTime)startDate;
                lastDay = (DateTime)endDate;
            }

            var list = ConstJournalTypes.GetList().ToList();
            var lang = AppSettings.GetLang();

            var models = (from m in db.Accounts
                          join m2 in db.JournalDets on m.Id equals m2.AccountID into j1
                          from j2 in j1.DefaultIfEmpty()
                          join m3 in db.Journals on j2.JournalId equals m3.Id into j3
                          from j4 in j3.DefaultIfEmpty()
                          where ids.Contains(m.Id) && (j4.Date == null || (j4.Date >= firstDay && j4.Date < lastDay))
                          group new { m, j2 } by m.Id into g
                          select new AccountM
                          {
                              Id = g.Select(m => m.m.Id).FirstOrDefault(),
                              Num = g.Select(m => m.m.Num).FirstOrDefault(),
                              EnName = lang == ConstLang.En ? g.Select(m => m.m.EnName).FirstOrDefault() :
                              g.Select(m => m.m.ArName).FirstOrDefault(),
                              ArName = g.Select(m => m.m.ArName).FirstOrDefault(),
                              AccountEndId = g.Select(m => m.m.AccountEndId).FirstOrDefault() != null ?
                              (int)g.Select(m => m.m.AccountEndId).FirstOrDefault() : 0,
                              AccountParentId = g.Select(m => m.m.AccountParentId).FirstOrDefault(),
                              CropId = g.Select(m => m.m.CropId).FirstOrDefault(),
                              Deleteable = g.Select(m => m.m.Deleteable).FirstOrDefault(),
                              UserId = g.Select(m => m.m.UserId).FirstOrDefault(),

                              Credit = g.Where(m => m.j2 != null).Select(m => m.j2.Credit).DefaultIfEmpty(0).Sum(),
                              Debt = g.Where(m => m.j2 != null).Select(m => m.j2.Debt).DefaultIfEmpty(0).Sum(),
                          }).OrderBy(m => m.Num).ToList();

            return models;
        }
        //public IEnumerable<Account> GetByParentId(Guid id)
        //{
        //    SqlParameter para = new SqlParameter("@id", id);
        //    return db.Database.SqlQuery<Account>("sp_AccountsByParentId @id", para).ToList();
        //}

        public PaginatedList<AccountM> GetTree(List<AccountM> models, List<AccountM> mainIds)
        {

            var query = from m in db.Accounts select m;


            PaginatedList<AccountM> model = new PaginatedList<AccountM>();
            model.Models = new ObservableCollection<AccountM>();

            foreach (var v in models)
            {
                model.Count++;

                if (!mainIds.Exists(m => m.Id == v.Id)) continue;
                AccountM curAcc = new AccountM { Id = v.Id, Credit = v.Credit };

                if (curAcc.Credit > 0)
                    v.Balance = v.Credit - v.Debt;
                else
                    v.Balance = v.Debt - v.Credit;

                model.Models.Add(v);
                v.Accounts = new ObservableCollection<AccountM>();

                foreach (var v1 in models)
                {
                    if (v1.AccountParentId == v.Id)
                    {
                        v.Accounts.Add(v1);
                        v.Credit += v1.Credit;
                        v.Debt += v1.Debt;
                        if (curAcc.Credit > 0)
                        {
                            v1.Balance = v1.Credit - v1.Debt;
                            v.Balance = v.Credit - v.Debt;
                        }
                        else
                        {
                            v1.Balance = v1.Debt - v1.Credit;
                            v.Balance = v.Debt - v.Credit;
                        }

                        v1.Accounts = new ObservableCollection<AccountM>();
                        foreach (var v2 in models)
                        {
                            if (v2.AccountParentId == v1.Id)
                            {
                                v1.Accounts.Add(v2);
                                v1.Credit += v2.Credit;
                                v1.Debt += v2.Debt;
                                v.Credit += v2.Credit;
                                v.Debt += v2.Debt;
                                if (curAcc.Credit > 0)
                                {
                                    v2.Balance = v2.Credit - v2.Debt;
                                    v1.Balance = v1.Credit - v1.Debt;
                                    v.Balance = v.Credit - v.Debt;
                                }
                                else
                                {
                                    v2.Balance = v2.Debt - v2.Credit;
                                    v1.Balance = v1.Debt - v1.Credit;
                                    v.Balance = v.Debt - v.Credit;
                                }

                                v2.Accounts = new ObservableCollection<AccountM>();
                                foreach (var v3 in models)
                                {
                                    if (v3.AccountParentId == v2.Id)
                                    {
                                        v2.Accounts.Add(v3);
                                        v2.Credit += v3.Credit;
                                        v2.Debt += v3.Debt;
                                        v1.Credit += v3.Credit;
                                        v1.Debt += v3.Debt;
                                        v.Credit += v3.Credit;
                                        v.Debt += v3.Debt;

                                        if (curAcc.Credit > 0)
                                        {
                                            v3.Balance = v3.Credit - v3.Debt;
                                            v2.Balance = v2.Credit - v2.Debt;
                                            v1.Balance = v1.Credit - v1.Debt;
                                            v.Balance = v.Credit - v.Debt;
                                        }
                                        else
                                        {
                                            v3.Balance = v3.Debt - v3.Credit;
                                            v2.Balance = v2.Debt - v2.Credit;
                                            v1.Balance = v1.Debt - v1.Credit;
                                            v.Balance = v.Debt - v.Credit;
                                        }
                                        v3.Accounts = new ObservableCollection<AccountM>();
                                        foreach (var v4 in models)
                                        {
                                            if (v4.AccountParentId == v3.Id)
                                            {
                                                v3.Accounts.Add(v4);
                                                v3.Credit += v4.Credit;
                                                v3.Debt += v4.Debt;
                                                v2.Credit += v4.Credit;
                                                v2.Debt += v4.Debt;
                                                v1.Credit += v4.Credit;
                                                v1.Debt += v4.Debt;
                                                v.Credit += v4.Credit;
                                                v.Debt += v4.Debt;
                                                if (curAcc.Credit > 0)
                                                {
                                                    v4.Balance = v4.Credit - v4.Debt;
                                                    v3.Balance = v3.Credit - v3.Debt;
                                                    v2.Balance = v2.Credit - v2.Debt;
                                                    v1.Balance = v1.Credit - v1.Debt;
                                                    v.Balance = v.Credit - v.Debt;
                                                }
                                                else
                                                {
                                                    v4.Balance = v4.Debt - v4.Credit;
                                                    v3.Balance = v3.Debt - v3.Credit;
                                                    v2.Balance = v2.Debt - v2.Credit;
                                                    v1.Balance = v1.Debt - v1.Credit;
                                                    v.Balance = v.Debt - v.Credit;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return model;
        }


        public ObservableCollection<AccountM> GetVisas()
        {
            var p = from m in db.Accounts
                    where m.AccountParentId == ConstAccounts.Bank
                    select m;

            var query = from m in db.Accounts
                        where p.Any(x => x.Id == m.AccountParentId)
                        select m;

            var entities = query.OrderBy(m => m.Num).ToList();

            ObservableCollection<AccountM> models = new ObservableCollection<AccountM>();
            foreach (var v in entities)
                models.Add(utils.FromEntity(v));

            return models;
        }
        public PaginatedList<AccountM> GetTreeVisas()
        {                   
            var p = from m in db.Accounts
                    where m.AccountParentId == ConstAccounts.Bank
                    select m;

            var entities = p.OrderBy(m => m.Num).ToList();

            var query = from m in db.Accounts
                        where p.Any(x => x.Id == m.AccountParentId)
                        select m;

            entities.AddRange(query.OrderBy(m => m.Num).ToList());

            ObservableCollection<AccountM> temp = new ObservableCollection<AccountM>();
            PaginatedList<AccountM> model = new PaginatedList<AccountM>();
            model.Models = new ObservableCollection<AccountM>();
            foreach (var v in entities)
                temp.Add(utils.FromEntity(v));

            DateTime now = DateTime.Now;
            var ids = entities.Select(m => m.Id).ToList();
            var alldets = new JournalDetBL(db).Get(m => m.Journal.Date.Year == now.Year 
            && ids.Any(x => x == m.Account.AccountParentId));

            model.Credit = alldets.Select(m => m.Credit).DefaultIfEmpty(0).Sum();
            model.Debt = alldets.Select(m => m.Debt).DefaultIfEmpty(0).Sum();
            model.Amount = model.Credit - model.Debt;
            foreach (var v in temp)
            {
                model.Count++;
                v.Credit = alldets.Where(m => m.AccountID == v.Id).Select(m => m.Credit).DefaultIfEmpty(0).Sum();
                v.Debt = alldets.Where(m => m.AccountID == v.Id).Select(m => m.Debt).DefaultIfEmpty(0).Sum();

                if (v.AccountParentId != ConstAccounts.Bank) continue;

                model.Models.Add(v);
                v.Accounts = new ObservableCollection<AccountM>();
                foreach (var vv in temp)
                {
                    if (vv.AccountParentId == v.Id)
                    {
                        v.Credit += vv.Credit;
                        v.Debt += vv.Debt;
                        v.Accounts.Add(vv);
                    }
                }
            }

            return model;
        }

        public AccountM GetTranss(Guid id, DateTime startDate, DateTime endDate)
        {
            var list = ConstJournalTypes.GetList().ToList();
            AccountM model = GetById(id);

            model.Journals = (from m in db.JournalDets
                         join m2 in db.Journals on m.JournalId equals m2.Id
                              where m.AccountID == id && m2.Date >= startDate && m2.Date < endDate
                              select new JournalM
                         {
                             Date = m2.Date,
                             Num = m2.Num,
                             Credit = m.Credit,
                             Debt = m.Debt,
                             Details = m2.Details,
                             JournalTypeId = m2.JournalTypeId,
                         }).ToList();


            decimal d = 0, c = 0;
            foreach(var v in model.Journals)
            {
                c += v.Credit;
                d += v.Debt;
                v.Details = v.Details != null ? v.Details : list.Where(m => m.Id == v.JournalTypeId)
                    .Select(m => m.Name).FirstOrDefault();
            }
            model.Balance = c - d;
            return model;
        }

        //reports 

        public Guid[] GetChildeGuids(Nullable<Guid>[] ids)
        {
            return (from m in db.Accounts
                     where ids.Contains(m.AccountParentId)
                     select m.Id).ToArray();
            
        }
        

        public Guid Insert(AccountM model)
        {
            var entity = utils.FromModel(model);
            db.Accounts.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public void Delete(Guid id)
        {
            var entity = db.Accounts.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(Account entityToDelete)
        {
            if (!entityToDelete.Deleteable)
                throw new Exception(); ;

            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.Accounts.Attach(entityToDelete);
            }
            db.Accounts.Remove(entityToDelete);
            db.SaveChanges();
        }

        public void Update(AccountM model)
        { 
            var entity = utils.FromModel(model);
            var e = db.Accounts.Where(m => m.Id == model.Id)
                .AsQueryable().FirstOrDefault();

            db.Entry(e).CurrentValues.SetValues(entity);
            db.SaveChanges();
        }

        public int GetNewNum(Guid id, int num)
        {
            var t = db.Accounts.Where(m => m.AccountParentId == id).OrderByDescending(m => m.Num).FirstOrDefault();
            int n = t != null? t.Num : 0;

            if (n == 0)
            {
                string s = num.ToString() + "1";
                n = int.Parse(s);
            }
            else
                ++n;
            return n;
        }

    }


}
