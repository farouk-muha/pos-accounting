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
    
    public partial class Account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Account()
        {
            this.Accounts1 = new HashSet<Account>();
            this.Clients = new HashSet<Client>();
            this.Emps = new HashSet<Emp>();
            this.Invoices = new HashSet<Invoice>();
            this.JournalDets = new HashSet<JournalDet>();
            this.Receipts = new HashSet<Receipt>();
        }
    
        public System.Guid Id { get; set; }
        public int Num { get; set; }
        public string EnName { get; set; }
        public string ArName { get; set; }
        public bool Deleteable { get; set; }
        public Nullable<int> AccountEndId { get; set; }
        public Nullable<System.Guid> AccountParentId { get; set; }
        public int CropId { get; set; }
        public int UserId { get; set; }
    
        public virtual AccountEnd AccountEnd { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Account> Accounts1 { get; set; }
        public virtual Account Account1 { get; set; }
        public virtual CorpInfo CorpInfo { get; set; }
        public virtual SecUser SecUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Client> Clients { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Emp> Emps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JournalDet> JournalDets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Receipt> Receipts { get; set; }
    }
}
