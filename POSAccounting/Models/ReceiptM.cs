using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
   public class ReceiptM : ViewModelBase 
    {
        private Guid id;
        private int num;
        private decimal amount;
        private DateTime date;
        private string note;
        private byte receiptTypeId;
        private Guid accountId;
        private bool isCash;
        private int userId;

        private AccountM account;
        private string clientName;
        private string paymentMethod;
        private Nullable<Guid> visaId;
        private AccountM visa;
        private decimal discount;

        public System.Guid Id{ get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public int Num { get { return num; } set { num = value; NotifyPropertyChanged("Num"); } }
        public decimal Amount { get { return amount; } set { amount = value; NotifyPropertyChanged("Amount"); } }
        public DateTime Date { get { return date; } set { date = value; NotifyPropertyChanged("Date"); } }
        public string Note{ get { return note; } set { note = value; NotifyPropertyChanged("Note"); } }
        public byte ReceiptTypeId{ get { return receiptTypeId; } set { receiptTypeId = value; NotifyPropertyChanged("ReceiptTypeId"); } }
        public Guid AccountId { get { return accountId; } set { accountId = value; NotifyPropertyChanged("AccountId"); } }
        public bool IsCash { get { return isCash; } set { isCash = value; NotifyPropertyChanged("IsCash"); } }
        public List<Guid> InvoiceIds;
        public int UserId { get { return userId; } set { userId = value; NotifyPropertyChanged("UserId"); } }

        public AccountM Account { get { return account; } set { account = value; NotifyPropertyChanged("Account"); } }
        public string ClientName { get { return clientName; } set { clientName = value; NotifyPropertyChanged("ClientName"); } }
        public string PaymentMethod { get { return paymentMethod; } set { paymentMethod = value; NotifyPropertyChanged("PaymentMethod"); } }
        public Nullable<Guid> VisaId { get { return visaId; } set { visaId = value; NotifyPropertyChanged("VisaId"); } }
        public AccountM Visa { get { return visa; } set { visa = value; NotifyPropertyChanged("Visa"); } }
        public decimal Discount { get { return discount; } set { discount = value; NotifyPropertyChanged("Discount"); } }


    }

    public class ReceiptUtils
    {
        public ReceiptM FromEntity(Receipt entity)
        {
            return entity == null ? null :
                new ReceiptM()
                {
                    Id = entity.Id,
                    Num = entity.Num,
                    Amount = entity.Amount,
                    Date = entity.Date,
                    Note = entity.Note,
                    ReceiptTypeId = entity.ReceiptTypeId,
                    AccountId = entity.AccountId,
                    IsCash = entity.IsCash,
                    UserId = entity.UserId,
                };
        }

        public Receipt FromModel(ReceiptM model)
        {
            return model == null ? null :
                new Receipt()
                {
                    Id = model.Id,
                    Num = model.Num,
                    Amount = model.Amount,
                    Date = model.Date,
                    Note = model.Note,
                    ReceiptTypeId = model.ReceiptTypeId,
                    AccountId = model.AccountId,
                    IsCash = model.IsCash,
                    UserId = model.UserId,
                };
        }
    }
}
