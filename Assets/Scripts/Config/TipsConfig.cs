using UnityEngine;
using System.Collections;
using SimpleJson;
using System;
using System.Collections.Generic;

public class TipsConfig : GameConfig
{
    public TipsConfig()
    {
        this.ConfigName = "Tips";
        foreach(JsonObject data in (JsonArray)ConfigJsonData)
        {
            TipsData tip = new TipsData(data);
            Configs.Add(tip);
        }
    }

    public enum TipsType
    {
        Party = 1,
        Dungeon = 2,
        BlackSmith = 3,
        Arena = 4,
        Social = 5,
        Mall = 6
    }

    public List<TipsData> GetTipsByType(TipsType type)
    {
        List<TipsData> list = new List<TipsData>();
        foreach(TipsData t in Configs)
        {
            if(t.TipsType == type)
            {
                list.Add(t);
            }
        }
        return list;
    }
}

public class TipsData : ConfigData
{
    public string TipsText;

    public TipsConfig.TipsType TipsType;

    public TipsData(JsonObject data)
    {
        try
        {
            TipsText = data["TipsText"].ToString();
            TipsType = (TipsConfig.TipsType)int.Parse(data["TipsTyp"].ToString());
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
}
