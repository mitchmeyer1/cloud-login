﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudLoginUnity;
using System.Linq;

public class CloudLoginExample : MonoBehaviour {

    public string gameToken = "cea75d4c-2148-4421-8227-cab9f7825ead";
    public string gameID = "1";

    string userEmail = "tom@mundo.com";
    string username = "tommundo";

    // Use this for initialization
    void Start () {
        var random = ((int)(Random.value * 10000)).ToString();
        userEmail = "tom"+ random + "@mundo.com";
        username = "tommundo" + random;

        Debug.Log("CloudLogin start");
        CloudLogin.SetVerboseLogging(false);
        CloudLogin.SetUpGame(gameID, gameToken, ApplicationSetUp);
    }

    void ApplicationSetUp(string message, bool hasError)
    {



        if (hasError)
        {
            print("error setting aplication");
            print(message);
        }
        else
        {
        
            print("<color=yellow>GAME CONNECTED!!" + CloudLogin.GetGameId() + "</color>");
            print("Store Items:");
            foreach (CloudLoginStoreItem storeItem in CloudLogin.GetStoreItems())
            {
                print("      " + storeItem.GetName() + ": " + storeItem.GetCost());
            }
            print("*****************************************");
            print("*****************************************");

            print("Signing Up");
            CloudLogin.SignUp(userEmail, "password", "password", username, SignedUp);
        }


    }

    void SignedUp(string message, bool hasError)
    {

        if (hasError)
        {
            print("Error signign up: "+message);
        }
        else
        {
            print("signed up!");
            print("<color=yellow>Adding credits!</color>");

            print("Before Credits: " + CloudLoginUser.CurrentUser.GetCredits());

            CloudLoginUser.CurrentUser.AddCredits(50, AddCreditsCallback);

        }

    }

    void AddCreditsCallback(string message, bool hasError)
    {
        if (hasError)
        {
            print("Error adding credits: " + message);
        }
        else
        {
            print("After Credits: " + CloudLoginUser.CurrentUser.GetCredits());
            print("currently attribute color is null?"+ (CloudLoginUser.CurrentUser.GetAttributeValue("Color") == null).ToString());
            print("<color=yellow>Setting attribute color = blue</color>");

            CloudLoginUser.CurrentUser.SetAttribute("Color","Blue", SetAttributeCallback);

        }
       

    }

    void SetAttributeCallback(string message, bool hasError)
    {
        if (hasError)
        {
            print("Error adding attribute: " + message);
        }
        else
        {
            print("currently attribute color is null?" + (CloudLoginUser.CurrentUser.GetAttributeValue("Color") == null).ToString());
            print("currently attribute color " + CloudLoginUser.CurrentUser.GetAttributeValue("Color"));

            var item = CloudLogin.GetStoreItems().First();
            print("Purchase Store Item: " + item.GetName() + ": " + item.GetCost());

            CloudLoginUser.CurrentUser.PurchaseStoreItem(CloudLogin.GetStoreItems().First(), PurchasedItem);

        }
    }

    void PurchasedItem(string message, bool hasError)
    {
        if (hasError)
        {
            Debug.Log("Error purchasing item: " + message);
        }
        else
        {
            print("Purchased Item");
            print("Current Credits: " + CloudLoginUser.CurrentUser.GetCredits());
        }
    }




}