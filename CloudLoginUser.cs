using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
using UnityEngine.Networking;

namespace CloudLoginUnity
{
    public class CloudLoginUser : MonoBehaviour
    {

        #region instance vars
        private bool signedIn = false;
        private int numberOfLogins;
        private DateTime lastLogin;
        private string authenticationToken;
        private string username;
        private int score;
        private int credits;
        private int id;
        private Dictionary<string, string> attributes = new Dictionary<string, string>();
        private List<CloudLoginStoreItem> purchasedStoreItems = new List<CloudLoginStoreItem>();

        #endregion


        #region instance setters
        internal void SetSignedInInternal()
        {
            this.signedIn = true;

        }
        internal void SetNumberOfLoginsInternal(int numberOfLogins)
        {
            this.numberOfLogins = numberOfLogins;
        }
        internal void SetLastLoginInternal(DateTime lastLogin)
        {
            this.lastLogin = lastLogin;
        }
        internal void SetAuthenticationTokenInternal(string authenticationToken)
        {
            this.authenticationToken = authenticationToken;
        }
        internal void SetUsernameInternal(string username)
        {
            this.username = username;
        }
        internal void SetScoreInternal(int score)
        {
            this.score = score;
        }
        internal void SetCreditsInternal(int credits)
        {
            this.credits = credits;
        }
        internal void SetIDInternal(int id)
        {
            this.id = id;
        }

        #endregion


        #region instance getters
        public bool IsSignedIn()
        {
            return signedIn;
        }
        public int GetNumberOfLogins()
        {
            return numberOfLogins;
        }
        public DateTime GetLastLogin()
        {
            return lastLogin;
        }
        internal string GetAuthenticationToken()
        {
            return authenticationToken;
        }
        public string GetUsername()
        {
            return username;
        }
        public int GetScore()
        {
            return score;
        }
        public int GetCredits()
        {
            return credits;
        }
        internal int GetID()
        {
            return id;
        }
        #endregion


        #region singleton management
        private static CloudLoginUser _instance;
        public static CloudLoginUser CurrentUser { get { return _instance; } }
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



        #region request: add credits
        public void AddCredits(int credits, Action<string, bool> callback = null)
        {
            StartCoroutine(AddCreditsRoutine(credits, callback));
        }

