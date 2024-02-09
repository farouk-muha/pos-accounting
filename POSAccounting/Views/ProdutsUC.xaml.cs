using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
using MahApps.Metro.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POSAccounting.Views
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class ProdutsUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private ProductsVM model;

        public ProdutsUC()
        {
            model = new ProductsVM(db, new ProductBL(db));
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }
        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var template = headerControl.Template;
            var txtKW = (TextBox)template.FindName("txtKW", headerControl);
            txtKW.SetValue(TextBoxHelper.WatermarkProperty, Properties.Resources.ProductSearchHint);
            var chk = (CheckBox)template.FindName("chkStatus", headerControl);
            chk.Visibility = Visibility.Collapsed;
            var typesCB = (ComboBox)template.FindName("typesCB", headerControl);
            typesCB.Visibility = Visibility.Collapsed;

            chk.Content = Properties.Resources.Status;
            ((TextBlock)template.FindName("blkFrom", headerControl)).Text = Properties.Resources.From;
            ((TextBlock)template.FindName("blkTo", headerControl)).Text = Properties.Resources.To;
            ((Button)template.FindName("btnSearch", headerControl)).Content = Properties.Resources.Search;
            ((Button)template.FindName("btnClear", headerControl)).Content = Properties.Resources.Clear;
            ((TextBlock)template.FindName("blkRecoreds", headerControl)).Text = Properties.Resources.Records;

            var card = cardCount.Template;
            ((TextBlock)card.FindName("blkTotal", cardCount)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardCount)).Text = Properties.Resources.Product;
            card = cardAmount.Template;
            ((TextBlock)card.FindName("blkTotal", cardAmount)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardAmount)).Text = Properties.Resources.Amount;

            await Task.Delay(20);

            model.Load();
            this.DataContext = model;
        }
        private void dg_GotFocus(object sender, RoutedEventArgs e)
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

        private void UnitSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (model.SelectedProduct != null && comboBox != null && comboBox.SelectedItem.GetType() == typeof(ProductUnitM))
            {
                var s = (ProductUnitM)comboBox.SelectedItem;

                model.SelectedProduct.QTY = new ProductFun().GetQTY(s.Multiplier, model.SelectedProduct.ProductUnit.Multiplier, (double)model.SelectedProduct.QTY);
                model.SelectedProduct.ProductUnit.PriceBuy = s.PriceBuy;
                model.SelectedProduct.ProductUnit.PriceSale = s.PriceSale;
                model.SelectedProduct.ProductUnit.Multiplier = s.Multiplier;
                model.SelectedProduct.TotalPrice = s.PriceBuy * (decimal)model.SelectedProduct.QTY;
            }
        }
    }
}
