namespace PBot
{
    using System;

    public class IssueState : IEquatable<IssueState>
    {
        string stateIdentifier;

        IssueState(string state)
        {
            stateIdentifier = state;
        }

        public override string ToString()
        {
            return stateIdentifier;
        }

        public static implicit operator string(IssueState state)
        {
            return state.stateIdentifier;
        }

        public static implicit operator IssueState(string state)
        {
            return new IssueState(state);
        }
        public bool Equals(IssueState other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return string.Equals(stateIdentifier, other.stateIdentifier);
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
            return Equals((IssueState)obj);
        }

        public override int GetHashCode()
        {
            return stateIdentifier?.GetHashCode() ?? 0;
        }

        public static bool operator ==(IssueState left, IssueState right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IssueState left, IssueState right)
        {
            return !Equals(left, right);
        }
    }
}