        private IEnumerator AddCreditsRoutine(int credits, Action<string, bool> callback = null)
        {

            CloudLogin.Log("CloudLoginUser Add Credits: " + credits.ToString());
            if (CloudLogin.GetGameId() == null)
                throw new CloudLoginException("Please set up your game with CloudLogin.SetUpGame before modifying users");


            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("credits", credits);
            var request = UnityWebRequest.Post(CloudLogin.GetBaseURL() + "/users/" + CurrentUser.id + "/add_credits", form);
            request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                CloudLogin.Log("CloudLoginUser Add Credits Success: " + credits.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));

            }
            else
            {
                CloudLogin.Log("CloudLoginUser Add Credits Failure: " + credits.ToString());

            }
            CloudLoginUtilities.HandleCallback(request, "Credits have been added to user", callback);


        }
        #endregion

        #region request: set credits
        public void SetCredits(int credits, Action<string, bool> callback = null)
        {
            StartCoroutine(SetCreditsRoutine(credits, callback));
        }

        private IEnumerator SetCreditsRoutine(int credits, Action<string, bool> callback = null)
        {

            CloudLogin.Log("CloudLoginUser Set Credits: " + credits.ToString());

            if (CloudLogin.GetGameId() == null)
                throw new CloudLoginException("Please set up your game with CloudLogin.SetUpGame before modifying users");


            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("credits", credits);
            var request = UnityWebRequest.Post(CloudLogin.GetBaseURL() + "/users/" + CurrentUser.id + "/set_credits", form);
            request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                CloudLogin.Log("CloudLoginUser Set Credits Success: " + credits.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
            }
            else
            {
                CloudLogin.Log("CloudLoginUser Set Credits Failure: " + credits.ToString());

            }

            CloudLoginUtilities.HandleCallback(request, "Credits have been added to user", callback);
        }
        #endregion

        #region request: add score
        public void AddScore(int credits, Action<string, bool> callback = null)
        {
            StartCoroutine(AddScoreRoutine(credits, callback));
        }

        private IEnumerator AddScoreRoutine(int score, Action<string, bool> callback = null)
        {

            CloudLogin.Log("CloudLoginUser Add Score: " + score.ToString());

            if (CloudLogin.GetGameId() == null)
                throw new CloudLoginException("Please set up your game with CloudLogin.SetUpGame before modifying users");


            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("score", score);

            var request = UnityWebRequest.Post(CloudLogin.GetBaseURL() + "/users/" + CurrentUser.id + "/add_score", form);
            request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                CloudLogin.Log("CloudLoginUser Add Score Succcess: " + score.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                SetScoreInternal(Convert.ToInt32(json.GetNumber("score")));
            }

            CloudLoginUtilities.HandleCallback(request, "Score have been added to user", callback);
        }
        #endregion

        #region request: set score
        public void SetScore(int score, Action<string, bool> callback = null)
        {
            StartCoroutine(SetScoreRoutine(score, callback));
        }

        private IEnumerator SetScoreRoutine(int score, Action<string, bool> callback = null)
        {
            CloudLogin.Log("CloudLoginUser Set Score: " + score.ToString());

            if (CloudLogin.GetGameId() == null)
                throw new CloudLoginException("Please set up your game with CloudLogin.SetUpGame before modifying users");

            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("score", score);
            var request = UnityWebRequest.Post(CloudLogin.GetBaseURL() + "/users/" + CurrentUser.id + "/set_score", form);
            request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                CloudLogin.Log("CloudLoginUser Set Score Success: " + score.ToString());

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                SetScoreInternal(Convert.ToInt32(json.GetNumber("score")));
            }

            CloudLoginUtilities.HandleCallback(request, "Score have been added to user", callback);
        }
        #endregion


        #region attributes


        internal void DownloadAttributes(bool chainedFromLogin, Action<string, bool> callback = null)
        {
            StartCoroutine(DownloadAttributesRoutine(chainedFromLogin, callback));

        }

        private IEnumerator DownloadAttributesRoutine(bool chainedFromLogin, Action<string, bool> callback = null)
        {
            CloudLogin.Log("CloudLoginUser Get Attributes");

            if (CloudLogin.GetGameId() == null)
                throw new CloudLoginException("Please set up your game with CloudLogin.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken();


            var request = UnityWebRequest.Get(CloudLogin.GetBaseURL() + "/users/" + this.id + "/game_user_attributes"+ parameters);
            request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                CloudLogin.Log("CloudLoginUser Get Attributes Success");

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var game_user_attributes = json.GetArray("game_user_attributes");

                attributes.Clear();
                foreach (var attribute in game_user_attributes)
                {
                    attributes.Add(attribute.Obj.GetString("key"), attribute.Obj.GetString("value"));
                }
                DownloadStoreItems(chainedFromLogin, callback);
            } else 
                CloudLoginUtilities.HandleCallback(request, chainedFromLogin ? "Users has been signed in successfully" : "Users attributes have been downloaded", callback);

        }

        public Dictionary<string,string> GetAttributes()
        {
            return attributes;
        }

        public Dictionary<string, string>.KeyCollection GetAttributesKeys()
        {
            return attributes.Keys;
        }

        public string GetAttributeValue(string key)
        {
            if (attributes.ContainsKey(key)){
                return attributes[key];
            }
            else
                return "";
        }

        public void SetAttribute(string key, string value, Action<string, bool> callback = null)
        {
            StartCoroutine(SetAttributeRoutine(key,value, callback));
        }

        private IEnumerator SetAttributeRoutine(string key, string value, Action<string, bool> callback = null)
        {
            CloudLogin.Log("CloudLoginUser Set Attributes: "+key);

            if (CloudLogin.GetGameId() == null)
                throw new CloudLoginException("Please set up your game with CloudLogin.SetUpGame before modifying users");

            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("key", key);
            form.AddField("value", value);

            var request = UnityWebRequest.Post(CloudLogin.GetBaseURL() + "/users/" + CurrentUser.id + "/add_game_user_attribute", form);
            request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                CloudLogin.Log("CloudLoginUser Set Attributes Success: " + key);

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                attributes.Add(key, value);
                foreach (var attribute in attributes)
                {
                    print(attribute.Key + "," + attribute.Value);
                }
            }

            CloudLoginUtilities.HandleCallback(request, "Attribute has been added to user", callback);
        }

        public void RemoveAttribute(string key, Action<string, bool> callback = null)
        {
            StartCoroutine(RemoveAttributeRoutine(key,callback));
        }

        private IEnumerator RemoveAttributeRoutine(string key, Action<string, bool> callback = null)
        {
            CloudLogin.Log("CloudLoginUser Remove Attributes: " + key);

            if (CloudLogin.GetGameId() == null)
                throw new CloudLoginException("Please set up your game with CloudLogin.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken() + "&game_user_attribute_key=" + key;
            var request = UnityWebRequest.Get(CloudLogin.GetBaseURL() + "/users/" + CurrentUser.id + "/remove_game_user_attributes" + parameters);
            request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                CloudLogin.Log("CloudLoginUser Remove Attributes Success: " + key);

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var game_user_attributes = json.GetArray("game_user_attributes");

                attributes.Clear();
                foreach (var attribute in game_user_attributes)
                {
                    attributes.Add(attribute.Obj.GetString("key"), attribute.Obj.GetString("value"));
                }
            }

            CloudLoginUtilities.HandleCallback(request, "Attribute has been removed", callback);
        }


        #endregion

        #region store items
        internal void DownloadStoreItems(bool chainedFromLogin, Action<string, bool> callback = null)
        {
            StartCoroutine(DownloadStoreItemsRoutine(chainedFromLogin, callback));

        }

        private IEnumerator DownloadStoreItemsRoutine(bool chainedFromLogin, Action<string, bool> callback = null)
        {
            CloudLogin.Log("CloudLoginUser Download Store Items: ");

            if (CloudLogin.GetGameId() == null)
                throw new CloudLoginException("Please set up your game with CloudLogin.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken();


            var request = UnityWebRequest.Get(CloudLogin.GetBaseURL() + "/users/" + CurrentUser.id + "/game_user_store_items" + parameters);
            request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                CloudLogin.Log("CloudLoginUser Download Store Items Success: ");

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var game_user_store_items = json.GetArray("game_user_store_items");
                purchasedStoreItems.Clear();

                foreach (var item in game_user_store_items)
                {
                    purchasedStoreItems.Add(new CloudLoginStoreItem(
                        item.Obj.GetString("name"),
                        item.Obj.GetString("category"),
                        item.Obj.GetString("description"),
                        Convert.ToInt32(item.Obj.GetNumber("cost")),
                        Convert.ToInt32(item.Obj.GetNumber("id"))));
                }


            }

            CloudLoginUtilities.HandleCallback(request, chainedFromLogin ? "Users has been signed in successfully" : "Users store items have been downloaded", callback);

        }

        public List<CloudLoginStoreItem> GetPurchasedStoreItems()
        {
            return purchasedStoreItems;
        }

        public void PurchaseStoreItem(CloudLoginStoreItem storeItem, Action<string, bool> callback = null)
        {
            StartCoroutine(PurchaseStoreItemRoutine(storeItem.GetId(), callback));
        }

        public void PurchaseStoreItem(int storeItemId, Action<string, bool> callback = null)
        {
            StartCoroutine(PurchaseStoreItemRoutine(storeItemId, callback));
        }

        private IEnumerator PurchaseStoreItemRoutine(int storeItemId, Action<string, bool> callback = null)
        {

            CloudLogin.Log("CloudLoginUser Purchase Store Items: ");

            if (CloudLogin.GetGameId() == null)
                throw new CloudLoginException("Please set up your game with CloudLogin.SetUpGame before modifying users");

            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("store_item_id", storeItemId.ToString());

            var request = UnityWebRequest.Post(CloudLogin.GetBaseURL() + "/users/" + CurrentUser.id + "/purchase_game_user_store_item", form);
            request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                CloudLogin.Log("CloudLoginUser Purchase Store Items Success: ");

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var game_user_store_items = json.GetArray("game_user_store_items");
                CurrentUser.SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
                purchasedStoreItems.Clear();

                foreach (var item in game_user_store_items)
                {
                    purchasedStoreItems.Add(new CloudLoginStoreItem(
                        item.Obj.GetString("name"),
                        item.Obj.GetString("category"),
                        item.Obj.GetString("description"),
                        Convert.ToInt32(item.Obj.GetNumber("cost")),
                        Convert.ToInt32(item.Obj.GetNumber("id"))));
                }
            }

            CloudLoginUtilities.HandleCallback(request, "Store Item has been purchased by user", callback);
        }

        public void RemoveStoreItem(int storeItemID, bool reimburseUser, Action<string, bool> callback = null)
        {
            StartCoroutine(RemoveStoreItemRoutine(storeItemID, reimburseUser, callback));
        }
        public void RemoveStoreItem(CloudLoginStoreItem storeItem, bool reimburseUser, Action<string, bool> callback = null)
        {
            StartCoroutine(RemoveStoreItemRoutine(storeItem.GetId(), reimburseUser, callback));
        }

        private IEnumerator RemoveStoreItemRoutine(int storeItemID, bool reimburseUser, Action<string, bool> callback = null)
        {
            CloudLogin.Log("CloudLoginUser Remove Store Item: "+ storeItemID);

            if (CloudLogin.GetGameId() == null)
                throw new CloudLoginException("Please set up your game with CloudLogin.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken() + "&store_item_id=" + storeItemID+ "&reimburse=" + reimburseUser.ToString();
            var request = UnityWebRequest.Get(CloudLogin.GetBaseURL() + "/users/" + CurrentUser.id + "/remove_game_user_store_item" + parameters);
            request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                CloudLogin.Log("CloudLoginUser Remove Store Item Success: " + storeItemID);

                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                CurrentUser.SetCreditsInternal(Convert.ToInt32(json.GetNumber("credits")));
                var game_user_store_items = json.GetArray("game_user_store_items");
                purchasedStoreItems.Clear();

                foreach (var item in game_user_store_items)
                {
                    purchasedStoreItems.Add(new CloudLoginStoreItem(
                        item.Obj.GetString("name"),
                        item.Obj.GetString("category"),
                        item.Obj.GetString("description"),
                        Convert.ToInt32(item.Obj.GetNumber("cost")),
                        Convert.ToInt32(item.Obj.GetNumber("id"))));
                }
            }

            CloudLoginUtilities.HandleCallback(request, "Store Item has been removed", callback);
        }

        #endregion

        #region Leaderboard

        public void AddLeaderboardEntry(string leaderboardName, int score, Dictionary<string, string> extraAttributes = null, Action<string, bool> callback = null)
        {
            StartCoroutine(AddLeaderboardEntryRoutine(leaderboardName, score, extraAttributes, callback));
        }

        public void AddLeaderboardEntry(string leaderboardName, int score, Action<string, bool> callback = null)
        {
            StartCoroutine(AddLeaderboardEntryRoutine(leaderboardName, score, new Dictionary<string, string>(), callback));
        }


        private IEnumerator AddLeaderboardEntryRoutine(string leaderboardName, int score, Dictionary<string, string> extraAttributes, Action<string, bool> callback = null)
        {
            CloudLogin.Log("CloudLoginUser Adding Leaderboard Entry: " + leaderboardName + ": "+score.ToString());

            if (CloudLogin.GetGameId() == null)
                throw new CloudLoginException("Please set up your game with CloudLogin.SetUpGame before modifying users");

            List<string> extraAttributesList = new List<string>();
            foreach (KeyValuePair<string, string> entry in extraAttributes)
            {
                extraAttributesList.Add("\""+entry.Key.ToString() + "\": " + entry.Value.ToString());
            }
            
            string extraAttributesJson = "{" + String.Join(", ", extraAttributesList.ToArray())+ "}";
            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("leaderboard_name", leaderboardName);
            form.AddField("extra_attributes", extraAttributesJson);
            form.AddField("score", score);

            var request = UnityWebRequest.Post(CloudLogin.GetBaseURL() + "/users/" + CurrentUser.id + "/add_leaderboard_entry", form);
            request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                CloudLogin.Log("CloudLoginUser Add Leaderboard Entry: " + leaderboardName+ ": "+score);

                var data = request.downloadHandler.text;
            }

            CloudLoginUtilities.HandleCallback(request, "Leaderboard Entry Has Been Added", callback);
        }

        public void ClearLeaderboardEntries(string leaderboardName, Action<string, bool> callback = null)
        {
            StartCoroutine(ClearLeaderboardEntriesRoutine(leaderboardName, callback));
        }


        private IEnumerator ClearLeaderboardEntriesRoutine(string leaderboardName, Action<string, bool> callback = null)
        {
            CloudLogin.Log("CloudLoginUser Clearing Leaderboard Entry: " + leaderboardName);

            if (CloudLogin.GetGameId() == null)
                throw new CloudLoginException("Please set up your game with CloudLogin.SetUpGame before modifying users");

            WWWForm form = new WWWForm();
            form.AddField("authentication_token", GetAuthenticationToken());
            form.AddField("leaderboard_name", leaderboardName);

            var request = UnityWebRequest.Post(CloudLogin.GetBaseURL() + "/users/" + CurrentUser.id + "/clear_my_leaderboard_entries", form);
            request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                CloudLogin.Log("CloudLoginUser Clear Leaderboard Entry: " + leaderboardName);

                var data = request.downloadHandler.text;
            }

            CloudLoginUtilities.HandleCallback(request, "Leaderboard Entries Have Been Cleared", callback);
        }

        public void GetLeaderboard(int limit, bool onePerUser, Action<string, bool> callback = null)
        {
            StartCoroutine(GetLeaderboardRoutine(limit, onePerUser, callback));
        }

        private IEnumerator GetLeaderboardRoutine(int limit, bool onePerUser, Action<string, bool> callback = null)
        {
            CloudLogin.Log("CloudLoginUser Get Leaderboard: " +limit.ToString());

            if (CloudLogin.GetGameId() == null)
                throw new CloudLoginException("Please set up your game with CloudLogin.SetUpGame before modifying users");

            var parameters = "?authentication_token=" + GetAuthenticationToken() + "&limit=" + limit.ToString()+ "&one_per_user="+ onePerUser.ToString();
            var request = UnityWebRequest.Get(CloudLogin.GetBaseURL() + "/users/" + CurrentUser.id + "/leaderboard_entries" + parameters);
            request.SetRequestHeader("authentication_token", CloudLoginUser.CurrentUser.GetAuthenticationToken());

            yield return request.SendWebRequest();

            if (CloudLoginUtilities.RequestIsSuccessful(request))
            {
                CloudLogin.Log("CloudLoginUser Get Leaderboard Success: : " + limit.ToString());

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

        #endregion Leaderboard 



    }



}
