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
    public class ReceiptBL
    {
        private CropAccountingAppEntities db;
        private ReceiptUtils utils;

        public ReceiptBL(CropAccountingAppEntities db)
        {
            this.db = db;
            utils = new ReceiptUtils();
        }

        public ReceiptM GetById(Guid id)
        {
            var entity = (from m in db.Receipts
                         where m.Id == id
                         select m).FirstOrDefault();

            var model = utils.FromEntity(entity);
            var c = new ClientUtils().FromEntity(db.Clients.Where(m => m.AccountId == model.AccountId).FirstOrDefault());
            if (c == null)
                model.ClientName = db.Accounts.Where(m => m.Id == model.AccountId).Select(m => m.EnName).FirstOrDefault();
            else
                model.ClientName = c.Name;

            if (model.IsCash)
            {
                model.PaymentMethod = Properties.Resources.Cash;
            }
            else
                model.PaymentMethod = Properties.Resources.Visa;

            if (!model.IsCash)
            {
                var journal = from m in db.Journals
                              join m1 in db.JournalDets on m.Id equals m1.JournalId
                              where m.RefId == model.Id
                              select m1;

                if (model.ReceiptTypeId == ConstReceiptTypes.PayReceipt || model.ReceiptTypeId == ConstReceiptTypes.Expenses
                    || model.ReceiptTypeId == ConstReceiptTypes.Drawings)
                    journal.Where(m => m.Debt == 0);
                else
                    journal.Where(m => m.Credit == 0);

                model.VisaId = journal.Select(m => m.AccountID).FirstOrDefault();

                var visa = db.Accounts.Include(m => m.Account1).FirstOrDefault();                   

                var ut = new AccountUtils();
                model.Visa = ut.FromEntity(visa);
                model.Visa.Account = ut.FromEntity(visa.Account1);
            }
            return model;
        }

        public PaginatedList<ReceiptM> Get(Expression<Func<Receipt, bool>> filter, int? sortOrder, int page, int pageSize)
        {
           var query = from m in db.Receipts select m;

            if (filter != null)
                query =  query.Where(filter);

            

            List<Receipt> entities;
            if (pageSize > 0)
                entities = query.OrderBy(m => m.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            else
                entities = query.AsNoTracking().OrderBy(m => m.Id).ToList();

            PaginatedList<ReceiptM> model = new PaginatedList<ReceiptM>();

            var q = query.GroupBy(m => 1).Select(g => new {
                sum = g.Select(m => m.Amount).DefaultIfEmpty(0).Sum(),
                count = g.Count(),
            }).FirstOrDefault();

            if(q != null)
            {
                model.Count = q.count;
                model.Amount = q.sum;
            }

            model.Models = new ObservableCollection<ReceiptM>();
            foreach (var v in entities)
            {
                var temp = utils.FromEntity(v);
                var c = new ClientUtils().FromEntity(db.Clients.Where(m => m.AccountId == temp.AccountId).FirstOrDefault());
                if (c == null)
                {
                    var lang = AppSettings.GetLang();
                    if(lang == ConstLang.En)
                    temp.ClientName = db.Accounts.Where(m => m.Id == temp.AccountId).Select(m => m.EnName).FirstOrDefault();
                    else
                        temp.ClientName = db.Accounts.Where(m => m.Id == temp.AccountId).Select(m => m.ArName).FirstOrDefault();
                }
                else
                    temp.ClientName = c.Name;

                if (temp.IsCash)
                {
                    temp.PaymentMethod = Properties.Resources.Cash;
                }
                else
                    temp.PaymentMethod = Properties.Resources.Visa;

                model.Models.Add(temp);
            }

            return model;
        }

        public PaginatedList<ReceiptM> Get(int? sortOrder, Nullable<Guid> accId, byte typeId, 
            string kw, DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            if (page == 0)
                page = 1;

            Expression<Func<Receipt, bool>> filter = null;
            Expression<Func<Receipt, bool>> filter2 = null;
            Expression<Func<Receipt, bool>> filter3 = null;
            Expression<Func<Receipt, bool>> filter4 = null;

            if (typeId > 0)
                filter = m => m.ReceiptTypeId == typeId;
            if (accId != null)
                filter2 = m => m.AccountId == accId;
            if(kw != null)
            {
                int i = 0; int.TryParse(kw, out i);
                if (i > 0)
                    filter3 = m => m.Num == i;
                else
                    filter3 = m => m.Account.EnName.Equals(kw);
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

        public int GetCount(Expression<Func<Receipt, bool>> filter)
        {
            var query = from m in db.Receipts select m;

            if (filter != null)
                query = query.Where(filter);

            return query.Count();
        }

        public void Insert(ReceiptM model)
        {
            var entity = utils.FromModel(model);
            db.Receipts.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var entity = db.Receipts.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(Receipt entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.Receipts.Attach(entityToDelete);
            }
            db.Receipts.Remove(entityToDelete);
            db.SaveChanges();
        }

        public void Update(ReceiptM model)
        {
            var entityToUpdate = utils.FromModel(model);
            var entity = db.Receipts.Where(m => m.Id == model.Id).AsQueryable().FirstOrDefault();
            db.Entry(entity).CurrentValues.SetValues(entityToUpdate);
            db.SaveChanges();
        }

        public int GetNewNum(byte typeId)
        {
            var num = db.Receipts.OrderByDescending(m => m.Num ).Where(m => m.ReceiptTypeId == typeId)
                .Select(m => m.Num).FirstOrDefault();
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

        public void InsertReceiptInvoice(Guid receiptId, Guid invoiceId)
        {
            var v = new ReceiptInvoice { ReceiptId = receiptId, InvoiceId = invoiceId, };
            db.ReceiptInvoices.Add(v);
            db.SaveChanges();
        }
    }
}
