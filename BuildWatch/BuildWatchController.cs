using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Serialization;
using BuildWatch.DataStructure;

namespace BuildWatch
{
    public class BuildWatchController : ApiController
    {
        private static string _teamcityUrl = "http://build:8000/";
        private static Dictionary<string, string> _builds = new Dictionary<string, string>
        {
            {"Slate","Slate_DevelopBuildAndDeploy"},
            {"SlateIva","SlateIva_IntegrationTests"},
            {"PrintManager","SlatePrintManager_DevelopBuildDeploy"},
            {"DrspPortal","DrspPortal_Develop"},
        };
        
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var failedBuilds = new List<BuildSummary>();
            var x = new BuildSummary
            {
                BuildTypeId = "Slate_DevelopBuildAndDeploy",
                DateTime = DateTime.Now.ToLongDateTime(),
                Username = "Pete Johnson",
                AvatarUrl = "http://foobar/test"
            };

            failedBuilds.Add(x);

            var failed = GetBuilds(_builds["Slate"]);

            return Request.CreateResponse(HttpStatusCode.OK, failedBuilds, Request.GetConfiguration().Formatters.JsonFormatter); ;
        }

        private static builds GetBuilds(string buildTypeId)
        {
            var uri = _teamcityUrl + string.Format("guestAuth/app/rest/buildTypes/id:{0}/builds/", buildTypeId);

            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.Method = "GET";
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            string response = string.Empty;

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
            if (string.IsNullOrEmpty(response)) return new builds();

            var serializer = new XmlSerializer(typeof (builds));
            var rdr = new StringReader(response);
            var builds = (builds) serializer.Deserialize(rdr);

            return builds;
        }
    }

    public class BuildSummary
    {
        public string BuildTypeId { get; set; }
        public string DateTime { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
    }
}