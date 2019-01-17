using UnityEngine;
using System.Collections;
using SimpleJson;

public class HeroConfig : GameConfig 
{
    public HeroConfig()
    {
        this.ConfigName = "Hero";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            HeroData m = new HeroData(data);
            Configs.Add(m);
        }
    }

    public HeroData GetHeroByLvl(int lvl)
    {
        foreach (HeroData hd in Configs)
        {
            if (hd.Lvl == lvl)
            {
                return hd;
            }
        }
        return null;
    }

    public int GetHeroMaxLevel()
    {
        HeroData hd = (HeroData)Configs[Configs.Count - 1];
        return hd.Lvl;
    }

    /// <summary>
    /// 角色等级
    /// </summary>
    public int GetLevelByHeroExp(int exp)
    {
        foreach(HeroData hd in Configs)
        {
            if(exp < hd.Exp)
            {
                return hd.Lvl - 1;
            }
            else if(exp == hd.Exp)
            {
                return hd.Lvl;
            }
        }
        return -1;
    }
}

public class HeroData : ConfigData
{
    public int Lvl;

    public int Exp;

    public int Hp;

    public int Attack;

    public int Def;

    public int Hcost;

    public int HPower;

    public HeroData(JsonObject data)
    {
        Lvl = int.Parse(data["Lv"].ToString());
        Exp = int.Parse(data["HeroEXP"].ToString());
        Hp = int.Parse(data["Hp"].ToString());
        Attack = int.Parse(data["Attack"].ToString());
        Def = int.Parse(data["Def"].ToString());
        Hcost = int.Parse(data["HCost"].ToString());
        HPower = int.Parse(data["HPow"].ToString());
    }
}
