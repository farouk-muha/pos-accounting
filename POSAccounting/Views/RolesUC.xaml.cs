using POSAccounting.Models;
using POSAccounting.BL;
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
using POSAccounting.Utils;

namespace POSAccounting.Views
{
    /// <summary>
    /// Interaction logic for RolesUC.xaml
    /// </summary>
    public partial class RolesUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private LocalRolesVM model;

        public RolesUC()
        {
            model = new LocalRolesVM(db, new LocalRoleBL(db));
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }
        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var card = cardCount.Template;
            ((TextBlock)card.FindName("blkTotal", cardCount)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardCount)).Text = Properties.Resources.Role;
            await Task.Delay(20);

            this.DataContext = model;
            model.Load();

        }
    }
}
