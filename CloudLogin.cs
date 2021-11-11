using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Boomlagoon.JSON;
//using UnityEditor;

namespace CloudLoginUnity
{
    /// <summary>Class <c>CloudLogin</c> is your connection with the CloudLogin
    /// API.  Through this class you can connect to your apps, login users,
    /// create users.  When a user is signed in you can use CloudLoginUser to 
    /// access your account, attributes and purchased store items.
    /// </summary>
    public class CloudLogin : MonoBehaviour
    {

        static UnityWebRequestAsyncOperation request;

        //#region editor actions
        //[MenuItem("Tools/CloudLogin/Generate CloudLogin Account")]
        //private static void GenerateCloudLoginAccount()
        //{
        //    EditorUtility.DisplayProgressBar("Generating CloudLogin Account...", "...", 0);
        //    UnityWebRequest www = UnityWebRequest.Post(baseURL+"/games","");


        //    request = www.SendWebRequest();

        //    while (!request.isDone)
        //    {

        //    }

        //    var data = www.downloadHandler.text;
        //    JSONObject json = JSONObject.Parse(data);

        //    EditorUtility.ClearProgressBar();
        //    EditorUtility.DisplayDialog("Account Created", "Your id is '" + json.GetNumber("id").ToString() + "'  and your token is '" + json.GetString("token") + "' - Write this down and use it with CloudLogin.SetUpGame in your code \n \ncheck out cloudlogin.dev/docs for instructions\n\nYou now may integrate sign up, login, metadata and in game store into your Unity Game!", "cool thanks");

        //}

       

        //#endregion

        #region instance vars
        private string id;
        private string token;
        private string name;
        private string description;
        private bool verboseLogging = false;
        private List<CloudLoginStoreItem> store = new List<CloudLoginStoreItem>();
        public List<CloudLoginLeaderboardEntry> leaderboardEntries = new List<CloudLoginLeaderboardEntry>();

        #endregion

        #region singleton management
        private static CloudLogin _instance;
        public static CloudLogin Instance { get { return _instance; } }
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion

        #region globals
        private static string baseURL = "https://cloudlogin.dev/api/v1";
        //private static string baseURL = "http://localhost:3000/api/v1";

        public static string GetBaseURL()
        {
            return baseURL;
        }
        #endregion


        #region instance getters
        public static string GetGameId()
        {
            return Instance.id;
        }
        public static string GetGameName()
        {
            return Instance.name;
        }
        public static string GetGameDescription()
        {
            return Instance.description;
        }
        internal static string GetGameToken()
        {
            return Instance.token;
        }

        internal static bool GetVerboseLogging()
        {
            return Instance.verboseLogging;
        }
        #endregion


        #region instance setters
        internal static void SetVerboseLogging(bool _verboseLogging)
        {
            Instance.verboseLogging = _verboseLogging;
        }
        #endregion

        #region logger
        internal static void Log(string log)
        {
            if (GetVerboseLogging())
                Debug.Log("<color=green> " + log + " </color>");
        }
        #endregion


        #region request: set up applicaton
        public static void SetUpGame(string gameId, string token, Action<string, bool> callback = null, bool seedStore = false)
        {
            Log("CloudLogin Setting Up Game: "+ gameId+": "+ token);
            Instance.SetUpGamePrivate(gameId, token, callback, seedStore);
        }

        private void SetUpGamePrivate(string gameId, string token, Action<string, bool> callback = null, bool seedStore = false)
        {
            StartCoroutine(SetUpGameRoutine(gameId, token, callback,seedStore));
        }

        private IEnumerator SetUpGameRoutine(string gameId, string token, Action<string, bool> callback = null, bool seedStore = false)
        {
            var body = "game_id=" + gameId + "&game_token=" + token;
            if (seedStore) body = body + "&seed_store=true";
            Log("CloudLogin Setting Up Game Sending Request: " + baseURL + "/games/verify?" + body);
            var request = UnityWebRequest.Get(baseURL + "/games/verify?" + body);
            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                Log("CloudLogin Setting Up Game Recieved Request Success: " + gameId + ": " + token);
                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                Instance.id = json.GetNumber("id").ToString();
                Instance.name = json.GetString("name");
                Instance.description = json.GetString("description");
                Instance.token = json.GetString("token");
                DownloadStoreItemsPrivate(callback);

            }
            else
            {
                Log("CloudLogin Setting Up Game Recieved Request Failure: " + gameId + ": " + token);
                CloudLoginUtilities.HandleCallback(request, "Game has failed to set up!", callback);
            }


        }

