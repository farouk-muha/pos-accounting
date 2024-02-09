using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public class CorpInfoM : ViewModelBase
    {
        private int id;
        private string name;
        private string phone;
        private string address;
        private string owner;
        private string email;
        private string logo;
        private byte statusId;
        private Nullable<System.DateTime> registerDate;
        private Nullable<System.DateTime> renewDate;
        private byte corpTypeId;
        private string localImg;
        private string displayImg;

        public int Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("Name"); } }
        public string Phone { get { return phone; } set { phone = value; NotifyPropertyChanged("Phone"); } }
        public string Address { get { return address; } set { address = value; NotifyPropertyChanged("Address"); } }
        public string Owner { get { return owner; } set { owner = value; NotifyPropertyChanged("Owner"); } }
        public string Email { get { return email; } set { email = value; NotifyPropertyChanged("Email"); } }
        public string Logo { get { return logo; } set { logo = value; NotifyPropertyChanged("Logo"); } }
        public byte StatusId { get { return statusId; } set { statusId = value; NotifyPropertyChanged("StatusId"); } }
        public Nullable<System.DateTime> RegisterDate { get { return registerDate; } set { registerDate = value; NotifyPropertyChanged("RegisterDate"); } }
        public Nullable<System.DateTime> RenewDate { get { return renewDate; } set { renewDate = value; NotifyPropertyChanged("RenewDate"); } }
        public byte CorpTypeId { get { return corpTypeId; } set { corpTypeId = value; NotifyPropertyChanged("CorpTypeId"); } }
        public string LocalImg { get { return localImg; } set { localImg = value; NotifyPropertyChanged("LocalImg"); } }
        public string DisplayImg { get { return displayImg; } set { displayImg = value; NotifyPropertyChanged("DisplayImg"); } }

        public CorpInfoM()
        {

        }

        public CorpInfoM(CorpInfoM model)
        {
            Id = model.Id;
            Name = model.Name;
            Phone = model.Phone;
            Address = model.Address;
            Owner = model.Owner;
            Email = model.Email;
            Logo = model.Logo;
            StatusId = model.StatusId;
            RegisterDate = model.RegisterDate;
            RenewDate = model.RenewDate;
            CorpTypeId = model.CorpTypeId;
            DisplayImg = model.DisplayImg;
            LocalImg = model.LocalImg;
        }
    }

    public class CorpInfoUtils
    {
        public CorpInfoM FromEntity(CorpInfo entity)
        {
            return entity == null ? null :
                new CorpInfoM()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Phone = entity.Phone,
                    Address = entity.Address,
                    Owner = entity.Owner,
                    Email = entity.Email,
                    Logo = entity.Logo,
                    StatusId = entity.StatusId,
                    RegisterDate = entity.RegisterDate,
                    RenewDate = entity.RenewDate,
                    CorpTypeId = entity.CorpTypeId,
                    LocalImg = entity.LocalImg,
                };
        }

        public CorpInfo FromModel(CorpInfoM model)
        {
            return model == null ? null :
                new CorpInfo()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Phone = model.Phone,
                    Address = model.Address,
                    Owner = model.Owner,
                    Email = model.Email,
                    Logo = model.Logo,
                    StatusId = model.StatusId,
                    RegisterDate = model.RegisterDate,
                    RenewDate = model.RenewDate,
                    CorpTypeId = model.CorpTypeId,
                    LocalImg = model.LocalImg,                };
        }
    }
}
