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
    
    public partial class sec_users_roles
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int role_id { get; set; }
    
        public virtual sec_roles sec_roles { get; set; }
        public virtual sec_users sec_users { get; set; }
    }
}