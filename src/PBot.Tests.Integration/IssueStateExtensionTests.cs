using System.Collections.Generic;
using NUnit.Framework;
using Octokit;
using PBot;

public class IssueStateExtensionTests
{
    [Test]
    public void ShouldDetectExistingState()
    {
        var issue = new Issue
        {
            Labels = new List<Label>
            {
                new Label
                {
                    Name = "Needs: Something"
                }
            }
        };


        Assert.True(issue.IsInState(MyStates.SomeState),"Should be in some state");
        Assert.False(issue.IsInState(MyStates.SomeOtherState),"Should not be in some other state");
        Assert.False(issue.IsInInitialState<MyStates>(),"Should not be in inital state");
    }

    [Test]
    public void ShouldDetectInitialState()
    {
        var issue = new Issue
        {
            Labels = new List<Label>()
        };


        Assert.False(issue.IsInState(MyStates.SomeState), "Should not be in some state");
        Assert.False(issue.IsInState(MyStates.SomeOtherState), "Should not be in some other state");
        Assert.True(issue.IsInInitialState<MyStates>(), "Should be in inital state");
    }

    class MyStates
    {
        public static IssueState SomeState = "Needs: Something";
        public static IssueState SomeOtherState = "Needs: Something else";
    }
}