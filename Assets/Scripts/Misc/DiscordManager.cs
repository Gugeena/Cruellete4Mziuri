using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DiscordManager : MonoBehaviour
{
    Discord.Discord discord;
    public String bossName;
    public String bossAssetKey;
    public String bossDescription;
    public bool inMainMenu = false;
    String rpString;
    // Start is called before the first frame update
    void Start()
    {
        discord = new Discord.Discord(1350580456772599879, (ulong)Discord.CreateFlags.NoRequireDiscord);
        if (!inMainMenu) rpString = "Current Boss - ";
        else rpString = "In the menu";

        changeActivity();
    }

    private void OnDisable()
    {
        discord.Dispose();
    }

    public void changeActivity()
    {
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            State = "Fighting",
            Details = rpString + bossName + ", Deaths : " + MovementScript.DeathCount
        };
        var activityAssets = new Discord.ActivityAssets
        {
            LargeImage = "bigicon",
            LargeText = "You should play this",
            SmallImage = bossAssetKey,
            SmallText = bossDescription
        };
        activity.Assets = activityAssets;

        activityManager.UpdateActivity(activity, (res) =>
        {
            if (res == Discord.Result.Ok)
            {
                Debug.Log("allgood");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        discord.RunCallbacks();
    }
}