using System;
using System.Collections.Generic;
using NUnit.Framework;
using Octokit;
using PBot;

public class IssueStateExtensionTests
{
    [Test]
    public void ShouldDetectExistingState()
    {
        var labels = new List<Label> { new Label(null, "Needs: Something", null) };

        var issue = CreateIssueWithLabels(labels);

        Assert.True(issue.IsInState(MyStates.SomeState), "Should be in some state");
        Assert.False(issue.IsInState(MyStates.SomeOtherState), "Should not be in some other state");
        Assert.False(issue.IsInInitialState<MyStates>(), "Should not be in inital state");
    }

    [Test]
    public void ShouldDetectInitialState()
    {
        var labels = new List<Label>();

        var issue = CreateIssueWithLabels(labels);

        Assert.False(issue.IsInState(MyStates.SomeState), "Should not be in some state");
        Assert.False(issue.IsInState(MyStates.SomeOtherState), "Should not be in some other state");
        Assert.True(issue.IsInInitialState<MyStates>(), "Should be in inital state");
    }

    Issue CreateIssueWithLabels(List<Label> labels)
    {
        return new Issue(null, null, null, null, 0, ItemState.All, "title", "body", null, labels, null, null, 0, null, null, DateTimeOffset.MinValue, null, 0, false);
    }

    class MyStates
    {
        public static IssueState SomeState = "Needs: Something";
        public static IssueState SomeOtherState = "Needs: Something else";
    }
}