//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace POSAccounting.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SecUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SecUser()
        {
            this.Accounts = new HashSet<Account>();
            this.Emps = new HashSet<Emp>();
            this.Journals = new HashSet<Journal>();
            this.Receipts = new HashSet<Receipt>();
        }
    
        public int Id { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public string UserName { get; set; }
        public string FristName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string Address { get; set; }
        public System.DateTime RegisterDate { get; set; }
        public string UserImg { get; set; }
        public Nullable<short> CityId { get; set; }
        public Nullable<byte> GenderId { get; set; }
        public byte StatusId { get; set; }
        public byte LocalStatusId { get; set; }
        public byte RoleId { get; set; }
        public System.Guid LocalRoleId { get; set; }
        public Nullable<int> CorpId { get; set; }
        public string LocalImg { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual City City { get; set; }
        public virtual CorpInfo CorpInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Emp> Emps { get; set; }
        public virtual Gender Gender { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Journal> Journals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Receipt> Receipts { get; set; }
        public virtual SecLocalRole SecLocalRole { get; set; }
        public virtual SecRole SecRole { get; set; }
        public virtual UserStatus UserStatus { get; set; }
        public virtual UserStatus UserStatus1 { get; set; }
    }
}