using UnityEngine;
using System.Collections;
using SimpleJson;

public class HardwareLevelConfig : GameConfig
{
    public HardwareLevelConfig()
    {
        this.ConfigName = "HardwareLv";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            HardwareLevelData hld = new HardwareLevelData(data);
            Configs.Add(hld);
        }
    }

    /// <summary>
    /// 获得Lv等级总经验值
    /// </summary>
    /// <returns></returns>
    public int GetHardwareExpByLv(int Lv)
    {
        foreach (HardwareLevelData hld in Configs)
        {
            if (Lv == hld.HardwareLv) return hld.HardwareExp;
        }
        return 0;
    }

    /// <summary>
    /// 获得Lv等级到Lv+1等级需要经验值
    /// </summary>
    /// <returns></returns>
    public int GetHardwareLvUpNeedByLv(int Lv)
    {
        foreach (HardwareLevelData hld in Configs)
        {
            if (Lv == hld.HardwareLv)
            {
                return GetHardwareExpByLv(Lv + 1) - GetHardwareExpByLv(Lv);
            }
        }
        return 0;
    }

    /// <summary>
    /// 等级
    /// </summary>
    public int GetLevelByExp(int exp)
    {
        foreach(HardwareLevelData hld in Configs)
        {
            if(exp < hld.HardwareExp)
            {
                return hld.HardwareLv - 1;
            }
            else if(exp == hld.HardwareExp)
            {
                return hld.HardwareLv;
            }
        }
        return -1;
    }
}

public class HardwareLevelData : ConfigData
{
    public int HardwareLv;

    public int HardwareExp;

    public HardwareLevelData(JsonObject data)
    {
        try
        {
            HardwareLv = int.Parse(data["HardwareLv"].ToString());
            HardwareExp = int.Parse(data["HardwareEXP"].ToString());
        }
        catch
        {
        }
    }
}
