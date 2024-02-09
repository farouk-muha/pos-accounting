using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public partial class StoreM : ViewModelBase
    {
        private Guid id;
        private int num;
        private string name;
        private int cropId;
        private double itemsCount;
        public Guid Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public int Num { get { return num; } set { num = value; NotifyPropertyChanged("Num"); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("Name"); } }
        public int CropId { get { return cropId; } set { cropId = value; NotifyPropertyChanged("CropId"); } }
        public double ItemsCount { get { return itemsCount; } set { itemsCount = value; NotifyPropertyChanged("ItemsCount"); } }
    }

    public class StoreUtils
    {
        public StoreM FromEntity(Store entity)
        {
            return entity == null ? null :
                new StoreM()
                {
                    Id = entity.Id,
                    Num = entity.Num,
                    Name = entity.Name,
                    CropId = entity.CropId,
                };
        }

        public Store FromModel(StoreM model)
        {
            return model == null ? null :
                new Store()
                {
                    Id = model.Id,
                    Num = model.Num,
                    Name = model.Name,
                    CropId = model.CropId,
                };
        }


    }
}
