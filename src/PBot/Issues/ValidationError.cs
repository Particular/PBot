﻿namespace PBot.Issues
{
    using Octokit;

    public class ValidationError
    {
        public string Reason;
        public Issue Issue;
// ReSharper disable once NotAccessedField.Global
        public Repository Repository;
    }
}