using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
using System.Collections.ObjectModel;
using POSAccounting.Models;

namespace POSAccounting.BL
{
    public class UserBL
    {
        private UserUtls utils = new UserUtls();
        private CropAccountingAppEntities db;
        public UserBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public void RelationName(UserM model)
        {
            model.CityName = db.Cities.Where(m => m.CityId == model.CityId).Select(m => m.CityName).FirstOrDefault();
            model.GenderName = db.Genders.Where(m => m.GenderId == model.GenderId).Select(m => m.GenderName).FirstOrDefault();
            model.LocalStatusName = ConstUserStatus.GetList().Where(m => m.Id == model.LocalStatusId).Select(m => m.Name).FirstOrDefault();
            model.LocalRoleName = db.SecLocalRoles.Where(m => m.Id == model.LocalRoleId).Select(m => m.Name).FirstOrDefault();
        }

        public SecUser GetById(int id)
        {
            var entity = db.SecUsers.Where(m => m.Id == id).AsNoTracking().FirstOrDefault();
            return entity;
        }

        public UserM GetModelById(int id)
        {
            var entity = GetById(id);
            var model = utils.FromEntity(entity);
            if (model != null)
                RelationName(model);
            return model;
        }

        public IEnumerable<SecUser> Get(Expression<Func<SecUser, bool>> filter)
        {
            var query = from m in db.SecUsers select m;
            if (filter != null)
                query = query.Where(filter);
            return query.ToList();
        }

        public PaginatedList<UserM> Get(int? sortOrder, int page, int pageSize)
        {
            page = page == 0 ? 1 : page;

            var query = from m in db.SecUsers select m;
            var entities = query.AsNoTracking().OrderBy(m => m.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PaginatedList<UserM> model = new PaginatedList<UserM>();
            model.Count = query.Count();
            model.Models = new ObservableCollection<UserM>();
            foreach (var v in entities)
            {
                var temp = utils.FromEntity(v);
                RelationName(temp);
                model.Models.Add(temp);
            }
            return model;
            }

        public UserM Get(string name, string password)
        {
            HashUtils hash = new HashUtils();
            var pass = hash.GenerateSaltedHash(password);

            var entity = db.SecUsers.Where(m => m.Email.ToUpper().Equals(name.Trim().ToUpper())
                  || m.UserName.ToUpper().Equals(name.Trim().ToUpper())).AsNoTracking().FirstOrDefault();

            var model = utils.FromEntity(entity);

            if (model == null || model.PassHash == null || !hash.CompareByteArrays(pass, model.PassHash))
            {
                return null;
            }

            return model;
        }
        public int Insert(UserM model)
        {
            HashUtils hash = new HashUtils();
            model.PassHash = hash.GenerateSaltedHash(model.Password);
            var entity = utils.FromModel(model);
            db.SecUsers.Add(entity);
            db.SaveChanges();
            return 0;
        }

        public void Update(UserM model)
        {
            var entity = db.SecUsers.Where(m => m.Id == model.Id).FirstOrDefault();
            entity.UserName = model.UserName;
            entity.FristName = model.FristName;
            entity.LastName = model.LastName;
            entity.Email = model.Email;
            entity.Phone = model.Phone;
            entity.Address = model.Address;
            entity.CityId = (short)model.CityId;
            entity.LocalImg = model.LocalImg;
            entity.LocalRoleId = model.LocalRoleId;
            db.SaveChanges();
        }

        public void Update(int id, byte statusId)
        {
            var entity = db.SecUsers.Where(m => m.Id == id).FirstOrDefault();
            entity.LocalStatusId = statusId;
            db.SaveChanges();
        }

        public bool ResetPass(ResetPasswordM model)
        {
            var entity = db.SecUsers.Where(m => m.Id == model.Id).FirstOrDefault();

            HashUtils hash = new HashUtils();
            var pass = hash.GenerateSaltedHash(model.Password);
            var newPass = hash.GenerateSaltedHash(model.NewPassword);
            if (hash.CompareByteArrays(pass, entity.Password))
            {
                entity.Password = newPass;
                db.SaveChanges();
                return true;
            }
            else
                return false;              
        }

        public void Delete(int id)
        {
            var entity = db.SecUsers.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(SecUser entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.SecUsers.Attach(entityToDelete);
            }
            db.SecUsers.Remove(entityToDelete);
            db.SaveChanges();
        }


    }


















    public class GenericRepository<TEntity> where TEntity : class
    {
        internal CropAccountingAppEntities context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(CropAccountingAppEntities context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }


        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }


    }
}