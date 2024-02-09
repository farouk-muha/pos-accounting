using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


    public abstract class ErrorBaseVM : ViewModelBase
    {
        private int id;
        public int Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        private string msg;
        public string Msg { get { return msg; } set { msg = value; NotifyPropertyChanged("msg"); } }
        private string errorMsg;
        public string ErrorMsg { get { return errorMsg; } set { errorMsg = value; NotifyPropertyChanged("errorMsg"); } }
    }
}
