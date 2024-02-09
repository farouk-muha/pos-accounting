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
    public class JournalBL
    {
        private CropAccountingAppEntities db;

        public JournalBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public JournalM GetById(Guid id)
        {
            var entity = db.Journals.Where(m => m.Id == id).Include(m => m.JournalDets).FirstOrDefault();

            var model = JournalUtils.FromEntity(entity);
            if (model == null)
                return model;

            model.JournalDets = new ObservableCollection<JournalDetM>();
            model.Credit = model.JournalDets.Select(m => m.Credit).DefaultIfEmpty(0).Sum();
            model.Debt = model.JournalDets.Select(m => m.Debt).DefaultIfEmpty(0).Sum();
            AccountUtils accUtils = new AccountUtils();
            foreach (var v in entity.JournalDets)
            {
                var temp = JournalDettUtils.FromEntity(v);
                var acc = db.Accounts.Where(m => m.Id == temp.AccountID).FirstOrDefault();
                temp.Account = accUtils.FromEntity(acc);
                model.JournalDets.Add(temp);
            }
            return model;
        }

        public IEnumerable<Journal> Get(Expression<Func<Journal, bool>> filter)
        {
            var query = db.Journals.Include(m => m.JournalDets).AsNoTracking();

            if (filter != null)
                query = query.Where(filter);
            return query.AsNoTracking();
        }

        public PaginatedList<JournalM> Get(Expression<Func<Journal, bool>> filter, int? sortOrder, int page, int pageSize)
        {
            var query = db.Journals.Include(m=> m.JournalDets).AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            List<Journal> entities;
            if (pageSize > 0)
                entities = query.OrderBy(m => m.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            else
                entities = query.OrderBy(m => m.Id).ToList();

            PaginatedList<JournalM> model = new PaginatedList<JournalM>();

            var q = query.Select(g => new {
                debt = g.JournalDets.Select(m => m.Debt).DefaultIfEmpty(0).Sum(),
                credit = g.JournalDets.Select(m => m.Credit).DefaultIfEmpty(0).Sum(),
            }).ToList();

            model.Credit = q.Select(m => m.credit).DefaultIfEmpty(0).Sum();
            model.Debt = q.Select(m => m.debt).DefaultIfEmpty(0).Sum();
            model.Count = query.Count();
            model.Models = new ObservableCollection<JournalM>();

            List<AccountM> accs = new List<AccountM>();
            AccountUtils acu = new AccountUtils();
            if (entities.Count > 0)
            {
                var a = db.Accounts.ToList();
                foreach (var v in a)
                {
                    accs.Add(acu.FromEntity(v));
                }
            }
            foreach (var v in entities)
            {
                var temp = JournalUtils.FromEntity(v);
                temp.JournalDets = new ObservableCollection<JournalDetM>();
                foreach (var vv in v.JournalDets)
                {
                    var t = JournalDettUtils.FromEntity(vv);
                    t.AccountName = accs.Where(m => m.Id == vv.AccountID).Select(m => m.EnName).FirstOrDefault();
                    temp.JournalDets.Add(t);
                }
                temp.Credit = temp.JournalDets.Select(m => m.Credit).DefaultIfEmpty(0).Sum();
                temp.Debt = temp.JournalDets.Select(m => m.Debt).DefaultIfEmpty(0).Sum();
                temp.JournalTypeName = ConstJournalTypes.GetList().Where(m => m.Id == temp.JournalTypeId)
                    .Select(m => m.Name).FirstOrDefault();
                model.Models.Add(temp);
            }
            return model;
        }


        public PaginatedList<JournalM> Get(int? sortOrder, Nullable<Guid> accId, byte typeId,
            string kw, DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            if (page == 0)
                page = 1;

            Expression<Func<Journal, bool>> filter = null;
            Expression<Func<Journal, bool>> filter2 = null;
            Expression<Func<Journal, bool>> filter3 = null;
            Expression<Func<Journal, bool>> filter4 = null;

            if (typeId > 0)
                filter = m => m.JournalTypeId == typeId;
            if (accId != null)
                filter2 = m => m.JournalDets.Any(x => x.AccountID == accId);
            if (kw != null)
            {
                int i = 0; int.TryParse(kw, out i);
                if (i > 0)
                    filter3 = m => m.JournalDets.Any(x => x.AccountID == accId);
                else
                    filter3 = m => m.Details.Equals(kw);
            }
            if (startDate != null && endDate != null)
                filter4 = m => m.Date >= startDate && m.Date < endDate;
            else
            {
                var year = DateTime.Now.Year;
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

        public void Insert(JournalM model)
        {
            var entity = JournalUtils.FromModel(model);
            db.Journals.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var entity = db.Journals.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(Journal entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.Journals.Attach(entityToDelete);
            }
            db.Journals.Remove(entityToDelete);
            db.SaveChanges();
        }

        public void Update(JournalM model)
        {
            var entity = db.Journals.Where(m => m.Id == model.Id).FirstOrDefault();
            entity.Note = model.Note;
            entity.JournalTypeId = model.JournalTypeId;
            entity.RefId = model.RefId;
            entity.UserId = model.UserId;
            entity.Details = (string.IsNullOrWhiteSpace(model.Details) || model.Details.Trim()
                .Equals(ConstJournalTypes.GetList().Where(m => m.Id == model.JournalTypeId).Select(m => m.Name)
                .FirstOrDefault()) ) ? null : model.Details;
            db.SaveChanges();
        }
        public int GetNewNum()
        {
            var num = db.Journals.OrderByDescending(m => m.Num).Select(m => m.Num).FirstOrDefault();
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

        //public string GetNewNum()
        //{
        //    string s = db.Journals.OrderByDescending(m => m.Num).Select(m => m.Num).FirstOrDefault();
        //    char cha = '-';
        //    if (!string.IsNullOrEmpty(s))
        //    {
        //        string ss;
        //        if (s.IndexOf("-", StringComparison.OrdinalIgnoreCase) >= 0)
        //        {
        //            var v = s.Split(cha);
        //            ss = v[0];
        //        }
        //        else
        //            ss = s;

        //        int i; int.TryParse(ss, out i);
        //        i++;
        //        s = i.ToString() + cha + Profile.UserProfile.Id;

        //    }
        //    else
        //    {
        //        s = "1" + cha + Profile.UserProfile.Id; ;
        //    }
        //    return s;
        //}

    }
}
