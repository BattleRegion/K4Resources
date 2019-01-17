using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class PvpMessage103 : BasePvpMessage
{
	public PvpMessage103(PvpGameControl gameControl, JsonObject pvpData) : base(gameControl, pvpData) 
	{
		this.messageID = 103;
	}
	
	public override void Run(Action callback)
	{
		// 如果在回合开始阶段
		if(this.gameControl.fightStep == PvpGameControl.PvpFightStep.ROUND_BEGIN)
		{
			// 因为前面角色没有行动过，所以回合需要把回合数加 1
			this.gameControl.PvpRounds ++;

			this.gameControl.GameRoundCoolingTimerChange();

			// 切换用户
			this.gameControl.GameRoundCharacterChange(()=>
			{
				if(callback != null) callback();
			});
		}
		else if(this.gameControl.fightStep == PvpGameControl.PvpFightStep.ACTION) // 如果是在战斗阶段
		{
			// 如果当前行动的角色与应当行动的角色不同，做弹回处理
			if(this.gameControl.CurrentCharacter.PvpUserInfo.UserId != int.Parse(pvpData["user_id"].ToString()))
			{
				this.gameControl.ReboundCharacterAction (()=>
				{
					if(callback != null) callback();
				});
			}else{ //如果当前行动的角色与应该行动的角色相同，这种情况不应该发生，除非服务器端切换用色间隔太快，所以前端肯定可以表现完。
				Debug.Log("这种情况不应该发生 ！！！");
			}
		}
	}
}
