using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public class RoleM : ViewModelBase
    {
        private byte id ;
        private string name ;
        private string notes ;

        public byte Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("Name"); } }
        public string Notes { get { return notes; } set { notes = value; NotifyPropertyChanged("Notes"); } }
    }

    public class RoleUtils
    {
        public RoleM FromEntity(SecRole entity)
        {
            return entity == null ? null :
                new RoleM()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Notes = entity.Notes,
                };
        }

        public SecRole FromModel(RoleM model)
        {
            return model == null ? null :
                new SecRole()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Notes = model.Notes,
                };
        }


    }

    public class LocalRoleM : ViewModelBase
    {
        private Guid id ;
        private string name ;
        private string notes ;
        private int corpId;

        public Guid Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("Name"); } }
        public string Notes { get { return notes; } set { notes = value; NotifyPropertyChanged("Notes"); } }
        public int CorpId { get { return corpId; } set { corpId = value; NotifyPropertyChanged("corpId"); } }

    }

    public class LocalRoleUtils
    {
        public LocalRoleM FromEntity(SecLocalRole entity)
        {
            return entity == null ? null :
                new LocalRoleM()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Notes = entity.Notes,
                    CorpId = entity.CorpId,
                };
        }

        public SecLocalRole FromModel(LocalRoleM model)
        {
            return model == null ? null :
                new SecLocalRole()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Notes = model.Notes,
                    CorpId = model.CorpId,
                };
        }


    }

}
