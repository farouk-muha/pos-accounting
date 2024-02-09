using POSAccounting.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace POSAccounting.Utils
{
   public class MainWinUtils
    {
        public static int Main { get; } = 1;
        //Invoices section
        public static int SellInvoice { get; } = 2;
        public static int SellInvoices { get; } = 3;
        public static int PurchasesInvoices { get; } = 4;
        public static int PurchasesInvoice { get; } = 5;

        //Receitpts section
        public static int Receitpt { get; } = 6;
        public static int Receitpts { get; } = 6;

        public static int Roles { get; } = 2;
        public static int Products { get; } = 3;
        public static int Accounts { get; } = 4;
        public static int Journal { get; } = 5;
        public static int Users { get; } = 6;
        public static int Journals { get; } = 7;
        public static int Product { get; } = 8;
        public static int Stores { get; } = 9;
        public static int Categories { get; } = 10;
        public static int Clients { get; } = 11;
        public static int Client { get; } = 12;
        public static int Account { get; } = 15;
        //settings section
        public static int Banks { get; } = 16;
        public static int Visas { get; } = 17;
        public static int Settings { get; } = 18;
        ////////////////////////////////////////////

    }

    public class AppFun
    {
        public static bool IsNumricTxt(string txt)
        {
            Regex regex = new Regex("[^0-9]+");
            return regex.IsMatch(txt);
        }

        public void FromTxtToDB(CropAccountingAppEntities db)
        {
            using (var mappedFile1 = MemoryMappedFile.CreateFromFile(@"C:\tmp\accounts.txt"))
            {
                using (Stream mmStream = mappedFile1.CreateViewStream())
                {
                    using (StreamReader sr = new StreamReader(mmStream, ASCIIEncoding.ASCII))
                    {
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            var lineWords = line.Split();
                            System.Diagnostics.Debug.WriteLine(lineWords);

                            Account acc = new Models.Account
                            {
                                Id = new Guid(lineWords[0]),
                                Num = int.Parse(lineWords[1]),
                                Deleteable = bool.Parse(lineWords[lineWords.Length - 1]),
                                CropId = int.Parse(lineWords[lineWords.Length - 2]),
                            };

                            acc.AccountEndId = 1;


                            string s = lineWords[2];
                            for (int i = 3; i < (lineWords.Length - 6); i++)
                            {
                                s = s + " " + lineWords[i];
                            }
                            acc.EnName = s;
                            acc.ArName = s;
                            db.Accounts.Add(acc);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

     
    }

    public class MyPrint
    {
        public static void ShowPrintPreview(FrameworkElement wpfElement)
        {

            string msg = "print_p.xps";
            if (File.Exists(msg)) File.Delete(msg);

            XpsDocument xps = new XpsDocument(msg, FileAccess.ReadWrite);

            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xps);
            SerializerWriterCollator coll = writer.CreateVisualsCollator();
            coll.BeginBatchWrite();
            coll.Write(wpfElement);
            coll.EndBatchWrite();
            FixedDocumentSequence preview = xps.GetFixedDocumentSequence();

            PrintPreviewWin.ShowMe(preview);
            xps.Close();
            writer = null;
            coll = null;
            xps = null;
        }

        public static void ShowPrintPreviewPanel(ScrollViewer panel)
        {

            string msg = "print_p.xps";
            if (File.Exists(msg)) File.Delete(msg);

            XpsDocument xps = new XpsDocument(msg, FileAccess.ReadWrite);

            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xps);
            SerializerWriterCollator coll = writer.CreateVisualsCollator();
            coll.BeginBatchWrite();
            coll.Write(panel.Content as Visual);
            coll.EndBatchWrite();
            FixedDocumentSequence preview = xps.GetFixedDocumentSequence();

            PrintPreviewWin.ShowMe(preview);
            xps.Close();
            writer = null;
            coll = null;
            xps = null;
        }
    }
}
