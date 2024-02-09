using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.Entity.Validation;
using System.IO;
using System.Collections.ObjectModel;

namespace POSAccounting
{
    /// <summary>
    /// Interaction logic for UserWin.xaml
    /// </summary>
    public partial class ProductUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private ProductVM model;
        private ProductBL bl;      
        private Nullable<Guid> id;
        private bool isEdt = false;
        private ImgUtils imgutils = new ImgUtils();
        private bool isImgUpdate = false;
        private ProductFun productFun;
        

        public ProductUC(Nullable<Guid> id = null)
        {
            this.id = id;
            productFun = new ProductFun();
            bl = new ProductBL(db);
            model = new ProductVM(db, bl);

            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }


        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                model.Load();
                if (id != null)
                {
                    await Task.Delay(20);

                    model.Model = bl.GetById((Guid)id);
                    if (model.Model == null)
                    {
                        return;
                    }

                    model.SetQtyUnits();
                    model.QTY = model.Model.QTY;

                    foreach (var v in model.Model.ProductUnits)
                    {
                        if (v.UnitId == model.Model.DefaultUnitId)
                        {
                            v.IsDefault = true;
                            cbQtyUnits.SelectedValue = v.UnitId;
                        }
                    }


                    isEdt = true;
                    //chek local img 
                    if (!String.IsNullOrEmpty(model.Model.LocalImg))
                    {
                        var path = imgutils.GetImgFullPath(model.Model.LocalImg, imgutils.ProductImgs);
                        if (File.Exists(path))
                            model.Model.DisplayImg = path;

                    }
                }
                else
                {
                    model.Set();
                }
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (string.IsNullOrEmpty(model.Model.DisplayImg))
                model.Model.DisplayImg = ImgUtils.ProductIcon;

            this.DataContext = model;
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            var errors = (from c in
                  (from object i in dg.ItemsSource
                   select dg.ItemContainerGenerator.ContainerFromItem(i))
                          where c != null
                          select Validation.GetHasError(c))
             .FirstOrDefault(x => x);
            
            //chek fields
            BindingExpression code = txtCode.GetBindingExpression(TextBox.TextProperty);
            code.UpdateSource();
            BindingExpression name = txtName.GetBindingExpression(TextBox.TextProperty);
            name.UpdateSource();


            if (errors || code.HasError || name.HasError)
            {
                saveBtn.IsEnabled = true;
                return;
            }


            //////////////////////////////////////////////////////
            string msg = ConstUserMsg.FaildProcess;
            try
            {
                //check duplicate units
                var anyDuplicate = model.Model.ProductUnits.GroupBy(x => x.UnitId).Any(g => g.Count() > 1);
                if (anyDuplicate)
                {
                    msg = ConstUserMsg.DuplicateUnits;
                    throw new Exception();
                }

                //chek product code, name is exist
                var p = bl.Get(m => m.Code.Equals(model.Model.Code) || m.Name.Equals(model.Model.Name)).FirstOrDefault();
                if (p != null && p.Id != model.Model.Id)
                {
                    if (p.Code.Equals(model.Model.Code))
                        msg = ConstUserMsg.CodeExist;
                    else
                        msg = ConstUserMsg.NameExist;

                    throw new Exception();
                }


                //set defual unit id and qty
                var to = model.Model.ProductUnits.Where(m => m.IsDefault).FirstOrDefault();
                model.Model.DefaultUnitId = (byte)to.UnitId;

                if (model.QTY > 0)
                {
                    //var from = model.Model.ProductUnits.Where(m => m.UnitId == model.QtyUnitId).FirstOrDefault();
                    //model.Model.QTY = new ProductFun().GetQTY(to.Multiplier, from.Multiplier, model.QTY);
                    //model.QTYPrice =(decimal) model.QTY * to.PriceBuy;
                }
                else
                    model.QTYPrice = 0;
                model.Model.QTY = model.QTY;

                //save image and delete old one
                if (isImgUpdate)
                    model.Model.LocalImg = imgutils.SaveImg(model.Model.DisplayImg, imgutils.ProductImgs, model.Model.LocalImg);
            }
            catch
            {
                MessageBox.Show(msg, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                saveBtn.IsEnabled = true;
                return;
            }

            JournalBL journalBL = new JournalBL(db);
            JournalDetBL detBL = new JournalDetBL(db);

            model.Model.CorpId = Profile.CorpProfile.Id;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    // update with void return or add with int return
                    ProductUnitBL productUnits = new ProductUnitBL(db);
                    if (!isEdt)
                    {
                        model.Model.Id = Guid.NewGuid();
                        bl.Insert(model.Model);
                    }
                    else
                    {
                        var productUnitsToDel = productUnits.Get(m => m.ProductId == id);
                        foreach (var v in productUnitsToDel)
                        {
                            productUnits.Delete(v);
                        }

                        bl.Update(model.Model);
                    }
                    
                    foreach (var v in model.Model.ProductUnits)
                    {
                        v.ProductId = model.Model.Id;
                        productUnits.Insert(v);
                    }
                    if (!isEdt)
                    {
                       model.QtyUnits = new ObservableCollection<SymbolTypeM>();
                        model.Set();
                    }

                    if (isEdt)
                    {
                        //delete journal
                        var journalToDet = journalBL.Get(m => m.RefId == model.Model.Id).FirstOrDefault();
                        if (journalToDet != null)
                        {
                            foreach (var v in journalToDet.JournalDets)
                                detBL.Delete(v.Id);
                            journalBL.Delete(journalToDet.Id);
                        }
                        
                    }

                    if (model.QTYPrice > 0)
                    {

                        //journal
                        JournalM journal = new JournalM()
                        {
                            Id = Guid.NewGuid(),
                            Num = journalBL.GetNewNum(),
                            Date = model.Model.Date,
                            JournalTypeId = ConstJournalTypes.OpeningBalance,
                            RefId = model.Model.Id,
                            UserId = Profile.UserProfile.Id,
                        };
                        journalBL.Insert(journal);
                        //journal det detp part
                        var lastNum = detBL.GetLastNum();
                            JournalDetM dept = new JournalDetM()
                            {
                                Id = Guid.NewGuid(),
                                Num = detBL.GetNewNum(lastNum),
                                JournalId = journal.Id,
                                AccountID = ConstAccounts.Inventory,
                                Debt = model.QTYPrice,
                            };

                            detBL.Insert(dept);
                            lastNum = dept.Num;

                            //journal det detp part
                            lastNum++;
                            JournalDetM credit = new JournalDetM()
                            {
                                Id = Guid.NewGuid(),
                                Num = lastNum,
                                JournalId = journal.Id,
                                AccountID = ConstAccounts.Capital,
                                Credit = model.QTYPrice,
                            };
                            detBL.Insert(credit);
                        
                    }

                    dbContextTransaction.Commit();
                    MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    MessageBox.Show(ConstUserMsg.FaildMsg, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                saveBtn.IsEnabled = true;
            }
        }        


