using UnityEngine;
using SimpleJson;
using System.Collections;

public class ArenaThisRankInfo : ArenaPrevRankInfo
{
	public ArenaThisRankInfo(JsonObject data, int rank) : base(data, rank)
	{
		if(data == null) return;
	}
}
