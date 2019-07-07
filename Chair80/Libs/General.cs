using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Web;

namespace Chair80.Libs
{
    public class General
    {
        public static object getResponse(string url , string data  = "", string method = "GET")
        {
            Type ExcelType = Type.GetTypeFromProgID("MSXML2.ServerXMLHTTP");
            dynamic xmlhttp = Activator.CreateInstance(ExcelType);

           // var xmlhttp = CreateObject();
            xmlhttp.open(method, url, false);
            xmlhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            xmlhttp.send(data);
            string s = xmlhttp.responseText;
            JsonConvert.DeserializeObject<object>(s);

            xmlhttp = null;

            return JsonConvert.DeserializeObject<object>(s);
        }


        public static IPResult GetResponse(string url, string data = "", string method = "GET")
        {
            Type ExcelType = Type.GetTypeFromProgID("MSXML2.ServerXMLHTTP");
            dynamic xmlhttp = Activator.CreateInstance(ExcelType);

            // var xmlhttp = CreateObject();
            xmlhttp.open(method, url, false);
            xmlhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            xmlhttp.send(data);
            string s = xmlhttp.responseText;
            //JsonConvert.DeserializeObject<object>(s);

            xmlhttp = null;

            return JsonConvert.DeserializeObject<IPResult>(s);
        }


        public static UserAgent getAgent(string agentdata)
        {
            string u_agent = agentdata;
            string bname = "Unknown";
            string platform = "Unknown";
            string version = "";

            //First get the platform? 
            if (u_agent.Contains("Android"))
            {
                platform = "android";
            }
            else if (u_agent.Contains("linux"))
            {
                platform = "linux";
            }
            else if (u_agent.Contains("macintosh") || u_agent.Contains("mac os x"))
            {
                platform = "mac";
            }
            else if (u_agent.Contains("windows") || u_agent.Contains("win32"))
            {
                platform = "windows";
            }

            string ub = "Unknown";

            // Next get the name of the useragent yes seperately and for good reason
            if (u_agent.Contains("MSIE") && !u_agent.Contains("Opera"))
            {
                bname = "Internet Explorer";
                ub = "MSIE";
            }
            else if (u_agent.Contains("Firefox"))
            {
                bname = "Mozilla Firefox";
                ub = "Firefox";
            }
            else if (u_agent.Contains("Chrome"))
            {
                bname = "Google Chrome";
                ub = "Chrome";
            }
            else if (u_agent.Contains("Safari"))
            {
                bname = "Apple Safari";
                ub = "Safari";
            }
            else if (u_agent.Contains("Opera"))
            {
                bname = "Opera";
                ub = "Opera";
            }
            else if (u_agent.Contains("Netscape"))
            {
                bname = "Netscape";
                ub = "Netscape";
            }

            UserAgent u = new UserAgent();
            u.name = bname;
            u.platform = platform;
            return u;

        }

        /// <summary>
        /// Equivalent to PHP preg_match but only for 3 requied parameters
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="input"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static bool preg_match(Regex regex, string input, out List<string> matches)
        {
            var match = regex.Match(input);
            var groups = (from object g in match.Groups select g.ToString()).ToList();

            matches = groups;
            return match.Success;
        }

        public static string fetchEntityError(DbEntityValidationException e)
        {
            string s = "";
            foreach (var eve in e.EntityValidationErrors)
            {
                s += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                foreach (var ve in eve.ValidationErrors)
                {
                    s += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                }
            }
            return s;
        }


        public static DataTable GetData(string sql)
        {
            if (HttpContext.Current.Request.Headers != null && HttpContext.Current.Request.Headers["ForceFilter"] != null)
            {
                string d = "";
                if (HttpContext.Current.Request.Headers["ForceFilterTable"] != null && HttpContext.Current.Request.Headers["ForceFilterTable"] != "")
                {
                    if (sql.Contains(HttpContext.Current.Request.Headers["ForceFilterTable"].ToString()) || HttpContext.Current.Request.Headers["ForceFilterTable"].ToString()=="*")
                    {
                         d=HttpContext.Current.Request.Headers["ForceFilter"].ToString();
                    }
                }
            

                if (d != "")
                {
                    if(sql.Contains(" where "))
                    {
                        sql= sql.Replace(" where ", " where " + d + " and ");
                    }
                    else
                    {
                        if (sql.Contains(" order by "))
                        {
                            sql = sql.Replace(" order by ", " where " + d + " order by ");
                        }
                        else
                        {
                            sql += " where "+d;
                        }
                    }
                }

            }
            var dt = DAL.DataAccess.getData(sql);
            return dt;
        }

