//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Chair80.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class trip_request
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public trip_request()
        {
            this.trip_request_details = new HashSet<trip_request_details>();
        }
    
        public int id { get; set; }
        public int rider_id { get; set; }
        public Nullable<int> seats { get; set; }
        public Nullable<System.DateTime> start_at_date { get; set; }
        public Nullable<System.DateTime> end_at_date { get; set; }
        public Nullable<int> trip_type_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<int> created_by { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<trip_request_details> trip_request_details { get; set; }
        public virtual trip_types trip_types { get; set; }
        public virtual tbl_accounts tbl_accounts { get; set; }
    }
}
