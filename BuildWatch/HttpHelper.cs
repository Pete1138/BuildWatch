using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace BuildWatch
{
    public static class HttpHelper
    {
        public static T Get<T>(string url) where T: new()
        {

            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "GET";
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            var response = string.Empty;

            using (var responseStream = webResponse.GetResponseStream())
            {
                if (responseStream != null)
                {
                    using (var myStreamReader = new StreamReader(responseStream, Encoding.Default))
                    {
                        response = myStreamReader.ReadToEnd();
                    }
                }
            }
            webResponse.Close();
            if (string.IsNullOrEmpty(response)) return new T();

            var serializer = new XmlSerializer(typeof(T));
            var rdr = new StringReader(response);
            var responseObject = (T)serializer.Deserialize(rdr);

            return responseObject;
        }
    }
}