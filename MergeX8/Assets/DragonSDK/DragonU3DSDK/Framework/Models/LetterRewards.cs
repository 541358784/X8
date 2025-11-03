using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LetterRewards
{
    [System.Serializable]
    public class RewardContent
    {
        public int energy;
        public int diamond;
        public int coin;
    };
    [System.Serializable]
    public class Info
    {
        public string title;
        public string message;
    };

    public string _id;
    public string updatedAt;
    public string createdAt;
    public int playerId;
    public RewardContent content;
    public Info info;
    public int type;
    public int status;
    public int __v;
}
