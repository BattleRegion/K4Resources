using UnityEngine;
using SimpleJson;
using System.Collections;

public class ArenaPrevRankInfo
{
	public int rank;

	public int id;
	
	public string name;

	public int level;

	public int pvpRank;

	public UserWare weapon;
	
	public UserWare helmet;
	
	public UserWare armor;
	
	public UserPet leader;
	
	public ArenaPrevRankInfo(JsonObject data, int rank)
	{
		this.rank = rank;
		this.pvpRank = rank;

		if(data == null) return;
		
		this.level = int.Parse(data["level"].ToString());

		if (data.ContainsKey("user_id")) this.id = int.Parse(data["user_id"].ToString());
		if (data.ContainsKey("name")) this.name = data["name"].ToString();
		if (data.ContainsKey("weapons")) this.AnalysisWare((JsonArray)data["weapons"]);
		if (data.ContainsKey("leader")) this.leader = new UserPet((JsonObject)data["leader"]);
		if (data.ContainsKey("star_level")) this.pvpRank = int.Parse(data["star_level"].ToString());
	}
	
	void AnalysisWare(JsonArray wares)
	{
		foreach (JsonObject data in wares)
		{
			UserWare userWare = new UserWare(data);
			if (userWare.IsWeapon())
			{
				this.weapon = userWare;
			}
			else if (userWare.IsHelmet())
			{
				this.helmet = userWare;
			}
			else if(userWare.IsArmor())
			{
				this.armor = userWare;
			}
		}
	}
}
