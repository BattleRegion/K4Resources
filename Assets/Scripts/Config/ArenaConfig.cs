using UnityEngine;
using System.Collections;
using SimpleJson;
using System;

public class ArenaConfig : GameConfig
{
	public ArenaConfig()
	{
		this.ConfigName = "Arena";
		foreach (JsonObject data in (JsonArray)ConfigJsonData)
		{
			ArenaData s = new ArenaData(data);
			Configs.Add(s);
		}
	}
	
	public ArenaData GetArenaByLv(int lv)
	{
		foreach (ArenaData s in Configs)
		{
			if (s.RankLv == lv)
			{
				return s;
			}
		}
		return null;
	}

	public int GetLevelByExp(int exp)
	{
		int configCount = this.Configs.Count;
		ArenaData arenaData = null;
		for(int index = configCount - 1; index >= 0; index --)
		{
			arenaData = this.Configs[index] as ArenaData;
			if(arenaData != null && exp >= arenaData.RankExp) return arenaData.RankLv;
		}
		return 0;
	}
}

public class ArenaData : ConfigData
{
	public int RankLv;
	public string RankName;
	public int RankExp;
	public int RankStar;
	public int CoinRate;
	public string RewardType;
	public int RewardRate;
	public string Chest;
    public string RankIcon;
	
	public ArenaData(JsonObject data)
	{
		try
		{
			this.RankLv = int.Parse(data["RankLv"].ToString());
			this.RankName = data["RankName"].ToString();
			this.RankExp = int.Parse(data["RankEXP"].ToString());
			this.RankStar = int.Parse(data["RankStar"].ToString());
			this.CoinRate = int.Parse(data["CoinRate"].ToString());
			this.RewardType = data["RewardTyp"].ToString();
			this.RewardRate = int.Parse(data["RewardRate"].ToString());
			this.Chest = data["Chest"].ToString();
            this.RankIcon = data["RankIcon"].ToString();
		}
		catch(Exception e)
		{
			Debug.Log(e);
		}
	}
}
