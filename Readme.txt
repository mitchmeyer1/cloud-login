Welcome to CloudLogin!

For full documentation Please check out cloudlogin.dev/docs


How to set your game up with CloudLogin
1) navigate to cloudlogin.dev
2) create your account, then create a 'game'
3) Import the prefab "CloudLoginInitializer" into your first scene
4) To connect, in a new script: CloudLogin.SetUpGame(gameID, gameToken, ApplicationSetUp); where ApplicationSetUp is a callback function
4.1) void ApplicationSetUp(string message, bool hasError) {} should be the function to impliment on callback from connecting to your server
5) Once connected, you can use any function on cloudlogin.dev/docs to get and set the data for your user.  