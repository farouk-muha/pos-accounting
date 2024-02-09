using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace POSAccounting.Models
{
    public abstract class PagingM<T> : ViewModelBase where T : class
    {
        private int pageSize;
        private int count;
        private string countDisplay;
        private string winName;
        private int curPage;
        private int pageCount;
        private string kw;
        //private Nullable<int> num;
        private Nullable<DateTime> startDate;
        private Nullable<DateTime> endDate;
        private int statusId;
        private bool status = true;
        private byte typeId;
        private ObservableCollection<T> _models;
        public List<IntM> PageSizeList { get; set; }
        public List<SimpleByteM> TypesList { get; set; }
        private string displayImg;
        private decimal amount;
        private decimal credit;
        private decimal debt;

        public Nullable<Guid> Id;

        public int PageSize { get { return pageSize; } set  { pageSize = value;  NotifyPropertyChanged("PageSize"); } }
        public int Count { get { return count; }
            set {
                count = value;
                if (pageSize != 0) PageCount = (Count + PageSize - 1) / PageSize;
                else
                    PageCount = 0;
                 NotifyPropertyChanged("Count"); } }
        public int CurPage { get { return curPage; } set { curPage = value; NotifyPropertyChanged("CurPage"); } }
        public string CountDisplay { get { return countDisplay; } set { countDisplay = value; NotifyPropertyChanged("CountDisplay"); } }
        public string WinName { get { return winName; } set { winName = value; NotifyPropertyChanged("WinName"); } }
        public int PageCount { get { return pageCount; } set { pageCount = value; NotifyPropertyChanged("PageCount"); } }
        //public Nullable<int> Num { get { return num; } set { num = value; NotifyPropertyChanged("Num"); } }
        public string KW { get { return kw; } set { kw = value; NotifyPropertyChanged("KW"); } }
        public Nullable<DateTime> StartDate { get { return startDate; } set { startDate = value; NotifyPropertyChanged("StartDate"); } }
        public Nullable<DateTime> EndDate { get { return endDate; } set { endDate = value; NotifyPropertyChanged("EndDate"); } }
        public int StatusId { get { return statusId; } set { statusId = value; NotifyPropertyChanged("StatusId"); }}
        public bool Status { get { return status; } set { status = value; NotifyPropertyChanged("status"); } }
        public byte TypeId { get { return typeId; } set { typeId = value; NotifyPropertyChanged("TypeId"); } }
        public string DisplayImg { get { return displayImg; } set { displayImg = value; NotifyPropertyChanged("DisplayImg"); } }
        public decimal Amount { get { return amount; } set { amount = value; NotifyPropertyChanged("Amount"); } }
        public decimal Credit { get { return credit; } set { credit = value; NotifyPropertyChanged("Credit"); } }
        public decimal Debt { get { return debt; } set { debt = value; NotifyPropertyChanged("Debt"); } }



        public ObservableCollection<T> Models
        {
            get { return _models; }
            set { _models = value; NotifyPropertyChanged("Models"); }
        }

        public PagingM()
        {
            PageSizeList = GetPagingList();
            PageSize = PageSizeList[0].Id;
        }

        private ICommand _editCommand;
        private ICommand _deleteCommand;
        private ICommand _addCommand;
        private ICommand _firstCommand;
        private ICommand _previousCommand;
        private ICommand _nextCommand;
        private ICommand _lastCommand;
        private ICommand _refreshCommand;
        public ICommand FirstCommand
        {
            get
            {
                if (_firstCommand == null)
                {
                    _firstCommand = new RelayCommand(param => this.Refresh(),
                       param =>
                       {
                           if (Count > 1 && CurPage > 1)
                           {
                               CurPage = 1;
                               return true;
                           }
                           return false;
                       });
                }
                return _firstCommand;
            }
        }
        public ICommand PreviousCommand
        {
            get
            {
                if (_previousCommand == null)
                {
                    _previousCommand = new RelayCommand(param => this.Refresh(),
                    param =>
                    {
                        if (CurPage > 1)
                        {
                            --CurPage;
                            return true;
                        }
                        return false;
                    });
                }
                return _previousCommand;
            }
        }
        public ICommand LastCommand
        {
            get
            {
                if (_lastCommand == null)
                {
                    _lastCommand = new RelayCommand(param => this.Refresh(),
                       param =>
                       {
                           if (PageCount > CurPage)
                           {
                               CurPage = PageCount;
                               return true;
                           }
                           return false;
                       });
                }
                return _lastCommand;
            }
        }
        public ICommand NextCommand
        {
            get
            {
                if (_nextCommand == null)
                {
                    _nextCommand = new RelayCommand(
                        param => { this.Refresh(); },
                    param =>
                    {
                        if (PageCount > CurPage)
                        {
                            ++CurPage;
                            return true;
                        }
                        return false;
                    });
                }
                return _nextCommand;
            }
        }
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(
                        param => { this.Refresh(param); });
                }
                return _refreshCommand;
            }
        }

        public ICommand EditCommand
        {
            get
            {
                if (_editCommand == null)
                {
                    _editCommand = new RelayCommand(param => { this.Update(param); });
                }
                return _editCommand;
            }
        }
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(param => { this.Delete(param); });
                }
                return _deleteCommand;
            }
        }
        public ICommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                {
                    _addCommand = new RelayCommand(param => { this.Add(); });
                }
                return _addCommand;
            }
        }

        public abstract void Load();
        public abstract void Refresh(object param = null);
        public abstract void Add();
        public abstract void Update(object id);
        public abstract void Delete(object id);

        public List<IntM> GetPagingList() => new List<IntM>
        {new IntM() { Id = 10 ,}, new IntM() { Id = 20 ,}, new IntM() { Id = 30 ,}
        , new IntM() { Id = 50 ,}, new IntM() { Id = 100 ,}, new IntM() { Id = 200 ,}
        , new IntM() { Id = 500 ,}};
    }

}
