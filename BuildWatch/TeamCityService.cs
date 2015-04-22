
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
            var builds = HttpHelper.Get<builds>(uri);
            return builds;
        }

        private build GetBuildById(string id)
        {
            var uri = TeamcityUrl + string.Format("guestAuth/app/rest/builds/id:{0}", id);
            var build = HttpHelper.Get<build>(uri);
            return build;
        }

        public List<BuildSummary> GetFailedBuilds(Dictionary<string, string> buildTypes)
        {
            var failedBuilds = new List<BuildSummary>();

            foreach (var kvp in buildTypes)
            {
                var buildSummary = GetFailedBuild(kvp.Value, kvp.Key);
                if (buildSummary != null)
                {
                    failedBuilds.Add(buildSummary);
                }
            }

            return failedBuilds;
        }

        private BuildSummary GetFailedBuild(string buildTypeId, string buildName)
        {
            var buildList = GetBuildsByTypeName(buildTypeId);

            if (buildList.build.Length == 0)
            {
                return null;
            }

            var currentBuild = buildList.build[0];

            if (currentBuild.status == BuildStatus.Success)
            {
                return null;
            }

            var numberOfBuilds = buildList.build.Length;

            var counter = 1;
            while (counter < (numberOfBuilds - 1) && currentBuild.status != BuildStatus.Success)
            {
                currentBuild = buildList.build[counter];
                if (currentBuild.status == BuildStatus.Success)
                {
                    counter = numberOfBuilds;
                    currentBuild = buildList.build[counter - 1];
                }
                else
                {
                    counter++;
                }
            }

            var firstFailedBuild = numberOfBuilds == 1 ? currentBuild : buildList.build[counter - 2];
            var buildInfo = GetBuildById(firstFailedBuild.id);
            var buildSummary = new BuildSummary
            {
                BuildTypeId = buildTypeId,
                BuildName = buildName,
                AvatarUrl = string.Empty, //TODO,
                DateTime = DateTime.ParseExact(buildInfo.triggered[0].date, "yyyyMMddTHHmmss+0000", CultureInfo.InvariantCulture).ToLongDateTime(),
                Username = buildInfo.triggered[0].user[0].username,
                Name = buildInfo.triggered[0].user[0].name
            };

            return buildSummary;
        }
    }
}