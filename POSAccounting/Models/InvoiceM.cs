using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public class InvoiceM : ViewModelBase
    {
        private Guid id;
        private int num;
        private DateTime date;
        private decimal amount;
        private decimal payed;
        private decimal discount;
        private bool isPayed;
        private string note;
        private byte invoiceTypeId;
        private Guid storeId;
        private int userId;
        private string clientName;
        private string clientPhone;
        private Nullable<Guid> accountId;
        private bool isCash;
        private bool isRemind;
        private AccountM account;
        private ObservableCollection<InvoiceLineM> invoiceLines;


        private decimal totalPrice;
        private decimal totalPriceSubDisc;
        private decimal totalQty;
        private decimal pay;
        private decimal debt;
        private decimal cash;
        private decimal visa;
        private decimal tax;
        private Nullable<Guid> visaId;
        private Nullable<Guid> clientId;
        private ClientM client;
        private byte percent;


        public Guid Id { get { return id; } set { id = value; NotifyPropertyChanged("id"); } }
        public int Num { get { return num; } set { num = value; NotifyPropertyChanged("Num"); } }
        public DateTime Date { get { return date; } set { date = value; NotifyPropertyChanged("Date"); } }
        public decimal Amount { get { return amount; } set { amount = value; NotifyPropertyChanged("Amount"); } }
        public decimal Payed { get { return payed; } set { payed = value; NotifyPropertyChanged("Payed"); } }
        public decimal Discount { get { return discount; } set { discount = value; NotifyPropertyChanged("Discount"); } }
        public bool IsPayed { get { return isPayed; } set { isPayed = value; NotifyPropertyChanged("IsPayed"); } }
        public string Note { get { return note; } set { note = value; NotifyPropertyChanged("Note"); } }
        public byte InvoiceTypeId { get { return invoiceTypeId; } set { invoiceTypeId = value; NotifyPropertyChanged("InvoiceTypeId"); } }
        public Guid StoreId { get { return storeId; } set { storeId = value; NotifyPropertyChanged("storeId"); } }
        public int UserId { get { return userId; } set { userId = value; NotifyPropertyChanged("UserId"); } }
        public string ClientName { get { return clientName; } set { clientName = value; NotifyPropertyChanged("ClientName"); } }
        public string ClientPhone { get { return clientPhone; } set { clientPhone = value; NotifyPropertyChanged("ClientPhone"); } }
        public Nullable<Guid> AccountId { get { return accountId; } set { accountId = value; NotifyPropertyChanged("AccountId"); } }
        public bool IsCash { get { return isCash; } set { isCash = value; NotifyPropertyChanged("IsCash"); } }
        public bool IsRemind { get { return isRemind; } set { isRemind = value; NotifyPropertyChanged("IsRemind"); } }
        public AccountM Account { get { return account; } set { account = value; NotifyPropertyChanged("Account"); } }
        public ObservableCollection<InvoiceLineM> InvoiceLines { get { return invoiceLines; } set { invoiceLines = value; NotifyPropertyChanged("InvoiceLines"); } }

        public decimal TotalPrice { get { return totalPrice; } set { totalPrice = value; NotifyPropertyChanged("TotalPrice"); } }
        public decimal TotalPriceSubDisc { get { return totalPriceSubDisc; } set { totalPriceSubDisc = value; NotifyPropertyChanged("TotalPriceSubDisc"); } }
        public decimal TotalQty { get { return totalQty; } set { totalQty = value; NotifyPropertyChanged("TotalQty"); } }
        public decimal Pay { get { return pay; } set { pay = value; NotifyPropertyChanged("Pay"); } }
        public decimal Debt { get { return debt; } set { debt = value; NotifyPropertyChanged("Debt"); } }
        public decimal Cash { get { return cash; } set { cash = value; NotifyPropertyChanged("Cash"); } }
        public decimal Visa { get { return visa; } set { visa = value; NotifyPropertyChanged("Visa"); } }
        public decimal Tax { get { return tax; } set { tax = value; NotifyPropertyChanged("Tax"); } }
        public Nullable<Guid> VisaId { get { return visaId; } set { visaId = value; NotifyPropertyChanged("VisaId"); } }
        public Nullable<Guid> ClientId { get { return clientId; } set { clientId = value; NotifyPropertyChanged("ClientId"); } }
        public ClientM Client { get { return client; } set { client = value; NotifyPropertyChanged("Client"); } }
        public byte Percent { get { return percent; } set { percent = value; NotifyPropertyChanged("Percent"); } }

    }

    public class InvoiceUtils
    {
        public InvoiceM FromEntity(Invoice entity)
        {
            var model = entity == null ? null :
                new InvoiceM()
                {
                    Id = entity.Id,
                    Num = entity.Num,
                    Date = entity.Date,
                    Amount = entity.Amount,
                    Payed = entity.Payed != null ? (decimal)entity.Payed : 0,
                    Discount = entity.Descount != null ? (decimal)entity.Descount : 0,
                    IsPayed = entity.IsPayed,
                    Note = entity.Note,
                    InvoiceTypeId = entity.InvoiceTypeId,
                    StoreId = entity.StoreId,
                    UserId = entity.UserId,
                    ClientName = entity.ClientName,
                    ClientPhone = entity.ClientPhone,
                    AccountId = entity.AccountId,
                    IsCash = entity.IsCash,
                    IsRemind = entity.IsRemind,
                    
                };
            if (entity.InvoiceLines == null)
                return model;

            return model;
        }

        public Invoice FromModel(InvoiceM model)
        {
            return model == null ? null :
                new Invoice()
                {
                    Id = model.Id,
                    Num = model.Num,
                    Date = model.Date,
                    Amount = model.Amount,
                    Payed = model.Payed == 0? null : (Nullable<decimal>)model.Payed,
                    Descount = model.Discount == 0? null : (Nullable<decimal>)model.Discount,
                    IsPayed =  model.IsPayed,
                    Note = model.Note,
                    InvoiceTypeId = model.InvoiceTypeId,
                    StoreId = model.StoreId,
                    UserId = model.UserId,
                    ClientName = string.IsNullOrWhiteSpace(model.ClientName) ? null : model.ClientName,
                    ClientPhone = string.IsNullOrWhiteSpace(model.ClientPhone) ? null : model.ClientPhone,
                    AccountId = model.AccountId,
                    IsCash = model.IsCash,
                    IsRemind = model.IsRemind,
                };
        }
    }
}
