using CorpAccountingApp.ViewModels;
using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.Views;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POSAccounting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MyMainWindow : MetroWindow
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private ProfileVM model;
        public static MyMainWindow Instance { get; private set; }
        public static int CurrentWin;

        public static MyMainWindow ShowMe()
        {
            if (Instance == null)
            {
                Instance = new MyMainWindow();
                Instance.Closed += (senderr, args) => Instance = null;

                Instance.Show();
            }
            else
                Instance.Activate();
            return Instance;
        }

        public MyMainWindow()
        {
            Instance = this;
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            model = new ProfileVM(db);

            Loaded += MyWindow_Loaded;
        }


        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            model.GetProfile();
            this.DataContext = model;
            Main();
            titleTxt.Text = Properties.Resources.Home;
            setup_Timer();

        }

        static Timer timer;
        static public void setup_Timer()
        {
            var t = AppSettings.GetBackupTime();

            DateTime nowTime = DateTime.Now;
            DateTime scheduledTime = 
                new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, t, 0, 0, 0); 
            if (nowTime > scheduledTime)
                scheduledTime = scheduledTime.AddDays(1);

            double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;
            timer = new Timer(tickTime);
            timer.Elapsed += new ElapsedEventHandler(TimerElapse);
            timer.Start();
        }

        static void TimerElapse(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            setup_Timer();
        }

        public void Main()
        { 
            container.Content = null;
            container.Content = new Analysis();
        }

        //Invoices section 
        public void Invoice(byte typeId, Nullable<Guid> id = null)
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Invoices, ConstActionType.Insert))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Insert, "Invoice"), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            container.Content = null;
            if(typeId == ConstInvoiceType.Purchases || typeId == ConstInvoiceType.PurchasesReturns)
                container.Content = new InvoiceBuyUC(typeId, id);
            else
                container.Content = new InvoiceSellUC(typeId, id);

            model.Title = Properties.Resources.Invoice + " " +
                ConstInvoiceType.GetList().Where(m => m.Id == typeId).Select(m => m.Name).FirstOrDefault();
        }
        public void Invoices(byte typeId, Nullable<Guid> accId = null, bool? isaccClientDebt = null)
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Invoices, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Select, "Invoice"), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            container.Content = null;
            container.Content = new InvoicesUC(typeId, accId, isaccClientDebt);

            if (isaccClientDebt != null)
            {
                if ((bool)isaccClientDebt)
                    model.Title = Properties.Resources.InvoicesDueSoon;
                else
                    model.Title = Properties.Resources.BillsDueSoon;
            }else
                model.Title = Properties.Resources.Review + " " +
    ConstInvoiceType.GetListTitle().Where(m => m.Id == typeId).Select(m => m.Name).FirstOrDefault();
        }

        //public void InvoicesNotPaid(bool isClientDebt, Nullable<Guid> accId = null)
        //{
        //    if (!RoleHelper.chkUserLocalRole(db, ConstTask.Invoices, ConstActionType.Select))
        //    {
        //        MessageBox.Show(string.Format(ConstNotAllowed.Select, "Invoice"), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
        //        return;
        //    }

        //    container.Content = null;
        //    container.Content = new InvoicesNotPaidUC(isClientDebt, accId);
        //    if (isClientDebt)
        //        model.Title = Properties.Resources.InvoicesDueSoon;
        //    else
        //        model.Title = Properties.Resources.BillsDueSoon;
        //}


        //Receitpt section 
        public void Receipt(byte typeId, Nullable<Guid> accId = null)
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Receipts, ConstActionType.Insert))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Insert, "Receipt"), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            container.Content = null;
            container.Content = new ReceiptUC(typeId, accId);
            model.Title = ConstReceiptTypes.GetList().Where(m => m.Id == typeId).Select(m => m.Name).FirstOrDefault();
        }
        public void ReceiptDrwaing(Nullable<Guid> accId = null)
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Receipts, ConstActionType.Insert))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Insert, "Receipt"), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            container.Content = null;
            container.Content = new DrawingUC(accId);
            model.Title = Properties.Resources.Drawing;
        }
        public void Receitpts(byte typeId, Nullable<Guid> accId = null)
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Receipts, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Select, "Receipt"), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            container.Content = null;
            container.Content = new ReceiptsUC(typeId, accId);
            model.Title = Properties.Resources.Review + " " + 
                ConstReceiptTypes.GetListReview().Where(m => m.Id == typeId).Select(m => m.Name).FirstOrDefault();
        }


        //Receitpt Clients 
        public void Clients(byte typeId)
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Clients, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Select, Properties.Resources.Clients), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Clients;
            container.Content = null;
            container.Content = new ClientsUC(typeId);
            model.Title = Properties.Resources.Clients;

        }
        public void Client(Nullable<Guid> id = null, bool onlyShow = false)
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Clients, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Insert, Properties.Resources.AddNewClient), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CurrentWin = MainWinUtils.Client;
            ClientWin.ShowMe(id, onlyShow);
        }


        //section Products 
        public void Products()
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Clients, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Select, Properties.Resources.Products), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Products;
            container.Content = null;
            container.Content = new ProdutsUC();
            model.Title = Properties.Resources.Products;

        }

        public void Product(Nullable<Guid> id = null)
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Products, ConstActionType.Insert))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Insert, Properties.Resources.Product), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            container.Content = MainWinUtils.Product;
            container.Content = new ProductUC(id);
            if (id == null)
                model.Title = Properties.Resources.AddNewProduct;
            else
                model.Title = Properties.Resources.EditProduct;
        }
        public void ProductTrans()
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Clients, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Select, Properties.Resources.Products), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Products;
            container.Content = null;
            model.Title = Properties.Resources.ProductTrans;

        }
        public void Categories()
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Categories, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Insert, Properties.Resources.Categories), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Categories;
            container.Content = null;
            container.Content = new CategoriesUC();
            model.Title = Properties.Resources.Categories;
        }

        public void Stores()
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Stores, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Select, Properties.Resources.Stores), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Stores;
            container.Content = null;
            container.Content = new StoresUC();
            model.Title = Properties.Resources.Stores;
        }

        //section Journals 
        public void Journal(Nullable<Guid> id = null)
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Journals, ConstActionType.Insert))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Insert, Properties.Resources.Journal), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Journals;
            container.Content = null;
            model.Title = Properties.Resources.AddNewJournal;

        }
        public void Journals()
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Journals, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Select, Properties.Resources.Journals), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Journals;
            container.Content = null;
            model.Title = Properties.Resources.Journals;
        }

        public void AccountTransactions()
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Journals, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Select, Properties.Resources.Journals), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Journals;
            container.Content = null;
            model.Title = Properties.Resources.AccountTransactions;

        }

        //section Accounts 
        public void AccountsTree()
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Accounts, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Select, Properties.Resources.Accounts), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Accounts;
            container.Content = null;
            container.Content = new AccountsUC();
            model.Title = Properties.Resources.AccountsTree;
        }
        public void Account(Nullable<Guid> id = null)
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Accounts, ConstActionType.Insert))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Insert, Properties.Resources.Account), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Account;
            container.Content = null;
            container.Content = new AccountUC(id);
            if(id == null)
                model.Title = Properties.Resources.AddNewAccount;
            else
                model.Title = Properties.Resources.EditAccount;
        }

        //Reports section 
        public void Reports()
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Receipts, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Select, Properties.Resources.Reports), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            container.Content = null;
            container.Content = null;
            model.Title = Properties.Resources.Reports;
        }

        //admin secction methods
        public void Visas()
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Visas, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Select, Properties.Resources.Visas), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Visas;
            container.Content = null;
            container.Content = new VisasUC();
            model.Title = Properties.Resources.Visas;
        }

        public void Users()
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Users, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Select, Properties.Resources.Users), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Users;
            container.Content = null;
            container.Content = new UsersUC();
            model.Title = Properties.Resources.Users;
        }

        public void User(int id = 0)
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Users, ConstActionType.Insert))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Insert, Properties.Resources.User), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Users;
            container.Content = null;
            container.Content = new UserUC(id);
            if (id > 0)
                model.Title = Properties.Resources.AddNewUser;
            else
                model.Title = Properties.Resources.EditUser;
        }

        public void Roles()
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Users, ConstActionType.Select))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Select, Properties.Resources.Roles), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Roles;
            container.Content = null;
            container.Content = new RolesUC();
            model.Title = Properties.Resources.Roles;
        }
        public void Role(Nullable<Guid> id = null)
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Users, ConstActionType.Insert))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Insert, Properties.Resources.Role), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Roles;
            container.Content = null;
            model.Title = Properties.Resources.AddNewRole;
        }

        //settings secction methods

        public void Settings()
        {
            if (!RoleHelper.chkUserLocalRole(db, ConstTask.Settings, ConstActionType.Update))
            {
                MessageBox.Show(string.Format(ConstNotAllowed.Update, Properties.Resources.Settings), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentWin = MainWinUtils.Settings;
            container.Content = null;
            model.Title = Properties.Resources.Settings;
        }
        private void MouseLeft_TreeView(object sender, MouseButtonEventArgs e)
        {
            string s = ((TreeViewItem)sender).Header.ToString();
            if (s.Equals(Properties.Resources.Home))
            {
                Main();
                titleTxt.Text = s;
            }
            else if (s.Equals(Properties.Resources.Settings))
            {
                Settings();
                titleTxt.Text = s;
            }
        }

        private void MouseLeft_TreeViewItem(object sender, MouseButtonEventArgs e)
        {
            string s = ((TreeViewItem)sender).Header.ToString();

             if (s.Equals(Properties.Resources.Home))
            {
                Main();
            }
            else if (s.Equals(Properties.Resources.Sale))
            {
                Invoice(ConstInvoiceType.Sales);
            }
            else if (s.Equals(Properties.Resources.SaleReturn))
            {
                Invoice(ConstInvoiceType.SalesReturns);
            }
            else if (s.Equals(Properties.Resources.Purchase))
            {
                Invoice(ConstInvoiceType.Purchases);
            }
            else if (s.Equals(Properties.Resources.PurchaseReturn))
            {
                Invoice(ConstInvoiceType.PurchasesReturns);
            }
            else if (s.Equals(Properties.Resources.Sales))
            {
                Invoices(ConstInvoiceType.Sales);
            }
            else if (s.Equals(Properties.Resources.SalesReturns))
            {
                Invoices(ConstInvoiceType.SalesReturns);
            }
            else if (s.Equals(Properties.Resources.Purchases))
            {
                Invoices(ConstInvoiceType.Purchases);
            }
            else if (s.Equals(Properties.Resources.PurchasesReturns))
            {
                Invoices(ConstInvoiceType.PurchasesReturns);
            }
            else if (s.Equals(Properties.Resources.Catch))
            {
                Receipt(ConstReceiptTypes.CatchReceipt);
            }
            else if (s.Equals(Properties.Resources.pay))
            {
                Receipt(ConstReceiptTypes.PayReceipt);
            }
            else if (s.Equals(Properties.Resources.Revenue))
            {
                Receipt(ConstReceiptTypes.Revenues);
            }
            else if (s.Equals(Properties.Resources.Expense))
            {
                Receipt(ConstReceiptTypes.Expenses);
            }
            else if (s.Equals(Properties.Resources.Drawing))
            {
                ReceiptDrwaing();
            }
            else if (s.Equals(Properties.Resources.Catchs))
            {
                Receitpts(ConstReceiptTypes.CatchReceipt);
            }
            else if (s.Equals(Properties.Resources.pays))
            {
                Receitpts(ConstReceiptTypes.PayReceipt);
            }
            else if (s.Equals(Properties.Resources.Revenues))
            {
                Receitpts(ConstReceiptTypes.Revenues);
            }
            else if (s.Equals(Properties.Resources.Expenses))
            {
                Receitpts(ConstReceiptTypes.Expenses);
            }
            else if (s.Equals(Properties.Resources.Drawings))
            {
                Receitpts(ConstReceiptTypes.Drawings);
            }
            else if (s.Equals(Properties.Resources.Suppliers))
            {
                Clients(ConstClientTypes.Supplier);
            }
            else if (s.Equals(Properties.Resources.Customers))
            {
                Clients(ConstClientTypes.Customer);
            }
            else if (s.Equals(Properties.Resources.Both))
            {
                Clients(ConstClientTypes.Both);
            }
            else if (s.Equals(Properties.Resources.AddNewClient))
            {
                Client();
            }
            else if (s.Equals(Properties.Resources.Products))
            {
                Products();
            }
            else if (s.Equals(Properties.Resources.ProductTrans))
            {
                ProductTrans();
            }
            else if (s.Equals(Properties.Resources.AddNewProduct))
            {
                Product();
            }
            else if (s.Equals(Properties.Resources.Categories))
            {
                Categories();
            }
            else if (s.Equals(Properties.Resources.Stores))
            {
                Stores();
            }
            else if (s.Equals(Properties.Resources.AccountsTree))
            {
                AccountsTree();
            }
            else if (s.Equals(Properties.Resources.AddNewAccount))
            {
                Account();
            }
            else if (s.Equals(Properties.Resources.Journals))
            {
                Journals();
            }
            else if (s.Equals(Properties.Resources.AccountTransactions))
            {
                AccountTransactions();
            }
            else if (s.Equals(Properties.Resources.AddNewJournal))
            {
                Journal();
            }
            else if (s.Equals(Properties.Resources.Reports))
            {
                Reports();
            }
            //admin section
            else if (s.Equals(Properties.Resources.Users))
            {
                Users();
            }
            else if (s.Equals(Properties.Resources.Roles))
            {
                Roles();
            }
            //settings section
            else if (s.Equals(Properties.Resources.Visas))
            {
                Visas();
            }

        }

        private void profileMenu_Click(object sender, RoutedEventArgs e)
        {
            User(Profile.UserProfile.Id);
        }

        private void exitMenu_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }










    public class ExTreeView : TreeView
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Loaded += (sL, eL) =>
            {
                foreach (var item in Items)
                {
                    var tvi = ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                    if (null != tvi)
                        tvi.Expanded += (s, e) => ManageExpanded(tvi);
                }
            };
        }

        private void ManageExpanded(TreeViewItem tvi)
        {
            foreach (var item in Items)
            {
                var other = ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (other != tvi)
                    other.IsExpanded = false;
            }
        }
    }
}
