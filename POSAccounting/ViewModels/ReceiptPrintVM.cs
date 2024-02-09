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
    public class ReceiptPrintVM : ViewModelBase
    {
        private CorpInfoM corp;
        private ReceiptM receipt;


        public CorpInfoM Corp { get { return corp; } set { corp = value; NotifyPropertyChanged("Corp"); } }
        public ReceiptM Receipt { get { return receipt; } set { receipt = value; NotifyPropertyChanged("Receipt"); } }
        private readonly UserVM u;
        public ReceiptPrintVM(CropAccountingAppEntities db, Guid id)
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
                Corp.DisplayImg = ImgUtils.CategoryIcon;

            Corp.Name += "\n" + Corp.Address + "\n" + Corp.Phone;
            Receipt = new ReceiptBL(db).GetById(id);
            var c = new ClientUtils().FromEntity(db.Clients.Where(m => m.AccountId == Receipt.AccountId).FirstOrDefault());
            if (c == null)
            {
                var lang = AppSettings.GetLang();
                if (lang == ConstLang.En)
                    Receipt.ClientName = db.Accounts.Where(m => m.Id == Receipt.AccountId).Select(m => m.EnName).FirstOrDefault();
                else
                    Receipt.ClientName = db.Accounts.Where(m => m.Id == Receipt.AccountId).Select(m => m.ArName).FirstOrDefault();

            }
            else
                Receipt.ClientName = c.Name;
            //if (Receipt.VisaId != null)
            //{
            //  //  Receipt.Visa = new VisaBL(db).GetById((Guid)Receipt.VisaId, new CorpBankBL(db));
            //}else
            //    Receipt.PaymentMethod = "Cash";

        }
    }
}
