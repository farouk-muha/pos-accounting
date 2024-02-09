using POSAccounting.Models;
using POSAccounting.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Utils
{
    public class AppSettings
    {
        public static void SetFirstLaunch(bool isFirstLaunch = false)
        {

            Properties.Settings.Default.FirstLaunch = isFirstLaunch;
            Settings.Default.Save();
        }

        public static bool IsFirstLaunch()
        {
            var v = Properties.Settings.Default.FirstLaunch;
            return v;
        }
        public static void SetBackupPath(string path)
        {

            Properties.Settings.Default.BackupPath = path;
            Settings.Default.Save();
        }

        public static string GetBackupPath()
        {

            return Properties.Settings.Default.BackupPath;
        }
        public static void SetBackupTime(byte time)
        {

            Properties.Settings.Default.BackupTime = time;
            Settings.Default.Save();
        }

        public static byte GetBackupTime()
        {

            return Properties.Settings.Default.BackupTime;
        }

        public static void SetBackupTime(DateTime lastBackup)
        {

            Properties.Settings.Default.LastBackp = lastBackup;
            Settings.Default.Save();
        }

        public static bool IsScheduleBackup()
        {

            return Properties.Settings.Default.IsScheduleBackup;
        }

        public static void SetScheduleBackup(bool isSchedule)
        {

            Properties.Settings.Default.IsScheduleBackup = isSchedule;
            Settings.Default.Save();
        }

        public static DateTime GetLastBackup()
        {

            return Properties.Settings.Default.LastBackp;
        }

        public static void SetInvoiceReminder(int days)
        {

            Properties.Settings.Default.InvoiceRemind = days;
            Settings.Default.Save();
        }

        public static int GetInvoiceReminder()
        {

            return Properties.Settings.Default.InvoiceRemind;
        }

        public static void SetLang(byte lang)
        {

            Properties.Settings.Default.Language = lang;
            Settings.Default.Save();
        }

        public static int GetLang()
        {

            return Properties.Settings.Default.Language;
        }
        public static void SetCurrentFiscalYear(FiscalYearM model)
        {

            Properties.Settings.Default.StartDate = model.StartDate;
            Properties.Settings.Default.EndDate = model.EndDate;
            Properties.Settings.Default.IsYearClosed = model.IsClosed;
            Settings.Default.Save();
        }

        public static FiscalYearM GetCurrentFiscalYear()
        {
             return new FiscalYearM() {
                StartDate = Properties.Settings.Default.StartDate,
                EndDate = Properties.Settings.Default.EndDate,
                IsClosed = Properties.Settings.Default.IsYearClosed
             };
        }

        public static string GetEntityConnectionStringBuilder()
        {
            var bil = new EntityConnectionStringBuilder()
            {
                Provider = "System.Data.SqlClient",
                Metadata = @"res://*/Models.Model1.csdl|res://*/Models.Model1.ssdl|res://*/Models.Model1.msl",
                ProviderConnectionString = @"data source=.\SqlExpress;initial catalog=CropAccountingApp;Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework;"
            };
            return bil.ToString();
        }
    }
}
