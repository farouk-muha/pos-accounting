using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CorpAccountingApp.ViewModels
{
    public class ProfileVM : ViewModelBase
    {
        public CropAccountingAppEntities db;
        private CorpInfoM corp;
        private UserM user;
        private string title;

        public CorpInfoM Corp { get { return corp; } set { corp = value; NotifyPropertyChanged("Corp"); } }
        public UserM User { get { return user; } set { user = value; NotifyPropertyChanged("User"); } }
        public string Title { get { return title; } set { title = value; NotifyPropertyChanged("Title"); } }

        public ProfileVM()
        {
        }

        public ProfileVM(CropAccountingAppEntities db)
        {
            this.db = db;  
        }

        public void GetProfile()
        {
            ImgUtils imgutils = new ImgUtils();
            try
            {
                Corp = new CorpInfoM(Profile.CorpProfile);
                User = new UserM(Profile.UserProfile);

                var path = imgutils.GetImgFullPath(Corp.LocalImg, imgutils.UserImgs);
                if (Directory.Exists(path))
                    Corp.DisplayImg = imgutils.GetImgFullPath(Corp.LocalImg, imgutils.UserImgs);
                else
                    Corp.DisplayImg = ImgUtils.UserIcon;

                path = imgutils.GetImgFullPath(User.LocalImg, imgutils.UserImgs);
                if (Directory.Exists(path))
                    user.DisplayImg = imgutils.GetImgFullPath(User.LocalImg, imgutils.UserImgs);
                else
                    user.DisplayImg = ImgUtils.UserIcon;

            }
            catch
            {
            }
        }

    }

}
