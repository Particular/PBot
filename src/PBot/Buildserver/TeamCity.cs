namespace PBot.Buildserver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using RestSharp;
    using RestSharp.Deserializers;

    public class TeamCity
    {
        RestClient client;

        public TeamCity(string url)
        {
            var username = Environment.GetEnvironmentVariable("PBOT_TCUSERNAME");
            var password = Environment.GetEnvironmentVariable("PBOT_TCPASSWORD");

            if (username != null && password != null)
            {
                client = new RestClient(url + "httpAuth/app/rest/")
                {
                    Authenticator = new HttpBasicAuthenticator(username, password)
                };
            }
            else
            {
                client = new RestClient(url + "guestAuth/app/rest/");
            }
        }

        public IEnumerable<Project> ListProjects()
        {
            var request = new RestRequest("projects");

            return ExecuteGet<ListProjectsResponse>(request).Projects;
        }

        public ProjectDetails GetProjectById(string id)
        {
            return GetProjectByLocator("id:" + id);
        }

        public ProjectDetails GetProject(string name)
        {
            return GetProjectByLocator("name:" + name);
        }

        ProjectDetails GetProjectByLocator(string locator)
        {
            var request = new RestRequest("projects/{locator}");

            request.AddUrlSegment("locator", locator);

            var response = ExecuteGet<ProjectDetailsResponse>(request);

            return new ProjectDetails
            {
                Id = response.Id,
                Name = response.Name,
                Url = response.Url,
                BuildTypes = response.BuildTypes.BuildType
            };
        }

        public IEnumerable<Build> ListCurrentBuilds(string projectId, string[] branches)
        {
            var project = GetProjectById(projectId);

            foreach (var buildType in project.BuildTypes)
            {
                foreach (var branch in branches)
                {
                    var build = GetCurrentBuild(project.Id, buildType.Id, branch);

                    if (build != null)
                    {
                        build.Project = project;
                        build.BuildType = buildType;

                        yield return build;
                    }
                }
            }
        }

        public BuildDetails GetBuild(string id)
        {
            var request = new RestRequest("builds/{locator}");

            request.AddUrlSegment("locator", id);

            return ExecuteGet<BuildDetails>(request);
        }

        Build GetCurrentBuild(string projectId, string buildTypeId, string branch)
        {
            var request = new RestRequest("projects/id:{project-id}/buildTypes/id:{build-type-id}/builds/?locator=count:1,branch:{branch}");

            request.AddUrlSegment("project-id", projectId);

            request.AddUrlSegment("branch", branch);
            request.AddUrlSegment("build-type-id", buildTypeId);
            var response = ExecuteGet<GetCurrentBuildResponse>(request);

            return response.build?.First();
        }

        TResponse ExecuteGet<TResponse>(RestRequest request) where TResponse : new()
        {
            request.DateFormat = "yyyyMMdd'T'HHmmsszzz";//20150226T123215+0000

            var response = client.Get<TResponse>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Requestfailed with code: {response.StatusCode} - {response.StatusDescription}");
            }

            return response.Data;
        }

        class GetCurrentBuildResponse
        {
            public List<Build> build { get; set; }
        }

        public bool IsBuildCurrentlyFailed(string projectId, string buildTypeId, string branch)
        {
            var request = new RestRequest("projects/id:{project-id}/buildTypes/id:{build-type-id}/builds/?locator=status:failure,sinceBuild:(status:success),count:1,branch:{branch}");

            request.AddUrlSegment("project-id", projectId);

            request.AddUrlSegment("branch", branch);
            request.AddUrlSegment("build-type-id", buildTypeId);
            var response = client.Get<BuildStatusResponse>(request);

            return response.Data.IsFailed();
        }

        class ProjectDetailsResponse
        {
            public string Id { get; set; }
            public string Name { get; set; }

            [DeserializeAs(Name = "webUrl")]
            public string Url { get; set; }

            public BuildTypesResponse BuildTypes { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        class BuildTypesResponse
        {
            public List<BuildType> BuildType { get; set; }
        }

        class BuildStatusResponse
        {
            public int Count { get; set; }

            public bool IsFailed()
            {
                return Count > 0;
            }
        }

        class ListProjectsResponse
        {
            [DeserializeAs(Name = "project")]
            public List<Project> Projects { get; set; }
        }
    }
}