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
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.ProductUnits = new HashSet<ProductUnit>();
        }
    
        public System.Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public System.DateTime Date { get; set; }
        public double QTY { get; set; }
        public bool Status { get; set; }
        public string Img { get; set; }
        public string Note { get; set; }
        public int CorpId { get; set; }
        public System.Guid CategoryId { get; set; }
        public byte DefaultUnitId { get; set; }
        public string LocalImg { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Unit Unit { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductUnit> ProductUnits { get; set; }
    }
}
