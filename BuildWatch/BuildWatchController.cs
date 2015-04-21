using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
            {"Slate Integration","Slate_DevelopBuildAndDeploy"},
            {"SlateIva Integration","SlateIva_IntegrationTests"},
            {"PrintManager Integration","SlatePrintManager_DevelopBuildDeploy"},
            {"DrspPortal Integration","DrspPortal_Develop"},
        };
        
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var failedBuilds = _teamCity.GetFailedBuilds(Builds);

            return Request.CreateResponse(HttpStatusCode.OK, failedBuilds, Request.GetConfiguration().Formatters.JsonFormatter); ;
        }

    }

    public class BuildSummary
    {
        public string BuildTypeId { get; set; }
        public string BuildName { get; set; }
        public string DateTime { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
    }
}