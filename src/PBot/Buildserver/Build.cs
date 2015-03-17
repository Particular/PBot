namespace PBot.Buildserver
{
    using System;
    using RestSharp.Deserializers;

    public class Build : IEquatable<Build>
    {
        public bool Equals(Build other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Build) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        public static bool operator ==(Build left, Build right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Build left, Build right)
        {
            return !Equals(left, right);
        }

        public string State { get; set; }
        public string Status { get; set; }

        public string Id { get; set; }
        public string Number { get; set; }

        [DeserializeAs(Name = "webUrl")]
        public string Url { get; set; }

        [DeserializeAs(Name = "branchName")]
        public string Branch { get; set; }

        public BuildType BuildType { get; set; }
        public ProjectDetails Project { get; set; }
      
        public override string ToString()
        {
            return string.Format("{0}({1})", Number, Branch);
        }
    }

    public class BuildStatus
    {
        public static string Failed = "FAILURE";
    }
}