        /// 
        /// datagrid 
        /// 
        private void DataGrid_CellGotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);

                Control control = MyDataGridHelper.GetFirstChildByType<Control>(e.OriginalSource as DataGridCell);
                if (control != null)
                {
                    control.Focus();
                }
            }
        }

        private void dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var columnIndex = dg.Columns.IndexOf(dg.CurrentColumn);
            //if enter selecet first row
            var u = e.OriginalSource as UIElement;
            if (e.Key == Key.Enter && u != null /*&& columnIndex == 4*/)
            {
                model.AddNewRow();
            }
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            if (model.SelectedProdectUnit == null)
                return;

            foreach (var v in model.Model.ProductUnits)
            {
                if (v.Id == model.SelectedProdectUnit.Id)
                {
                    v.IsDefault = true;
                }
                else
                    v.IsDefault = false;
            }
            e.Handled = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var co = (ComboBox)sender;

            if (co.SelectedItem != null && co.SelectedItem.GetType() == typeof(SymbolTypeM) 
                && model.SelectedProdectUnit != null)
            {
                var unit = ((SymbolTypeM)co.SelectedItem);
                if (unit == null)
                    return;
                var uExist = model.Model.ProductUnits.Where(m => m.UnitId == unit.Id && m.Id != model.SelectedProdectUnit.Id).FirstOrDefault();
                if(uExist != null)
                {
                    co.SelectedItem = null;
                    return;
                }

                if (model.SelectedProdectUnit.IsDefault)
                    return;
                var from = model.Model.ProductUnits.Where(m => m.IsDefault).FirstOrDefault();
                if (from == null)
                    return;

                model.SelectedProdectUnit.PriceBuy = productFun.GetPrice(model.SelectedProdectUnit.Multiplier, from.Multiplier, from.PriceBuy);
                model.SelectedProdectUnit.PriceSale = productFun.GetPrice(model.SelectedProdectUnit.Multiplier, from.Multiplier, from.PriceSale);
            }           
        }

        /// 
        /// qty 
        /// 
        private void GridDG_LostFocus(object sender, RoutedEventArgs e)
        {
            model.SetQtyUnits();
            cbQtyUnits.SelectedValue = model.QtyUnitId;
        }

        private void CBQty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetQtyPrice();
        }

        private void txtQty_KeyUp(object sender, RoutedEventArgs e)
        {
            BindingExpression i = txtQty.GetBindingExpression(TextBox.TextProperty);
            if (i.HasError)
                return;
            SetQtyPrice();
        }


        private void SetQtyPrice()
        {
            if (cbQtyUnits.SelectedItem != null && cbQtyUnits.SelectedItem.GetType() == typeof(SymbolTypeM))
            {
                var unit = ((SymbolTypeM)cbQtyUnits.SelectedItem);
                if (unit == null)
                    return;

                var lastUnitId = model.QtyUnits.Where(m => m.Id == model.QtyUnitId).Select(m => m.Id).FirstOrDefault();
                var to = model.Model.ProductUnits.Where(m => m.UnitId == unit.Id).FirstOrDefault();
                model.QtyUnitId = unit.Id;

                try
                {
                    var from = model.Model.ProductUnits.Where(m => m.UnitId == lastUnitId).FirstOrDefault();
                    model.QTY = new ProductFun().GetQTY(to.Multiplier, from.Multiplier, model.QTY);
                    model.QTYPrice = (decimal)model.QTY * to.PriceBuy;
                    totalTxtBlk.Text = ((decimal)model.QTY * to.PriceBuy).ToString();
                }
                catch
                {

                }
            }
        }
        //
        //img
        //
        private void img_MouseDown(object sender, MouseEventArgs e)
        {
            var re = imgutils.ImgDialog();
            if (re == null)
                return;

            model.Model.DisplayImg = re.FileName;
            isImgUpdate = true;
        }

    }
}
