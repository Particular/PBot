namespace PBot.Buildserver
{
    using System;
    using RestSharp.Deserializers;

    public class BuildDetails : Build
    {


        [DeserializeAs(Name = "finishDate")]
        public DateTime FinishedAt { get; set; }
    }
}