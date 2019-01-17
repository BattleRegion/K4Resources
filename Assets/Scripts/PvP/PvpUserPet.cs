using UnityEngine;
using SimpleJson;
using System.Collections;

public class PvpUserPet : UserPet 
{
	private int Pvp_Level;

	public PvpUserPet(JsonObject data):base(data)
	{
	}

	public override int Level
	{
		get
		{
			return this.Pvp_Level;
		}
		set
		{
			this.Pvp_Level = value;
		}
	}
}
