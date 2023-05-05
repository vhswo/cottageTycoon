using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using UnityEngine;
using System;

public class PlayerStatus
{
    public PlayerStatus() { }
    public PlayerStatus(string id) { this.id = id; name = "AAA"; level = 1; gold = 0; }
    public PlayerStatus(string id,string name, int level, int gold) { this.id = id; this.name = name; this.level = level; this.gold = gold; }
    

    public string id;
    public string name;
    public int level;
    public int gold;

    public void SetGold(int gold) => this.gold += gold;
    public void LevelUp() => level += 1;
    public void ChangeName(string name) => this.name = name;
}


public class Player
{
    public PlayerStatus status { get; }

    DatabaseReference dataRef;

    public Player() { }
    public Player(PlayerStatus status) { this.status = status; }

    public Action<PlayerStatus> SubScript;

    public void SetGold(int gold)
    {
        status.SetGold(gold);
        SubScript?.Invoke(status);
        SaveData(status);
    }

    public void SaveData(PlayerStatus player)
    {
        dataRef = FirebaseDatabase.DefaultInstance.RootReference;
        string saveJson = JsonUtility.ToJson(player);
        dataRef.Child("users").Child(player.id).Child("status").SetRawJsonValueAsync(saveJson).ContinueWith(task =>
        {
            if(task.IsCanceled)
            {
                gameManger.Instance.testText.text = "SaveCancle";
                return;
            }

            if(task.IsFaulted)
            {
                gameManger.Instance.testText.text = "SaveFaild";
                return;
            }

            gameManger.Instance.testText.text = "succesave" + saveJson;

        });
    }

   //public void LoadData(string userId)
   //{
   //    gameManger.Instance.testText.text = "InLoadFuncSuceece but Cancel";
   //    dataRef.Child("users").Child(userId).Child("status").GetValueAsync().ContinueWith(task =>
   //    {
   //        gameManger.Instance.testText.text = "InLoadFuncSuceece but Failed";
   //        if (task.Exception != null)
   //            {
   //                gameManger.Instance.testText.text = "LoadException";
   //                return;
   //            }
   //
   //            if (!task.IsCompleted)
   //            {
   //                gameManger.Instance.testText.text = "Load not Completed";
   //                return;
   //            }
   //
   //            DataSnapshot snapshot = task.Result;
   //
   //        //로드완료
   //        string status = snapshot.GetRawJsonValue();
   //        this.status = JsonUtility.FromJson<PlayerStatus>(status);
   //
   //        SubScript?.Invoke(this.status);
   //
   //        gameManger.Instance.testText.text = "Load 성공 : " + JsonUtility.ToJson(this.status);
   //     });
   //
   //}
}
