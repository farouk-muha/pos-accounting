using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public class ClientM : ViewModelBase
    {
        private Guid id;
        private int num;
        private string name;
        private string phone;
        private string address;
        private string img;
        private string localImg;
        private byte clientTypeId;
        private bool isCompany;
        private bool status;
        private Guid accountId;
        private AccountM account;
        private string displayImg;
        private int invoiceCount;
        private int receiptCount;
        private decimal ballance;
        private decimal credit;
        private decimal debt;

        public Guid Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public int Num { get { return num; } set { num = value; NotifyPropertyChanged("Num"); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("Name"); } }
        public string Phone { get { return phone; } set { phone = value; NotifyPropertyChanged("Phone"); } }
        public string Address { get { return address; } set { address = value; NotifyPropertyChanged("Address"); } }
        public string Img { get { return img; } set { img = value; NotifyPropertyChanged("Img"); } }
        public string LocalImg { get { return localImg; } set { localImg = value; NotifyPropertyChanged("LocalImg"); } }    
        public byte ClientTypeId { get { return clientTypeId; } set { clientTypeId = value; NotifyPropertyChanged("ClientTypeId"); } }
        public bool IsCompany { get { return isCompany; } set { isCompany = value; NotifyPropertyChanged("IsCompany"); } }
        public bool Status { get { return status; } set { status = value; NotifyPropertyChanged("Status"); } }
        public Guid AccountId { get { return accountId; } set { accountId = value; NotifyPropertyChanged("AccountId"); } }
        public AccountM Account { get { return account; } set { account = value; NotifyPropertyChanged("Account"); } }
        public string DisplayImg { get { return displayImg; } set { displayImg = value; NotifyPropertyChanged("DisplayImg"); } }
        public int InvoiceCount { get { return invoiceCount; } set { invoiceCount = value; NotifyPropertyChanged("InvoiceCount"); } }
        public int ReceiptCount { get { return receiptCount; } set { receiptCount = value; NotifyPropertyChanged("ReceiptCount"); } }
        public decimal Ballance { get { return ballance; } set { ballance = value; NotifyPropertyChanged("Ballance"); } }
        public decimal Credit { get { return credit; } set { credit = value; NotifyPropertyChanged("credit"); } }
        public decimal Debt { get { return debt; } set { debt = value; NotifyPropertyChanged("debt"); } }
    }

    public class ClientUtils
    {
        public ClientM FromEntity(Client entity)
        {
            var model = entity == null ? null :
                new ClientM()
                {
                    Id = entity.Id,
                    Num = entity.Num,
                    Name = entity.Name,
                    Phone = entity.Phone,
                    Address = entity.Address,
                    Img = entity.Img,
                    LocalImg = entity.LocalImg,
                    ClientTypeId = entity.ClinetTypeId,
                    IsCompany = entity.IsCompany,
                    Status = entity.StatusN,
                    AccountId = entity.AccountId,
                };
            return model;
        }

        public Client FromModel(ClientM model)
        {
            return model == null ? null :
                new Client()
                {
                    Id = model.Id,
                    Num = model.Num,
                    Name = model.Name,
                    Phone = model.Phone,
                    Address = model.Address,
                    Img = model.Img,
                    LocalImg = model.LocalImg,
                    ClinetTypeId = model.ClientTypeId,
                    IsCompany = model.IsCompany,
                    StatusN = model.Status,
                    AccountId = model.AccountId,
                };
        }
    }

}
