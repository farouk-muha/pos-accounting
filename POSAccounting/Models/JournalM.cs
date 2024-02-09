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
    public class JournalM : ViewModelBase
    {
        private Guid id;
        private int num;
        private DateTime date;
        private string details;
        private string note;
        private byte journalTypeId;
        private Nullable<Guid> refId;
        private Nullable<int> userId;
        private string journalTypeName;
        private decimal credit;
        private decimal debt;
        private ObservableCollection<JournalDetM> journalDets;

        public Guid Id  { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public int Num { get { return num; } set { num = value; NotifyPropertyChanged("Num"); } }
        public DateTime Date { get { return date; } set { date = value; NotifyPropertyChanged("Date"); } }
        public string Note { get { return note; } set { note = value; NotifyPropertyChanged("Note"); } }
        public string Details { get { return details; } set { details = value; NotifyPropertyChanged("Details"); } }
        public byte JournalTypeId { get { return journalTypeId; } set { journalTypeId = value; NotifyPropertyChanged("JournalTypeId"); } }
        public Nullable<Guid> RefId { get { return refId; } set { refId = value; NotifyPropertyChanged("RefId"); } }
        public Nullable<int> UserId { get { return userId; } set { userId = value; NotifyPropertyChanged("UserId"); } }
        public string JournalTypeName { get { return journalTypeName; } set { journalTypeName = value; NotifyPropertyChanged("JournalTypeName"); } }
        public decimal Credit { get { return credit; } set { credit = value; NotifyPropertyChanged("Credit"); } }
        public decimal Debt { get { return debt; } set { debt = value; NotifyPropertyChanged("Debt"); } }
        public ObservableCollection<JournalDetM> JournalDets { get { return journalDets; } set { journalDets = value; NotifyPropertyChanged("JournalDets"); } }

    }

    public class JournalUtils
    {
        public static JournalM FromEntity(Journal entity)
        {
            var model = entity == null ? null :
                new JournalM()
                {
                    Id = entity.Id,
                    Num = entity.Num,
                    Date = entity.Date,
                    Details = entity.Details == null ?
                    ConstJournalTypes.GetList().Where(m => m.Id == entity.JournalTypeId).Select(m => m.Name).FirstOrDefault() : entity.Details,
                    Note = entity.Note,
                    JournalTypeId = entity.JournalTypeId,
                    RefId = entity.RefId,
                    UserId = entity.UserId,
                };
            return model;
        }

        public static Journal FromModel(JournalM model)
        {
            string s = ConstJournalTypes.GetList().Where(m => m.Id == model.JournalTypeId).Select(m => m.Name).FirstOrDefault();
            return model == null ? null :
                new Journal()
                {
                    Id = model.Id,
                    Num = model.Num,
                    Date = model.Date,
                    Details = 
                    (
                    string.IsNullOrWhiteSpace(model.Details) ||
                    model.Details.Trim().Equals(ConstJournalTypes.GetList().Where(m => m.Id == model.JournalTypeId).Select(m => m.Name).FirstOrDefault())
                    )
                    ? null : model.Details,
                    Note = model.Note,
                    JournalTypeId = model.JournalTypeId,
                    RefId = model.RefId,
                    UserId = model.UserId,
                };
        }
    }
}
