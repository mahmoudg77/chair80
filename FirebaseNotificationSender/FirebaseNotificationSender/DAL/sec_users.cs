//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FirebaseNotificationSender.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class sec_users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sec_users()
        {
            this.sec_sessions = new HashSet<sec_sessions>();
        }
    
        public int id { get; set; }
        public string pwd { get; set; }
        public string reset_pwd_token { get; set; }
        public string facebook_token { get; set; }
        public string twitter_token { get; set; }
        public string google_token { get; set; }
        public string instagram_token { get; set; }
        public string confirm_mail_token { get; set; }
        public Nullable<bool> mail_verified { get; set; }
        public Nullable<bool> phone_verified { get; set; }
        public string firebase_uid { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sec_sessions> sec_sessions { get; set; }
        public virtual tbl_accounts tbl_accounts { get; set; }
    }
}
