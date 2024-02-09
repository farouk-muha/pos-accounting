using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public class ProductTransM : ViewModelBase
    {
        private Guid id;
        private string code;
        private string name;
        private DateTime date;
        private double qTY;
        private byte typeId;
        private string typeName;
        private byte unitId;
        private string unitName;
        private string notes;
        private byte defaultUnitId;
        private double totalQTY;

        public Guid Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public string Code { get { return code; } set { code = value; NotifyPropertyChanged("Code"); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("Name"); } }
        public DateTime Date { get { return date; } set { date = value; NotifyPropertyChanged("Date"); } }
        public double QTY { get { return qTY; } set { qTY = value; NotifyPropertyChanged("QTY"); } }
        public byte TypeId { get { return typeId; } set { typeId = value; NotifyPropertyChanged("TypeId"); } }
        public string TypeName { get { return typeName; } set { typeName = value; NotifyPropertyChanged("TypeName"); } }
        public string Notes { get { return notes; } set { notes = value; NotifyPropertyChanged("Notes"); } }
        public byte UnitId { get { return unitId; } set { unitId = value; NotifyPropertyChanged("UnitId"); } }
        public string UnitName { get { return unitName; } set { unitName = value; NotifyPropertyChanged("UnitName"); } }
        public byte DefaultUnitId { get { return defaultUnitId; } set { defaultUnitId = value; NotifyPropertyChanged("DefaultUnitId"); } }
        public double TotalQTY { get { return totalQTY; } set { totalQTY = value; NotifyPropertyChanged("TotalQTY"); } }

    }


}
