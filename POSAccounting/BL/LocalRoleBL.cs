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
    public class LocalRoleBL
    {
        private CropAccountingAppEntities db;
        private LocalRoleUtils localRoleUtils = new LocalRoleUtils();
        private ActionUtils actionUtils = new ActionUtils();

        public LocalRoleBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        public LocalRoleM GetByID(Guid id)
        {
            var entity = db.SecLocalRoles.AsNoTracking().Where(m => m.Id == id).FirstOrDefault();
            return localRoleUtils.FromEntity(entity);
        }

        public PaginatedList<LocalRoleM> Get(int? sortOrder, int page, int pageSize)
        {
            page = page == 0 ? 1 : page;

            var query = from m in db.SecLocalRoles select m;
            var entities = query.AsNoTracking().OrderBy(m => m.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            PaginatedList<LocalRoleM> model = new PaginatedList<LocalRoleM>();
            model.Count = query.Count();
            model.Models = new ObservableCollection<LocalRoleM>();
            foreach (var v in entities)
            {
                var temp = localRoleUtils.FromEntity(v);
                model.Models.Add(temp);
            }
            return model;
        }

        public ObservableCollection<LocalRoleM> GetModels(Expression<Func<SecLocalRole, bool>> filter)
        {
            var query = from m in db.SecLocalRoles select m;
            if (filter != null)
                query = query.Where(filter);
            var entities = query.ToList();
            ObservableCollection<LocalRoleM> models = new ObservableCollection<LocalRoleM>();
            foreach (var v in entities)
                models.Add(localRoleUtils.FromEntity(v));
            return models;
        }

        public List<SecLocalRole> Get(Expression<Func<SecLocalRole, bool>> filter)
        {
            var query = from m in db.SecLocalRoles select m;
            if (filter != null)
                query = query.Where(filter);
            return query.ToList();
        }


        public Guid Insert(LocalRoleM model)
        {
            var entity = localRoleUtils.FromModel(model);
            db.SecLocalRoles.Add(entity);
            db.SaveChanges();
            return entity.Id;
        }

        public void Delete(Guid id)
        {
            var entity = db.SecLocalRoles.Where(m => m.Id == id).FirstOrDefault();
            Delete(entity);
        }

        public void Delete(SecLocalRole entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.SecLocalRoles.Attach(entityToDelete);
            }
            db.SecLocalRoles.Remove(entityToDelete);
            db.SaveChanges();
        }

        public void DeleteLocalRoleTask(Guid localRoleId)
        {
            var entity = db.SecLocalRoleTasks.Where(m => m.LocalRoleId == localRoleId);
            foreach(var v in entity)
                DeleteLocalRoleTask(v);
        }

        public void DeleteLocalRoleTask(SecLocalRoleTask entityToDelete)
        {
            if (db.Entry(entityToDelete).State == EntityState.Detached)
            {
                db.SecLocalRoleTasks.Attach(entityToDelete);
            }
            db.SecLocalRoleTasks.Remove(entityToDelete);
            db.SaveChanges();
        }

        public void Update(LocalRoleM model)
        {
            var entity = db.SecLocalRoles.Where(m => m.Id == model.Id).FirstOrDefault();
            entity.Name = model.Name;
            entity.Notes = model.Notes;
            db.SaveChanges();
        }

    }

}
