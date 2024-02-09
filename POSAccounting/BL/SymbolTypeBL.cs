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
    public class SymbolTypeBL
    {
        private CropAccountingAppEntities db;
        private SymbolUtils symbolUtils = new SymbolUtils();
        private SymbolTypeUtils utils = new SymbolTypeUtils();

        public SymbolTypeBL(CropAccountingAppEntities db) { this.db = db; }

        public List<SymbolM> GetSymbols()
        {
             var entities = db.Symbols.ToList();
            return entities.Select(m => symbolUtils.Fromentity(m)).ToList();
        }

        public List<SymbolM> GetSymbols(Expression<Func<Symbol, bool>> filter)
        {

            var entities = db.Symbols.Where(filter).ToList();
            List<SymbolM> models = entities.Select(m => symbolUtils.Fromentity(m)).ToList();

            if (models == null)
                return models;

            foreach (var v in models)
            {
                if (v.SymbolName.Trim().ToUpper().Equals(ConstSymbol.Cities))
                {
                    var entities2 = db.Cities.ToList();
                    foreach (var vv in entities2)
                        v.SymbolTypes.Add(utils.FromCityEntity(vv));
                }
                else if (v.SymbolName.Trim().ToUpper().Equals(ConstSymbol.Genders))
                {
                    var entities2 = db.Genders.ToList();
                    foreach (var vv in entities2)
                        v.SymbolTypes.Add(utils.FromGenderEntity(vv));
                }
                else if (v.SymbolName.Trim().ToUpper().Equals(ConstSymbol.Units))
                {
                    var entities2 = db.Units.ToList();
                    foreach (var vv in entities2)
                        v.SymbolTypes.Add(utils.FromUnitMasterEntity(vv));
                }
            }
            return models;
        }

        public void InsertSymbols(SymbolM model)
        {
            Symbol entity = symbolUtils.Fromentity(model);
            db.Symbols.Add(entity);
            db.SaveChanges();
        }

        public List<SymbolTypeM> Get(string symbolName)
        {
            List<SymbolTypeM> models = null;

            if (symbolName.Trim().ToUpper().Equals(ConstSymbol.Cities))
            {
                var entities = db.Cities.ToList();
                models = entities.Select(m => utils.FromCityEntity(m)).ToList();
            }
            else if (symbolName.Trim().ToUpper().Equals(ConstSymbol.Genders))
            {
                var entities = db.Genders.ToList();
                models = entities.Select(m => utils.FromGenderEntity(m)).ToList();
            }
            else if (symbolName.Equals(ConstSymbol.Units))
            {
                var entities = db.Units.ToList();
                models = entities.Select(m => utils.FromUnitMasterEntity(m)).ToList();
            }
            return models;
        }

        public SymbolTypeM GetById(string symbolName, int id)
        {
            SymbolTypeM models = null;

            if (symbolName.Trim().ToUpper().Equals(ConstSymbol.Cities))
            {
                var entity = db.Cities.Where(m => m.CityId == id).FirstOrDefault();
                models = utils.FromCityEntity(entity);
            }
            else if (symbolName.Trim().ToUpper().Equals(ConstSymbol.Genders))
            {
                var entity = db.Genders.Where(m => m.GenderId == id).FirstOrDefault();
                models = utils.FromGenderEntity(entity);
            }
            else if (symbolName.Trim().ToUpper().Equals(ConstSymbol.Units))
            {
                var entity = db.Units.Where(m => m.Id == id).FirstOrDefault();
                models = utils.FromUnitMasterEntity(entity);
            }
            return models;
        }

        public bool Update(string symbolName, SymbolTypeM model)
        {
            if (symbolName.Trim().ToUpper().Equals(ConstSymbol.Cities))
            {
                var entity = utils.FromCityModel(model);
                db.Entry(entity).State = EntityState.Modified;
            }
            else if (symbolName.Trim().ToUpper().Equals(ConstSymbol.Genders))
            {
                var entity = utils.FromGenderModel(model);
                db.Entry(entity).State = EntityState.Modified;
            }
            else if (symbolName.Trim().ToUpper().Equals(ConstSymbol.Units))
            {
                var entity = utils.FromUnitMasterModel(model);
                db.Entry(entity).State = EntityState.Modified;
            }

            db.SaveChanges();
            return true;
        }

        public bool Delete(string symbolName, SymbolTypeM model)
        {

            if (symbolName.Trim().ToUpper().Equals(ConstSymbol.Cities))
            {
                var entity = utils.FromCityModel(model);
                if (db.Entry(entity).State == EntityState.Detached)
                {
                    db.Cities.Attach(entity);
                }
            }
            else if (symbolName.Trim().ToUpper().Equals(ConstSymbol.Genders))
            {
                var entity = utils.FromGenderModel(model);
                if (db.Entry(entity).State == EntityState.Detached)
                {
                    db.Genders.Attach(entity);
                }
            }
            else if (symbolName.Trim().ToUpper().Equals(ConstSymbol.Units))
            {
                var entity = utils.FromUnitMasterModel(model);
                if (db.Entry(entity).State == EntityState.Detached)
                {
                    db.Units.Attach(entity);
                }
            }

            db.SaveChanges();
            return true;
        }

        public bool DeleteAllSymbole()
        {
            var entities = from s in db.Units select s;

            foreach (var entity in entities)
            {
                if (db.Entry(entity).State == EntityState.Detached)
                {
                    db.Units.Attach(entity);
                }
            }
            db.SaveChanges();

            var symbols = from s in db.Symbols select s;

            foreach (var entity in symbols)
            {
                if (db.Entry(entity).State == EntityState.Detached)
                {
                    db.Symbols.Attach(entity);
                }
            }
            db.SaveChanges();
            return true;
        }
    }

}
