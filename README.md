IssueButler
===========

Provides pbot commands to help us manage our issues.

### How do I build and deploy a change?

1. Commit the fix
2. Build server will build and auto tag the commit, this means that each green build will get a unique version
3. The bot is setup to watch the TC nuget feed so just go into a channel and type `pbot update issuebutler.mmbot restart`
4. The bot will now update and restart, this take 1-2 min, just check that the bot is alive using `pbot ping`

### Required setup on local machine

* Octokit environment variables: OCTOKIT_GITHUBUSERNAME + OCTOKIT_GITHUBPASSWORD