        public static string SerializeDataTable(DataTable dt)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return JsonConvert.SerializeObject(rows);
        }
        public static List<Dictionary<string, object>> DataTableToDictionary(DataTable dt)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return rows;
        }

        public static DataTableResponse<Dictionary<string, object>> getDataTabe(Requests.DataTableRequest request,string tableName,string sqlWhere="")
        {
            DataTableResponse<Dictionary<string, object>> dt = new DataTableResponse<Dictionary<string, object>>();
            if(request == null)
            {
                request = new Requests.DataTableRequest();
            }
            if (request.length == null || request.length <= 0) request.length = 1000;

            //IEnumerable<DAL.Security.sec_sessions> result = ses.GetAll();
            var sqlData = General.GetData("select count(*) from "+ tableName + (sqlWhere==""?"":" where " + sqlWhere));
            dt.recordsTotal = (int)sqlData.Rows[0][0];


            string whr = sqlWhere;
            if (request.search != null)
            {

                string subWhr = "(";
                foreach (var item in request.columns.Where(a => a.searchable).ToList())
                {
                    subWhr += (subWhr == "(" ? "" : " or ") + item.data + " like '%" + request.search.value + "%' ";
                }
                subWhr += ")";
                whr += (whr == "" ? "" : " and ") + subWhr;

            }

            string orders = "";
            if (request.order != null && request.order.Count > 0)
            {
                foreach (var ord in request.order)
                {
                    if (request.columns[ord.column].orderable)
                    {
                        orders += (orders == "" ? "" : " , ") + request.columns[ord.column].data + " " + ord.dir;
                    }
                }
                orders = (orders == "" ? "" : " order by ") + orders;
            }
           

            sqlData = General.GetData("select count(*) from " + tableName  + " " + (whr == "" ? "" : " where " + whr));

            dt.recordsFiltered = (int)sqlData.Rows[0][0];

            sqlData = General.GetData("select top " + request.length + " * from "
                                           + " ("
                                           + " select *,"
                                           + " ROW_NUMBER() " + (orders == "" ? " OVER(Order By id asc)" : " OVER(" + orders + ")") + " AS ROW_NUM"
                                           + " from  " + tableName  + " " + " " + (whr == "" ? "" : " where " + whr)
                                           + " ) x"
                                           + " where ROW_NUM > " + request.start );


            //Sessions ses = new Sessions();

            dt.data = General.DataTableToDictionary(sqlData).AsEnumerable();
            return dt;
        }

        
        public static bool ValidateMobile(string no,out string FixedNo,string perfix= "20")
        {

	        no =no.Trim();
            //Clean number
            FixedNo = "";
            try
            {

            
            string[] spechialChars = { "'", "\"", "\\", "/", ":", ".", "[", "]", "(", ")", "<", ">", "@", "!", "#", "", "%", "^", "&", "*", "-", "_", "+", "=", "~" };

            string[] items2 = no.Split().Where(x => spechialChars.Contains(x)==false).ToArray();

            no = string.Join("",items2);
            

            if (perfix != "20"){
                if (no.Substring( 0, perfix.Length) ==perfix){
	                FixedNo =no;
                    return true;
                }else{
	                FixedNo =perfix + no;
                    return true;
                }
            }
            if (no.Length < 9)
            {
                return false;
            }
            else if(no.Length == 9){
                if (no.Substring( 0, 1) == "1")
                {
			        no = "0" + no;
			        string newNo="" ;
			        bool r = ValidateMobile(no,out newNo);
			        FixedNo =newNo;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if(no.Length == 10){
                if (no.Substring( 0, 1) == "1")
                {
                    if (no.Substring( 0, 3) == "150")
                    {
				        FixedNo = "20120"+no.Substring( 3, 12);
                        return true;
                    }
                    else if(no.Substring( 0, 3) == "151"){
				        FixedNo = "20101" + no.Substring( 3, 12);
                        return true;
                    }
                    else if(no.Substring( 0, 3) == "152"){
				        FixedNo = "20112"+ no.Substring( 3, 12);
                        return true;
                    }
                    else if(no.Substring( 0, 2) == "10" || no.Substring( 0, 2) == "11" || no.Substring( 0, 2) == "12"){
				        FixedNo = "20"+no;
                        return true;
                    }else{

                        return false;
                    }
                }
                else if(no.Substring( 0, 2) == "01"){
                    string comno = "";
                    if (no.Substring( 0, 3) == "011" || no.Substring( 0, 3) == "014")
                    {
				          comno = "011";
                    }
                    else if(no.Substring( 0, 3) == "012" || no.Substring( 0, 3) == "018" || no.Substring( 0, 3) == "017"){
                          comno = "012";
                    }
                    else if(no.Substring( 0, 3) == "010" || no.Substring( 0, 3) == "016" || no.Substring( 0, 3) == "019"){
                          comno = "010";
                    }else{
                        
                        return false;
                    }
			
			        FixedNo = "2"+comno+no.Substring( 2, 12);
                    return true;

                }else{
                    return false;
                }
            }
            else if(no.Length == 11){
                if (no.Substring( 0, 1) != "0")
                {
                    return false;
                }
                else
                {
			FixedNo = "2" + no;
                    return true;
                }
            }
            else if(no.Length == 12){

                if (no.Substring( 0, 4) == "2010" || no.Substring( 0, 4) == "2011" || no.Substring( 0, 4) == "2012")
                {
			        FixedNo =no;
                    return true;
                }
                else
                {

                    return false;

                }
            }
            else if(no.Length == 13){
                if (no.Substring( 0, 2) != "+2" || no.Substring( 0, 2) != "02")
                {
                    return false;
                }
                else
                {
			        FixedNo = "2"+no.Substring( 2, 12);
                    return true;
                }
            }
            else if(no.Length == 14){
                if (no.Substring( 0, 3) != "002")
                {
                    return false;
                }
                else
                {
			        FixedNo = "2"+no.Substring( 3, 13);
                    return true;
                }
            }
            }
            catch (Exception ex)
            {

                return false;
            }
            return false;

        }
        public static string[] ValidateMobileArray(string[] Numbers,string perfix= "20")
        {
            List<string> ValidList = new List<string>();

            foreach (string no in Numbers ){
                string FixedNo = "";
                if (ValidateMobile(no,out FixedNo,perfix))
                {
                    ValidList.Add(FixedNo);
                }
            }

            return ValidList.Distinct().ToArray();
        }

        public static void SendEmail(string ToEmail, string subject, string body)
        {
            SmtpClient smtpClient = new SmtpClient();
            MailAddress from = new MailAddress(ConfigurationManager.AppSettings["EmailSender"], ConfigurationManager.AppSettings["EmailSenderName"]);
            MailAddress to = new MailAddress(ToEmail, ToEmail);
            MailMessage mail = new MailMessage(from, to);
            mail.Subject = subject;
            mail.Body = body;
            mail.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
            mail.IsBodyHtml = true;
            SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"]);
            SmtpServer.Port = int.Parse(ConfigurationManager.AppSettings["SMTPPort"]);
            SmtpServer.EnableSsl = true;
            SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTPLogin"], ConfigurationManager.AppSettings["SMTPPassword"]);
            SmtpServer.Send(mail);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mTo"></param>
        /// <param name="mSubject"></param>
        /// <param name="mBody"></param>
        /// <param name="SystemName"></param>
        /// <param name="mCC"></param>
        /// <param name="mBCC"></param>
        /// <param name="attachmentsFiles"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool SendMail(string mTo, string mSubject, string mBody, string SystemName = "Metookey", string mCC = "", string mBCC = "", string attachmentsFiles = "", bool isHTML = false)
        {
            if (string.IsNullOrEmpty(mTo) | string.IsNullOrEmpty(mSubject) | string.IsNullOrEmpty(mBody))
                return false;


            System.Net.Mail.SmtpClient SmtpClient = new System.Net.Mail.SmtpClient();
            MailMessage message = new MailMessage();

            try
            {


                // MailAddress fromAddress = new MailAddress(txtEmail.Text, txtName.Text)

                // You can specify the host name or ipaddress of your server
                // Default in IIS will be localhost 
                //var configurationFile = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath + "/Web.config");
                //MailSettingsSectionGroup mailSettings = configurationFile.GetSectionGroup("system.net/mailSettings");

                //int port = int.Parse(ConfigurationManager.AppSettings["SMTPPort"]);
                //string host = ConfigurationManager.AppSettings["SMTPServer"];
                //string password = ConfigurationManager.AppSettings["SMTPPassword"];
                //string username = ConfigurationManager.AppSettings["SMTPLogin"];
                //string From = ConfigurationManager.AppSettings["EmailSender"];

                int port = int.Parse(Settings.Get("smtp_port"));
                string host = Settings.Get("smtp_host");
                string password = Settings.Get("smtp_password");
                string username = Settings.Get("smtp_user");
                string From = Settings.Get("email_sender");
                string SenderName = Settings.Get("email_name");


                SmtpClient.Host = host;
                SmtpClient.Port = port; // 587

                if (!(attachmentsFiles == null) && !(attachmentsFiles == ""))
                {
                    string[] Attach = attachmentsFiles.Split(new char[]{ ';'});
                    var loopTo = Attach.Count() - 1;
                    for (int n = 0; n <= loopTo; n++)
                    {
                        if (!string.IsNullOrEmpty(Attach[n]))
                            message.Attachments.Add(new System.Net.Mail.Attachment(Attach[n].Replace( "/", @"\"), "text/html"));
                    }
                }

                SmtpClient.Credentials = new System.Net.NetworkCredential(username, password);

                SmtpClient.EnableSsl = true;
                // Default port will be 25

                // From address will be given as a MailAddress Object
                message.From = new MailAddress(From, SystemName==""? SenderName: SystemName);

                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                // To address collection of MailAddress
                string[] Tos = mTo.Split(new char[] { ';' }); 
                var loopTo1 = Tos.Count() - 1;
                for (int n = 0; n <= loopTo1; n++)
                {
                    if (!string.IsNullOrEmpty(Tos[n]))
                        message.To.Add(Tos[n]);
                }
                message.Subject = mSubject;

                // CC and BCC optional
                // MailAddressCollection class is used to send the email to various users
                // You can specify Address as new MailAddress("admin1@yoursite.com")
                if (!(mCC == null))
                {
                    string[] CCs =mCC.Split(new char[] { ';' }); ;
                    var loopTo2 = CCs.Count() - 1;
                    for (int n = 0; n <= loopTo2; n++)
                    {
                        if (!string.IsNullOrEmpty(CCs[n]))
                            message.CC.Add(CCs[n]);
                    }
                }

                // You can specify Address directly as string
                if (!(mBCC == null))
                {
                    string[] BCCs = mBCC.Split(new char[] { ';' }); ;
                    var loopTo3 = BCCs.Count() - 1;
                    for (int n = 0; n <= loopTo3; n++)
                    {
                        if (!string.IsNullOrEmpty(BCCs[n]))
                            message.Bcc.Add(BCCs[n]);
                    }
                }
                // Body can be Html or text format
                // Specify true if it  is html message
                message.IsBodyHtml = isHTML;

               

                message.Body = mBody;

                // Send SMTP mail
                SmtpClient.Send(message);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string MD5(string password)
        {
            //Encrypt the password
            System.Security.Cryptography.MD5 md5Hasher =MD5CryptoServiceProvider.Create();
            byte[] hashedBytes;
            ASCIIEncoding encoder = new ASCIIEncoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(password));

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hashedBytes.Length; i++)

            {

                sb.Append(hashedBytes[i].ToString("X2"));

            }

            return sb.ToString();
        }

        public static bool EmailIsValid(string emailaddress)
        {

            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static string ApplicationUrl()
        {
            return string.Format("{0}://{1}{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.ServerVariables["HTTP_HOST"], HttpContext.Current.Request.ApplicationPath.Equals("/") ? string.Empty : HttpContext.Current.Request.ApplicationPath);
        }
        public static string HostUrl()
        {
            return string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.ServerVariables["HTTP_HOST"]);
        }
    }
}