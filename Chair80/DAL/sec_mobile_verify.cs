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
    
    public partial class sec_mobile_verify
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string mobile { get; set; }
        public string code { get; set; }
        public Nullable<bool> is_used { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.Guid> verification_id { get; set; }
    }
}
