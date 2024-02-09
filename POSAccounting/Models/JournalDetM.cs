using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public class JournalDetM : ViewModelBase
    {
        private Guid id;
        private int num;
        private decimal credit;
        private decimal debt;
        private string note;
        private Guid journalId;
        private Guid accountID;
        private AccountM account;
        private JournalM journal;

        private string accountName;

        public Guid Id  { get { return id; } set { id = value; NotifyPropertyChanged("id"); } }
        public int Num { get { return num; } set { num = value; NotifyPropertyChanged("Num"); } }
        public decimal Credit  { get { return credit; } set { credit = value; NotifyPropertyChanged("credit"); } }
        public decimal Debt  { get { return debt; } set { debt = value; NotifyPropertyChanged("debt"); } }
        public string Note  { get { return note; } set { note = value; NotifyPropertyChanged("note"); } }
        public Guid JournalId  { get { return journalId; } set { journalId = value; NotifyPropertyChanged("journalId"); } }
        public Guid AccountID  { get { return accountID; } set { accountID = value; NotifyPropertyChanged("accountID"); } }
        public AccountM Account { get { return account; } set { account = value; NotifyPropertyChanged("account"); } }
        public JournalM Journal { get { return journal; } set { journal = value; NotifyPropertyChanged("Journal"); } }
        public string AccountName { get { return accountName; } set { accountName = value; NotifyPropertyChanged("AccountName"); } }


    }

    public class JournalDettUtils
    {
        public static JournalDetM FromEntity(JournalDet entity)
        {
            var model = entity == null ? null :
                new JournalDetM()
                {
                    Id = entity.Id,
                    Num = entity.Num,
                    Credit = entity.Credit,
                    Debt = entity.Debt,
                    Note = entity.Note,
                    JournalId = entity.JournalId,
                    AccountID = entity.AccountID,
                };
            return model;
        }

        public static JournalDet FromModel(JournalDetM model)
        {
            return model == null ? null :
                new JournalDet()
                {
                    Id = model.Id,
                    Num = model.Num,
                    Credit = model.Credit,
                    Debt = model.Debt,
                    Note = model.Note,
                    JournalId = model.JournalId,
                    AccountID = model.AccountID,
                };
        }
    }
}
