using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class PvpMessage105 : BasePvpMessage
{
	public int round;

	public PvpMessage105(PvpGameControl gameControl, JsonObject pvpData) : base(gameControl, pvpData)
	{
		this.messageID = 105;
		this.round = int.Parse (pvpData ["round"].ToString ());
	}

	public override void Run(Action callback)
	{
		JsonArray playersData = this.pvpData["players"] as JsonArray;
		if(playersData.Count == 2)
		{
			// 角色
			JsonObject oneData = playersData[0] as JsonObject;
			JsonObject twoData = playersData[1] as JsonObject;

			int oneHp = int.Parse(oneData["hp"].ToString());
			int twoHp = int.Parse(twoData["hp"].ToString());

			int onePower = int.Parse(oneData["power"].ToString());
			int twoPower = int.Parse(twoData["power"].ToString());
			
			// 如果是自己
			if(int.Parse(oneData["user_id"].ToString()) == this.gameControl.PvpCharacterSelf.PvpUserInfo.UserId)
			{
				this.gameControl.RefreshHpAndPower(this.gameControl.PvpCharacterSelf, oneHp, onePower);
				this.gameControl.RefreshHpAndPower(this.gameControl.PvpCharacterEnemy, twoHp, twoPower);
			}
			else if(int.Parse(twoData["user_id"].ToString()) == this.gameControl.PvpCharacterSelf.PvpUserInfo.UserId)
			{
				this.gameControl.RefreshHpAndPower(this.gameControl.PvpCharacterEnemy, oneHp, onePower);
				this.gameControl.RefreshHpAndPower(this.gameControl.PvpCharacterSelf, twoHp, twoPower);
			}
			Debug.Log("调用同步血量消息！" + oneHp + ":" + twoHp);
		}

		if(callback != null) callback();
	}
}
