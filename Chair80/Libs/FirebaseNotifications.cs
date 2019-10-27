using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FCM.Net;
namespace Chair80.Libs
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
                var result = await sender.SendAsync(message);
                Logger.log($"Success: {result.MessageResponse.Success} ids = {string.Join(",",divices)}");

                //var json = "{\"notification\":{\"title\":\"json message\",\"body\":\"works like a charm!\"},\"to\":\"" + registrationId + "\"}";
                //result = await sender.SendAsync(json);
                //Console.WriteLine($"Success: {result.MessageResponse.Success}");
            }

            //Object to JSON STRUCTURE => using Newtonsoft.Json;


            //// Create request to Firebase API
            //string FireBasePushNotificationsURL = "https://fcm.googleapis.com/fcm/send";

            //foreach (string div in divices)
            //{
            //    var messageInformation = new

            //    {

            //        data = new

            //        {

            //            title = title,

            //            text = body

            //        },

            //        //data = data,

            //        content_available = true,
            //        priority = "high",
            //        to = div,//divices
            //        mutable_content=true

            //    };
            //    string jsonMessage = JsonConvert.SerializeObject(messageInformation);
                

            //        using (var client = new HttpClient())

            //        {

            //                var request = new HttpRequestMessage(HttpMethod.Post, FireBasePushNotificationsURL);

            //                request.Headers.TryAddWithoutValidation("Authorization", "key=" + ServerKey);

            //                request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

            //                HttpResponseMessage result;
            //                result = await client.SendAsync(request);
            //                var d = result.Content.ReadAsStringAsync();
            //                 // result.IsSuccessStatusCode;
            //        }
            //}

            return true;
            
        }
    }
    //public class Message

    //{

    //    public string to { get; set; }

    //    public Notification notification { get; set; }

    //    public object data { get; set; }

    //}

    //public class Notification

    //{

    //    public string title { get; set; }

    //    public string text { get; set; }

    //}
}