using UnityEngine;
using System.Collections.Generic;
using SimpleJson;
using System;

public class PveFaceConfig : GameConfig
{
	public PveFaceConfig()
	{
		this.ConfigName = "PvEFace";
		foreach (JsonObject data in (JsonArray)ConfigJsonData)
		{
			PveFaceData s = new PveFaceData(data);
			Configs.Add(s);
		}
	}
	
	public List<PveFaceData> GetFaceListByType(int type)
	{
		List<PveFaceData> resultList = new List<PveFaceData>();
		foreach (PveFaceData s in Configs)
		{
			if (s.PveFaceId == type)
			{
				resultList.Add(s);
			}
		}
		return resultList;
	}
}

public class PveFaceData : ConfigData
{
	public int PveFaceId;
	public int Chance;
	public string FaceId;
	
	public PveFaceData(JsonObject data)
	{
		try
		{
			this.PveFaceId = int.Parse(data["PveFaceId"].ToString());
			this.Chance = int.Parse(data["Chance"].ToString());
			this.FaceId = data["FaceId"].ToString();
		}
		catch(Exception e)
		{
			Debug.Log(e);
		}
	}
}
