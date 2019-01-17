using UnityEngine;
using System.Collections;
using SimpleJson;
using System.Collections.Generic;

public class GuiderConfig : GameConfig
{

    public GuiderConfig()
    {
        this.ConfigName = "Guider";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            GuiderData m = new GuiderData(data);
            Configs.Add(m);
        }
    }

    public List<GuiderData> GetAllData()
    {
        List<GuiderData> dd = new List<GuiderData>();
        foreach (GuiderData d in Configs)
        {
            dd.Add(d);
        }
        return dd;
    }
}
public class GuiderData : ConfigData
{
    public string Scene;
    public string Condition;
    public string Tag;
    //public int GId;
    public int Id;
    //public int Group;
    public string LightObjectName;
    public string LightObjectTag;
    public string Description;
    public string Route;
    public string Map;
    public float DelayTime;

    public GuiderData(JsonObject data=null)
    {
        try
        {
            Condition = data["Condition"].ToString();
            Tag = data["Tag"].ToString();
            //GId = int.Parse(data["GId"].ToString());
            Id = int.Parse(data["Id"].ToString());
            //Group = int.Parse(data["Group"].ToString());
            Scene = data["Scene"].ToString();
            LightObjectName = data["LightObjectName"].ToString();
            LightObjectTag = data["LightObjectTag"].ToString();
            Description = data["Description"].ToString();
            Route = data["Route"].ToString();
            Map = data["Map"].ToString();
            DelayTime = float.Parse(data["Time"].ToString());
        }
        catch
        {
        }
    }
}