        private void DownloadStoreItemsPrivate(Action<string, bool> callback = null)
        {
            StartCoroutine(DownloadStoreItemsRoutine(callback));
        }

        private IEnumerator DownloadStoreItemsRoutine(Action<string, bool> callback = null)
        {
            Log("CloudLogin Downloading Store Items");
            var body = "game_id=" + id + "&game_token=" + token;
            var request = UnityWebRequest.Get(baseURL + "/games/store_items?" + body);
            if (CloudLoginUser.CurrentUser.GetAuthenticationToken() != null)
                request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                Log("CloudLogin Downloading Store Items Success");

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var storeItems = json.GetArray("store_items");
                store.Clear();
                foreach (var storeItem in storeItems)
                {
                    store.Add(new CloudLoginStoreItem(
                        storeItem.Obj.GetString("name"),
                        storeItem.Obj.GetString("category"),
                        storeItem.Obj.GetString("description"),
                        Convert.ToInt32(storeItem.Obj.GetNumber("cost")),
                        Convert.ToInt32(storeItem.Obj.GetNumber("id"))));
                }


            }
            else
            {
                CloudLoginUtilities.HandleCallback(request, "Game has failed to set up!", callback);
                Log("CloudLogin Downloading Store Items FAiled");

            }

            CloudLoginUtilities.HandleCallback(request, "Game has been set up!", callback);

        }

        public static List<CloudLoginStoreItem> GetStoreItems()
        {
            return Instance.store;
        }
        #endregion



        #region request: sign in
        public static void SignIn(string email, string password, Action<string, bool> callback = null)
        {
            Instance.SignInPrivate(email, password, callback);
        }

        private void SignInPrivate(string email, string password, Action<string, bool> callback = null)
        {
            StartCoroutine(SignInRoutine(email, password, callback));
        }

        private IEnumerator SignInRoutine(string email, string password, Action<string, bool> callback = null)
        {

            Log("CloudLogin Sign In: " + email );

            if (GetGameId() == null)
                throw new CloudLoginException("Please set up your game with PainLessAuth.SetUpGame before signing in users");

            WWWForm form = new WWWForm();
            form.AddField("email", email);
            form.AddField("password", password);
            form.AddField("game_id", GetGameId());

            var request = UnityWebRequest.Post(baseURL + "/sessions", form);

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                Log("CloudLogin Sign In Success: " + email);

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                CloudLoginUser.CurrentUser.SetSignedInInternal();
                CloudLoginUser.CurrentUser.SetScoreInternal(Convert.ToInt32(json.GetNumber("score")));
                CloudLoginUser.CurrentUser.SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
                CloudLoginUser.CurrentUser.SetUsernameInternal(json.GetString("username"));
                CloudLoginUser.CurrentUser.SetLastLoginInternal(DateTime.Parse(json.GetString("last_login")));
                CloudLoginUser.CurrentUser.SetNumberOfLoginsInternal(Convert.ToInt32(json.GetNumber("number_of_logins")));
                CloudLoginUser.CurrentUser.SetAuthenticationTokenInternal(json.GetString("authentication_token"));
                CloudLoginUser.CurrentUser.SetIDInternal(Convert.ToInt32(json.GetNumber("id")));
                CloudLoginUser.CurrentUser.DownloadAttributes(true, callback); // Chain next request - download users attributes

            }
            else
            {
                Log("CloudLogin Sign In Failure: " + email);

                CloudLoginUtilities.HandleCallback(request, "User has been signed in successfully", callback);
            }



        }



        #endregion

        #region request: sign up
        public static void SignUp(string email, string password, string password_confirmation, string username, Action<string, bool> callback = null)
        {
            Instance.SignUpPrivate(email, password, password_confirmation, username, callback);
        }

        private void SignUpPrivate(string email, string password, string password_confirmation, string username, Action<string, bool> callback = null)
        {
            StartCoroutine(SignUpRoutine(email, password, password_confirmation, username, callback));
        }

        private IEnumerator SignUpRoutine(string email, string password, string password_confirmation, string username, Action<string, bool> callback = null)
        {
            Log("CloudLogin Sign Up: " + email);

            if (GetGameId() == null)
                throw new CloudLoginException("Please set up your game with PainLessAuth.SetUpGame before signing up users");

            WWWForm form = new WWWForm();
            form.AddField("email", email);
            form.AddField("password", password);
            form.AddField("password_confirmation", password_confirmation);
            form.AddField("username", username);

            form.AddField("game_id", GetGameId());
            form.AddField("game_token", GetGameToken());

            var request = UnityWebRequest.Post(baseURL + "/users", form);

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {

                Log("CloudLogin Sign Up Success: " + email);
                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                CloudLoginUser.CurrentUser.SetSignedInInternal();
                CloudLoginUser.CurrentUser.SetScoreInternal(Convert.ToInt32(json.GetNumber("score")));
                CloudLoginUser.CurrentUser.SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
                CloudLoginUser.CurrentUser.SetUsernameInternal(json.GetString("username"));
                CloudLoginUser.CurrentUser.SetLastLoginInternal(DateTime.Parse(json.GetString("last_login")));
                CloudLoginUser.CurrentUser.SetNumberOfLoginsInternal(Convert.ToInt32(json.GetNumber("number_of_logins")));
                CloudLoginUser.CurrentUser.SetAuthenticationTokenInternal(json.GetString("authentication_token"));
                CloudLoginUser.CurrentUser.SetIDInternal(Convert.ToInt32(json.GetNumber("id")));
                CloudLoginUser.CurrentUser.DownloadAttributes(true, callback); // Chain next request - download users attributes

            }
            else
            {
                Log("CloudLogin Sign Up Failure: " + email);
                CloudLoginUtilities.HandleCallback(request, "User could not sign up: " + request.error, callback);
            }

        }



        #endregion

        #region Leaderboard

        public void GetLeaderboard(int limit, bool onePerUser, string LeaderboardName, Action<string, bool> callback = null)
        {
            StartCoroutine(GetLeaderboardRoutine(limit, onePerUser, LeaderboardName, callback));
        }

        private IEnumerator GetLeaderboardRoutine(int limit, bool onePerUser, string LeaderboardName, Action<string, bool> callback = null)
        {
            CloudLogin.Log("CloudLogin Get Leaderboard: " + limit.ToString());

            if (CloudLogin.GetGameId() == null)
                throw new CloudLoginException("Please set up your game with CloudLogin.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + CloudLoginUser.CurrentUser.GetAuthenticationToken() + "&limit=" + limit.ToString() + "&one_per_user=" + onePerUser.ToString()+ "&leaderboard_name="+ LeaderboardName.ToString();
            var request = UnityWebRequest.Get(CloudLogin.GetBaseURL() + "/games/" + CloudLogin.GetGameId() + "/leaderboard_entries" + parameters);
            request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                CloudLogin.Log("CloudLogin Get Leaderboard Success: : " + limit.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                Debug.Log("got " + json);
                var storeItems = json.GetArray("leaderboard_entries");
                CloudLogin.Instance.leaderboardEntries.Clear();
                foreach (var storeItem in storeItems)
                {
                    CloudLogin.Instance.leaderboardEntries.Add(new CloudLoginLeaderboardEntry(
                        storeItem.Obj.GetString("username"),
                        Convert.ToInt32(storeItem.Obj.GetNumber("score")),
                        storeItem.Obj.GetString("leaderboard_name"),
                        storeItem.Obj.GetString("extra_attributes"),
                        Convert.ToInt32(storeItem.Obj.GetNumber("game_user_id"))
                        )
                   );
                }

            }

            CloudLoginUtilities.HandleCallback(request, "Store Item has been removed", callback);
        }
        #endregion

    }

}



public class CloudLoginException : Exception
{
    public CloudLoginException()
    {
    }

    public CloudLoginException(string message)
        : base(message)
    {
    }

    public CloudLoginException(string message, Exception inner)
        : base(message, inner)
    {
    }
}