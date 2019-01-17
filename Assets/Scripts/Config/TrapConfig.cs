using UnityEngine;
using System.Collections;
using SimpleJson;
public class TrapConfig : GameConfig
{

    public TrapConfig()
    {
        this.ConfigName = "Traps";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            TrapData barrierData = new TrapData(data);
            Configs.Add(barrierData);
        }
    }

    public TrapData GetTrapById(string Id)
    {
        foreach (TrapData t in Configs)
        {
            if (t.Id == Id)
            {
                return t;
            }
        }
        return null;
    }
}

public class TrapData : ConfigData
{
    public string Id;

    public string SkinId;

    public string Description;

    public int DamagePersent;

    public TrapData(JsonObject data)
    {
        Id = data["TrapId"].ToString();
        SkinId = data["SkinId"].ToString();
        Description = data["Description"].ToString();
        DamagePersent = int.Parse(data["DamagePersent"].ToString());
    }
}   
