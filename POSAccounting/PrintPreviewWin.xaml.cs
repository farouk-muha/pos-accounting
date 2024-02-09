using POSAccounting.Utils;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
using System.Windows.Xps;

namespace POSAccounting
{
    /// <summary>
    /// Interaction logic for PrintPreviewWin.xaml
    /// </summary>
    public partial class PrintPreviewWin : MetroWindow
    {
        private FixedDocumentSequence _document;
        private int asests = 1;
        private static PrintPreviewWin _instance;
        public static void ShowMe(FixedDocumentSequence document)
        {
            if (_instance == null)
            {
                _instance = new PrintPreviewWin(document);
                _instance.Closed += (senderr, args) => _instance = null;
                _instance.Show();
            }
        }

        public PrintPreviewWin(FixedDocumentSequence document)
        {

            _document = document;
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            PreviewD.Document = document;
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            Print_Document();
        }

        public void Print_Document()

        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
            printDialog.PrintTicket = printDialog.PrintQueue.DefaultPrintTicket;
            printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
            printDialog.PrintTicket.PageScalingFactor = 90;
            printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);

            //printDialog.PrintableAreaHeight ; //*get
            //printDialog.PrintableAreaWidth;   //get
            //printDialog.PrintDocument.

            printDialog.PrintTicket.PageBorderless = PageBorderless.None;

            if (printDialog.ShowDialog() == true)

            {
                _document.PrintTicket = printDialog.PrintTicket;
                XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);
                writer.WriteAsync(_document, printDialog.PrintTicket);
            }
        }
    }
}
