using UnityEngine;
using System.Collections;
using SimpleJson;
using System;

public class EggConfig : GameConfig
{
	public EggConfig()
	{
		this.ConfigName = "Egg";
		foreach (JsonObject data in (JsonArray)ConfigJsonData)
		{
			EggData s = new EggData(data);
			Configs.Add(s);
		}
	}
	
	public EggData GetEggById(string EggId)
	{
		foreach (EggData s in Configs)
		{
			if (s.EggId == EggId)
			{
				return s;
			}
		}
		return null;
	}
}

public class EggData : ConfigData
{
	public string EggId;
	public string SkinId;
	public string Description;
	public int Hp;
	public int Def;
	public string MonsterId;
	
	public EggData(JsonObject data)
	{
		try
		{
			this.EggId = data["EggId"].ToString();
			this.SkinId = data["SkinId"].ToString();
			this.Description = data["Description"].ToString();
			this.Hp = int.Parse(data["Hp"].ToString());
			this.Def = int.Parse(data["Def"].ToString());
			this.MonsterId = data["MonsterId"].ToString();
		}
		catch(Exception e)
		{
			Debug.Log(e);
		}
	}
}
