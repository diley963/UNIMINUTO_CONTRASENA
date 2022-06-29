using System;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Helpers.Implementations
{
    public class SendSmsWhitTwilio
    {
        public SendSmsWhitTwilio()
        {
                
        }

        /// <summary>
        /// Method to send text messages to mobile phone using Twilio 
        /// </summary>
        /// <param name="destination">mobile Number destination</param>
        public void SendMessage(string destination = "573217115109",string code="000000")
        {
            try
            {
                const string accountSid = "AC2d3fd91bc6478a1bce616f175680074f";
                const string authToken = "39c3470c69a2180d58623a6e5001a7d5";

                //we validate that the phone number does not have a zip code, otherwise we add it to it
                var codPostal = destination.Substring(0, 2);
                if (!code.Contains("57"))
                {
                    destination = "57" + destination;
                }

                TwilioClient.Init(accountSid, authToken);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                    | SecurityProtocolType.Tls11
                                                    | SecurityProtocolType.Tls12
                                                    | SecurityProtocolType.Ssl3;
                var message = MessageResource.Create(
                    body: "Hola Su codigo de verificacion Uniminuto es => "+code,
                    from: new Twilio.Types.PhoneNumber("13608105616"),
                     to: new Twilio.Types.PhoneNumber(destination)
                // to: new Twilio.Types.PhoneNumber("573022517878")

                );
            }
            catch (Exception e)
            {

                throw;
            }
            

        }
    }
}
