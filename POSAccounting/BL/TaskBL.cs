using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using POSAccounting.Utils;
using POSAccounting.BL;
using POSAccounting.Models;

namespace POSAccounting.BL
{
    public class TaskBL
    {
        private CropAccountingAppEntities db;
        private TaskUtils taskUtils = new TaskUtils();
        private ActionUtils actionUtils = new ActionUtils();
        private MasterTaskUtils masterTaskUtils = new MasterTaskUtils();

        public TaskBL(CropAccountingAppEntities db)
        {
            this.db = db;
        }

        // tasks
        public IEnumerable<TaskM> Get(Expression<Func<SecTask, bool>> filter)
        {
            var taskQuery = db.SecTasks.Select(m => m);
            if (filter != null)
                taskQuery = taskQuery.Where(filter);
            var entities = taskQuery.ToList();
           
            return entities.Select(m => taskUtils.FromEntity(m)).ToList();
        }

        private void Enabeld(TaskM task, ActionTypeM action)
        {
            if (action.EnActionName.Equals(ConstActionType.Select))
            {
                if (task.cSelect)
                    action.Enabled = true;
            }
            else if (action.EnActionName.Equals(ConstActionType.Insert))
            {
                if (task.cInsert)
                    action.Enabled = true;
            }
            else if (action.EnActionName.Equals(ConstActionType.Update))
            {
                if (task.cUpdate)
                    action.Enabled = true;
            }
            else if (action.EnActionName.Equals(ConstActionType.Delete))
            {
                if (task.cDelete)
                    action.Enabled = true;
            }
            else if (action.EnActionName.Equals(ConstActionType.Print))
            {
                if (task.cDelete)
                    action.Enabled = true;
            }
        }
        public List<TaskM> GetTasksActions(Expression<Func<SecTask, bool>> filter = null)
        {
            var query = from m in db.SecTasks select m;

            if (filter != null)
                query = query.Where(filter);

            var entities = query.ToList();
            var tasks = entities.Select(m => taskUtils.FromEntity(m)).ToList();
            var actions = db.SecActionTypes.ToList();

            foreach (var task in tasks)
            {
                task.Actions = new ObservableCollection<ActionTypeM>();
                foreach (var a in actions)
                {
                    var action = actionUtils.FromEntity(a);
                    Enabeld(task, action);
                    task.Actions.Add(action);
                }
            }
            return tasks;
        }

        //  tasks by role id
        public List<TaskM> GetAllLocalRoleTasks(Guid id)
        {
            var allTasks = GetTasksActions();
            var userTasks = GetLocalRoleTasks(id);

            if (allTasks == null || allTasks.Count < 1 || userTasks == null || userTasks.Count < 1)
                return allTasks;

            for (int i = 0; i < userTasks.Count(); i++)
            {
                foreach (var v in allTasks[i].Actions)
                    Enabeld(allTasks[i], v);

                for (int x = 0; x < allTasks.Count; x++)
                {
                    if (userTasks[i].Id == allTasks[x].Id)
                    {
                        for (int y = 0; y < userTasks[i].Actions.Count(); y++)
                        {
                            for (int z = 0; z < allTasks[x].Actions.Count(); z++)
                            {
                                if (userTasks[i].Actions[y].ActionId == allTasks[x].Actions[z].ActionId)
                                {
                                    allTasks[x].Actions[z].ActionSelected = true;
   
                                }
                            }
                        }
                    }
                }
            }
            return allTasks;
        }

        public List<TaskM> GetLocalRoleTasks(Guid id)
        {
            var tasks = (from ut in db.SecLocalRoleTasks
                    join t in db.SecTasks on ut.TaskId equals t.Id
                    join a in db.SecActionTypes on ut.ActionId equals a.ActionId
                    where ut.LocalRoleId == id
                    group new { ut, t, a } by new { t.Id } into g
                    select new TaskM
                    {
                        Id = g.Key.Id,
                        EnName = g.FirstOrDefault().t.EnName,
                        ArName = g.FirstOrDefault().t.ArName,
                        TempActions = g
                        .GroupBy(k => k.a.ActionId)
                        .Select(a => new ActionTypeM
                        {
                            ActionId = a.Select(aa => aa.a.ActionId).FirstOrDefault(),
                            EnActionName = a.Select(aa => aa.a.EnName).FirstOrDefault(),
                            ArActionName = a.Select(aa => aa.a.ArName).FirstOrDefault(),
                            TaskId = g.Key.Id,
                        }).ToList()
                    }).ToList();

            foreach(var v in tasks)
            {
                v.Actions = new ObservableCollection<ActionTypeM>();
                foreach (var vv in v.TempActions)
                {
                    Enabeld(v, vv);
                    v.Actions.Add(vv);
                }
            }
            return tasks;
        }



