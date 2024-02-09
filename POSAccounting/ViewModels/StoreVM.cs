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
    public class StoreVM : ViewModelBase
    {
        private CropAccountingAppEntities db;
        private StoreBL bl;
        private StoreM model;

        public StoreM Model { get { return model; } set { model = value; NotifyPropertyChanged("Model"); } }

        public StoreVM(CropAccountingAppEntities db, StoreBL bl)
        {
            this.db = db;
            this.bl = bl;
            Model = new StoreM();
        }

        public void Set()
        {
            Model.Num = bl.GetNewNum();
            Model.Name = string.Empty;
        }
    }

    public class StoresVM : PagingM<StoreM>
    {
        public CropAccountingAppEntities db;
        public StoreBL bl;

        public StoresVM(CropAccountingAppEntities db, StoreBL bl)
        {
            Models = new ObservableCollection<StoreM>();
            this.db = db;
            this.bl = bl;
        }
        public override void Load()
        {
            WinName = Properties.Resources.Stores;
            DisplayImg = ImgUtils.StoreIcon;
            Refresh();
            RefreshHead();
        }

        private void RefreshHead()
        {
            CurPage = Models.Count > 0 ? 1 : 0;
        }

        public override void Refresh(object param = null)
        {
            try
            {
                var temp = bl.Get(null, null, CurPage, PageSize);
                Models = temp.Models;
                Count = temp.Count;
                Amount = temp.Amount;
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override void Add()
        {
            StoreWin.ShowMe(null);
        }

        public override void Update(object id)
        {
            if (id.GetType() != typeof(Guid))
                return;
            StoreWin.ShowMe((Guid)id);
        }

        public override void Delete(object id)
        {

            if (id == null || id.GetType() != typeof(Guid))
                return;

            var model = Models.Where(m => m.Id == (Guid)id).FirstOrDefault();
            if (model == null)
                return;

            var res = MessageBox.Show(string.Format(ConstUserMsg.DeleteConfirmMsg, Properties.Resources.TheStore + " \"" + model.Name + "\"")
                + "\n \n " + Properties.Resources.StoreDelNote,
              ConstUserMsg.DeleteConfirm, MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (res == MessageBoxResult.Cancel)
                return;

            try
            {
                bl.Delete(model.Id);
                Models.Remove(model);

                RefreshHead();
                MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
