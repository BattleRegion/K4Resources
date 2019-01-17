using UnityEngine;
using System.Collections;
using SimpleJson;
public class PetLevelConfig : GameConfig
{

	public PetLevelConfig()
    {
        //this.ConfigName = "pet_level";
        this.ConfigName = "PetLv";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            PetLevelData pl = new PetLevelData(data);
            Configs.Add(pl);
        }
    }

    public int GetCurLevel(int exp)
    {
        foreach (PetLevelData pl in Configs)
        {
            if (exp < pl.NeedExp)
            {
                return pl.Lvl  -1;
            }
            else if(exp == pl.NeedExp)
            {
                return pl.Lvl;
            }
        }
        return -1;
    }

    /// <summary>
    /// 等级总经验
    /// </summary>
    public int GetExpByLevel(int level)
    {
        foreach (PetLevelData pl in Configs)
        {
            if (pl.Lvl == level)
            {
                return pl.NeedExp;
            }
        }
        return 0;
    }

    /// <summary>
    /// 等级升级所需经验
    /// </summary>
    public int GetLevelExp(int level)
    {
        if (GetExpByLevel(level + 1) == 0) return 0;
        return GetExpByLevel(level + 1) - GetExpByLevel(level);
    }
}

public class PetLevelData : ConfigData
{
    public int Lvl;

    public int NeedExp;

    public PetLevelData(JsonObject data)
    {
        try
        {
            Lvl = int.Parse(data["PetLv"].ToString());
            NeedExp = int.Parse(data["PetEXP"].ToString());
        }
        catch
        {
        }
    }
}
