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
    
    public partial class Invoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Invoice()
        {
            this.InvoiceLines = new HashSet<InvoiceLine>();
        }
    
        public System.Guid Id { get; set; }
        public int Num { get; set; }
        public System.DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public Nullable<decimal> Descount { get; set; }
        public bool IsPayed { get; set; }
        public string Note { get; set; }
        public byte InvoiceTypeId { get; set; }
        public int UserId { get; set; }
        public System.Guid StoreId { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public Nullable<System.Guid> AccountId { get; set; }
        public Nullable<decimal> Payed { get; set; }
        public bool IsCash { get; set; }
        public bool IsRemind { get; set; }
    
        public virtual Account Account { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceLine> InvoiceLines { get; set; }
        public virtual InvoiceType InvoiceType { get; set; }
        public virtual Store Store { get; set; }
    }
}
