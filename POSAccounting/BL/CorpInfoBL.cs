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
    public class CorpInfoBL
    {
        private CropAccountingAppEntities db;
        private CorpInfoUtils utils = new CorpInfoUtils();

        public CorpInfoBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public CorpInfoM GetById(int id)
        {
            var entity = (from m in db.CorpInfoes
                         where m.Id == id
                         select m).FirstOrDefault();

          return utils.FromEntity(entity);
        }

        public IEnumerable<CorpInfo> Get(Expression<Func<CorpInfo, bool>> filter)
        {
            if (filter == null)
                return db.CorpInfoes.AsEnumerable();
            return db.CorpInfoes.AsNoTracking().Where(filter).AsEnumerable();
        }

        public void Update(CorpInfoM model)
        {
            var entity = db.CorpInfoes.Where(m => m.Id == model.Id).FirstOrDefault();
            entity.Name = model.Name;
            entity.Phone = model.Phone;
            entity.Address = model.Address;
            entity.Owner = model.Owner;
            entity.Email = model.Email;
            entity.RegisterDate = DateTime.Now;
            entity.LocalImg = model.LocalImg;            
            db.SaveChanges();
        }

    }

    public class Profile
    {
        public static UserM UserProfile = null;
        public static CorpInfoM CorpProfile = null;
    }
}
