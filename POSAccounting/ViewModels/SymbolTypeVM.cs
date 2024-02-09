using POSAccounting.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace POSAccounting.ViewModels
{
    public class SymbolVM 
    {
        private ObservableCollection<SymbolM> _models;
        public ObservableCollection<SymbolM> Models { get { return _models; } set { _models = value; } }

        private ICommand _SubmitCommand;
        public ICommand SubmitCommand
        {
            get
            {
                if (_SubmitCommand == null)
                {
                    _SubmitCommand = new RelayCommand(parm => this.Submit(), null);
                }
                return _SubmitCommand;
            }
        }
        public void LoadModels(CropAccountingAppEntities db)
        {
            Models = new ObservableCollection<SymbolM>();
        }

        private void Submit()
        {
        }
    }

    public class SymbolTypeVM : ViewModelBase
    {

    }

}
