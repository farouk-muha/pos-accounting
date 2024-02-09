using POSAccounting.Utils;
using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public class AccountM : ViewModelBase
    {
        private Guid id;
        private int num;
        private string enName;
        private string arName;
        private decimal credit;
        private decimal debt;
        private int accountEndId;
        private Nullable<Guid> accountParentId;
        private int cropId;
        private bool deleteable;
        private int userId;
        private ObservableCollection<AccountM> accounts;
        private AccountM account;
        private ObservableCollection<JournalDetM> journalDets;
        private decimal balance;
        private decimal balanceD;
        public List<JournalM> Journals { get; set; }

        public Guid Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public int Num { get { return num; } set { num = value; NotifyPropertyChanged("Num"); } }
        public string EnName { get { return enName; } set { enName = value; NotifyPropertyChanged("EnName"); } }
        public string ArName { get { return arName; } set { arName = value; NotifyPropertyChanged("ArName"); } }
        public decimal Credit { get { return credit; } set { credit = value; NotifyPropertyChanged("Credit"); } }
        public decimal Debt { get { return debt; } set { debt = value; NotifyPropertyChanged("Debt"); } }
        public int AccountEndId { get { return accountEndId; } set { accountEndId = value; NotifyPropertyChanged("AccountEndId"); } }
        public Nullable<Guid> AccountParentId { get { return accountParentId; } set { accountParentId = value; NotifyPropertyChanged("AccountParentId"); } }
        public int CropId { get { return cropId; } set { cropId = value; NotifyPropertyChanged("CropId"); } }
        public bool Deleteable { get { return deleteable; } set { deleteable = value; NotifyPropertyChanged("Deleteable"); } }
        public int UserId { get { return userId; } set { userId = value; NotifyPropertyChanged("UserId"); } }
        public ObservableCollection<AccountM> Accounts { get { return accounts; } set { accounts = value; NotifyPropertyChanged("Accounts"); } }
        public AccountM Account { get { return account; } set { account = value; NotifyPropertyChanged("Account"); } }
        public ObservableCollection<JournalDetM> JournalDets { get { return journalDets; } set { journalDets = value; NotifyPropertyChanged("JournalDets"); } }
        public decimal Balance { get { return balance; } set { balance = value; NotifyPropertyChanged("Balance"); } }
        public decimal BalanceD { get { return balanceD; } set { balanceD = value; NotifyPropertyChanged("BalanceD"); } }
        public CorpInfo CorpInfo { get; set; }
        public AccountEnd AccountEnd { get; set; }
        public ICollection<Receipt> Receiptes { get; set; }
        public ICollection<Client> Clients { get; set; }
        public ICollection<Emp> Emps { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
    }

    public class AccountUtils
    {
        public AccountM FromEntity(Account entity)
        {
            var lang = AppSettings.GetLang();
            var model = entity == null ? null :
                new AccountM()
                {
                    Id = entity.Id,
                    Num = entity.Num,
                    EnName = lang == ConstLang.En ? entity.EnName : entity.ArName,
                    ArName = entity.ArName,
                    AccountEndId = entity.AccountEndId != null ? (int)entity.AccountEndId : 0,
                    AccountParentId = entity.AccountParentId,
                    CropId = entity.CropId,
                    Deleteable = entity.Deleteable,
                    UserId = entity.UserId,
                };
            return model;
        }

        public Account FromModel(AccountM model)
        {
            return model == null ? null :
                new Account()
                {
                    Id = model.Id,
                    Num = model.Num,
                    EnName = model.EnName,
                    ArName = model.ArName,
                    AccountEndId = model.AccountEndId,
                    AccountParentId = model.AccountParentId,
                    CropId = model.CropId,
                    Deleteable = model.Deleteable,
                    UserId = model.UserId,
                };
        }
    }
}
