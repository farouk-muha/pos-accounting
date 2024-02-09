using POSAccounting.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace POSAccounting.Utils
{
   public class ImgUtils
    {
        public readonly string UserImgs = "UserImgs";
        public readonly string ClientImgs = "ClientImgs";
        public readonly string ProductImgs = "ProductImgs";
        public static readonly string UserIcon = "/POSAccounting;component/Assets/UserImgs/default_user.png";
        public static readonly string RoleIcon = "/POSAccounting;component/Assets/UserImgs/RoleIcon.png";
        public static readonly string VisaIcon = "/POSAccounting;component/Assets/Default/VisaIcon.png";
        public static readonly string AccountIcon = "/POSAccounting;component/Assets/Default/AccountIcon.png";
        public static readonly string JournalIcon = "/POSAccounting;component/Assets/Default/JournalIcon.jpg";
        public static readonly string ProductIcon = "/POSAccounting;component/Assets/Default/ProductIcon.png";
        public static readonly string CategoryIcon = "/POSAccounting;component/Assets/Default/CategoryIcon.jpg";
        public static readonly string StoreIcon = "/POSAccounting;component/Assets/Default/StoreIcon.png";
        public static readonly string InvoiceIcon = "/POSAccounting;component/Assets/Default/invoice.png";

        //get orignal image and path from dialog  and create new path
        public Microsoft.Win32.OpenFileDialog ImgDialog()
        {
            //open dialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                return dlg;          
            }

            return null;
        }

        public string SaveImg(string imgPath, string dir, string oldImgPath)
        {
            string appImgsDir = AppDomain.CurrentDomain.BaseDirectory + dir;
            if (!Directory.Exists(appImgsDir))
            {
                DirectoryInfo di = Directory.CreateDirectory(appImgsDir);
            }
            string imgName = Guid.NewGuid().ToString();
            string newPath = Path.Combine(appImgsDir, imgName + ".png");

            //save image
            Bitmap original = new Bitmap(imgPath);
            Bitmap img = new Bitmap(original, new System.Drawing.Size(200, 200));
            img.Save(newPath, ImageFormat.Png);

            // delete old image
            if(!String.IsNullOrEmpty(oldImgPath))
            {
                string oldPath = Path.Combine(appImgsDir, oldImgPath + ".png");
                if (File.Exists(oldPath))
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    File.Delete(oldPath);
                }
            }

            return imgName;
        }

        public string GetImgFullPath(string imgName, string dir)
        {
            string appImgsDir = AppDomain.CurrentDomain.BaseDirectory + dir;
            return Path.Combine(appImgsDir, imgName + ".png");           
        }

        public void Delete(string dir, string imgPath)
        {
            string appImgsDir = AppDomain.CurrentDomain.BaseDirectory + dir;
            if (!Directory.Exists(appImgsDir))
            {
                DirectoryInfo di = Directory.CreateDirectory(appImgsDir);
            }
            string oldPath = Path.Combine(appImgsDir, imgPath + ".png");
            if (File.Exists(oldPath))
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                File.Delete(oldPath);
            }
        }
    }
}
