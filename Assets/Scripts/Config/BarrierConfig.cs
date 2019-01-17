using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
public class BarrierConfig : GameConfig {
	// Use this for initialization
    public BarrierConfig()
    {
        this.ConfigName = "Barriers";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            BarrierData barrierData = new BarrierData(data);
            Configs.Add(barrierData);
        }
    }

    public BarrierData GetBarrierById(string Id)
    {
        foreach (BarrierData b in Configs)
        {
            if (b.Id == Id)
            {
                return b;
            }
        }
        return null;
    }
}

public class BarrierData:ConfigData
{
    public string Id;

    public string SkinId;

    public string Description;

    public float Hp;

    public BarrierData(JsonObject data)
    {
        Id = data["BarrierId"].ToString();
        SkinId = data["SkinId"].ToString();
        Description = data["Description"].ToString();
        Hp = float.Parse(data["Hp"].ToString());
    }
}
