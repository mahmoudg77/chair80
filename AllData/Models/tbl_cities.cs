//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AllData.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_cities
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_cities()
        {
            this.tbl_accounts = new HashSet<tbl_accounts>();
        }
    
        public int id { get; set; }
        public int country_id { get; set; }
        public string name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_accounts> tbl_accounts { get; set; }
        public virtual tbl_countries tbl_countries { get; set; }
    }
}
