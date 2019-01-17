using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class PvpMessage107 : BasePvpMessage
{
	public PvpMessage107(PvpGameControl gameControl, JsonObject pvpData) : base(gameControl, pvpData) 
	{
		this.messageID = 107;
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
			
			if(oneHp <= 0 && twoHp <= 0)
			{
				this.gameControl.fightResult = PvpGameControl.PvpFightResult.TIE; //
			}else
			{
				// 如果有一方血量小于零
				if(oneHp <= 0 || twoHp <= 0)
				{
					// 如果是自己
					if(int.Parse(oneData["user_id"].ToString()) == this.gameControl.PvpCharacterSelf.PvpUserInfo.UserId)
					{
						//
						if(oneHp <= 0)
						{
							this.gameControl.fightResult = PvpGameControl.PvpFightResult.FAILED;
						}else if(twoHp <= 0)
						{
							this.gameControl.fightResult = PvpGameControl.PvpFightResult.SUCCESS;
						}
					}else if(int.Parse(twoData["user_id"].ToString()) == this.gameControl.PvpCharacterSelf.PvpUserInfo.UserId)
					{
						if(oneHp <= 0)
						{
							this.gameControl.fightResult = PvpGameControl.PvpFightResult.SUCCESS;
						}else if(twoHp <= 0)
						{
							this.gameControl.fightResult = PvpGameControl.PvpFightResult.FAILED;
						}
					}
				}
			}
			
			// 显示战斗结果
			if(this.gameControl.fightResult != PvpGameControl.PvpFightResult.DEFAULT)
			{
				this.gameControl.FightResultProgress(); // 处理战斗结果
			}
		}
		if(callback != null) callback();
	}
}
