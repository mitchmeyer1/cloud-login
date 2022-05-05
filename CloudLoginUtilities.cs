using System;
using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;
using UnityEngine;
using UnityEngine.Networking;


namespace CloudLoginUnity
{
    /// <summary>Class <c>CloudLoginUtilities</c> is your connection with the CloudLogin
    /// API.  Thorugh this class you can connect to your apps, login users,
    /// create users.  When a user is signed in you can use CloudLoginUser to 
    /// access your account, attributes and purchased store items.
    /// </summary>
    public class CloudLoginUtilities : MonoBehaviour
    {

        internal static void HandleCallback(UnityWebRequest request, string successString, Action<string, bool> callback = null)
        {
            if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("Request had error: " + request.error);
                if (callback != null)
                    callback("An unknown error occurred: " + request.error, true);
            }
            else if (request.responseCode == 299)
            {
                var data = request.downloadHandler.text;
                JSONObject json = JSONObject.Parse(data);
                var errorString = json.GetString("error");

                if (errorString.Contains("has already been taken"))
                    errorString = "Username or email already taken";

                if (callback != null)
                    callback(errorString, true);
               
             }

            else
            {
                if (callback != null)
                    callback(successString, false);
            }
        }


        internal static bool RequestIsSuccessful(UnityWebRequest request)
        {
            return request.result != UnityWebRequest.Result.ProtocolError && request.result != UnityWebRequest.Result.ConnectionError && request.responseCode != 299;
        }
    }

}


