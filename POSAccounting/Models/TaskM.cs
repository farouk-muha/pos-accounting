using POSAccounting.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace POSAccounting.Models
{
    public class TaskM : ViewModelBase
    {
        private short id;
        private string arName;
        private string enName;
        private short tOrder;
        private string tableName;
        private bool _cSelect;
        private bool _cInsert;
        private bool _cUpdate;
        private bool _cDelete;
        private bool _cPrint;
        private byte masterTaskId;
        public List<ActionTypeM> TempActions;
        private ObservableCollection<ActionTypeM> actions;

        public short Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public string ArName { get { return arName; } set { arName = value; NotifyPropertyChanged("ArName"); } }
        public string EnName { get { return enName; } set { enName = value; NotifyPropertyChanged("EnName"); } }
        public short TOrder { get { return tOrder; } set { tOrder = value; NotifyPropertyChanged("TOrder"); } }
        public string TableName { get { return tableName; } set { tableName = value; NotifyPropertyChanged("TableName"); } }
        public bool cSelect { get { return _cSelect; } set { _cSelect = value; NotifyPropertyChanged("cSelect"); } }
        public bool cInsert {
            get { return _cInsert; }
            set { _cInsert = value; NotifyPropertyChanged("_cInsert"); } }
        public bool cUpdate { get { return _cUpdate; } set { _cUpdate = value; NotifyPropertyChanged("cUpdate"); } }
        public bool cDelete { get { return _cDelete; } set { _cDelete = value; NotifyPropertyChanged("cDelete"); } }
        public bool cPrint { get { return _cPrint; } set { _cPrint = value; NotifyPropertyChanged("cPrint"); } }
        public ObservableCollection<ActionTypeM> Actions { get { return actions; } set { actions = value; } }
        public byte MasterTaskId { get { return masterTaskId; } set { masterTaskId = value; NotifyPropertyChanged("MasterTaskId"); } }

    }

    public class TaskUtils
    {
        public TaskM FromEntity(SecTask entity)
        {
            return entity == null ? null :
                new TaskM()
                {
                    Id = entity.Id,
                    ArName = entity.ArName,
                    EnName = entity.EnName,
                    TOrder = entity.TOrder != null ? (short)entity.TOrder : (short)0,
                    TableName = entity.TableName,
                    cSelect = entity.cSelect != null ? (bool)entity.cSelect : false,
                    cInsert = entity.cInsert != null ? (bool)entity.cInsert : false,
                    cUpdate = entity.cUpdate != null ? (bool)entity.cUpdate : false,
                    cDelete = entity.cDelete != null ? (bool)entity.cDelete : false,
                    cPrint = entity.cUpdate != null ? (bool)entity.cUpdate : false,
                    MasterTaskId = entity.MasterTaskId != null ? (byte)entity.MasterTaskId : (byte)0,
                };
        }

        public SecTask FromModel(TaskM model)
        {
            return model == null ? null :
                new SecTask()
                {
                    Id = model.Id,
                    ArName = model.ArName,
                    EnName = model.EnName,
                    TOrder = model.TOrder,
                    TableName = model.TableName,
                    cSelect = model.cSelect,
                    cUpdate = model.cUpdate,
                    cDelete = model.cDelete,
                    cPrint = model.cUpdate,
                    MasterTaskId = model.MasterTaskId,
                };
        }
    }


    public class ActionTypeM : ViewModelBase
    {
        private byte actionId;
        private string arActionName;
        private string enActionName;
        private bool actionSelected;
        private short taskId;
        private bool enabled;

        public byte ActionId { get { return actionId; } set { actionId = value; NotifyPropertyChanged("actionId"); } }
        public string ArActionName { get { return arActionName; } set { arActionName = value; NotifyPropertyChanged("arActionName"); } }
        public string EnActionName { get { return enActionName; } set { enActionName = value; NotifyPropertyChanged("enActionName"); } }
        public bool ActionSelected {
            get { return actionSelected; } set { actionSelected = value; NotifyPropertyChanged("actionSelected"); } }
        public short TaskId { get { return taskId; } set { taskId = value; NotifyPropertyChanged("TaskId"); } }
        public bool Enabled { get { return enabled; } set { enabled = value; NotifyPropertyChanged("Enabled"); } }


    }

    public class ActionUtils
    {
        public ActionTypeM FromEntity(SecActionType entity)
        {
            return entity == null ? null :
                new ActionTypeM()
                {
                    ActionId = entity.ActionId,
                    ArActionName = entity.ArName,
                    EnActionName = entity.EnName,
                };
        }

        public SecActionType FromModel(ActionTypeM model)
        {
            return model == null ? null :
                new SecActionType()
                {
                    ActionId = model. ActionId,
                    ArName = model.ArActionName,
                    EnName = model.EnActionName,
                };
        }
    }

    public class MasterTaskM : ViewModelBase
    {
        private byte id;
        private string name;
        private ObservableCollection<TaskM> tasks;

        public byte Id{ get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("Name"); } }
        public List<TaskM> Tasks { get; set; }
        public ObservableCollection<TaskM> TasksObserv { get { return tasks; }set { tasks = value; NotifyPropertyChanged("Tasks"); }}

    }

    public class MasterTaskUtils
    {
        public MasterTaskM FromEntity(SecMasterTask entity)
        {
            return entity == null ? null :
                new MasterTaskM()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                };
        }

        public SecMasterTask FromModel(MasterTaskM model)
        {
            return model == null ? null :
                new SecMasterTask()
                {
                    Id = model.Id,
                    Name = model.Name,
                };
        }
    }

}
