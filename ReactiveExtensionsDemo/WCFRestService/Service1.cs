using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace WCFRestService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class Service1
    {

        [WebGet(UriTemplate = "")]
        public List<string> GetNames()
        {
            return new List<string>() {"John", "Ellen", "Tom", "Olivia" };
        }

        [WebInvoke(UriTemplate = "", Method = "POST")]
        public string Create(string instance)
        {
            // TODO: Add the new instance of SampleItem to the collection
            throw new NotImplementedException();
        }

        [WebGet(UriTemplate = "names")]
        public List<string> Get()
        {
            return GetNames();
        }

        [WebGet(UriTemplate = "detect/{text}")]
        public string DetectLanguage(string text)
        {
            if (text.ToLower().Contains("je"))
            {
                return "fr";
            }
            else
                return "en";
        }

        [WebGet(UriTemplate = "translate/{from}/{to}/{text}")]
        public string Translate(string text, string from, string to)
        {
            if (to != "es")
                return "I started this service at 5:45, give me a break!";//finished at 6:20, wcf ftw
            if (from == "fr")
            {

                var text1 = "je suis joel";
                var text2 = "je suis l'homme";
                var text3 = "j'aime le rx";
                if(text1.Contains(text.ToLower()))
                {
                    if(text.Length< 4)
                    {
                        return "yo";
                    }
                    if(text.Length< 9)
                    {
                        return "yo soy";
                    }
                    if(text.Length< 30)
                    {
                        return "me llamo Joel";
                    }
                }
            }
            else if (from == "en")
            {
                var en1 = "hello world";
                var en2 = "where is the library";
                if(en1.Contains(text.ToLower()))
                {
                    if (text.Length < 7)
                    {
                        return "hola";
                    }
                    else
                        return "hola mundo";
                }
                if (en2.Contains(text.ToLower()))
                {
                    var whereisthelibrary = "donde esta la bibliotecha";
                    if (text.Length < "where is the libary".Length -2)
                    {
                        return whereisthelibrary.Substring(0, text.Length);
                    }
                    else
                        return whereisthelibrary;
                }

            }
            return text + "? I started this service at 5:45, give me a break!";
        }
    }
}
