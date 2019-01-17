using UnityEngine;
using System.Collections;
using SimpleJson;
using System.Collections.Generic;
using System;

public class MissionConfig : GameConfig
{
    public MissionConfig()
    {
        this.ConfigName = "Mission";
        foreach(JsonObject data in (JsonArray)ConfigJsonData)
        {
            MissionData m = new MissionData(data);
            Configs.Add(m);
        }
    }

    public MissionData GetMissionDataById(string Id)
    {
        foreach(MissionData m in Configs)
        {
            if(m.MissionId == Id)
            {
                return m;
            }
        }
        return null;
    }
}

public class MissionData : ConfigData
{
    public string MissionId;

    public string MissionName;

    public int Goal;

    public string Description;

    public int Turn;

    public HardWareData.HardWareType Weapon;

    public string RewardId;

    public int RewardRate;

    public MissionData(JsonObject data)
    {
        try
        {
            MissionId = data["MissionId"].ToString();
            MissionName = data["MissionName"].ToString();
            Goal = int.Parse(data["Goal"].ToString());
            Description = data["Description"].ToString();
            Turn = int.Parse(data["Turn"].ToString());
            Weapon = (HardWareData.HardWareType)int.Parse(data["Weapon"].ToString());
            RewardId = data["RewardId"].ToString();
            RewardRate = int.Parse(data["RewardRate"].ToString());
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
}
