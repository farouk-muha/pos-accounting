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
    public class ClientVM : ViewModelBase
    {
        private CropAccountingAppEntities db;
        private ClientBL bl;
        private ClientM model;
        public List<SimpleByteM> Types { get; set; }
        public ClientM Model { get { return model; } set { model = value; NotifyPropertyChanged("Model"); } }


        public ClientVM(CropAccountingAppEntities db, ClientBL bl)
        {
            this.db = db;
            this.bl = bl;
            Model = new ClientM();
            Types = ConstClientTypes.GetList();
        }

        public void Set()
        {
            if (Types != null && Types.Count > 0)
                Model.ClientTypeId = (byte)Types[0].Id;
            Model.Status = true;
            model.Name = string.Empty;
            model.Phone = string.Empty;
            model.Address = null;
        }
    }

    public class ClientsVM : PagingM<ClientM>
    {
        public CropAccountingAppEntities db;
        public ClientBL bl;
        public ClientUtils utils = new ClientUtils();
        private bool isCompany;
        public bool IsCompany { get { return isCompany; } set { isCompany = value; NotifyPropertyChanged("IsCompany"); } }
        public byte typeId { get; set; }


        private ICommand isCompanyCommand;
        public ICommand IsCompanyCommand
        {
            get
            {
                if (isCompanyCommand == null)
                {
                    isCompanyCommand = new RelayCommand(param => this.Refresh(), null);
                }
                return isCompanyCommand;
            }
        }

        public ClientsVM(CropAccountingAppEntities db, ClientBL bl, byte typeId)
        {
            Models = new ObservableCollection<ClientM>();
            this.db = db;
            this.bl = bl;
            this.typeId = typeId;
        }

        public override void Load()
        {
            WinName = Properties.Resources.Clients;
            DisplayImg = ImgUtils.UserIcon;
            Refresh();
            RefreshHead();
        }

        private void RefreshHead()
        {
            CurPage = Models.Count > 0 ? 1 : 0;
        }
        public override void Refresh(object param = null)
        {
            if (param != null)
            {
                string s = param.ToString();
                if (s.Equals("c"))
                {
                    StartDate = null;
                    EndDate = null;
                    KW = null;
                    CurPage = 0;
                    Status = true;
                }
            }

            try
            {
                var temp = bl.Get(null, Status, KW, typeId, CurPage, PageSize);
                Models = temp.Models;
                Count = temp.Count;
                Amount = temp.Amount;
                Credit = temp.Credit;
                Debt = temp.Debt;
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override void Add()
        {
            MyMainWindow.Instance.Client();
        }

        public override void Update(object id)
        {
            if (id == null || id.GetType() != typeof(Guid))
                return;

            MyMainWindow.Instance.Client((Guid)id);
        }

        public override void Delete(object id)
        {

            if (id == null || id.GetType() != typeof(Guid))
                return;

            var model = Models.Where(m => m.Id == (Guid)id).FirstOrDefault();
            if (model == null)
                return;

            AccountBL accbl = new AccountBL(db);
            var acc = db.Accounts.Where(m => m.Id == model.AccountId).FirstOrDefault();
            if (!acc.Deleteable)
                return;

             var res = MessageBox.Show(string.Format(ConstUserMsg.DeleteConfirmMsg, Properties.Resources.TheClient + " \"" + model.Name + "\"")
    + "\n \n " + Properties.Resources.ClientDelNote,
   ConstUserMsg.DeleteConfirm, MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (res == MessageBoxResult.Cancel)
                return;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    new AccountBL(db).Delete(acc);
                    bl.Delete(model.Id);
                    Models.Remove(model);

                    ImgUtils img = new ImgUtils();
                    img.Delete(img.ClientImgs, model.LocalImg);

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

}
