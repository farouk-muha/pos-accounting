using CorpAccountingApp.ViewModels;
using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
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
using System.Windows.Shapes;

namespace POSAccounting
{
    /// <summary>
    /// Interaction logic for SplashWin.xaml
    /// </summary>
    public partial class SplashWin : Window
    {
        private static SplashWin _instance;
        private bool isDbLoaded = false;
        public static SplashWin NewInstance(bool isDbLoaded)
        {
            if (_instance == null)
            {
                _instance = new SplashWin(isDbLoaded);
                _instance.Closed += (senderr, args) => _instance = null;
            }
            else
                _instance.Activate();
            return _instance;
        }

        public SplashWin(bool isDbLoaded)
        {
            this.isDbLoaded = isDbLoaded;
            InitializeComponent();
            Loaded += MyWindow_Loaded;
           
        }

        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            ProfileVM model;

            model = new ProfileVM(new CropAccountingAppEntities());
            model.GetProfile();
            this.DataContext = model;

            MyMainWindow mainWin = new MyMainWindow();
            Application.Current.MainWindow = mainWin;

            await Task.Delay(1000);
            mainWin.Show();
            Close();
        }
    }
}
