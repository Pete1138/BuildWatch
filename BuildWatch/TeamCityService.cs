
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using BuildWatch.DataStructure;

namespace BuildWatch
{
    public class TeamCityService
    {
        private const string TeamcityUrl = "http://build:8000/";

        public builds GetBuildsByTypeName(string buildTypeId)
        {
            var uri = TeamcityUrl + string.Format("guestAuth/app/rest/buildTypes/id:{0}/builds/", buildTypeId);

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

            var serializer = new XmlSerializer(typeof(builds));
            var rdr = new StringReader(response);
            var builds = (builds)serializer.Deserialize(rdr);

            return builds;
        }

        public List<BuildSummary> GetFailedBuilds(Dictionary<string, string> buildTypes)
        {
            var failedBuilds = new List<BuildSummary>();

            var buildList = GetBuildsByTypeName("Slate_DevelopBuildAndDeploy");
            if (buildList.build.Length > 0)
            {
                var currentBuild = buildList.build[0];
                var numberOfBuilds = buildList.build.Length;
                if (currentBuild.status == BuildStatus.Failure)
                {
                    var counter = 1;
                    while (counter < (numberOfBuilds - 1) && currentBuild.status != BuildStatus.Success)
                    {
                        counter++;
                    }
                }
            }

            return failedBuilds;
        }
    }
}