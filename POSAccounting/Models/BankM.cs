using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public class BankM : ViewModelBase
    {
        private short id;
        private string name;

        public short Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("Name"); } }
    }

    public class BankUtils
    {
        public BankM FromEntity(Bank entity)
        {
            return entity == null ? null :
                new BankM()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                };
        }

        public Bank FromModel(BankM model)
        {
            return model == null ? null :
                new Bank()
                {
                    Id = model.Id,
                    Name = model.Name,
                };
        }


    }
}
