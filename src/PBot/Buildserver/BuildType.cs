namespace PBot.Buildserver
{
    using System;

    public class BuildType : IEquatable<BuildType>
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
        public bool Equals(BuildType other)
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
            return Equals((BuildType) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        public static bool operator ==(BuildType left, BuildType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BuildType left, BuildType right)
        {
            return !Equals(left, right);
        }
    }
}