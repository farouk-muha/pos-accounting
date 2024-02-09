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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POSAccounting.Views
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class StoresUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private StoresVM model;

        public StoresUC()
        {
            model = new StoresVM(db, new StoreBL(db));
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }
        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var card = cardCount.Template;
            ((TextBlock)card.FindName("blkTotal", cardCount)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardCount)).Text = Properties.Resources.Store;
            card = cardAmount.Template;
            ((TextBlock)card.FindName("blkTotal", cardAmount)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardAmount)).Text = Properties.Resources.ItemsCount;
            await Task.Delay(20);

            model.Load();
            this.DataContext = model;
        }
    }
}
