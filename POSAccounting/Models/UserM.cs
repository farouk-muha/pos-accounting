using POSAccounting.BL;
using POSAccounting.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace POSAccounting.Models
{
    public class UserM : ErrorBaseVM
    {
        private int id ;
        private string userName;
        private string email ;
        public byte[] PassHash { get; set; }
        private string userPassword;
        private string fristName ;
        private string lastName ;
        private string userPhone ; 
        private DateTime birthday ; 
        private string userAddress ;
        private DateTime userRegisterDate ;
        private int cityId ;
        private string cityName ;
        private byte genderId ;
        private string genderName ; 
        private byte statusId ;
        private string userStatusName ;
        private byte localStatusId ;
        private string userLocalStatusName ;
        private byte roleId ;
        private string roleName ;
        private Guid localRoleId ;
        private string localRoleName;
        private int corpId ;
        private string corpName ;
        private string userImg;
        private string localImg;
        private ObservableCollection<TaskM> tasks ;
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken;
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType;
        [JsonProperty(PropertyName = "expires_in")]
        public string ExpiresIn;
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken;
        [JsonProperty(PropertyName = ".issued")]
        public string Issued;
        [JsonProperty(PropertyName = ".expires")]
        public string Expires;

        public int Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public string UserName { get { return userName; }   set { userName = value; NotifyPropertyChanged("UserName"); } }
        public string Email { get { return email; } set { email = value; NotifyPropertyChanged("Email"); } }
        public string Password { get { return userPassword; } set { userPassword = value; NotifyPropertyChanged("Password"); } }
        public string FristName { get { return fristName; } set { fristName = value; NotifyPropertyChanged("FristName"); } }
        public string LastName { get { return lastName; } set { lastName = value; NotifyPropertyChanged("LastName"); } }
        public string Phone { get { return userPhone; } set { userPhone = value; NotifyPropertyChanged("Phone"); } }
        public DateTime Birthday { get { return birthday; } set { birthday = value; NotifyPropertyChanged("Birthday"); } }
        public string Address { get { return userAddress; } set { userAddress = value; NotifyPropertyChanged("Address"); } }
        public DateTime RegisterDate { get { return userRegisterDate; } set { userRegisterDate = value; NotifyPropertyChanged("RegisterDate"); } }
        public int CityId { get { return cityId; } set { cityId = value; NotifyPropertyChanged("CityId"); } }
        public string CityName { get { return cityName; } set { cityName = value; NotifyPropertyChanged("CityName"); } }
        public byte GenderId { get { return genderId; } set { genderId = value; NotifyPropertyChanged("GenderId"); } }
        public string GenderName { get { return genderName; } set { genderName = value; NotifyPropertyChanged("GenderName"); } }
        public byte StatusId { get { return statusId; } set { statusId = value; NotifyPropertyChanged("StatusId"); } }
        public string StatusName { get { return userStatusName; } set { userStatusName = value; NotifyPropertyChanged("StatusName"); } }
        public byte LocalStatusId { get { return localStatusId; } set { localStatusId = value; NotifyPropertyChanged("LocalStatusId"); } }
        public string LocalStatusName { get { return userLocalStatusName; } set { userLocalStatusName = value; NotifyPropertyChanged("LocalStatusName"); } }
        public byte RoleId { get { return roleId; } set { roleId = value; NotifyPropertyChanged("RoleId"); } }
        public string RoleName { get { return roleName; } set { roleName = value; NotifyPropertyChanged("RoleName"); } }
        public Guid LocalRoleId { get { return localRoleId; } set { localRoleId = value; NotifyPropertyChanged("LocalRoleId"); } }
        public string LocalRoleName { get { return localRoleName; } set { localRoleName = value; NotifyPropertyChanged("LocalRoleName"); } }
        public int CorpId { get { return corpId; } set { corpId = value; NotifyPropertyChanged("CorpId"); } }
        public string CorpName { get { return corpName; } set { corpName = value; NotifyPropertyChanged("CorpName"); } }
        public string UserImg { get { return userImg; } set { userImg = value; NotifyPropertyChanged("UserImg"); } }
        public string LocalImg { get { return localImg; } set { localImg = value; NotifyPropertyChanged("LocalImg"); } }
        public ObservableCollection<TaskM> Tasks { get { return tasks; } set { tasks = value; } }


        private string displayImg;
        public string DisplayImg { get { return displayImg; } set { displayImg = value; NotifyPropertyChanged("DisplayImg"); } }

        public UserM()
        {
        }
        public UserM(UserM model)
        {
            Id = model.Id;
            PassHash = model.PassHash;
            Password = model.Password;
            Email = model.Email;
            UserName = model.UserName;
            FristName = model.FristName;
            LastName = model.LastName;
            Phone = model.Phone;
            Birthday = model.Birthday;
            Address = model.Address;
            RegisterDate = model.RegisterDate;
            CityId = model.CityId;
            GenderId = model.GenderId;
            StatusId = model.StatusId;
            LocalStatusId = model.LocalStatusId;
            RoleId = model.RoleId;
            LocalRoleId = model.LocalRoleId;
            CorpId = model.CorpId;
            UserImg = model.UserImg;
            LocalImg = model.LocalImg;
            Tasks = model.Tasks;
            AccessToken = model.AccessToken;
            RefreshToken = model.RefreshToken;
            TokenType = model.TokenType;
            ExpiresIn = model.ExpiresIn;
            Expires = model.Expires;
            Issued = model.Issued;
            CityName = model.CityName;
            DisplayImg = model.DisplayImg;
            CorpName = model.CorpName;
            GenderName = model.GenderName;
            LocalRoleName = model.LocalRoleName;
            LocalStatusName = model.LocalStatusName;
            RoleName = model.RoleName;
            StatusName = model.StatusName;
        }
    }

    public class UserUtls
    {
        public UserM FromEntity(SecUser entity)
        {
            return entity == null ? null : new UserM()
                {
                    Id = entity.Id,
                    PassHash = entity.Password,
                    Email = entity.Email,
                    UserName = entity.UserName,
                    FristName = entity.FristName,
                    LastName = entity.LastName,
                    Phone = entity.Phone,
                    Birthday = entity.Birthday != null ? (DateTime)entity.Birthday : new DateTime(2000),
                    Address = entity.Address,
                    RegisterDate = entity.RegisterDate != null ? (DateTime)entity.RegisterDate : DateTime.Now,
                    CityId = entity.CityId != null ? (byte)entity.CityId : (byte)0,
                    GenderId = entity.GenderId != null ? (byte)entity.GenderId : (byte)0,
                    StatusId = entity.StatusId,
                    LocalStatusId = entity.LocalStatusId,
                    RoleId = entity.RoleId,
                    LocalRoleId = entity.LocalRoleId,
                    CorpId = entity.CorpId != null ? (int)entity.CorpId : 0,
                    UserImg = entity.UserImg,
                    LocalImg = entity.LocalImg,

                };

        }

        public SecUser FromModel(UserM model)
        {
            return model == null ? null :
                new SecUser()
                {
                    Id = model.Id,
                    Email = model.Email?.Trim(),
                    Password = model.PassHash,
                    UserName = model.UserName?.Trim(),
                    FristName = model.FristName?.Trim(),
                    LastName = model.LastName?.Trim(),
                    Phone = model.Phone?.Trim(),
                    Birthday = model.Birthday != null ? model.Birthday : new DateTime(2000),
                    Address = model.Address?.Trim(),
                    RegisterDate = model.RegisterDate != null ? (DateTime)model.RegisterDate : DateTime.Now,
                    CityId = model.CityId != 0 ? (Nullable<byte>)model.CityId : null,
                    GenderId = model.GenderId != 0 ? (Nullable<byte>)model.GenderId : null,
                    StatusId = model.StatusId,
                    LocalStatusId = model.LocalStatusId,
                    RoleId = model.RoleId,
                    LocalRoleId = model.LocalRoleId,
                    CorpId = model.CorpId != 0 ? (Nullable<byte>)model.GenderId : null,
                    UserImg = model.UserImg?.Trim(),
                    LocalImg = model.LocalImg?.Trim(),
                };
        }
    }


    public class LoginM : ErrorBaseVM
    {
        private string userName;
        private string userPassword;
        private string userPasswordError;

        public string UserName { get { return userName; } set { userName = value; NotifyPropertyChanged("UserName"); } }
        public string Password { get { return userPassword; } set { userPassword = value; NotifyPropertyChanged("Password"); } }
        public string PasswordError
        {
            get { return userPasswordError; }
            set
            {
                userPasswordError = value;
                NotifyPropertyChanged("PasswordError");
            }
        }
    }


    public class EditEmailM : ErrorBaseVM
    {
        private string email;
        private string password;
        public string Email { get { return email; } set { email = value; NotifyPropertyChanged("Email"); } }
        public string Password { get { return password; } set { password = value; NotifyPropertyChanged("Password"); } }

    }

    public class ResetPasswordM : LoginM
    {
        private string newPassword;
        public string NewPassword { get { return newPassword; } set { newPassword = value; NotifyPropertyChanged("NewPassword"); } }
        private string newPasswordError;
        public string NewPasswordError { get { return newPasswordError; } set { newPasswordError = value; NotifyPropertyChanged("NewPasswordError"); } }
    }

    public class EditPhoneM : ErrorBaseVM
    {
        public string phone;
        public string password;
        public string Phone { get { return phone; } set { phone = value; NotifyPropertyChanged("Phone"); } }
        public string Password { get { return password; } set { password = value; NotifyPropertyChanged("Password"); } }

    }

}