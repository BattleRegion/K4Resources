using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System;
public class DungeonConfig : GameConfig 
{
    public DungeonConfig()
    {
        this.ConfigName = "Dungeon";
        foreach (JsonObject data in (JsonArray)ConfigJsonData)
        {
            DungeonData d = new DungeonData(data);
            Configs.Add(d);
        }
    }

    public DungeonData GetDungeonById(string Id)
    {
        foreach (DungeonData d in Configs)
        {
            if (d.Id == Id)
            {
                return d;
            }
        }
        return null;
    }

	public DungeonData GetRandomData()
	{
		return Configs [UnityEngine.Random.Range (0, Configs.Count - 1)] as DungeonData;
	}

    public List<DungeonData> GetAllData()
    {
        List<DungeonData> dd = new List<DungeonData>();
        foreach (DungeonData d in Configs)
        {   
            dd.Add(d);
        }
        return dd;
    }

    public List<DungeonData> GetChapterDungeons(string chapterId)
    {
        List<DungeonData> dd = new List<DungeonData>();
        foreach (DungeonData d in Configs)
        {
            if (d.ChapterId == chapterId)
            {
                dd.Add(d);
            }
        }
        return dd;
    }

    public List<string> GetOpenHardwares()
    {
        List<string> s = new List<string>();
        foreach(DungeonData dd in Configs)
        {
            if(!s.Contains(dd.OpenHardware) && !string.IsNullOrEmpty(dd.OpenHardware))
            {
                Debug.Log(dd.OpenHardware);
                s.Add(dd.OpenHardware);
            }
        }
        return s;
    }
}

public class DungeonData : ConfigData
{
    public string Id;
    public string Description;
    public int FloorCount;
    public string Scene;
    public string ChapterId;
    public DungeonEnum.ElementAttributes Element;
    public int SortId;
    public int Exp;
    public int Energy;
    public string DungeonIcon;
    public string[] DungeonIcons;

    //public string DungeonIcon2;
    //public string DungeonIcon3;
    public int Round;
    public int Warefare;
    public string ChanceTips;
    public string ExDungeon;
    public string[] ExDungeons;
    public string OpenHardware;

    public string BossTips;

	public string QuestTips;
	public string Quest;
	public string QuestParameter;

    public DungeonData(JsonObject data)
    {
        try
        {
            Id = data["DungeonId"].ToString();
            //DungeonIcon = data["DungeonIcon"].ToString();
            //DungeonIcon2 = data["DungeonIcon2"].ToString();
            //DungeonIcon3 = data["DungeonIcon3"].ToString();
            Energy = int.Parse(data["Energy"].ToString());
            Description = data["Description"].ToString();
            FloorCount = int.Parse(data["FloorCount"].ToString());
            Scene = data["Scene"].ToString();
            ChapterId = data["ChapterId"].ToString();
            SortId = int.Parse(data["SortId"].ToString());
            Exp = int.Parse(data["EXP"].ToString());
            Warefare = int.Parse(data["Warefare"].ToString());
            ChanceTips = data["ChanceTIps"].ToString();
            Round = int.Parse(data["Round"].ToString());
            Element = (DungeonEnum.ElementAttributes)int.Parse(data["Element"].ToString());


            //ExDungeon
            if (data["ExDungeon"] != null)
            {
                ExDungeon = data["ExDungeon"].ToString();
                if(ExDungeon.Contains(","))
                {
                    ExDungeons = ExDungeon.Split(',');
                }
                else
                {
                    ExDungeons = null;
                }
            }
            else
            {
                ExDungeon = null;
                ExDungeons = null;
            }

            //DungeonIcon
            if (data["DungeonIcon"] != null)
            {
                DungeonIcon = data["DungeonIcon"].ToString();
                if (DungeonIcon.Contains(","))
                {
                    DungeonIcons = DungeonIcon.Split(',');
                }
                else
                {
                    DungeonIcons = new string[1];
                    DungeonIcons[0] = data["DungeonIcon"].ToString();
                }
            }
            else
            {
                DungeonIcon = null;
                DungeonIcons = null;
            }


            OpenHardware = data["OpenHardware"].ToString();

			BossTips = data["BossTips"].ToString();
			
			this.QuestTips = data["QuestTips"].ToString();
			this.Quest = data["Quest"].ToString();
			this.QuestParameter = data["QuestParameter"].ToString();
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
}