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
    
    public partial class SecLocalRoleTask
    {
        public System.Guid LocalRoleId { get; set; }
        public short TaskId { get; set; }
        public byte ActionId { get; set; }
    
        public virtual SecActionType SecActionType { get; set; }
        public virtual SecLocalRole SecLocalRole { get; set; }
        public virtual SecTask SecTask { get; set; }
    }
}
