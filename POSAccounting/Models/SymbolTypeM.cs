using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public class SymbolM : ViewModelBase
    {
        private byte symbolId ;
        private string symbolName ;
        private string tableName ;
        private bool cSelect ;
        private bool cUpdate ;
        private bool cInsert ;
        private bool cDelete ;
        private ObservableCollection<SymbolTypeM> symbolTypes ;

        public byte SymbolId { get { return symbolId; } set { symbolId = value; NotifyPropertyChanged("symbolId"); } }
        public string SymbolName { get { return symbolName; } set { symbolName = value; NotifyPropertyChanged("symbolName"); } }
        public string TableName { get { return tableName; } set { tableName = value; NotifyPropertyChanged("tableName"); } }
        public bool CSelect { get { return cSelect; } set { cSelect = value; NotifyPropertyChanged("cSelect"); } }
        public bool CInsert { get { return cInsert; } set { cInsert = value; NotifyPropertyChanged("cInsert"); } }
        public bool CUpdate { get { return cUpdate; } set { cUpdate = value; NotifyPropertyChanged("cUpdate"); } }
        public bool CDelete { get { return cDelete; } set { cDelete = value; NotifyPropertyChanged("cDelete"); } }
        public ObservableCollection<SymbolTypeM> SymbolTypes { get { return symbolTypes; } set { symbolTypes = value; } }

    }

    public class SymbolUtils
    {
        public SymbolM Fromentity(Symbol entity)
        {

            return entity == null ? null :
                new SymbolM()
                {
                    SymbolId = entity.SymbolId,
                    SymbolName = entity.SymbolName,
                    TableName = entity.TableName,
                    CSelect = entity.CSelect,
                    CUpdate = entity.CUpdate,
                    CInsert = entity.CInsert,
                    CDelete = entity.Cdelete,

                };

        }

        public Symbol Fromentity(SymbolM model)
        {

            return model == null ? null :
                new Symbol()
                {
                    SymbolId = model.SymbolId,
                    SymbolName = model.SymbolName,
                    TableName = model.TableName,
                    CSelect = model.CSelect,
                    CUpdate = model.CUpdate,
                    CInsert = model.CInsert,
                    Cdelete = model.CDelete,

                };

        }

    }

    public class SymbolTypeM :ViewModelBase
    {
        private int id ;
        private string name ;
        private int symbolId ;

        public int Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("Name"); } }
        public int SymbolId { get { return symbolId; } set { symbolId = value; NotifyPropertyChanged("SymbolId"); } }
    }

    public class SymbolTypeUtils
    {
        public SymbolTypeM FromCityEntity(City entity)
        {
            if (entity == null)
                return null;
            return entity == null ? null :
                    new SymbolTypeM()
                    {
                        Id = entity.CityId,
                        Name = entity.CityName,
                        SymbolId = entity.SymbolId,
                    };
        }

        public City FromCityModel(SymbolTypeM model)
        {
            if (model == null)
                return null;
            return model == null ? null :
                new City()
                {
                    CityId = (byte)model.Id,
                    CityName = model.Name,
                    SymbolId = (byte)model.SymbolId,
                };
        }

        public SymbolTypeM FromGenderEntity(Gender entity)
        {
            if (entity == null)
                return null;
            return entity == null ? null :
                new SymbolTypeM()
                {
                    Id = entity.GenderId,
                    Name = entity.GenderName,
                    SymbolId = entity.SymbolId,
                };
        }

        public Gender FromGenderModel(SymbolTypeM model)
        {
            if (model == null)
                return null;
            return model == null ? null :
                new Gender()
                {
                    GenderId = (byte)model.Id,
                    GenderName = model.Name,
                    SymbolId = (byte)model.SymbolId,
                };
        }

        public SymbolTypeM FromUnitMasterEntity(Unit entity)
        {
            if (entity == null)
                return null;
            return entity == null ? null :
                new SymbolTypeM()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    SymbolId = entity.SymbolId,
                };
        }

        public Unit FromUnitMasterModel(SymbolTypeM model)
        {
            if (model == null)
                return null;

            return model == null ? null :
                new Unit()
                {
                    Id = (byte)model.Id,
                    Name = model.Name,
                    SymbolId = (byte)model.SymbolId,
                };
        }
    }

}
