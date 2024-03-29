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
    
    public partial class Receipt
    {
        public System.Guid Id { get; set; }
        public int Num { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime Date { get; set; }
        public string Note { get; set; }
        public byte ReceiptTypeId { get; set; }
        public System.Guid AccountId { get; set; }
        public bool IsCash { get; set; }
        public int UserId { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual ReceiptType ReceiptType { get; set; }
        public virtual SecUser SecUser { get; set; }
    }
}
