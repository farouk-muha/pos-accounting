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
    
    public partial class UnitConversion
    {
        public System.Guid Id { get; set; }
        public System.Guid ProductId { get; set; }
        public byte BaseUnitId { get; set; }
        public double Multiplier { get; set; }
    }
}