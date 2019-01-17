using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class PvpMessage102 : BasePvpMessage
{
	private PvpMessageTimer102 messageTimer;

	public PvpMessage102(PvpGameControl gameControl, JsonObject pvpData) : base(gameControl, pvpData)
	{
		this.messageID = 102;

		GameObject gameObject = new GameObject ();
		gameObject.name = "PvpMessage102";
		messageTimer = gameObject.AddComponent<PvpMessageTimer102> ();
	}
	
	public override void Run(Action callback)
	{
		// 如果还在战斗阶段
		if(this.gameControl.fightStep == PvpGameControl.PvpFightStep.ACTION)
		{
			// 切换用户
			this.gameControl.GameRoundCharacterChange();
			// 暂停 1 秒后再执行下次行动
			if(this.messageTimer != null) this.messageTimer.Run(this.gameControl, this.pvpData, 1f, callback);
		} // 如果在回合开始阶段，直接行动
		else if(this.gameControl.fightStep == PvpGameControl.PvpFightStep.ROUND_BEGIN)
		{
			if(this.messageTimer != null) this.messageTimer.Run(this.gameControl, this.pvpData, 0f, callback);
		}
	}
}
class PvpMessageTimer102 : MonoBehaviour
{
	private PvpGameControl gameControl;
	private JsonObject pvpData;
	private Action callback;

	public void Run(PvpGameControl gameControl, JsonObject pvpData, float delayTime, Action callback)
	{
		this.callback = callback;
		this.gameControl = gameControl;
		this.pvpData = pvpData;

		this.StartCoroutine (this.RunItem (delayTime));
	}
	private IEnumerator RunItem(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		
		// 对方行走数据
		JsonArray stepsData = this.pvpData ["steps"] as JsonArray;
		// 对方怪物数据
		JsonObject monsterData = null;
		// 对方怪物数据
		if(pvpData.ContainsKey("monster")) monsterData = pvpData["monster"] as JsonObject;
		
		// 当前回合
		this.gameControl.PvpRounds = int.Parse(pvpData["round"].ToString());
		// 敌人回合数
		this.gameControl.EnemyRoundsCount = (int)Mathf.Ceil((this.gameControl.PvpRounds + 1) * 0.5f);
		// 敌人行动
		this.gameControl.EnemyRound (stepsData, monsterData, ()=>
		{
			// 切换用户
			this.gameControl.GameRoundCharacterChange();
			// 执行回调
			if(this.callback != null) this.callback();
			// 消息元素
			GameObject.Destroy (this.gameObject);
		});
	}
}
