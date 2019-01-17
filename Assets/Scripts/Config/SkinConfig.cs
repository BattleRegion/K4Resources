using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
public class SkinConfig : GameConfig 
{
    public SkinConfig()
    {
        this.ConfigName = "Skin";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            SkinConfigData skinData = new SkinConfigData(data);
            Configs.Add(skinData);
        }
    }

    public SkinConfigData GetSkinDataById(string Id)
    {
        foreach(SkinConfigData skinData in Configs)
        {
            if(skinData.Id == Id)
            {
                return skinData;
            }
        }
        return null;
    }
}

public class SkinConfigData : ConfigData
{
    public string Id;
    public string IconId;
    public float XFrameRate;
    public float YFrameRate;
    public int FireFrame;
    public float WaitingFrameRate;
    public float RunFrameRate;
    public float AttackFrameRate;
    public float HurtFrameRate;
    public float DeadFrameRate;

    public List<float> WaitingRate = new List<float>();
    public List<float> RunRate = new List<float>();
    public List<float> AttackRate = new List<float>();
    public List<float> HurtRate = new List<float>();
    public List<float> DeadRate = new List<float>();
    public string FlyFXName;
    public string FireFXName;
    public SkinConfigData(JsonObject data)
    {
        try
        {
            Id = data["SKinID"].ToString();
            IconId = data["Icon"].ToString();
            //FireFrame = int.Parse(data["FireFrame"].ToString());
            //WaitingFrameRate = float.Parse(data["WaitingFrameRate"].ToString());
            //AttackFrameRate = float.Parse(data["AttackFrameRate"].ToString());
            //DeadFrameRate = float.Parse(data["DeathFrameRate"].ToString());
            //RunFrameRate = float.Parse(data["RunFrameRate"].ToString());
            FlyFXName = data["FlyFX"].ToString();
            FireFXName = data["FireFX"].ToString();
        }
        catch
        {
        }

        //SetFrame(WaitingRate, data["WaitingRate"].ToString());
        //SetFrame(RunRate, data["RunRate"].ToString());
        //SetFrame(AttackRate, data["AttackRate"].ToString());
        //SetFrame(DeadRate, data["DeathRate"].ToString());
    }

    void SetFrame(List<float> list, string frameStr)
    {
        if (frameStr != "null")
        {
            try
            {
                string[] frameRates = frameStr.Split(',');
                for (int i = 0; i < frameRates.Length; i++)
                {
                    float rate = float.Parse(frameRates[i]);
                    list.Add(rate);
                }
                if (list.Count < 2)
                {
                    list.Add(0);
                }
            }
            catch
            {
                list.Add(0);
                list.Add(0);
            }
        }
        else
        {
            list.Add(0);
            list.Add(0);
        }
    }
}
