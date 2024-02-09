using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace POSAccounting.ViewModels
{
    public class CorpInfoVM : ViewModelBase
    {
        private CropAccountingAppEntities db;
        private CorpInfoBL bl;
        private CorpInfoM model;
        private FiscalYearM fiscalYear;
        private int prograssValue;
        private string backUpPath;
        private byte time;

        public CorpInfoM Model { get { return model; } set { model = value; NotifyPropertyChanged("Model"); } }
        public FiscalYearM FiscalYear { get { return fiscalYear; } set { fiscalYear = value; NotifyPropertyChanged("FiscalYear"); } }
        public int PrograssValue { get { return prograssValue; } set { prograssValue = value; NotifyPropertyChanged("PrograssValue"); } }
        public string BackUpPath { get { return backUpPath; } set { backUpPath = value; NotifyPropertyChanged("BackUpPath"); } }
        public byte Time { get { return time; } set { time = value; NotifyPropertyChanged("Time"); } }

        public CorpInfoVM(CropAccountingAppEntities db, CorpInfoBL bl)
        {
            this.db = db;
            this.bl = bl;
            Model = new CorpInfoM();
            var date = DateTime.Now;
            FiscalYear = new FiscalYearM() { StartDate = date, EndDate = new DateTime(date.Year, 12, 31) };
        }
    }
}
