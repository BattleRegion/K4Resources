using UnityEngine;
using System.Collections;
using SimpleJson;

public class PvpUserWare : UserWare 
{
	public PvpUserWare(JsonObject data) : base(data)
	{
		if (data.ContainsKey("level")) this.Level = int.Parse(data["level"].ToString());
	}

	public override int Level
	{
		get
		{
			return this.level;
		}
		set
		{
			this.level = value;
		}
	}
}
