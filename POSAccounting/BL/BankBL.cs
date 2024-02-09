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
    public class BankBL
    {
        private CropAccountingAppEntities db;
        private BankUtils utils = new BankUtils();

        public BankBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public BankM GetById(short id)
        {
            var entity = db.Banks.AsNoTracking().Where(m => m.Id == id).FirstOrDefault();
            return utils.FromEntity(entity);
        }

        public List<Bank> Get(Expression<Func<Bank, bool>> filter)
        {
            var query = from m in db.Banks select m;
            if (filter != null)
                query = query.Where(filter);
            var entities = query.ToList();
            return entities;
        }
        public ObservableCollection<BankM> GetModels(Expression<Func<Bank, bool>> filter)
        {
            var entities = Get(filter);

            ObservableCollection<BankM> models = new ObservableCollection<BankM>();
            foreach (var v in entities)
                models.Add(utils.FromEntity(v));
            return models;
                }

        public void Insert(BankM model)
        {
            var entity = utils.FromModel(model);
            db.Banks.Add(entity);
            db.SaveChanges();
        }

        public void Delete(short id)
        {
            var entity = db.Banks.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(Bank entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.Banks.Attach(entityToDelete);
            }
            db.Banks.Remove(entityToDelete);
            db.SaveChanges();
        }

        public void Update(BankM model)
        {
            var entity = utils.FromModel(model);
            var e = db.Banks.Where(m => m.Id == model.Id)
                .AsQueryable().FirstOrDefault();

            db.Entry(e).CurrentValues.SetValues(entity);
            db.SaveChanges();
        }

    }

}
