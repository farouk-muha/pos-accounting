using POSAccounting.BL;
using POSAccounting.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.ViewModels
{
    public class FiscalYearVM : ViewModelBase
    {
        public CropAccountingAppEntities db;
        private FiscalYearBL bl;
        private FiscalYearM model;
        private ObservableCollection<FiscalYearM> fiscalYears;

        public FiscalYearM Model { get { return model; } set { model = value; NotifyPropertyChanged("Model"); } }
        public ObservableCollection<FiscalYearM> FiscalYears { get { return fiscalYears; } set { fiscalYears = value; NotifyPropertyChanged("FiscalYears"); } }

        public FiscalYearVM(CropAccountingAppEntities db, FiscalYearBL bl)
        {
            this.db = db;
            this.bl = bl;
            Model = new FiscalYearM();
        }

        public void Load()
        {
            Model = bl.GetLast();
            Model.NewStartDate = Model.StartDate.AddDays(1);
            Model.NewEndDate = new DateTime(Model.NewStartDate.Year, 12, 31);
        }
    }
}
