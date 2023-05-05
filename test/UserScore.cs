using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Database;

//Json을 하기 위해 serialize
[Serializable]
public class UserScore
{
    public static string userIdPath = "user_id";
    public static string userNamePath = "username";
    public static string scorePath = "score";
    public static string timestampPath = "timestamp";
    public static string otherDataPath = "data";

    public string userId;
    public string userName;
    public long score;
    public long timestamp;
    public Dictionary<string, object> otherData;

    public UserScore(string userId,string userName,long score,long timestamp, Dictionary<string,object> otherData = null)
    {
        this.userId = userId;
        this.userName = userName;
        this.score = score;
        this.timestamp = timestamp;
        this.otherData = otherData;
    }

    public UserScore(DataSnapshot record)
    {
        userId = record.Child(userIdPath).Value.ToString();
        if(record.Child(userNamePath).Exists)
        {
            userName = record.Child(userNamePath).Value.ToString();
        }

        long score;
        if(Int64.TryParse(record.Child(scorePath).Value.ToString(),out score))
        {
            this.score = score;
        }
        else
        {
            this.score = Int64.MinValue;
        }

        long timestamp;
        if(Int64.TryParse(record.Child(timestampPath).Value.ToString(),out timestamp))
        {
            this.timestamp = timestamp;
        }

        if(record.Child(otherDataPath).Exists && record.Child(otherDataPath).HasChildren)
        {
            this.otherData = new Dictionary<string, object>();
            foreach(var keyValue in record.Child(otherDataPath).Children)
            {
                otherData[keyValue.Key] = keyValue.Value;
            }
        }
    }

    public static UserScore CreateScroeFromRecored(DataSnapshot record)
    {
        if(record == null)
        {
            Debug.LogWarning("Null DataSnapshot record in UserScore.CreateScoreFromRecored");
            return null;
        }

        if(record.Child(userIdPath).Exists && record.Child(scorePath).Exists && record.Child(timestampPath).Exists)
        {
            return new UserScore(record);
        }

        Debug.LogWarning("Invalid record format in UserScore.CreateScreFromRecored");
        return null;
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>()
        {
            {userIdPath, userId },
            {userNamePath, userName },
            {scorePath, score },
            {timestampPath, timestamp },
            {otherDataPath,otherData }
        };
    }
}
