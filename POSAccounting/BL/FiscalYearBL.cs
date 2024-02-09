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
    public class FiscalYearBL
    {
        private CropAccountingAppEntities db;
        private FiscalYearUtils utils = new FiscalYearUtils();

        public FiscalYearBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public FiscalYearM GetById(int id)
        {
            var entity = db.FiscalYears.AsNoTracking().Where(m => m.Id == id).FirstOrDefault();
            return utils.FromEntity(entity);
        }

        public FiscalYearM GetLast()
        {
            var entity = db.FiscalYears.OrderByDescending(m => m.Id).FirstOrDefault();
            return utils.FromEntity(entity);
        }

        public PaginatedList<FiscalYearM> Get(int? sortOrder, Expression<Func<FiscalYear, bool>> filter, int page, int pageSize)
        {
            page = page == 0 ? 1 : page;
            var query = from m in db.FiscalYears select m;
            if (filter != null)
                query = query.Where(filter);

            var entities = query.AsNoTracking().OrderBy(m => m.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PaginatedList<FiscalYearM> model = new PaginatedList<FiscalYearM>();
            model.Count = query.Count();
            model.Models = new ObservableCollection<FiscalYearM>();
            foreach (var v in entities)
            {
                var temp = utils.FromEntity(v);
                model.Models.Add(temp);
            }
            return model;
        }

        public ObservableCollection<FiscalYearM> GetModels(Expression<Func<FiscalYear, bool>> filter)
        {
            var query = from m in db.FiscalYears select m;
            if (filter != null)
                query = query.Where(filter);
            var entities = query.ToList();
            ObservableCollection<FiscalYearM> models = new ObservableCollection<FiscalYearM>();
            foreach (var v in entities)
                models.Add(utils.FromEntity(v));
            return models;
        }

        public int Insert(FiscalYearM model)
        {
            var entity = utils.FromModel(model);
            db.FiscalYears.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public void Delete(int id)
        {
            var entity = db.FiscalYears.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(FiscalYear entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.FiscalYears.Attach(entityToDelete);
            }
            db.FiscalYears.Remove(entityToDelete);
            db.SaveChanges();
        }

        public void Update(FiscalYearM model)
        {
            var entity = utils.FromModel(model);
            var e = db.FiscalYears.Where(m => m.Id == model.Id)
                .AsQueryable().FirstOrDefault();

            db.Entry(e).CurrentValues.SetValues(entity);
            db.SaveChanges();
        }
    }

    public class FiscalYearFun
    {
        private CropAccountingAppEntities db;
        private AccountBL bl;

        public FiscalYearFun(CropAccountingAppEntities db, AccountBL bl)
        {
            this.db = db;
            this.bl = bl;
        }

        public bool IsYearClosed(DateTime date)
        {
            var v = db.FiscalYears.Where(m => m.StartDate == date).FirstOrDefault();
            if (v != null && v.IsClosed)
                return true;
            else
                return false;
        }

        public bool IsYearBeforClosed(DateTime date)
        {
            var v = db.FiscalYears.Where(m => m.StartDate < date).OrderByDescending(m => m.Id)
                .FirstOrDefault();
            if (v == null || (v != null && v.IsClosed))
                return false;
            else
                return false;
        }

        public int CloseAccounts(DateTime date, DateTime start, DateTime end)
        {

            var lastYear = start.AddYears(-1);
            if (!IsYearBeforClosed(lastYear))
                return 1;

            if (IsYearClosed(start))
                return 2;

            JournalBL journalBL = new JournalBL(db);
            JournalDetBL detBL = new JournalDetBL(db);

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    List<AccountM> ids = new List<AccountM>()
                    { new AccountM {Id = ConstAccounts.Revenue, Credit = 1},
                        new AccountM {Id = ConstAccounts.Expenses}};
                    var model = bl.GetModels(start, end);
                    var profitModels = bl.GetTree(model, ids).Models;
                    var lastNum = detBL.GetNewNum(detBL.GetLastNum());
                    var lastNumJournal = journalBL.GetNewNum();

                    //journal Revenue and Expenses
                    JournalM journalRevExp = new JournalM()
                    {
                        Id = Guid.NewGuid(),
                        Num = lastNumJournal,
                        Date = date,
                        JournalTypeId = ConstJournalTypes.AccountClose,
                        UserId = Profile.UserProfile.Id,
                    };
                    journalBL.Insert(journalRevExp);

                    //journal dets Expenses
                    JournalDetM profitLossDeb = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = lastNum,
                        JournalId = journalRevExp.Id,
                        AccountID = ConstAccounts.ProfitLoss,
                        Debt = profitModels[1].Balance,
                    };
                    detBL.Insert(profitLossDeb);

                    JournalDetM tradingExpenses = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = ++lastNum,
                        JournalId = journalRevExp.Id,
                        AccountID = ConstAccounts.TradingExpenses,
                        Credit = profitModels[1].Accounts.Where(m => m.Id == ConstAccounts.TradingExpenses).
                        Select(m => m.Balance).FirstOrDefault(),
                    };
                    detBL.Insert(tradingExpenses);

                    JournalDetM utilitiesExpenses = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = ++lastNum,
                        JournalId = journalRevExp.Id,
                        AccountID = ConstAccounts.UtilitiesExpenses,
                        Credit = profitModels[1].Accounts.Where(m => m.Id == ConstAccounts.UtilitiesExpenses).
                        Select(m => m.Balance).FirstOrDefault(),
                    };
                    detBL.Insert(utilitiesExpenses);

                    //journal dets Revenues
                    JournalDetM sales = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = ++lastNum,
                        JournalId = journalRevExp.Id,
                        AccountID = ConstAccounts.SalesRevenues,
                        Debt = profitModels[0].Accounts.Where(m => m.Id == ConstAccounts.SalesRevenues).
                        Select(m => m.Balance).FirstOrDefault(),
                    };
                    detBL.Insert(sales);

                    JournalDetM otherRevenues = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = ++lastNum,
                        JournalId = journalRevExp.Id,
                        AccountID = ConstAccounts.OtherRevenues,
                        Debt = profitModels[0].Accounts.Where(m => m.Id == ConstAccounts.OtherRevenues).
                        Select(m => m.Balance).FirstOrDefault(),
                    };
                    detBL.Insert(otherRevenues);

                    JournalDetM profitLossCred = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = ++lastNum,
                        JournalId = journalRevExp.Id,
                        AccountID = ConstAccounts.ProfitLoss,
                        Credit = profitModels[0].Balance,
                    };
                    detBL.Insert(profitLossCred);


                    var profit = profitModels[0].Balance - profitModels[1].Balance;

                    //journal PrfotLoss
                    JournalM journalPrfotLoss = new JournalM()
                    {
                        Id = Guid.NewGuid(),
                        Num = ++lastNumJournal,
                        Date = date,
                        JournalTypeId = ConstJournalTypes.AccountClose,
                        UserId = Profile.UserProfile.Id,
                    };
                    journalBL.Insert(journalPrfotLoss);
                    //journal det 
                    JournalDetM proftLoss = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = ++lastNum,
                        JournalId = journalPrfotLoss.Id,
                        AccountID = ConstAccounts.ProfitLoss,
                    };
                    JournalDetM partener = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = ++lastNum,
                        JournalId = journalPrfotLoss.Id,
                        AccountID = ConstAccounts.Partners,
                    };
                    if (profit > 0)
                    {
                        proftLoss.Debt = profit;
                        partener.Credit = profit;

                    }
                    else if (profit < 0)
                    {
                        proftLoss.Credit = profit;
                        partener.Debt = profit;
                    }
                    detBL.Insert(proftLoss);
                    detBL.Insert(partener);


                    //journal drawings
                    JournalM drawingsJour = new JournalM()
                    {
                        Id = Guid.NewGuid(),
                        Num = ++lastNumJournal,
                        Date = date,
                        JournalTypeId = ConstJournalTypes.AccountClose,
                        UserId = Profile.UserProfile.Id,
                    };
                    journalBL.Insert(journalPrfotLoss);
                    //journal det 
                    JournalDetM drawingsDeb = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = ++lastNum,
                        JournalId = drawingsJour.Id,
                        AccountID = ConstAccounts.Partners,
                        Debt = profit,
                    };
                    detBL.Insert(drawingsDeb);

                    JournalDetM partenrCre = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = ++lastNum,
                        JournalId = drawingsJour.Id,
                        AccountID = ConstAccounts.Drawings,
                        Credit = profit,
                    };
                    detBL.Insert(partenrCre);

                    dbContextTransaction.Commit();
                    return 0;
                }
                catch
                {
                    return 3;
                }
            }
        }

        private void AddDet(JournalDetBL detBL, int lastNum, Guid journalId, decimal cre, AccountM acc)
        {
            JournalDetM det = new JournalDetM()
            {
                Id = Guid.NewGuid(),
                Num = lastNum,
                JournalId = journalId,
                AccountID = acc.Id,
            };

            if (cre > 0)
            {
                acc.Balance = acc.Credit - acc.Debt;
                if (acc.Balance > 0)
                    det.Credit = acc.Balance;
                else
                    det.Debt = acc.Balance;
            }
            else
            {
                acc.Balance = acc.Debt - acc.Credit;
                if (acc.Balance > 0)
                    det.Debt = acc.Balance;
                else
                    det.Credit = acc.Balance;
            }

            if (acc.Balance != 0)
            {
                detBL.Insert(det);
            }
        }
        public bool TransAccounts(DateTime start, DateTime end, DateTime date)
        {
            List<AccountM> ids = new List<AccountM>()
                    {new AccountM {Id = ConstAccounts.Assests},
                     new AccountM {Id = ConstAccounts.Liabilities, Credit = 1},
                     new AccountM {Id = ConstAccounts.Revenue, Credit = 1},
                     new AccountM {Id = ConstAccounts.Expenses}};

            var models = bl.GetModels(start, end);

            JournalBL journalBL = new JournalBL(db);
            JournalDetBL detBL = new JournalDetBL(db);

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    var lastNum = detBL.GetNewNum(detBL.GetLastNum());
                    var lastNumJournal = journalBL.GetNewNum();
                    
                    JournalM journal = new JournalM()
                    {
                        Id = Guid.NewGuid(),
                        Num = lastNumJournal,
                        Date = date,
                        JournalTypeId = ConstJournalTypes.TransferredBalance,
                        UserId = Profile.UserProfile.Id,
                    };
                    journalBL.Insert(journal);


                    foreach (var v in models)
                    {
                        if (!ids.Exists(m => m.Id == v.Id)) continue;
                        AccountM curAcc = new AccountM { Id = v.Id, Credit = v.Credit };
                        AddDet(detBL, lastNum, journal.Id, curAcc.Credit, v);

                        foreach (var v1 in models)
                        {
                            if (v1.AccountParentId == v.Id)
                            {
                                AddDet(detBL, lastNum++, journal.Id, curAcc.Credit, v1);

                                foreach (var v2 in models)
                                {
                                    if (v2.AccountParentId == v1.Id)
                                    {
                                        AddDet(detBL, lastNum++, journal.Id, curAcc.Credit, v2);

                                        foreach (var v3 in models)
                                        {
                                            if (v3.AccountParentId == v2.Id)
                                            {
                                                AddDet(detBL, lastNum++, journal.Id, curAcc.Credit, v3);

                                                foreach (var v4 in models)
                                                {
                                                    if (v4.AccountParentId == v3.Id)
                                                    {
                                                        AddDet(detBL, lastNum++, journal.Id, curAcc.Credit, v4);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        //public bool IsYearHaveJournals(DateTime date)
        //{
        //    var v = db.Journals.Where(m => m.Date.Year == date.Year).FirstOrDefault();
        //    if (v == null)
        //        return true;
        //    else
        //        return false;
        //}

        //public bool IsYearClosed(DateTime date)
        //{
        //    var model = (from ac in db.Accounts
        //            join m2 in db.JournalDets on ac.Id equals m2.AccountID
        //            join m3 in db.Journals on m2.JournalId equals m3.Id
        //            where ac.Id == ConstAccounts.ProfitLoss &&  m3.Date.Year == date.Year
        //            select ac).FirstOrDefault();
        //    if (model != null)
        //        return true;
        //    else
        //        return false;
        //}

    }

}
