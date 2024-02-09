using POSAccounting.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.ViewModels
{
    public class DiscountVM : ViewModelBase
    {
        private decimal amount = 0;
        private decimal percent = 0;
        public decimal SenderAmount { get; set; } = 0;

        public decimal Amount { get { return amount; } set {amount = value; NotifyPropertyChanged("Amount"); } }
        public decimal Percent { get { return percent; } set {percent = value; NotifyPropertyChanged("Percent"); } }



        private Nullable<Guid> visaId = null;
        private ObservableCollection<AccountM> visas = null;
        public Nullable<Guid> VisaId { get { return visaId; } set { visaId = value; NotifyPropertyChanged("VisaId"); } }
        public ObservableCollection<AccountM> Accounts { get { return visas; } set { visas = value; NotifyPropertyChanged("Visas"); } }

    }
}
