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
    public class BankVM : ViewModelBase
    {
        private CropAccountingAppEntities db;
        private BankBL bl;
        private BankM model;

        public BankM Model { get { return model; } set { model = value; NotifyPropertyChanged("Model"); } }

        public BankVM(CropAccountingAppEntities db, BankBL bl)
        {
            this.db = db;
            this.bl = bl;
            Model = new BankM();
        }

        public void Set()
        {
            Model.Name = string.Empty;
        }
    }

    public class BanksVM : PagingM<BankM>
    {
        public CropAccountingAppEntities db;
        public BankBL bl;

        public BanksVM(CropAccountingAppEntities db, BankBL bl)
        {
            Models = new ObservableCollection<BankM>();
            this.db = db;
            this.bl = bl;
        }

        public override void Load()
        {
            Refresh();
        }
        public override void Refresh(object param = null)
        {
            BankUtils utils = new BankUtils();
            var entities = bl.Get(null);
            Models.Clear();            
            foreach (var v in entities)
            {
                var temp = utils.FromEntity(v);
                Models.Add(temp);
            }
        }

        public override void Add()
        {
            //BankWin.ShowMe(null);
        }

        public override void Update(object id)
        {
        }

        public override void Delete(object id)
        {

            if (id == null || id.GetType() != typeof(short))
                return;

            var model = Models.Where(m => m.Id == (short)id).FirstOrDefault();
            if (model == null)
                return;

            var res = MessageBox.Show(string.Format(ConstUserMsg.DeleteConfirmMsg, Properties.Resources.TheBank + " \"" + model.Name + "\""),
                ConstUserMsg.DeleteConfirm, MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (res == MessageBoxResult.Cancel)
                return;

            try
            {
                bl.Delete(model.Id);
                Models.Remove(model);
                MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
