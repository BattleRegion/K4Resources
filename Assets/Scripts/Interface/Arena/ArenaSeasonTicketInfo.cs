using UnityEngine;
using SimpleJson;
using System.Collections.Generic;

public class ArenaSeasonTicketInfo
{
	public int id;
	public JsonArray consumeList;

	//解析数据
	public void AnalysisData(JsonObject data)
	{
		this.id = int.Parse (data ["ticket_id"].ToString ());
		this.consumeList = (JsonArray)data["consumes"];
	}
}
