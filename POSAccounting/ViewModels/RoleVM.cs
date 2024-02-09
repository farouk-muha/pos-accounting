using POSAccounting.Models;
using POSAccounting.BL;
using POSAccounting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using POSAccounting.Views;

namespace POSAccounting.ViewModels
{

    public class LocalRolesVM : PagingM<LocalRoleM>
    {
        public CropAccountingAppEntities db;
        public LocalRoleBL bl;
        public LocalRolesVM(CropAccountingAppEntities db, LocalRoleBL bl)
        {
            Models = new ObservableCollection<LocalRoleM>();
            this.db = db;
            this.bl = bl;

        }

        public override void Load()
        {
            WinName = Properties.Resources.Roles;
            DisplayImg = ImgUtils.RoleIcon;
            Refresh();
        }
        public override void Refresh(object param = null)
        {
            try
            {
                var temp = bl.Get(null, CurPage, PageSize);
                Models = temp.Models;
                Count = temp.Count;
                CurPage = Models.Count > 0 ? 1 : 0;
                CountDisplay = Count.ToString();
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override void Add()
        {
            MyMainWindow.Instance.Role();
        }

        public override void Update(object id)
        {
            if (id == null || id.GetType() != typeof(Guid))
                return;

            MyMainWindow.Instance.Role((Guid)id);
        }

        public override void Delete(object id)
        {
        }

     
    }
}
