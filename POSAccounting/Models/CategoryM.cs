using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public partial class CategoryM : ViewModelBase
    {
        private Guid id;
        private int num;
        private string name;
        private byte corpTypeId;
        private double itemsCount;

        public Guid Id { get { return id; } set { id = value; NotifyPropertyChanged("id"); } }
        public int Num { get { return num; } set { num = value; NotifyPropertyChanged("Num"); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("name"); } }
        public byte CorpTypeId { get { return corpTypeId; } set { corpTypeId = value; NotifyPropertyChanged("corpTypeId"); } }
        public double ItemsCount { get { return itemsCount; } set { itemsCount = value; NotifyPropertyChanged("ItemsCount"); } }
    }

    public class CategoryUtils
    {
        public CategoryM FromEntity(Category entity)
        {
            return entity == null ? null :
                new CategoryM()
                {
                    Id = entity.Id,
                    Num = entity.Num,
                    Name = entity.Name,
                    CorpTypeId = entity.CorpTypeId,
                };
        }

        public Category FromModel(CategoryM model)
        {
            return model == null ? null :
                new Category()
                {
                    Id = model.Id,
                    Num = model.Num,
                    Name = model.Name,
                    CorpTypeId = model.CorpTypeId,
                };
        }


    }
}
