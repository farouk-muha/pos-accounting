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
    public class ClientBL
    {
        private CropAccountingAppEntities db;
        private ClientUtils utils = new ClientUtils();

        public ClientBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public ClientM GetById(Guid id)
        {
            var entity = db.Clients.AsNoTracking().Where(m => m.Id == id).FirstOrDefault();
            return utils.FromEntity(entity);
        }

        public IEnumerable<Client> Get(Expression<Func<Client, bool>> filter)
        {
            if(filter == null)
                return db.Clients.AsEnumerable();
            return db.Clients.AsNoTracking().Where(filter).AsEnumerable();
        }


        public PaginatedList<ClientM> Get(Expression<Func<Client, bool>> filter, int? sortOrder, int page, int pageSize)
        {
            var query = from m in db.Clients
                        select m;

            if (filter != null)
                query = query.Where(filter);

            var entities = query.AsNoTracking().OrderBy(m => m.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PaginatedList<ClientM> model = new PaginatedList<ClientM>();
            model.Count = query.Count();
            model.Models = new ObservableCollection<ClientM>();
            InvoiceBL invBL = new InvoiceBL(db);
            ReceiptBL recBL = new ReceiptBL(db);

            DateTime now = DateTime.Now;
            var ids = entities.Select(m => m.AccountId).ToList();
            var alldets = new JournalDetBL(db).Get(m => m.Journal.Date.Year == now.Year
            && ids.Any(x => x == m.AccountID));

            model.Credit = alldets.Select(m => m.Credit).DefaultIfEmpty(0).Sum();
            model.Debt = alldets.Select(m => m.Debt).DefaultIfEmpty(0).Sum();
            model.Amount = model.Credit - model.Debt;
            foreach (var v in entities)
            {
                var temp = utils.FromEntity(v);
                temp.Credit = alldets.Where(m => m.AccountID == temp.AccountId).Select(m => m.Credit).DefaultIfEmpty(0).Sum();
                temp.Debt = alldets.Where(m => m.AccountID == temp.AccountId).Select(m => m.Debt).DefaultIfEmpty(0).Sum();
                temp.Ballance = temp.Credit - temp.Debt;
                temp.InvoiceCount = invBL.GetCount(m => m.Date.Year == now.Year && m.AccountId == temp.AccountId);
                temp.ReceiptCount = recBL.GetCount(m => m.Date.Year == now.Year && m.AccountId == temp.AccountId);
                model.Models.Add(temp);
            }
            return model;
        }

        public PaginatedList<ClientM> Get(int? sortOrder, bool status, string kw, byte clientType, int page, int pageSize)
        {
            if (page == 0)
                page = 1;

            Expression<Func<Client, bool>> filter = null;
            if (kw != null)
            {
                int i = 0; int.TryParse(kw, out i);

                if (i > 0)
                    filter = m => m.ClinetTypeId == clientType && m.StatusN == status && m.Num == i;
                else
                    filter = m => m.ClinetTypeId == clientType && m.StatusN == status && m.Name.StartsWith(kw.Trim());
            }
            else
                filter = m => m.ClinetTypeId == clientType && m.StatusN == status;

            return Get(filter, sortOrder, page, pageSize);
        }

        public Guid Insert(ClientM model)
        {
            var entity = utils.FromModel(model);
            db.Clients.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public void Delete(Guid id)
        {
            var entity = db.Clients.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(Client entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.Clients.Attach(entityToDelete);
            }
            db.Clients.Remove(entityToDelete);
            db.SaveChanges();
        }

        public void Update(ClientM model)
        {
            var entity = utils.FromModel(model);
            var e = db.Clients.Where(m => m.Id == model.Id)
                .AsQueryable().FirstOrDefault();

            db.Entry(e).CurrentValues.SetValues(entity);
            db.SaveChanges();
        }

        public int GetNewNum()
        {
            var num = db.Clients.OrderByDescending(m => m.Num).Select(m => m.Num).FirstOrDefault();
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
