PBot
===========

Provides pbot commands to help us manage our issues.

### How do I build and deploy a change?

1. Commit the fix
2. Build server will build and auto tag the commit, this means that each green build will get a unique version
3. The bot is setup to watch the TC nuget feed so just go into a channel and type `pbot update package pbot restart`
4. The bot will now update and restart, this take 1-2 min, just check that the bot is alive using `pbot ping`

### Required setup on local machine

#### For our unit tests

* Octokit environment variables: OCTOKIT_GITHUBUSERNAME + OCTOKIT_GITHUBPASSWORD

#### For our integration tests

The integration tests is meant to do as much end to end testing as possible.

##### Testing operations against GitHub repos

We do this kind of testing by creating repos on the fly (and deleting them after) under the github account you specify.

For this to work you need to specify the following environment variables:


* PBOT_GITHUBUSERNAME: The username to use, most likely your regular GH user
* PBOT_OAUTHTOKEN: Since we all have 2 factor auth on you need to [create a access token](https://github.com/settings/tokens/new). Remember to tick the delete_repo scope since the tests will delete repos that it creates. 
