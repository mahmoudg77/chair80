using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FCM.Net;
namespace FirebaseNotificationSender.Libs
{
    public static class FirebaseNotifications
    {
        public static async Task<bool> Send(string[] divices,string title,string body,object data)
        {

            string ServerKey = "AAAAwLYKviI:APA91bFeScuIohBOgrhVbe0f3ZwJSXP7GZHRm5wKCqOlCvq36viLuN5oK9N_KAvGtJDy9Ff_AD7QDCMd7CNcwj5cL-zwZC5SkjYT7JwPdm1eL6lhRn4cF1AWou7pDg6mahZkDC-TVozM";
            using (var sender=new Sender(ServerKey))
            {
                var message = new Message
                {
                    RegistrationIds =  divices.ToList(),
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body,
                        Sound="true",
                        
                        
                    },
                    Data=data,
                    Priority=Priority.High,
                    ContentAvailable=true,
                   
                };
                var result =  sender.SendAsync(message).Result;
                Logger.log($"Success: {result.MessageResponse.Success}");

               
            }

           
            return true;
            
        }
    }
   
}