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
    public class AccountVM : ViewModelBase
    {
        public CropAccountingAppEntities db;
        private AccountUtils utils = new AccountUtils();
        private AccountBL bl;
        private AccountM model;        
        private ObservableCollection<AccountM> accounts;

        public AccountM Model { get { return model; } set { model = value; NotifyPropertyChanged("Model"); } }
        public ObservableCollection<AccountM> Accounts { get { return accounts; } set { accounts = value; NotifyPropertyChanged("Accounts"); } }
        public ObservableCollection<AccountEnd> AccountEnds { get;  set;  }


        public AccountVM(CropAccountingAppEntities db, AccountBL bl)
        {
            this.db = db;
            this.bl = bl;
            Model = new AccountM();
        }

        public void Load()
        {
            var temp = db.AccountEnds;
            AccountEnds = new ObservableCollection<AccountEnd>();
            foreach (var v in temp)
                AccountEnds.Add(new AccountEnd() { Id = v.Id, Name = v.Name, });

            Accounts = bl.GetModels(null);
            //Accounts = new ObservableCollection<AccountM>();
            //foreach (var v in acc)
            //{
            //    if (v.AccountParentId == null)
            //        Accounts.Add(v);
            //    else
            //    {
            //        var a = acc.Where(m => m.Id == v.AccountParentId).FirstOrDefault();
            //        if (a.AccountParentId == null)
            //            Accounts.Add(v);
            //    }
            //}
        }
        public void Set()
        {
            Model.AccountEndId = AccountEnds[0].Id;
            Model.EnName = string.Empty;
            Model.ArName = string.Empty;            
        }
    }

    public class AccountsVM : PagingM<AccountM>
    {
        public CropAccountingAppEntities db;
        public AccountBL bl;
        private ObservableCollection<AccountM> accounts;
        public ObservableCollection<AccountM> Accounts { get { return accounts; } set { accounts = value; NotifyPropertyChanged("accounts"); } }
        public AccountsVM(CropAccountingAppEntities db, AccountBL bl)
        {
            Accounts = new ObservableCollection<AccountM>();
            this.db = db;
            this.bl = bl;
        }

        public override void Load()
        {
            Refresh();
        }
        public override void Refresh(object param = null)
        {
            try
            {
                List<AccountM> ids = new List<AccountM>()
                    {new AccountM {Id = ConstAccounts.Assests}, new AccountM {Id = ConstAccounts.Liabilities, Credit = 1},
                    new AccountM {Id = ConstAccounts.Revenue, Credit = 1}, new AccountM {Id = ConstAccounts.Expenses}};

                var v = bl.GetTree(bl.GetModels(StartDate, EndDate), ids);
                Accounts = v.Models;
                Credit = v.Credit;
                Debt = v.Debt;
                Count = v.Count;
                CountDisplay = Count.ToString();
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public override void Add()
        {
            MyMainWindow.Instance.Account();
        }

        public override void Update(object id)
        {
            if (id == null || id.GetType() != typeof(Guid))
                return;

            MyMainWindow.Instance.Account((Guid)id);
        }

        public override void Delete(object id)
        {
            if (id == null || id.GetType() != typeof(Guid))
                return;

            Guid idd = (Guid)id;
            foreach (var v in Accounts)
            {
                if (v.Id == idd)
                {
                    DelAcc(v);
                    goto re;
                }
                foreach (var v2 in v.Accounts)
                {
                    if (v2.Id == idd)
                    {
                        DelAcc(v2);
                        goto re;
                    }
                    foreach (var v3 in v2.Accounts)
                    {
                        if (v3.Id == idd)
                        {
                            DelAcc(v3);
                            goto re;
                        }

                        foreach (var v4 in v3.Accounts)
                        {
                            if (v4.Id == idd)
                            {
                                DelAcc(v4);
                                goto re;
                            }
                        }
                    }
                }
            }

            re:
            Refresh();

        }

        public void DelAcc(AccountM model)
        {
            var res = MessageBox.Show(string.Format(ConstUserMsg.DeleteConfirmMsg, Properties.Resources.TheAccount 
                + " \"" + model.EnName + "\"")
                + "\n \n " + Properties.Resources.AccDelNotAllowed,
               ConstUserMsg.DeleteConfirm, MessageBoxButton.OKCancel, MessageBoxImage.Information);

            if (res == MessageBoxResult.Cancel)
                return;

            string msg = ConstUserMsg.FaildProcess;

            try
            {
                if (!model.Deleteable)
                {
                    msg = ConstUserMsg.AccDelNotAllowed;
                    throw new Exception();
                }

                bl.Delete(model.Id);
                Accounts.Remove(model);
                Count--;
                MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show(msg, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class AccountType
    {
        public static byte Public { get; } = 1;
        public static byte Visa { get; } = 2;
    }
}
