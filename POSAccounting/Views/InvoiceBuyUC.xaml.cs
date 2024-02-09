using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POSAccounting.Views
{
    /// <summary>
    /// Interaction logic for InvoiceUC.xaml
    /// </summary>
    public partial class InvoiceBuyUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private InvoiceVM model;
        private InvoiceBL bl;
        private ClientBL clientBL;
        private ClientUtils clientUtils = new ClientUtils();
        private ProductBL productBl;
        private Nullable<Guid>id = null;
        private bool isEdt = false;

        public InvoiceBuyUC(byte typeId, Nullable<Guid> id = null)
        {
            this.id = id;
            bl = new InvoiceBL(db);
            clientBL = new ClientBL(db);
            productBl = new ProductBL(db);
            model = new InvoiceVM(db, bl);
            model.Model.InvoiceTypeId = typeId;
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }

        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (id != null)
                {
                    await Task.Delay(20);

                    isEdt = true;
                    model.SetEdt();
                    model.Model = bl.GetById((Guid)id);

                    if (model.Model == null)
                    {
                        return;
                    }
                    txtDate.IsEnabled = false;

                    model.Model.Discount = 0;
                    model.Model.Payed = 0;
                    model.Model.Debt = 0;
                    model.Model.TotalPrice = 0;
                    model.Model.TotalQty = 0;

                }
                else
                    model.SetNew();

                if (model.Stores.Count() == 1)
                    stackStore.Visibility = Visibility.Collapsed;

                if (model.Model.InvoiceTypeId == ConstInvoiceType.PurchasesReturns)
                {
                    var style = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                    style.BasedOn = this.TryFindResource("DGHeaderStyleReturns") as Style;
                    dg.ColumnHeaderStyle = style;
                    GridHeader.Background = (Brush)this.TryFindResource("blue2Brush");
                    GridDG.Background = (Brush)this.TryFindResource("blue3Brush");
                    addBtn.Background = (Brush)this.TryFindResource("blue2Brush");
                }
            }
            catch
            {
            }

            initBtns();
            this.DataContext = model;
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            addLine();
        }
        private void addLine()
        {
            BindingExpression qty = txtQty.GetBindingExpression(TextBox.TextProperty);
            qty.UpdateSource();

            if (model.InvLineToAdd.Product == null || model.InvLineToAdd.Product.ProductUnits == null ||
           model.InvLineToAdd.Product.ProductUnits.Count == 0 || qty.HasError)
            {
                return;
            }

            var temp = model.InvLineToAdd.Product.ProductUnits;
            foreach (var v in model.Model.InvoiceLines)
            {
                if (v.ProductUnitId == temp[0].Id)
                {
                    v.QTY += model.InvLineToAdd.QTY;
                    model.UpdateTotalSum();
                    model.RestInvLineToAdd();
                    return;
                }
                    
            }
            model.InvLineToAdd.Price = temp[0].PriceSale;
            model.InvLineToAdd.ProductUnitId = temp[0].Id;
            model.InvLineToAdd.ProductUnit = temp[0];

            model.Model.InvoiceLines.Add(model.InvLineToAdd);
            model.UpdateTotalSum();

            model.RestInvLineToAdd();
            coProduct.Text = string.Empty;
        }


        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            string msg = ConstUserMsg.FaildProcess;
            try
            {
                //chek pay
                if (model.Model.IsPayed && model.Model.TotalPriceSubDisc != model.Model.Pay)
                {
                    msg = ConstUserMsg.MustPayTotalPrice;
                    throw new Exception();
                }
                //chek datagrid fields
                var errors = (from c in
                      (from object i in dg.ItemsSource
                       select dg.ItemContainerGenerator.ContainerFromItem(i))
                              where c != null
                              select Validation.GetHasError(c))
                 .FirstOrDefault(x => x);

                //chek fields
                BindingExpression num = txtNum.GetBindingExpression(TextBox.TextProperty);
                num.UpdateSource();
                BindingExpression phone = txtPhone.GetBindingExpression(TextBox.TextProperty);
                phone.UpdateSource();

                if (errors || num.HasError || phone.HasError)
                {
                    saveBtn.IsEnabled = true;
                    return;
                }
                if (model.Model.InvoiceLines.Count() == 0)
                {
                    msg = ConstUserMsg.AddInvoiceLines;
                    throw new Exception();
                }

                //chk cleint 
                if (model.Model.Client == null)
                {
                    if (!model.Model.IsPayed)
                    {
                        msg = ConstUserMsg.ClienNotChosenError;
                        throw new Exception();
                    }
                }
                else
                {
                    model.Model.AccountId = model.Model.Client.AccountId;
                    model.Model.ClientName = null;
                    model.Model.ClientPhone = null;
                }

                //chk num exist
                var v = bl.Get(m => m.Id != model.Model.Id && m.Num == model.Model.Num).FirstOrDefault();
                if (v != null)
                {
                    msg = ConstUserMsg.NameExist;
                    throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show(msg, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                saveBtn.IsEnabled = true;
                return;
            }

            if(model.Model.Visa > 0)
                model.Model.IsCash = false;
            else
                model.Model.IsCash = true;

            model.Model.UserId = Profile.UserProfile.Id;
            model.Model.Amount = model.Model.TotalPriceSubDisc;
            model.Model.Payed = model.Model.Pay;
            model.Model.IsRemind = !model.Model.IsPayed;

            if (model.Model.InvoiceTypeId == ConstInvoiceType.Purchases)
                savePurchases();
            else
                savePurchasesReturns();
        }

        private void savePurchases()
        {
            InvoicelineBL lineBL = new InvoicelineBL(db);
            JournalBL journalBL = new JournalBL(db);
            JournalDetBL detBL = new JournalDetBL(db);

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    // invoice 
                    if (isEdt)
                    {
                        //delete journal
                        var journalToDet = journalBL.Get(m => m.RefId == model.Model.Id).FirstOrDefault();
                        if (journalToDet == null)
                            throw new Exception();
                        foreach (var v in journalToDet.JournalDets)
                            detBL.Delete(v.Id);
                        journalBL.Delete(journalToDet.Id);

                        //delete inv lines
                        var linesToDelete = lineBL.Get(m => m.InvoiceId == model.Model.Id);
                        foreach (var v in linesToDelete)
                        {
                            lineBL.Delete(v.Id);
                        }

                        // update receipt
                        bl.Update(model.Model);
                    }
                    else
                    {
                        model.Model.Id = Guid.NewGuid();
                        model.Model.InvoiceTypeId = ConstInvoiceType.Purchases;
                        model.Model.UserId = 1;
                        bl.Insert(model.Model);
                    }

                    //invoice lines
                    var productFun = new ProductFun();
                    var productBL = new ProductBL(db);
                    foreach (var v in model.Model.InvoiceLines)
                    {
                        v.InvoiceId = model.Model.Id;
                        v.Id = Guid.NewGuid();
                        lineBL.Insert(v);

                        //update products qty
                        var unit = v.Product.ProductUnits.Where(m => m.UnitId == v.Product.DefaultUnitId).FirstOrDefault();
                        var q = productFun.GetQTY(unit.Multiplier, v.ProductUnit.Multiplier, v.QTY);
                        productBL.UpdateQTY(v.Product.Id, q);
                    }

                    //journal
                    JournalM journal = new JournalM()
                    {
                        Id = Guid.NewGuid(),
                        Num = journalBL.GetNewNum(),
                        Date = model.Model.Date,
                        JournalTypeId = ConstJournalTypes.Invoice,
                        RefId = model.Model.Id,
                        UserId = Profile.UserProfile.Id,
                    };

                    journalBL.Insert(journal);

                    //journal det detp part Inventory
                    var lastNum = detBL.GetNewNum(detBL.GetLastNum());
                    JournalDetM dept = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = lastNum,
                        JournalId = journal.Id,
                        AccountID = ConstAccounts.Inventory,
                        Debt = (model.Model.Amount - model.Model.Tax),
                    };

                    detBL.Insert(dept);

                    //journal det detp part Tax
                    if (model.Model.Tax > 0)
                    {
                        JournalDetM credit = new JournalDetM()
                        {
                            Id = Guid.NewGuid(),
                            Num = ++lastNum,
                            JournalId = journal.Id,
                            AccountID = ConstAccounts.PurchaseTax,
                            Debt = model.Model.Tax,
                        };
                        detBL.Insert(credit);
                    }

                    //journal det credit part if visa add det with bacnk account
                    if (model.Model.VisaId != null && model.Model.Visa > 0)
                    {
                        JournalDetM credit = new JournalDetM()
                        {
                            Id = Guid.NewGuid(),
                            Num = ++lastNum,
                            JournalId = journal.Id,
                            AccountID = (Guid)model.Model.VisaId,
                            Credit = model.Model.Visa,                            
                        };
                        detBL.Insert(credit);
                    }
                    //if cash add credit det with box safe account
                    if (model.Model.Cash > 0)
                    {
                        JournalDetM credit = new JournalDetM()
                        {
                            Id = Guid.NewGuid(),
                            Num = ++lastNum,
                            JournalId = journal.Id,
                            AccountID = ConstAccounts.BoxSafe,
                            Credit = model.Model.Cash,
                        };
                        detBL.Insert(credit);
                    }

                    //journal det credit part if not cash add det with client account 
                    if (!model.Model.IsPayed)
                    {
                        JournalDetM credit = new JournalDetM()
                        {
                            Id = Guid.NewGuid(),
                            Num = ++lastNum,
                            JournalId = journal.Id,
                            AccountID = model.Model.Client.AccountId,
                            Credit = (model.Model.Amount - model.Model.Pay),
                        };
                        detBL.Insert(credit);
                    }

                    dbContextTransaction.Commit();
                    MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void savePurchasesReturns()
        {
            InvoicelineBL lineBL = new InvoicelineBL(db);
            JournalBL journalBL = new JournalBL(db);
            JournalDetBL detBL = new JournalDetBL(db);

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    // invoice 
                    if (isEdt)
                    {
                        //delete journal
                        var journalToDet = journalBL.Get(m => m.RefId == model.Model.Id).FirstOrDefault();
                        if (journalToDet == null)
                            throw new Exception();
                        foreach (var v in journalToDet.JournalDets)
                            detBL.Delete(v.Id);
                        journalBL.Delete(journalToDet.Id);

                        //delete inv lines
                        var linesToDelete = lineBL.Get(m => m.InvoiceId == model.Model.Id);
                        foreach (var v in linesToDelete)
                        {
                            lineBL.Delete(v.Id);
                        }

                        // update receipt
                        bl.Update(model.Model);
                    }
                    else
                    {
                        model.Model.Id = Guid.NewGuid();
                        model.Model.InvoiceTypeId = ConstInvoiceType.Purchases;
                        model.Model.UserId = 1;
                        bl.Insert(model.Model);
                    }

                    //invoice lines
                    var productFun = new ProductFun();
                    var productBL = new ProductBL(db);
                    foreach (var v in model.Model.InvoiceLines)
                    {
                        v.InvoiceId = model.Model.Id;
                        v.Id = Guid.NewGuid();
                        lineBL.Insert(v);

                        //update products qty
                        var unit = v.Product.ProductUnits.Where(m => m.UnitId == v.Product.DefaultUnitId).FirstOrDefault();
                        var q = productFun.GetQTY(unit.Multiplier, v.ProductUnit.Multiplier, v.QTY);
                        productBL.UpdateQTY(v.Product.Id, q);
                    }

                    //journal
                    JournalM journal = new JournalM()
                    {
                        Id = Guid.NewGuid(),
                        Num = journalBL.GetNewNum(),
                        Date = model.Model.Date,
                        JournalTypeId = ConstJournalTypes.Invoice,
                        RefId = model.Model.Id,
                        UserId = Profile.UserProfile.Id,
                    };

                    journalBL.Insert(journal);

                    //journal det credit part
                    var lastNum = detBL.GetNewNum(detBL.GetLastNum());
                    JournalDetM dept = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = lastNum,
                        JournalId = journal.Id,
                        AccountID = ConstAccounts.Inventory,
                        Credit = (model.Model.Amount - model.Model.Tax),
                    };

                    detBL.Insert(dept);

                    //journal det credit part Tax
                    if (model.Model.Tax > 0)
                    {
                        JournalDetM credit = new JournalDetM()
                        {
                            Id = Guid.NewGuid(),
                            Num = ++lastNum,
                            JournalId = journal.Id,
                            AccountID = ConstAccounts.PurchaseTax,
                            Credit = model.Model.Tax,
                        };
                        detBL.Insert(credit);
                    }

                    //journal det Debt part if visa add det with bacnk account, 
                    if (model.Model.VisaId != null && model.Model.Visa > 0)
                    {
                        JournalDetM credit = new JournalDetM()
                        {
                            Id = Guid.NewGuid(),
                            Num = ++lastNum,
                            JournalId = journal.Id,
                            AccountID = (Guid)model.Model.VisaId,
                            Debt = model.Model.Visa,
                        };
                        detBL.Insert(credit);
                    }
                    //if cash add det with box safe account
                    if (model.Model.Cash > 0)
                    {
                        JournalDetM credit = new JournalDetM()
                        {
                            Id = Guid.NewGuid(),
                            Num = ++lastNum,
                            JournalId = journal.Id,
                            AccountID = ConstAccounts.BoxSafe,
                            Debt = model.Model.Cash,
                        };
                        detBL.Insert(credit);
                    }

                    //journal det credit part if not cash add det with client account 
                    if (!model.Model.IsPayed)
                    {
                        JournalDetM credit = new JournalDetM()
                        {
                            Id = Guid.NewGuid(),
                            Num = ++lastNum,
                            JournalId = journal.Id,
                            AccountID = model.Model.Client.AccountId,
                            Debt = (model.Model.Amount - model.Model.Pay),
                        };
                        detBL.Insert(credit);
                    }

                    dbContextTransaction.Commit();
                    MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        /// /////////////////////////////////
        /// Search Client 
        #region
        private void coClient_DropDownOpened(object sender, EventArgs e)
        {
            coClient.IsDropDownOpen = false;
            if (brClientsSearch.Visibility == Visibility.Collapsed)
            {
                brClientsSearch.Visibility = Visibility.Visible;
                var temp = clientBL.Get(null);
                model.Clients = new ObservableCollection<ClientM>();
                foreach (var vv in temp)
                    model.Clients.Add(clientUtils.FromEntity(vv));
            }
            else
            {
                brClientsSearch.Visibility = Visibility.Collapsed;
                txtClientSearch.Text = string.Empty;
            }
        }
        private void coClient_KeyUp(object sender, KeyEventArgs e)
        {
            if (model.Model.Client != null)
            {
                model.Model.Client = null;
                model.Model.ClientName = null;
                model.Model.ClientPhone = null;
            }
        }
        private void txtClientSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            string s = txtClientSearch.Text;
            if (string.IsNullOrEmpty(s))
                return;

            s = s.Trim();
            int i;
            int.TryParse(s, out i);
            IEnumerable<Client> temp;
            if (i == 0)
                temp = clientBL.Get(m => m.Name.StartsWith(s));
            else
                temp = clientBL.Get(m => m.Num == i);

            model.Clients = new ObservableCollection<ClientM>();
            foreach (var vv in temp)
                model.Clients.Add(clientUtils.FromEntity(vv));
        }
        private void lbClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (model.Clients != null && lbClients.SelectedItem != null && lbClients.SelectedItem.GetType() == typeof(ClientM))
            {
                model.Model.Client = (ClientM)lbClients.SelectedItem;
                model.Model.ClientName = model.Model.Client.Name;
                model.Model.ClientPhone = model.Model.Client.Phone;
                txtClientSearch.Text = string.Empty;
                coClient.Text = model.Model.Client.Name;
                brClientsSearch.Visibility = Visibility.Collapsed;
            }

        }
        #endregion

        /// /////////////////////////////////
        /// Search product 
        #region
        private void coProduct_KeyUp(object sender, KeyEventArgs e)
        {
            string s = null;
            if (e.Key == Key.Enter)
            {
                ProductM temp = null;
                s = coProduct.Text;
                if (string.IsNullOrEmpty(s))
                    return;
                s = s.Trim();
                try
                {
                    temp = productBl.GetModels(m => m.Code.Equals(s)).FirstOrDefault();
                    if (temp == null)
                        temp = productBl.GetModels(m => m.Name.StartsWith(s)).FirstOrDefault();


                    if (temp != null)
                    {
                        SetProduct(temp);
                    }
                    else
                        model.InvLineToAdd.QTY = 0;
                }
                catch
                {

                }
                return;
            }
            else
            {
                if (model.InvLineToAdd.Product != null)
                {
                    model.InvLineToAdd.Product = null;
                    model.InvLineToAdd.QTY = 0;
                }
            }
        }
        private void coProduct_DropDownOpened(object sender, EventArgs e)
        {
            coProduct.IsDropDownOpen = false;
            if (brProductSearch.Visibility == Visibility.Collapsed)
            {
                brProductSearch.Visibility = Visibility.Visible;
                model.Products = productBl.GetModels(null);
            }
            else
            {
                brProductSearch.Visibility = Visibility.Collapsed;
                txtProductSearch.Text = string.Empty;
            }
        }
        private void txtProductSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {

            string s = txtProductSearch.Text;
            if (string.IsNullOrEmpty(s))
                return;

            s = s.Trim();
            model.Products = productBl.GetModels(m => m.Code.Equals(s) || m.Name.StartsWith(s));

        }
        private void lbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (model.Products != null && lbProducts.SelectedItem != null && lbProducts.SelectedItem.GetType() == typeof(ProductM))
            {
                SetProduct((ProductM)lbProducts.SelectedItem);
                brProductSearch.Visibility = Visibility.Collapsed;
                txtProductSearch.Text = string.Empty;
            }

        }

        private void SetProduct(ProductM product)
        {
            model.InvLineToAdd.Product = product;
            model.InvLineToAdd.QTY = 1;
            coProduct.Text = model.InvLineToAdd.Product.Name;
        }
        #endregion

        //////////////////////////////////////
        /// datagrid
        /// 
        private void dg_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (model.SelectedInv == null)
                return;

            model.UpdateTotalSum();
        }

        //visa dialog
        private void VisaBtn_Click(object sender, RoutedEventArgs e)
        {
            if (model.Model.TotalPrice == 0)
                return;

            var dialog = new VisaPayWin(true, model.Model.TotalPriceSubDisc, model.Model.Visa, model.Model.VisaId);
            if (dialog.ShowDialog() == true)
            {
                if (dialog.model.VisaId == null)
                    dialog.model.Amount = 0;

                var p = dialog.model.Amount + model.Model.Cash;
                if (p > model.Model.TotalPriceSubDisc)
                    return;

                model.Model.Visa = dialog.model.Amount;
                model.Model.VisaId = dialog.model.VisaId;
                model.Model.Pay = p;
                model.DeptUpdate();
            }
        }

        //cash dialog
        private void CashBtn_Click(object sender, RoutedEventArgs e)
        {
            if (model.Model.TotalPrice == 0)
                return;

            var dialog = new CashPayWin(model.Model.TotalPriceSubDisc, model.Model.Cash);
            if (dialog.ShowDialog() == true)
            {
                var p = dialog.model.Amount + model.Model.Visa;
                if (p > model.Model.TotalPriceSubDisc)
                    return;

                model.Model.Cash = dialog.model.Amount;
                model.Model.Pay = p;
                model.DeptUpdate();
            }
            }

        //discount dialog
        private void DescountBtn_Click(object sender, RoutedEventArgs e)
        {
            if (model.Model.TotalPrice == 0)
                return;
            var t = model.Model.TotalPrice;

            var dialog = new DiscountWin(model.Model.TotalPrice, model.Model.Discount);
            if (dialog.ShowDialog() == true)
            {
                if (dialog.model.Amount >= t)
                    return;
                model.Model.Discount = dialog.model.Amount;
                model.DiscountPercent = dialog.model.Percent;

                model.Model.Tax = MathFun.GetAmount(model.Model.TotalPriceSubDisc, model.TaxPercent);
                model.DeptUpdate();

            }
        }
        private void btnTax_Click(object sender, RoutedEventArgs e)
        {
            if (model.Model.TotalPrice == 0)
                return;
            var dialog = new TaxWin(model.TaxPercent);
            if (dialog.ShowDialog() == true)
            {
                model.TaxPercent = dialog.model.Percent;
                model.Model.Tax = MathFun.GetAmount(model.Model.TotalPriceSubDisc, dialog.model.Percent);
                model.DeptUpdate();
            }

        }

        private void initBtns()
        {
            var db = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
            db.AddValueChanged(txtPay, (se, args) =>
            {
                var send = (TextBlock)se;
                if (send == null)
                    return;
                var s = send.Text;
                if (string.IsNullOrWhiteSpace(s))
                    return;

                decimal d = 0; decimal.TryParse(s, out d);
                if (d == 0)
                {
                    if (btnTax.IsEnabled == false)
                        btnTax.IsEnabled = true;
                    if (DescountBtn.IsEnabled == false)
                        DescountBtn.IsEnabled = true;
                }
                else
                {
                    if (btnTax.IsEnabled == true)
                        btnTax.IsEnabled = false;
                    if (DescountBtn.IsEnabled == true)
                        DescountBtn.IsEnabled = false;
                }
            });

            var dbTotal = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
            dbTotal.AddValueChanged(txtblkPay, (se, args) =>
            {
                var send = (TextBlock)se;
                if (send == null)
                    return;
                var s = send.Text;
                if (string.IsNullOrWhiteSpace(s))
                    return;

                decimal d = 0; decimal.TryParse(s, out d);
                if (d > 0)
                {
                    if (saveBtn.IsEnabled == false)
                        saveBtn.IsEnabled = true;
                    if (savePrintBtn.IsEnabled == false)
                        savePrintBtn.IsEnabled = true;
                }
                else
                {
                    if (saveBtn.IsEnabled == true)
                        saveBtn.IsEnabled = false;
                    if (savePrintBtn.IsEnabled == true)
                        savePrintBtn.IsEnabled = false;
                }
            });
        }
    }

}
