using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System;
public class MonsterConfig : GameConfig  
{
    public MonsterConfig()
    {
        this.ConfigName = "Monsters";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            MonsterData m = new MonsterData(data);
            Configs.Add(m);
        }
    }

    public MonsterData GetMonsterById(string Id)
    {
        foreach (MonsterData m in Configs)
        {
            if (m.Id == Id)
            {
                return m;
            }
        }
        return null;
    }
}

public class MonsterData:ConfigData
{

    public enum Properties
    {
        Earth = 1,
        Fire = 2,
        Wind = 3,
        Water = 4
    }
    /// <summary>
    /// 怪物攻击类型
    /// </summary>
    public enum AttackType
    {
        Close = 1,
        Far = 2
    }

    public enum MonsterType
    {
        Comm = 0,//普通
        RunAway = 1//逃避型
    }

    //public DungeonEnum.AttackType Type;

    public string SkinId;

    public string Id;

    public float Attack;

    public float Hp;

    //射程
    public int Range;

    public int RunPower;

    public int Def;

    public DungeonEnum.ElementAttributes Element;

    public MonsterType CurMonsterType;


    public MonsterData(JsonObject data)
    {
        try
        {
            Id = data["MonsterId"].ToString();
            Def = int.Parse(data["Def"].ToString());
            SkinId = data["SkilId"].ToString();
            Hp = float.Parse(data["Hp"].ToString());
            Attack = float.Parse(data["Attack"].ToString());
            //Type = (DungeonEnum.AttackType)int.Parse(data["AttackType"].ToString());
            Range = int.Parse(data["Range"].ToString());
            RunPower = int.Parse(data["RunPower"].ToString());
            Element = (DungeonEnum.ElementAttributes)int.Parse(data["Element"].ToString());
            CurMonsterType = (MonsterType)int.Parse(data["MonsterType"].ToString());
            //CurMonsterType = MonsterType.RunAway;
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
}
