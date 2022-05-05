using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Boomlagoon.JSON;
using UnityEngine;



namespace CloudLoginUnity
{
    /// <summary>Class <c>CloudLoginStoreItem</c> models a store item added 
    /// through the CloudLogin web portal
    /// they can be retrieved and 'purchased' by CloudLoginUsers through
    /// the API SDK, but not created.</summary>
    ///
    public class CloudLoginLeaderboardEntry : MonoBehaviour
    {

        #region instance vars
        private string leaderboard_name;
        private int score;
        private string username;
        private int game_user_id;
        private string extra_attributes;
        #endregion

        #region instance getters
        public string GetUsername()
        {
            return username;
        }
        public int GetScore()
        {
            return score;
        }
        public string GetLeaderboardName()
        {
            return leaderboard_name;
        }
        public Dictionary<string,string> GetExtraAttributes()
        {
            var dictionary = extra_attributes.Replace("\\", "").Replace("{", "").Replace("}", "").Replace(", ", ",").Replace(": ", ":")
            .Split(',')
            .Select(part => part.Split(':'))
            .Where(part => part.Length == 2)
            .ToDictionary(sp => sp[0].Replace("\"", ""), sp => sp[1].Replace("\"", ""));

            return dictionary;

        }


        #endregion

        #region constructor

        public CloudLoginLeaderboardEntry(string username, int score, string leaderboard_name, string extra_attributes, int game_user_id)
        {
            this.username = username;
            this.score = score;
            this.leaderboard_name = leaderboard_name;
            this.extra_attributes = extra_attributes;
            this.game_user_id = game_user_id;

        }

        #endregion
    }



}

