using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using POSAccounting.Models;

namespace POSAccounting.Utils
{
    public static class ConstUserMsg
    {
        // DataBase Model
        public static string ModelNotFound { get; } = "{0} " + Properties.Resources.ModelNotFound;
        public static string CorpNotFound { get; } = Properties.Resources.CorpNotFound;

        //user
        public static string WrongEmailOrPass { get; } = Properties.Resources.WrongEmailOrPass;
        public static string WrongPassword { get; } = Properties.Resources.WrongPassword;
        public static string StatusNotActive { get; } = Properties.Resources.StatusNotActive;
        public static string UserNameExist { get; } = Properties.Resources.NameExist;
        public static string EmailExist { get; } = Properties.Resources.EmailExist;
        public static string ConfirmUserStatus { get; } = Properties.Resources.ConfirmUserStatus;

        //fields
        public static string RequiredField { get; } = Properties.Resources.RequiredField;
        public static string EmailNotValid { get; } = Properties.Resources.EmailNotValid;
        public static string LittersTooLong { get; } = Properties.Resources.LittersTooLong;
        public static string NumOnly { get; } = Properties.Resources.NumOnly;
        public static string NumIs0 { get; } = Properties.Resources.NumIs0;
        public static string NumTooLong { get; } = Properties.Resources.NumTooLong;
        public static string PercentOnly { get; } = Properties.Resources.PercentOnly;
        public static string BigNum { get; } = Properties.Resources.BigNum;

        //success operation 
        public static string SuccessProcess { get; } = Properties.Resources.SuccessProcess;
        public static string SuccessLogin { get; } = Properties.Resources.SuccessLogin;
        public static string SuccessMsg { get; } = Properties.Resources.SuccessMsg;

        //faild operation 
        public static string FaildProcess { get; } = Properties.Resources.FaildProcess;
        public static string FaildMsg { get; } = Properties.Resources.FaildMsg;



        // confirm
        public static string DeleteConfirm { get; } = Properties.Resources.DeleteConfirm;
        public static string UpdateConfirm { get; } = Properties.Resources.UpdateConfirm;
        public static string DeleteConfirmMsg { get; } = Properties.Resources.DeleteConfirmMsg + " {0} ?";

        //date
        public static string StartDateBiger { get; } = Properties.Resources.StartDateBiger;

        // Account Window
        public static string AccDelNotAllowed { get; } = Properties.Resources.AccDelNotAllowed;
        public static string NumExist { get; } = Properties.Resources.NumExist;
        public static string NameExist { get; } = Properties.Resources.NameExist;

        // Journal Window
        public static string CreditDebtNotEqaul { get; } = Properties.Resources.CreditDebtNotEqaul;
        public static string CreditDebtZero { get; } = Properties.Resources.CreditDebtZero;
        public static string MustChoseAccount { get; } = Properties.Resources.MustChoseAccount;
        public static string AtLeastOneJournal { get; } = Properties.Resources.AtLeastOneJournal;
        public static string OpenBalance { get; } = Properties.Resources.OpenBalance;

        // Product
        public static string CodeExist { get; } = Properties.Resources.CodeExist;
        public static string DuplicateUnits { get; } = Properties.Resources.DuplicateUnits;

        // Invoice
        public static string MustPayTotalPrice { get; } = Properties.Resources.MustPayTotalPrice;
        public static string CashOperation { get; } = Properties.Resources.CashOperation;
        public static string ClienNotChosenError { get; } = Properties.Resources.ClienNotChosenError;
        public static string AddInvoiceLines { get; } = Properties.Resources.AddInvoiceLines;

        // Not Allowed
        public static string NotAllowed { get; } = Properties.Resources.NotAllowed;

    }

    public class ConstNotAllowed
    {
        public static string Select { get; } = Properties.Resources.SelectUnAuth + " {0}";
        public static string Insert { get; } = Properties.Resources.InsertUnAuth + " {0}";
        public static string Update { get; } = Properties.Resources.UpdateUnAuth + " {0}";
        public static string Delete { get; } = Properties.Resources.DeleteUnAuth + " {0}";
        public static string Print { get; } = Properties.Resources.PrintUnAuth + " {0}";

    }

    public static class UserMsgUtils
    {
        public static bool GetMsg<T>(ResponseM<T> response, out string msg)
        {
            msg = ConstUserMsg.FaildProcess;
            bool isOk = false;
            if (response == null)
                return isOk;

            if (response.Code == HttpStatusCode.OK && response.Model != null )
            {
                isOk = true;
            }
            else
            {
                if (response.ErrorResponseM != null && response.ErrorResponseM.Message != null)
                    msg = response.ErrorResponseM.Message;
            }
            return isOk;
        }
    }
}
