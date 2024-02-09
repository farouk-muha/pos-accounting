using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public class FiscalYearM : ViewModelBase
    {
        private int id;
        private DateTime year;
        private DateTime startDate;
        private DateTime endDate;
        private bool isClosed;
        private DateTime newStartDate;
        private DateTime newEndDate;

        public int Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public DateTime Year { get { return year; } set { year = value; NotifyPropertyChanged("Year"); } }
        public DateTime StartDate { get { return startDate; } set { startDate = value; NotifyPropertyChanged("StartDate"); } }
        public DateTime EndDate { get { return endDate; } set { endDate = value; NotifyPropertyChanged("EndDate"); } }
        public bool IsClosed { get { return isClosed; } set { isClosed = value; NotifyPropertyChanged("IsClosed"); } }
        public DateTime NewStartDate { get { return newStartDate; } set { newStartDate = value; NotifyPropertyChanged("NewStartDate"); } }
        public DateTime NewEndDate { get { return newEndDate; } set { newEndDate = value; NotifyPropertyChanged("NewEndDate"); } }
    }

    public class FiscalYearUtils
    {
        public FiscalYearM FromEntity(FiscalYear entity)
        {
            return entity == null ? null :
                new FiscalYearM()
                {
                    Id = entity.Id,
                    Year = entity.Year,
                    StartDate = entity.StartDate,
                    EndDate = entity.EndDate,
                    IsClosed = entity.IsClosed,
                };
        }

        public FiscalYear FromModel(FiscalYearM model)
        {
            return model == null ? null :
                new FiscalYear()
                {
                    Id = model.Id,
                    Year = model.Year,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    IsClosed = model.IsClosed,
                };
        }
    }
}
