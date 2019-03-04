using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chair80.DAL;

namespace Chair80.BLL.Security
{
    public class Sessions  
    {
        public sec_sessions Entity { get; set; }
        public Sessions()
        {
            using(MainEntities ctx=new MainEntities())
            {
                this.Entity = new sec_sessions();
                this.Entity.id = Guid.NewGuid();
                this.Entity.start_time = DateTime.Now;
            }
        }

        public Sessions(Guid Token)
        {
            using (MainEntities ctx = new MainEntities())
            {
                this.Entity = ctx.sec_sessions.Include("sec_users").Include("sec_users.tbl_accounts").SingleOrDefault(a=>a.id==Token);
            }
        }
       
       
       

        
    }
}
