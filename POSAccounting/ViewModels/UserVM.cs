using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using POSAccounting.Models;
using POSAccounting.Server;
using POSAccounting.BL;
using POSAccounting.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace POSAccounting.ViewModels
{

    public class UserVM : ViewModelBase
    {
        private UserM model;

        public UserM Model { get { return model; } set { model = value; NotifyPropertyChanged("Model"); } }
        public ObservableCollection<SimpleByteM> LocalStatusList { get; set; } = new ObservableCollection<SimpleByteM>();
        public ObservableCollection<LocalRoleM> LocalRolesList { get; set; } = new ObservableCollection<LocalRoleM>();
        public ObservableCollection<SymbolTypeM> CitiesList { get; set; } = new ObservableCollection<SymbolTypeM>();

        private ICommand _imgCommand;
        public ICommand ImgCommand
        {
            get
            {
                if (_imgCommand == null)
                {
                    _imgCommand = new RelayCommand(param => this.LoadImg(), null);
                }
                return _imgCommand;
            }
        }

        public UserVM(CropAccountingAppEntities db)
        {
            Model = new UserM();
            var status = ConstUserStatus.GetList();
            foreach (var v in status)
                LocalStatusList.Add(v);
            var roles = new LocalRoleBL(db).GetModels(null);
            foreach (var r in roles)
                LocalRolesList.Add(r);
            var cites = new SymbolTypeBL(db).Get(ConstSymbol.Cities);
            foreach (var c in cites)
                CitiesList.Add(c);
        }

        public void Set(CropAccountingAppEntities db)
        {
            if(LocalStatusList != null && LocalStatusList.Count > 0)
            Model.LocalStatusId = LocalStatusList[0].Id;
            if (LocalRolesList != null && LocalRolesList.Count > 0)
                Model.LocalRoleId = LocalRolesList[0].Id;
            if (CitiesList != null && CitiesList.Count > 0)
                Model.CityId = CitiesList[0].Id;
        }
      
        public void LoadImg()
        {
            ImgUtils img = new ImgUtils();
            var re = img.ImgDialog();
            if (re == null)
                return;

            Model.DisplayImg = re.FileName;
        }
    }

   public class UsersVM : PagingM<UserM>
    {
        public CropAccountingAppEntities db;
        public UserBL bl;

        //private int roleId       
        //public int RoleId { get { return roleId; } set { roleId = value; NotifyPropertyChanged("roleId"); } }
        //public List<SimpleM> UserStatus { get; set; }
        private ICommand _statusCommand;
        public ICommand StatusCommand
        {
            get
            {
                if (_statusCommand == null)
                {
                    _statusCommand = new RelayCommand(param => this.UpdateStatus(param));
                }
                return _statusCommand;
            }
        }
        private ICommand _passCommand;
        public ICommand PassCommand
        {
            get
            {
                if (_passCommand == null)
                {
                    _passCommand = new RelayCommand(param => this.PassRest(param));
                }
                return _passCommand;
            }
        }

        public UsersVM(CropAccountingAppEntities db, UserBL bl)
        {
            Models = new ObservableCollection<UserM>();
            this.db = db;
            this.bl = bl;
        }

        public override void Load()
        {
            //UserStatus = ConstUserStatus.GetList();
            //StatusId = UserStatus[0].Id;  
            WinName = Properties.Resources.Users;
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
            try
            {
                var temp = bl.Get(null, CurPage, PageSize);
                Models = temp.Models;
                Count = temp.Count;
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override void Add()
        {
           MyMainWindow.Instance.User();
        }

        public override void Update(object id)
        {
            if (id == null || id.GetType() != typeof(int))
                return;

            MyMainWindow.Instance.User((int)id);
        }

        public void PassRest(object id)
        {
            if (id == null || id.GetType() != typeof(int))
                return;

            PassRestWin.ShowMe((int)id);
        }

        public void UpdateStatus(object id)
        {
            if (id == null || id.GetType() != typeof(int))
                return;

            var model = Models.Where(m => m.Id == (int)id).FirstOrDefault();

            string msg = ConstUserMsg.ConfirmUserStatus;
            var res =  MessageBox.Show(msg, ConstUserMsg.UpdateConfirm, MessageBoxButton.OKCancel, MessageBoxImage.Information);

            if (res == MessageBoxResult.Cancel)
                return;

            var b = model.LocalStatusId;
            if (b == ConstUserStatus.Active)
            {
                b = ConstUserStatus.Disable;
                msg += Properties.Resources.Disable;
            }
            else
            {
                b = ConstUserStatus.Active;
                msg += Properties.Resources.Active;
            }

            try
            {
                bl.Update(model.Id, model.LocalStatusId);
                model.LocalStatusId = b;
                model.LocalStatusName = ConstUserStatus.GetList().Where(m => m.Id == model.LocalStatusId).Select(m => m.Name).FirstOrDefault();
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override void Delete(object id)
        {
            if (id == null || id.GetType() != typeof(int))
                return;

            var model = Models.Where(m => m.Id == (int)id).FirstOrDefault();
            if (model == null)
                return;

            var res = MessageBox.Show(string.Format(ConstUserMsg.DeleteConfirmMsg, "The User " +"\"" + model.UserName + "\""), 
                ConstUserMsg.DeleteConfirm, MessageBoxButton.OKCancel, MessageBoxImage.Information);

            if (res == MessageBoxResult.Cancel)
                return;

            try
            {
                ImgUtils img = new ImgUtils();
                img.Delete(img.UserImgs, model.LocalImg);
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