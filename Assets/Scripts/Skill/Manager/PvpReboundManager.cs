using System;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class PvpReboundManager
{
	// 弹回重置的数据
	public static JsonObject reboundData;

	public static void Progress(PvpGameControl gameControl, JsonObject jsonData, bool drawGrid = false)
	{
		JsonArray playerDataList = jsonData ["players"] as JsonArray;
		if(playerDataList == null || playerDataList.Count < 2) return;

		// 重置弹回状态
		PvpMonsterReboundReset (gameControl);

		// 重置双方 buff
		gameControl.PvpCharacterSelf.pvpPlayerBuff.ClearBuffList();
		gameControl.PvpCharacterEnemy.pvpPlayerBuff.ClearBuffList();

		// 重置双方技能 cd
		gameControl.PvpCharacterSelf.pvpPlayerSkill.ClearCd ();
		gameControl.PvpCharacterEnemy.pvpPlayerSkill.ClearCd ();

		PvpFightUnit selfUnit = null;
		PvpFightUnit enemyUnit = null;

		JsonObject bottomInfo = playerDataList [0] as JsonObject;

		JsonObject selfInfo = null;
		JsonObject enemyInfo = null;

		if(int.Parse(bottomInfo["user_id"].ToString()) == UserManager.CurUserInfo.UserId)
		{
			selfInfo = playerDataList[0] as JsonObject;
			enemyInfo = playerDataList[1] as JsonObject;
		}else{
			selfInfo = playerDataList[1] as JsonObject;
			enemyInfo = playerDataList[0] as JsonObject;
		}

		// 如果需要重绘格子
		if(drawGrid)
		{
			// 设置游戏回合
			gameControl.PvpRounds = int.Parse(jsonData["round"].ToString());
			// 设置双方回合
			gameControl.PvpRoundProgress(int.Parse(bottomInfo["user_id"].ToString()));
				
			JsonArray mapsData = jsonData["maps"] as JsonArray;
			foreach(PvpEliminate pvpEliminate in gameControl.AllEliminates)
			{
				int gridColor = GetGridColor(pvpEliminate.XPosition, pvpEliminate.YPosition, mapsData);
				// 重置格子颜色
				if(gridColor != -1) pvpEliminate.AttrubuteToRender(-1, gridColor);
			}
		}

		// 处理自己
		ProgressItem (gameControl, selfInfo, gameControl.PvpCharacterSelf, gameControl.PvpCharacterEnemy, drawGrid);
		// 处理敌人
		ProgressItem (gameControl, enemyInfo, gameControl.PvpCharacterEnemy, gameControl.PvpCharacterSelf, drawGrid);

		// 处理多余的怪物
		List<PvpFightUnit> destoryMonsterFightUnit = GetReboundMonsterList (gameControl);
		if(destoryMonsterFightUnit != null && destoryMonsterFightUnit.Count > 0)
		{
			for(int destoryMonsterIndex = 0; destoryMonsterIndex < destoryMonsterFightUnit.Count; destoryMonsterIndex ++)
			{
				GameObject.Destroy(destoryMonsterFightUnit[destoryMonsterIndex].gameObject);
				gameControl.AllBarrier.Remove(destoryMonsterFightUnit[destoryMonsterIndex]);
			}
			destoryMonsterFightUnit.Clear();
		}

		if(drawGrid)
		{
			gameControl.GameReboundCharacterChange();
		}

		// 弹回数据为空
		PvpReboundManager.reboundData = null;
	}

	private static void ProgressItem(PvpGameControl gameControl, JsonObject playerData, PvpFightUnit sourceItem, PvpFightUnit targetItem, bool drawGrid = false)
	{
		// ====================================================== 角色处理开始 ==============================================
		// 设置位置
		// 如果需要重绘格子
		if(drawGrid)
		{
			sourceItem.SetPosition (int.Parse(playerData["x"].ToString()), int.Parse(playerData["y"].ToString()));
		}
		sourceItem.CurHp = int.Parse(playerData ["hp"].ToString ());
		sourceItem.PvpUserInfo.Pvp_SkillPower = int.Parse (playerData ["skill_power"].ToString ());
		// ====================================================== 角色处理开始 ==============================================

		// ====================================================== 怪物处理开始 ==============================================
		// 处理召唤的怪物
		JsonArray monsterDataList = playerData ["call_monsters"] as JsonArray;
		if(monsterDataList != null && monsterDataList.Count > 0)
		{
			foreach(JsonObject monsterData in monsterDataList)
			{
				PvpMonster pvpMonster = GetPvpMonsterByID(gameControl, monsterData["monster_id"].ToString(), sourceItem) as PvpMonster;
				// 如果怪物数据为空，弹回处理
				if(pvpMonster == null)
				{
					BaseSkillItem skillItem = sourceItem.pvpPlayerSkill.GetSkillItemBySkillID(monsterData["skill_id"].ToString());
					Vector3 monsterPosition = new Vector3(int.Parse(monsterData["x"].ToString()), int.Parse(monsterData["y"].ToString()), 0f);
					pvpMonster = gameControl.CreateMonster(skillItem, 
					                                       monsterPosition, 
					                                       monsterData["monster_id"].ToString(), 
					                                       sourceItem, 
					                                       int.Parse(monsterData["round"].ToString()),
					                                       true
					                                       );
				}
				// 设置弹回状态为真
				pvpMonster.MonsterReboundStatus = true;
				// 设置位置
				pvpMonster.SetPosition(int.Parse(monsterData["x"].ToString()), int.Parse(monsterData["y"].ToString()));
				pvpMonster.CurHp = int.Parse(monsterData["hp"].ToString());
				pvpMonster.ChangeHiddenStatus(false, 1f);
			}
		}
		// ====================================================== 怪物处理结束 ==============================================

		// ====================================================== 技能效果处理开始 ==============================================
		JsonArray skillDataList = playerData ["skill_effects"] as JsonArray;
		if(skillDataList != null && skillDataList.Count > 0)
		{
			BaseSkillItem skillItem = null;
			SkillData skillConfig = null;
			List<PvpBuffData> buffList = null;
			int buffRound = 0;

			foreach(JsonObject skillData in skillDataList)
			{
				buffRound = int.Parse(skillData["round"].ToString());
				if(buffRound <= 10) // 这儿写死，
				{
					string skillID = skillData["skill_id"].ToString();
					// 如果技能不是眩晕以及传送
					if(skillID != "reject_halo" && skillID != "reject_stealth")
					{
						skillConfig = ConfigManager.SkillConfig.GetSkillById(skillID);
						// 获取技能配置
						skillItem = SkillManager.GetSkillItem(skillConfig);

						buffList = skillItem.skillData.conditionData.buffList;
						foreach(PvpBuffData pvpBuffData in buffList)
						{
							if(pvpBuffData.roundValue > 0 && pvpBuffData.buffType != BuffTypeEnum.Trigger_Skill && pvpBuffData.buffType != BuffTypeEnum.Summon && pvpBuffData.buffType != BuffTypeEnum.Stop_Dizziness)
							{
								// 设置回合
								pvpBuffData.roundValue = buffRound;
								// 添加 buff
								sourceItem.pvpPlayerBuff.InsertBuff(skillConfig.Id, pvpBuffData, false);
							}
						}
					}else{
						// 如果是眩晕
						if(skillID == "reject_halo")
						{
							// 添加阻止眩晕的 buff
							sourceItem.pvpPlayerBuff.InsertBuff("", new PvpBuffData(BuffTypeEnum.Stop_Dizziness,
							                                                    BuffTargetTypeEnum.SELF, 
							                                                    BuffStageTypeEnum.Attack, 
							                                                    BuffEffectTypeEnum.DEFAULT, 
							                                                    0f, 
							                                                    false, 
							                                                    int.Parse(skillData["round"].ToString()), 
							                                                    false
							                                                    ), false);
						}else if(skillID == "reject_stealth")
						{
							// 添加阻止传送的 buff
							sourceItem.pvpPlayerBuff.InsertBuff("", new PvpBuffData(BuffTypeEnum.Stop_Transmit,
						                                                        BuffTargetTypeEnum.SELF, 
						                                                        BuffStageTypeEnum.Attack, 
						                                                        BuffEffectTypeEnum.DEFAULT, 
						                                                        0f, 
						                                                        false, 
						                                                        int.Parse(skillData["round"].ToString()), 
						                                                        false
						                                                        ), false);
						}
					}
				}
			}
		}
		// ====================================================== 技能效果处理开始 ==============================================

		// ====================================================== 技能处理开始 ==============================================
		JsonArray petDataList = playerData ["pets"] as JsonArray;
		if(petDataList != null && petDataList.Count > 0)
		{
			PvpSkillCdData pvpSkillCdData = null;
			foreach(JsonObject petData in petDataList)
			{
				JsonArray petSkillDataList = petData["skills"] as JsonArray;
				foreach(JsonObject petSkillData in petSkillDataList)
				{
					pvpSkillCdData = sourceItem.pvpPlayerSkill.GetSkillCdDataBySkillID(petSkillData["skill_id"].ToString(), int.Parse(petData["house_id"].ToString()));
					if(pvpSkillCdData != null) pvpSkillCdData.cd = int.Parse(petSkillData["cd_times"].ToString());
				}
			}
		}
		// ====================================================== 技能处理结束 ==============================================
	}
	
	private static int GetGridColor(int x, int y, JsonArray maps)
	{
		foreach(JsonObject mapData in maps)
		{
			if(int.Parse(mapData["x"].ToString()) == x && int.Parse(mapData["y"].ToString()) == y)
			{
				if(int.Parse(mapData["color"].ToString()) == 0) return int.Parse(mapData["original"].ToString());
				return int.Parse(mapData["color"].ToString());
			}
		}
		return -1;
	}

	private static PvpBuffData GetBuffStopDizzinessData(List<PvpBuffData> buffList)
	{
		foreach(PvpBuffData buffData in buffList)
		{
			if(buffData.buffType == BuffTypeEnum.Stop_Dizziness) return buffData;
		}
		return null;
	}

	/// <summary>
	/// 怪物弹回状态重置
	/// </summary>
	/// <param name="gameControl">Game control.</param>
	private static void PvpMonsterReboundReset(PvpGameControl gameControl)
	{
		PvpMonster pvpMonster = null;
		foreach(PvpFightUnit fightUnit in gameControl.AllBarrier)
		{
			if(fightUnit.GetType() == typeof(PvpMonster))
			{
				pvpMonster = (PvpMonster)fightUnit;
				pvpMonster.MonsterReboundStatus = false;
			}
		}
	}
	/// <summary>
	/// 根据怪物 ID 和角色查找怪物
	/// </summary>
	/// <returns>The pvp monster by I.</returns>
	/// <param name="gameControl">Game control.</param>
	/// <param name="monsterID">Monster I.</param>
	/// <param name="pvpCharacter">Pvp character.</param>
	private static PvpFightUnit GetPvpMonsterByID(PvpGameControl gameControl, string monsterID, PvpFightUnit pvpCharacter)
	{
		PvpMonster pvpMonster = null;
		foreach(PvpFightUnit fightUnit in gameControl.AllBarrier)
		{
			if(fightUnit.GetType() == typeof(PvpMonster) && fightUnit.UserType == pvpCharacter.UserType)
			{
				pvpMonster = (PvpMonster)fightUnit;
				if(pvpMonster.MonsterID == monsterID) return fightUnit;
			}
		}
		return null;
	}

	/// <summary>
	/// 查找多余的怪物
	/// </summary>
	/// <returns>The rebound monster list.</returns>
	/// <param name="gameControl">Game control.</param>
	private static List<PvpFightUnit> GetReboundMonsterList(PvpGameControl gameControl)
	{
		List<PvpFightUnit> resultList = new List<PvpFightUnit> ();

		PvpMonster pvpMonster = null;
		foreach(PvpFightUnit fightUnit in gameControl.AllBarrier)
		{
			if(fightUnit.GetType() == typeof(PvpMonster))
			{
				pvpMonster = (PvpMonster)fightUnit;
				if(!pvpMonster.MonsterReboundStatus) resultList.Add(pvpMonster);
			}
		}
		return resultList;
	}
}
