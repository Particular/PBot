PBot
===========

Provides pbot commands to help us manage our issues.

### How do I build and deploy a change?

1. Commit the fix
2. Build server will build and auto tag the commit, this means that each green build will get a unique version
3. The bot is setup to watch the TC nuget feed so just go into a channel and type `pbot update package PBot restart`
4. The bot will now update and restart, this take 1-2 min, just check that the bot is alive using `pbot ping`

### Required setup on local machine

1. Copy c:\mmbot from deploy server. This way you get both binaries, packages and brain(persisted storage). Deploy server details in lastpass. (deploy.nservicebus.com - RDP access)
2. Set environment variables OCTOKIT_GITHUBUSERNAME and OCTOKIT_GITHUBPASSWORD. Check lastpass by searching for `pbot`.
3. Edit mmbot.ini 
	* Run against TEST - https://particular-test.slack.com (add yourself as a user by signing up)
	```		
		TEAM = particular-test
		TOKEN = <get from lastpass>		
	```
	* Run against PROD - https://particularsoftware.slack.com
	```		
		TEAM = particularsoftware
		TOKEN = <get from lastpass>		
	```
4. Fork pbot https://github.com/Particular/Pbot
5. Make changes in local pbot repo
6. Copy `PBot.Mmbot.dll` and `PBot.Mmbot.pdb` from local pbot output to <mmbot-dir>/packages/PBot.x.x.xx
7. (Re)start mmbot
8. Repeat 5 to 7
9. Push when happy :)

#### For our unit tests

* Octokit environment variables: OCTOKIT_GITHUBUSERNAME + OCTOKIT_GITHUBPASSWORD

#### For our integration tests

The integration tests is meant to do as much end to end testing as possible.

##### Testing operations against GitHub repos

We do this kind of testing by creating repos on the fly (and deleting them after) under the github account you specify.

For this to work you need to specify the following environment variables:


* PBOT_GITHUBUSERNAME: The username to use, most likely your regular GH user
* PBOT_OAUTHTOKEN: Since we all have 2 factor auth on you need to [create a access token](https://github.com/settings/tokens/new). Remember to tick the delete_repo scope since the tests will delete repos that it creates. 
