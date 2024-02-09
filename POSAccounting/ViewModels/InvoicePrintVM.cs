using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.ViewModels
{
    public class InvoicePrintVM : ViewModelBase
    {
        private CorpInfoM corp;
        private InvoiceM invoice;

        public CorpInfoM Corp { get { return corp; } set { corp = value; NotifyPropertyChanged("Corp"); } }
        public InvoiceM Invoice { get { return invoice; } set { invoice = value; NotifyPropertyChanged("Invoice"); } }

        public InvoicePrintVM(CropAccountingAppEntities db, Guid id)
        {
            ImgUtils imgutils = new ImgUtils();
            Corp = new CorpInfoM(Profile.CorpProfile);

            if (!string.IsNullOrEmpty(Corp.LocalImg))
            {
                var path = imgutils.GetImgFullPath(Corp.LocalImg, imgutils.UserImgs);
                if (Directory.Exists(path))
                    Corp.DisplayImg = imgutils.GetImgFullPath(Corp.LocalImg, imgutils.UserImgs);
            }

            if (string.IsNullOrEmpty(Corp.DisplayImg))
                Corp.DisplayImg = ImgUtils.UserIcon;

            Corp.Name += "\n" + Corp.Address + "\n" + Corp.Phone;
            Invoice = new InvoiceBL(db).GetById(id);

            if (Invoice.Client != null)
            {
                Invoice.ClientName = Invoice.Client.Name + "\n" + Invoice.Client.Address + "\n" + Invoice.Client.Phone;
            }
            else if (string.IsNullOrWhiteSpace(Invoice.ClientName))
            {
                if (Invoice.InvoiceTypeId == ConstInvoiceType.Purchases || Invoice.InvoiceTypeId == ConstInvoiceType.PurchasesReturns)
                    Invoice.ClientName = Properties.Resources.Supplier;
                else
                    Invoice.ClientName = Properties.Resources.Customer;
            }
            else
                Invoice.ClientName += "\n" + Invoice.ClientPhone;

            JournalDet v = null;
            if (Invoice.InvoiceTypeId == ConstInvoiceType.Purchases || Invoice.InvoiceTypeId == ConstInvoiceType.PurchasesReturns)
            {
                 v = (from m in db.JournalDets
                         join m2 in db.Journals on m.JournalId equals m2.Id
                         join m3 in db.Invoices on m2.RefId equals m3.Id
                         where m.AccountID == ConstAccounts.PurchaseTax
                         select m).FirstOrDefault();
            }
            else
            {
                v = (from m in db.JournalDets
                     join m2 in db.Journals on m.JournalId equals m2.Id
                     join m3 in db.Invoices on m2.RefId equals m3.Id
                     where m.AccountID == ConstAccounts.SalesTax
                     select m).FirstOrDefault();
            }
                
            if(v != null)
                invoice.Tax = v.Credit > 0 ? v.Credit : v.Debt;
            invoice.TotalPrice = invoice.Amount - invoice.Tax;
            invoice.TotalPriceSubDisc = invoice.TotalPrice + invoice.Discount;

        }
    }
}
