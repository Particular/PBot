﻿namespace PBot.Buildserver
{
    using System.Collections.Generic;
    using System.Linq;
    using RestSharp;
    using RestSharp.Deserializers;

    public class TeamCity
    {
        RestClient client;

        public TeamCity(string url)
        {
            client = new RestClient(url + "/guestAuth/app/rest/");
        }

        public IEnumerable<Project> ListProjects()
        {
            var request = new RestRequest("projects");

            return client.Get<ListProjectsResponse>(request).Data.Projects;
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

            var response = client.Get<ProjectDetailsResponse>(request);

            return new ProjectDetails
            {
                Id = response.Data.Id,
                Name = response.Data.Name,
                Url = response.Data.Url,
                BuildTypes = response.Data.BuildTypes.BuildType
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

        Build GetCurrentBuild(string projectId, string buildTypeId, string branch)
        {
            var request = new RestRequest("projects/id:{project-id}/buildTypes/id:{build-type-id}/builds/?locator=count:1,branch:{branch}");

            request.AddUrlSegment("project-id", projectId);

            request.AddUrlSegment("branch", branch);
            request.AddUrlSegment("build-type-id", buildTypeId);
            var response = client.Get<GetCurrentBuildResponse>(request);

            return response.Data.build != null ? response.Data.build.First() : null;
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