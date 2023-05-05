using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Database;
using Firebase;
using System.Threading.Tasks;
using System.Linq;


//유저 한명에 대한 파라미터
public class UserScoreArgs : EventArgs
{
    public UserScore score;
    public string message;

    public UserScoreArgs(UserScore score, string message)
    {
        this.score = score;
        this.message = message;
    }
}


public class LeaderboardArgs : EventArgs
{
    //순위, 날짜 라던가의 가져올때
    public DateTime startData;
    public DateTime endData;

    public List<UserScore> scores;
}

public class LeaderBoard : MonoBehaviour
{
    bool initialized = false;
    bool readyToInitialize = false;

    DatabaseReference databaseRef;
    public string AllScoreDataPath => "all_scores";

    public event EventHandler Oninitialized;

    bool addingUserScore = false;
    bool sendAddedScoreEvent = false;
    UserScoreArgs addedScoreArgs;
    event EventHandler<UserScoreArgs> OnAddedScore;

    bool gettingUserScore = false;
    bool sendRetrievedScoreEvent = false;
    UserScoreArgs retrievedScoreArgs;
    event EventHandler<UserScoreArgs> OnRetrivedScore;

    bool sendUpdatedLeaderboardEvent = false;
    event EventHandler<LeaderboardArgs> OnUpdatedLeaderBoard;

    private void Start()
    {
        FirebaseInitialize.Initialize(dependencyStatus =>
        {
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                readyToInitialize = true;
                InitializeDatabase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencise : " + dependencyStatus);
            }
        });
    }

    private void InitializeDatabase()
    {
        if(initialized)
        {
            return;
        }

        FirebaseApp app = FirebaseApp.DefaultInstance;
        if(app.Options.DatabaseUrl != null)
        {
            Debug.LogWarning("LeaderBoard app.DatabaseUrl error");
        }

        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        initialized = true;
        readyToInitialize = false;
        Oninitialized(this, null);
    }

    void Update()
    {
        if(sendAddedScoreEvent)
        {
            sendAddedScoreEvent = false;
            OnAddedScore(this, addedScoreArgs);
        }

        if(sendRetrievedScoreEvent)
        {
            sendRetrievedScoreEvent = false;
            OnRetrivedScore(this, retrievedScoreArgs);
        }

        if(sendUpdatedLeaderboardEvent)
        {
            sendUpdatedLeaderboardEvent = false;
            OnUpdatedLeaderBoard(this, new LeaderboardArgs
            {
                scores = topScores,
                startData = DateTime.MinValue,
                endData = DateTime.MinValue
            });
        }
    }

    public Task AddScore(string userId, string userName,int score, long timestemp = 1L, Dictionary<string,object> otherData = null)
    {
        if(timestemp <=0)
        {
            timestemp = DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond;
        }

        var userScore = new UserScore(userId, userName, score, timestemp, otherData);
        return AddScore(userScore);
    }

    public Task<UserScore> AddScore(UserScore userScore)
    {
        if(addingUserScore)
        {
            Debug.LogError("Running add user score task!");
            return null;
        }

        var scoreDictionary = userScore.ToDictionary();
        addingUserScore = true;

        return Task.Run(() =>
        {
            var newEntry = databaseRef.Child(AllScoreDataPath).Push();

            return newEntry.SetValueAsync(scoreDictionary).ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    Debug.LogWarning("Exception adding score : " + task.Exception);
                    return null;
                }

                if (!task.IsCompleted)
                {
                    return null;
                }

                addingUserScore = false;

                addedScoreArgs = new UserScoreArgs(userScore, userScore.userId + " Added!");
                sendAddedScoreEvent = true;

                return userScore;

            }).Result;
        });
    }

    public void GetUserScore(string userId)
    {
        gettingUserScore = true;

        databaseRef.Child(AllScoreDataPath)
            .OrderByChild(UserScore.userIdPath)
            .StartAt(userId)
            .EndAt(userId)
            .GetValueAsync().ContinueWith(task => {
            if(task.Exception != null)
            {
                throw task.Exception;
            }

            if(!task.IsCompleted)
            {
                return;
            }

            if(task.Result.ChildrenCount == 0)
            {
                retrievedScoreArgs = new UserScoreArgs(null, string.Format("No Score for  User {0}", userId));
            }
            else
            {
                var scores = ParseVaildUserScoreRecords(task.Result, -1,-1).ToList();

                if(scores.Count == 0)
                {
                    retrievedScoreArgs = new UserScoreArgs(null, string.Format("No Score for  User {0} within time range ({1} - {2})", userId,-1,-1));

                }
                else
                {
                    var orderedScored = scores.OrderBy(scores => scores.score);
                    var userScore = orderedScored.Last();

                    retrievedScoreArgs = new UserScoreArgs(userScore, userScore.userId + "Retrieved");
                }
            }

            gettingUserScore = false;
            sendAddedScoreEvent = true;

        });
    }

    List<UserScore> ParseVaildUserScoreRecords(DataSnapshot snapshot,long startTicks,long endTicks)
    {
        return snapshot.Children
            .Select(scoreRecord => UserScore.CreateScroeFromRecored(scoreRecord))
            .Where(score => score != null && score.timestamp > startTicks && score.timestamp <= endTicks)
            .Reverse()
            .ToList();
    }

    bool gettingTopScores = false;

    List<UserScore> topScores = new();
    public List<UserScore> TopScores => topScores;

    Dictionary<string, UserScore> userScores = new();

    void GetInitialTopScores(long batchEnd)
    {
        gettingUserScore = true;

        var query = databaseRef.Child(AllScoreDataPath).OrderByChild("score");
        query = query.LimitToLast(20);

        query.GetValueAsync().ContinueWith(task =>
        {
            if (task.Exception != null)
            {
                //error
                return;
            }

            if (!task.IsCompleted || !task.Result.HasChildren)
            {
                //error
                return;
            }

            var scores = ParseVaildUserScoreRecords(task.Result, -1, -1);
            foreach(var userScore in scores)
            {
                if(!userScores.ContainsKey(userScore.userId))
                {
                    userScores[userScore.userId] = userScore;
                }
                else
                {
                    if(userScores[userScore.userId].score < userScore.score)
                    {
                        userScores[userScore.userId] = userScore;
                    }
                }
            }

            SetTopScores();
        });
    }

    void SetTopScores()
    {
        topScores.Clear();

        topScores.AddRange(userScores.Values.OrderByDescending(score => score.score));

        sendUpdatedLeaderboardEvent = true;
        gettingTopScores = false;
    }
}
