using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;

public class DiscordController : MonoBehaviour
{
    public Discord.Discord discord;
    public static DiscordController instance { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            instance = this; 
        } 
    }

    void Start()
    {
        discord = new Discord.Discord(1046654363021623377, (System.UInt64)Discord.CreateFlags.Default);
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            Details = Application.version,
            State = "Beginning an adventure",
            Timestamps = {
                Start = System.DateTimeOffset.Now.ToUnixTimeMilliseconds()
            },
            Assets = {
                LargeImage = "farlogo"
            }
        };
        activityManager.UpdateActivity(activity, (res) =>{
            if(res == Discord.Result.Ok)
            {
                //Debug.Log($"Estado de discord funcando");
            }
            else
            {
                Debug.LogError($"Estado de discord fallido");
            }
        });
    }

    void Update()
    {
        discord.RunCallbacks();
    }

    void OnApplicationQuit()
    {
        discord.Dispose();
    }
}