        // role task action

        public SecRoleTask GetLocalRoleTaskAction(Guid localRoleId, int taskId, string actionName)
        {
            return (from a in db.SecActionTypes
                    join u in db.SecLocalRoleTasks on a.ActionId equals u.ActionId
                    where u.LocalRoleId == localRoleId && u.TaskId == taskId && a.EnName == actionName
                    select new SecRoleTask
                    {
                        ActionId = a.ActionId,
                    }).FirstOrDefault();
        }

    }

    // role helper class to chech user roles
    public class RoleHelper
    {

        public static bool chkUserRole(CropAccountingAppEntities db, int userId, string taskName, string actionName, UserM user = null)
        {
            if (user == null)
            {
                user = new UserBL(db).GetModelById(userId);
                if (user == null)
                    return false;
            }

            if (user.StatusId != ConstUserStatus.Active)
                return false;

            SecRole group = db.SecRoles.Where(m => m.Id == user.RoleId).FirstOrDefault();

            if (group.Name.Trim().ToUpper().Equals(ConstRole.Admin))
                return true;

            TaskBL taskBL = new TaskBL(db);
            Expression<Func<SecTask, bool>> filter = null;

            if (actionName.Equals(ConstActionType.Select))
            {
                filter = m => m.EnName.Trim().ToUpper().Equals(taskName) && m.cSelect == true;
            }
            else if (actionName.Equals(ConstActionType.Insert))
            {
                filter = m => m.EnName.Trim().ToUpper().Equals(taskName) && m.cInsert == true;

            }
            else if (actionName.Equals(ConstActionType.Update))
            {
                filter = m => m.EnName.Trim().ToUpper().Equals(taskName) && m.cUpdate == true;
            }
            else if (actionName.Equals(ConstActionType.Delete))
            {
                filter = m => m.EnName.Trim().ToUpper().Equals(taskName) && m.cDelete == true;
            }
            else if (actionName.Equals(ConstActionType.Print))
            {
                filter = m => m.EnName.Trim().ToUpper().Equals(taskName) && m.cPrint == true;
            }

            TaskM task = taskBL.Get(filter).FirstOrDefault();
            if (task == null)
                return false;
            if (taskBL.GetLocalRoleTaskAction(user.LocalRoleId, task.Id, actionName) != null)
                return true;

            return false;
        }

        public static bool chkUserLocalRole(CropAccountingAppEntities db, string taskName, string actionName, UserM user = null)
        {
            if (user == null)
            {
                user = Profile.UserProfile;
                if (user == null)
                    return false;
            }

            
            if (user.StatusId != ConstUserStatus.Active)
                return false;

            SecLocalRole group = db.SecLocalRoles.Where(m => m.Id == user.LocalRoleId).FirstOrDefault();

            if (group.Name.Equals(ConstRole.Admin))
                return true;

            TaskBL taskBL = new TaskBL(db);
            Expression<Func<SecTask, bool>> filter = null;

            if (actionName.Equals(ConstActionType.Select))
            {
                filter = m => m.EnName.Trim().Equals(taskName) && m.cSelect == true;
            }
            else if (actionName.Equals(ConstActionType.Insert))
            {
                filter = m => m.EnName.Trim().Equals(taskName) && m.cInsert == true;

            }
            else if (actionName.Equals(ConstActionType.Update))
            {
                filter = m => m.EnName.Trim().Equals(taskName) && m.cUpdate == true;
            }
            else if (actionName.Equals(ConstActionType.Delete))
            {
                filter = m => m.EnName.Trim().Equals(taskName) && m.cDelete == true;
            }
            else if (actionName.Equals(ConstActionType.Print))
            {
                filter = m => m.EnName.Trim().Equals(taskName) && m.cPrint == true;
            }

            TaskM task = taskBL.Get(filter).FirstOrDefault();

            if (task == null)
                return false;
            if (taskBL.GetLocalRoleTaskAction(user.LocalRoleId, task.Id, actionName) != null)
                return true;

            return false;
        }

    }
}