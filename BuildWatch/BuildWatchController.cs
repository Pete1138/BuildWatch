using BuildWatch.DataStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml.Serialization;

namespace BuildWatch
{
    public class BuildWatchController : ApiController
    {
        private readonly TeamCityService _teamCity;
        public BuildWatchController()
        {
            _teamCity = new TeamCityService();
        }

        private static readonly Dictionary<string, string> Builds = new Dictionary<string, string>
        {
            {"Slate","Slate_DevelopBuildAndDeploy"},
            {"SlateIva","SlateIva_IntegrationTests"},
            {"PrintManager","SlatePrintManager_DevelopBuildDeploy"},
            {"DrspPortal","DrspPortal_Develop"},
        };
        
        [HttpGet]
        public HttpResponseMessage Get()
        {
            //var failedBuilds = new List<BuildSummary>();
            //var x = new BuildSummary
            //{
            //    BuildTypeId = "Slate_DevelopBuildAndDeploy",
            //    DateTime = DateTime.Now.ToLongDateTime(),
            //    Username = "Pete Johnson",
            //    AvatarUrl = "http://foobar/test"
            //};

            //failedBuilds.Add(x);

            var failedBuilds = _teamCity.GetFailedBuilds(Builds);

            return Request.CreateResponse(HttpStatusCode.OK, failedBuilds, Request.GetConfiguration().Formatters.JsonFormatter); ;
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