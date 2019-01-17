using UnityEngine;
using System.Collections;
using SimpleJson;
using System;



public class RandomDataConfig : GameConfig
{
    public RandomDataConfig()
    {
        this.ConfigName = "Seed";
      
    }

    public JsonArray GetData()
    {
        return (JsonArray)ConfigJsonData;
    }
}

public class RandomData : ConfigData
{
    public string DataString;
    public RandomData(JsonObject data)
    {
        try
        {
            DataString = data.ToString();           
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}