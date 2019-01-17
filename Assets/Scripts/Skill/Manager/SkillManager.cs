using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

public class SkillManager
{
	/// <summary>
	/// 技能数量
	/// </summary>
	public static int RepeatMagicCount;

	/// <summary>
	/// 延迟展示
	/// </summary>
	public static Dictionary<string, List<BaseSkillEffect>> DelayEffectDict;

	/// <summary>
	/// 初始化
	/// </summary>
	public static void Init()
	{
		// 处理未展示完的效果
		SkillManager.DelayEffect ();
		// 重置多重施法数量
		SkillManager.RepeatMagicCount = 0;
	}

	public static List<string> CommonMonsterList = new List<string> ();

	/// <summary>
	/// 延迟添加
	/// </summary>
	/// <param name="skillID">Skill I.</param>
	/// <param name="skillEffect">Skill effect.</param>
	private static void DelayInsert(string skillID, BaseSkillEffect skillEffect)
	{
		// 如果在列表中不存在，创建列表
		if(!SkillManager.DelayEffectDict.ContainsKey(skillID))
		{
			SkillManager.DelayEffectDict.Add(skillID, new List<BaseSkillEffect>());
		}
		// 添加数据
		SkillManager.DelayEffectDict [skillID].Add (skillEffect);
	}

	/// <summary>
	/// 处理延时效果
	/// </summary>
	private static void DelayEffect()
	{
		// 如果存在数据
		if(SkillManager.DelayEffectDict != null)
		{
			List<string> skillList = new List<string>(SkillManager.DelayEffectDict.Keys);
			foreach(string skillID in skillList)
			{
				// 处理单个效果
				SkillManager.DelayEffectItem(skillID);
			}
		}
		SkillManager.DelayEffectDict = new Dictionary<string, List<BaseSkillEffect>> ();
	}

	/// <summary>
	/// 处理单个延时效果
	/// </summary>
	/// <param name="skillID">Skill I.</param>
	private static void DelayEffectItem(string skillID)
	{
		// 如果包含数据
		if(SkillManager.DelayEffectDict.ContainsKey(skillID))
		{
			List<BaseSkillEffect> skillEffectList = SkillManager.DelayEffectDict[skillID];
			if(skillEffectList != null && skillEffectList.Count > 0)
			{
				// 遍历表现效果
				while(skillEffectList.Count > 0)
				{
					BaseSkillEffect skillEffect = skillEffectList[0];
					skillEffect.Run();
					skillEffectList.RemoveAt(0);
				}
			}
			// 移除数据
			SkillManager.DelayEffectDict.Remove(skillID);
		}
	}

	/// <summary>
	/// 技能触发，不需要打击目标
	/// </summary>
	/// <param name="skillData">Skill data.</param>
	/// <param name="step">Step.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="cd">If set to <c>true</c> cd.</param>
	public static void Trigger(SkillData skillData, int step, PvpFightUnit sourceItem, bool cd = true, Action callback = null)
	{
		// 如果需要检测 CD
		if(cd)
		{
			// 设置技能数量
			SkillManager.RepeatMagicCount ++;
		}

		BaseSkillItem skillItem = sourceItem.pvpPlayerSkill.GetSkillItemBySkillID (skillData.Id);
		if(skillItem == null) return;

		// 如果攻击的角色数量为 0
		List<PvpFightUnit> resultList = sourceItem.GameControl.GetSkillFightUnitTarget (skillData, sourceItem);
		if(resultList == null || resultList.Count == 0)
		{
			/*
			#if UNITY_STANDALONE_WIN
				StringBuilder stringBuilder = new StringBuilder ();
				stringBuilder.Append ("---> 触发技能信息\n");
				stringBuilder.Append ("技能名称：");
				stringBuilder.Append (skillData.Name);
				
				if(sourceItem != null && sourceItem.PvpUserInfo != null)
				{
					stringBuilder.Append ("，攻击方 ID：");
					stringBuilder.Append (sourceItem.PvpUserInfo.UserId);
					stringBuilder.Append ("，攻击方 X 位置：");
					stringBuilder.Append (sourceItem.XPosition);
					stringBuilder.Append ("，攻击方 Y 位置：");
					stringBuilder.Append (sourceItem.YPosition);
				}
				stringBuilder.Append ("目标个数为零\n");
				PvpLogManager.Log (sourceItem.GameControl.PvpTableID, stringBuilder.ToString ());
			#endif
			*/
			// 设置 技能 CD
			if(sourceItem.UserType == sourceItem.GameControl.PvpCharacterSelf.UserType)
			{
				sourceItem.pvpPlayerSkill.RefreshCd (skillData.Id, sourceItem.GameControl.GetHouseID());
			}
			// 表现
			SkillEffectManager.Trigger(null, skillData.Id, sourceItem, null, null, (r)=>
			{
				// 如果需要检测 CD
				if(cd)
				{
					// 设置技能数量
					SkillManager.RepeatMagicCount --;
				}
				if(callback != null) callback();
			}, null);
			return;
		}
		// 如果不是范围伤害
		if(skillItem.skillData.skillTarget != SkillTargetTypeEnum.Range)
		{
			// 触发法术
			Trigger (skillData, step, sourceItem, resultList [0], null, cd, callback);
		}else
		{
			// 触发法术
			Trigger(skillData, step, sourceItem, null, resultList, cd, callback);
		}
	}

	/// <summary>
	/// 技能触发，需要打击目标
	/// </summary>
	/// <param name="skillData">Skill data.</param>
	/// <param name="step">Step.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="targetItemList">Target item list.</param>
	/// <param name="cd">If set to <c>true</c> cd.</param>
	public static void Trigger(SkillData skillData, int step, PvpFightUnit sourceItem, PvpFightUnit targetItem, List<PvpFightUnit> targetItemList = null, bool cd = true, Action callback = null)
	{
		// 如果是自己，需要刷新技能 CD
		if(sourceItem.UserType == sourceItem.GameControl.PvpCharacterSelf.UserType)
		{
			sourceItem.pvpPlayerSkill.RefreshCd (skillData.Id, sourceItem.GameControl.GetHouseID());
		}
		// 如果需要检测 CD
		if(cd)
		{
			// 检测触发技能 Buff
			List<PvpBuffData> triggerBuffList = sourceItem.pvpPlayerBuff.TriggerSkillCheck (skillData.Id);
			// 如果存在触发技能的 Buff
			if(triggerBuffList != null && triggerBuffList.Count > 0)
			{
				// 遍历触发技能
				foreach(PvpBuffData buffData in triggerBuffList)
				{
					// 如果触发条件达成
					if(ConditionOddsTriggerCheck(sourceItem, buffData))
					{
						// 设置多重施法状态
						TriggerBuffItem(sourceItem.pvpPlayerSkill.GetSkillItemBySkillID(skillData.Id), buffData, sourceItem, targetItem);
					}
				}
			}
		}
		// 触发技能
		TriggerSkillItem(GetSkillItem(skillData), step, sourceItem, targetItem, targetItemList, callback);
	}

	/// <summary>
	/// 普通攻击触发、行走触发
	/// </summary>
	/// <param name="triggerType">Trigger type.</param>
	/// <param name="step">Step.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="targetItemList">Target item list.</param>
	/// <param name="strikeCallback">Strike callback.</param>
	public static void Trigger(int triggerType, int step, PvpFightUnit sourceItem, PvpFightUnit targetItem, List<PvpFightUnit> targetItemList = null, Action<string> strikeCallback = null)
	{
		// 如果是普通攻击
		if(triggerType == SkillTriggerTypeEnum.Attack)
		{
			// 如果双方都是角色
			if(sourceItem.GetType() == typeof(PvpCharacter) && StrikeCheck(targetItemList))
			{
				// 获取已方武器类型
				HardWareData.HardWareType ht = sourceItem.PvpUserInfo.CurWeapon.CurHardWareData.Style;
				// 如果是远程武器
				if(ht == HardWareData.HardWareType.Far1 || ht == HardWareData.HardWareType.Far2)
				{
					
				}else
				{
					// 如果可以移动（没有眩晕等 Buff）
					if(targetItem.pvpPlayerBuff.CanOrNotMoveCheck())
					{
						List<PvpBuffData> strikeList = targetItem.pvpPlayerBuff.GetListByBuffType(BuffTypeEnum.Strike, targetItem.pvpPlayerBuff.fixedBuffList);
						// 如果有反击 Buff
						if(strikeList != null && strikeList.Count > 0)
						{
							// 反击处理
							TriggerBuffItemStrike(strikeList, step, sourceItem, targetItem, targetItem, strikeCallback);
							return;
						}
					}
				}
			}
			if(strikeCallback != null) strikeCallback("");
		}

		List<BaseSkillItem> resultList = GetSkillItemListByTriggerType (sourceItem.pvpPlayerSkill.skillList, triggerType);
		// 如果有相关的触发技能
		if(resultList != null && resultList.Count > 0)
		{
			foreach(BaseSkillItem skillItem in resultList)
			{
				// 处理单个技能
				TriggerSkillItem(GetSkillItem(skillItem.configData), step, sourceItem, targetItem, targetItemList);
			}
		}
    }

	private static bool StrikeCheck(List<PvpFightUnit> targetItemList)
	{
		if(targetItemList == null || targetItemList.Count == 0) return false;

		foreach(PvpFightUnit pvpFightUnit in targetItemList)
		{
			if(pvpFightUnit.GetType() == typeof(PvpCharacter)) return true;
		}

		return false;
	}

	/// <summary>
	/// 普通攻击召唤怪物触发，特殊、单独处理
	/// </summary>
	/// <param name="step">Step.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="position">Position.</param>
	public static void Trigger(int step, PvpFightUnit sourceItem, PvpFightUnit targetItem, Vector2 position)
	{
		// 获取天赋召唤技能
		List<BaseSkillItem> skillItemList = sourceItem.pvpPlayerSkill.GetSkillDataBySkillTypeAndOddsType (SkillTypeEnum.Talent, SkillOddsTypeEnum.Summon);
		// 如果存在天赋召唤技能
		if(skillItemList != null && skillItemList.Count > 0)
		{
			foreach(BaseSkillItem skillItem in skillItemList)
			{
				bool randomValue = sourceItem.SummonRandom(sourceItem.GameControl.PvpRounds, step, (int)(skillItem.skillData.conditionData.conditionValue / 100f), skillItem.houseID);
				// 判断召唤条件达成情况
				if(randomValue)
				{
					List<PvpBuffData> buffList = skillItem.skillData.conditionData.buffList;
					foreach(PvpBuffData buffData in buffList)
					{
						// 如果是召唤，其他效果，触发时才处理
						if(buffData.buffType == BuffTypeEnum.Summon)
						{
							// 如果没有召唤出来怪物
							if(SkillManager.CommonMonsterList.IndexOf(buffData.valueString) == -1)
							{
								// 召唤出来
								SkillManager.CommonMonsterList.Add(buffData.valueString);
								// 处理召唤出来的怪物
								Vector3 areaPosition = sourceItem.AreaRandom (sourceItem.GameControl.PvpRounds, position);
								sourceItem.GameControl.CreateMonster (skillItem, areaPosition, buffData.valueString, sourceItem, buffData.roundValue, true);
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// 战斗阶段触发，回合开始前触发
	/// </summary>
	/// <param name="stageType">Stage type.</param>
	/// <param name="pvpFightUnit">Pvp fight unit.</param>
	public static void Trigger(int stageType, PvpFightUnit pvpFightUnit)
	{
		List<PvpBuffData> buffList = null;
		// 如果是回合开始阶段
		if(stageType == BuffStageTypeEnum.Round_Begin)
		{
			buffList = pvpFightUnit.pvpPlayerBuff.roundBeginBuffList;
		}
		if(buffList == null || buffList.Count == 0) return;

		foreach(PvpBuffData buffData in buffList)
		{
			// 如果需要表现效果
			if(!string.IsNullOrEmpty(buffData.skillID))
			{
				BaseSkillItem skillItem = pvpFightUnit.pvpPlayerSkill.GetSkillItemBySkillID(buffData.skillID);
				// 触发 Buff
				TriggerBuffItem(skillItem, buffData, pvpFightUnit, null, true);
				// 触发表现
				SkillEffectManager.Trigger(new SkillEffectParam(buffData, skillItem), buffData.skillID, pvpFightUnit, null, null, null, (r)=>
				{
					if(SkillManager.DelayEffectDict.ContainsKey(r.skillItem.skillData.skillID))
					{
						List<BaseSkillEffect> skillEffectList = SkillManager.DelayEffectDict[r.skillItem.skillData.skillID];
						if(skillEffectList != null && skillEffectList.Count > 0)
						{
							// 遍历表现效果
							while(skillEffectList.Count > 0)
							{
								BaseSkillEffect skillEffect = skillEffectList[0];
								skillEffect.Run();
								skillEffectList.RemoveAt(0);
							}
						}
						// 移除数据
						SkillManager.DelayEffectDict.Remove(r.skillItem.skillData.skillID);
					}
				});
			}else
			{
				// 触发 Buff
				TriggerBuffItem(null, buffData, pvpFightUnit, null, false);
			}
		}
	}

	/// <summary>
	/// 技能表现
	/// </summary>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="step">Step.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="targetItemList">Target item list.</param>
	public static void TriggerSkillItem(BaseSkillItem skillItem, int step, PvpFightUnit sourceItem, PvpFightUnit targetItem, List<PvpFightUnit> targetItemList = null, Action callback = null)
	{
		bool triggerBool = true;
		// 判断触发条件
		if(skillItem.skillData.conditionData.conditionType != ConditionTypeEnum.Default)
		{
			switch(skillItem.skillData.conditionData.conditionType)
			{
				case ConditionTypeEnum.Odds : // 几率触发
						if(!ConditionOddsCheck(skillItem, sourceItem, step)) triggerBool = false ;
					;break;
				case ConditionTypeEnum.Element : // 元素触发
						if(!ConditionElementCheck(sourceItem, skillItem.skillData)) triggerBool = false ; 
					;break;
			}
		}
		// 如果达成触发条件
		if(triggerBool)
		{
			/*
			#if UNITY_STANDALONE_WIN
				StringBuilder stringBuilder = new StringBuilder ();
				stringBuilder.Append ("---> 触发技能信息\n");
				stringBuilder.Append ("技能名称：");
				stringBuilder.Append (skillItem.configData.Name);

				if(sourceItem != null && sourceItem.PvpUserInfo != null)
				{
					stringBuilder.Append ("，攻击方 ID：");
					stringBuilder.Append (sourceItem.PvpUserInfo.UserId);
					stringBuilder.Append ("，攻击方 X 位置：");
					stringBuilder.Append (sourceItem.XPosition);
					stringBuilder.Append ("，攻击方 Y 位置：");
					stringBuilder.Append (sourceItem.YPosition);
				}
				if(targetItem != null && sourceItem.PvpUserInfo != null)
				{
					stringBuilder.Append ("，防御方 ID：");
					stringBuilder.Append (targetItem.PvpUserInfo.UserId);
					stringBuilder.Append ("，防御方 X 位置：");
					stringBuilder.Append (targetItem.XPosition);
					stringBuilder.Append ("，防御方 Y 位置：");
					stringBuilder.Append (targetItem.YPosition);
				}
				if(targetItemList != null && targetItemList.Count > 0)
				{
					foreach(PvpFightUnit logFightUnit in targetItemList)
					{
						if(logFightUnit != null && logFightUnit.PvpUserInfo != null)
						{
							stringBuilder.Append ("，防御方 ID：");
							stringBuilder.Append (logFightUnit.PvpUserInfo.UserId);
							stringBuilder.Append ("，防御方 X 位置：");
							stringBuilder.Append (logFightUnit.XPosition);
							stringBuilder.Append ("，防御方 Y 位置：");
							stringBuilder.Append (logFightUnit.YPosition);
						}
					}
				}
				stringBuilder.Append ("\n");
				PvpLogManager.Log (sourceItem.GameControl.PvpTableID, stringBuilder.ToString ());
			#endif
			*/
			// 如果是召唤效果，并且是法术技能
			if(skillItem.BuffTypeCheck(BuffTypeEnum.Summon) && skillItem.IsMagic)
			{
				List<PvpBuffData> buffList = skillItem.skillData.conditionData.buffList;
				// 遍历 Buff
				foreach(PvpBuffData buffData in buffList)
				{
					if(targetItemList == null || targetItemList.Count == 0)
					{
						// 处理 Buff 效果
						TriggerBuffItem(skillItem, buffData, sourceItem, targetItem, false);
					}else
					{
						foreach(PvpFightUnit targetFightUnit in targetItemList)
						{
							// 处理 Buff 效果
							TriggerBuffItem(skillItem, buffData, sourceItem, targetFightUnit, false);
						}
					}
				}
				// 执行技能，设置回调
				SkillManager.RepeatMagicCount --;
				if(callback != null) callback();
			}else
			{
				// 如果是普通攻击、或者行走，则先把 buff 加到上面，然后再执行 buff 效果
				if(skillItem.skillData.triggerType == SkillTriggerTypeEnum.Attack || skillItem.skillData.triggerType == SkillTriggerTypeEnum.Walk)
				{
					// 触发攻击回调
					List<PvpBuffData> buffList = skillItem.skillData.conditionData.buffList;
					foreach(PvpBuffData buffData in buffList)
					{
						// 如果不是召唤效果，普通攻击的召唤效果不触发
						if(buffData.buffType != BuffTypeEnum.Summon)
						{
							if(targetItemList == null || targetItemList.Count == 0)
							{
								// 处理 Buff 效果
								TriggerBuffItem(skillItem, buffData, sourceItem, targetItem, true);
							}else{
								foreach(PvpFightUnit targetFightUnit in targetItemList)
								{
									// 处理 Buff 效果
									TriggerBuffItem(skillItem, buffData, sourceItem, targetFightUnit, true);
								}
							}
						}
					}
					// 后执行表现
					SkillEffectManager.Trigger (new SkillEffectParam(null, skillItem), skillItem.skillData.skillID, sourceItem, targetItem, targetItemList, null, (r) =>
					{
						if(SkillManager.DelayEffectDict.ContainsKey(r.skillItem.skillData.skillID))
						{
							List<BaseSkillEffect> skillEffectList = SkillManager.DelayEffectDict[r.skillItem.skillData.skillID];
							if(skillEffectList != null && skillEffectList.Count > 0)
							{
								// 遍历表现效果
								while(skillEffectList.Count > 0)
								{
									BaseSkillEffect skillEffect = skillEffectList[0];
									skillEffect.Run();
									skillEffectList.RemoveAt(0);
								}
							}
							// 移除数据
							SkillManager.DelayEffectDict.Remove(r.skillItem.skillData.skillID);
						}
					});
				}else
				{ // 如果是法术
					// 触发效果
					SkillEffectManager.Trigger (new SkillEffectParam(null, skillItem, sourceItem, targetItem, targetItemList), skillItem.skillData.skillID, sourceItem, targetItem, targetItemList, (r) =>
					{
						// 如果是法术，才执行回调，
						if(r.skillItem.IsMagic)
						{
							// 执行技能，设置回调
							SkillManager.RepeatMagicCount --;
							if(callback != null) callback();
						}
					}, (r) => 
					{
						// 触发攻击回调
						List<PvpBuffData> buffList = r.skillItem.skillData.conditionData.buffList;
						foreach(PvpBuffData buffData in buffList)
						{
							if(r.targetItemList == null || r.targetItemList.Count == 0)
							{
								// 处理 Buff 效果
								TriggerBuffItem(r.skillItem, buffData, r.sourceItem, r.targetItem, false);
							}else{
								foreach(PvpFightUnit targetFightUnit in r.targetItemList)
								{
									// 处理 Buff 效果
									TriggerBuffItem(r.skillItem, buffData, r.sourceItem, targetFightUnit, false);
								}
							}
						}
					});
				}
			}
		}
	}

	/// <summary>
	/// 触发 Buff 效果
	/// </summary>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="buffData">Buff data.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="delayStatus">If set to <c>true</c> delay status.</param>
	private static void TriggerBuffItem(BaseSkillItem skillItem, PvpBuffData buffData, PvpFightUnit sourceItem, PvpFightUnit targetItem, bool delayStatus = true)
	{
		PvpFightUnit buffItem = null;
		if(buffData.targetType == BuffTargetTypeEnum.ENEMY)
		{
			buffItem = targetItem;
		}else{
			buffItem = sourceItem;
		}
		// 如果回合数为零（主动触发）
		if(buffData.roundValue == 0 || buffData.buffType == BuffTypeEnum.Trigger_Skill || buffData.buffType == BuffTypeEnum.Summon) //这儿处理立刻触发的 Buff
		{
			switch(buffData.buffType)
			{
				case BuffTypeEnum.Attack :
						TriggerBuffItemAttack(buffData, sourceItem, targetItem, buffItem, skillItem, delayStatus);
					; break;
				case BuffTypeEnum.Recover_Hp :
						TriggerBuffItemRecoverHp(buffData, sourceItem, targetItem, buffItem, skillItem, delayStatus);
					; break;
				case BuffTypeEnum.Exchange_Hp :
						TriggerBuffItemExchangeHp(buffData, sourceItem, targetItem, buffItem, skillItem, delayStatus);
					; break;
				case BuffTypeEnum.Exchange_Attack_Hp :
						TriggerBuffItemExchangeAttackHp(buffData, sourceItem, targetItem, buffItem, skillItem, delayStatus);
					; break;
				case BuffTypeEnum.Summon :
						TriggerBuffItemSummon(buffData, sourceItem, targetItem, buffItem, skillItem, delayStatus);
					; break;
				case BuffTypeEnum.Trigger_Skill :
						TriggerBuffItemTriggerSkill(buffData, sourceItem, targetItem, buffItem, skillItem, delayStatus);
					; break;
				case BuffTypeEnum.Transmit :
						TriggerBuffItemTransmit(buffData, sourceItem, targetItem, buffItem, skillItem, delayStatus);
					; break;
				default :
					; break;
			}
		}else
		{
			if(skillItem != null)
			{
				if(buffData.stageType == BuffStageTypeEnum.Attack)
				{
					// 如果是眩晕或者阻止眩晕
					if(buffData.buffType == BuffTypeEnum.Dizziness)
					{
						// 如果都是角色
						if(sourceItem.GetType() == typeof(PvpCharacter) && targetItem.GetType() == typeof(PvpCharacter))
						{
							// 如果自己身上没有阻止眩晕的 buff，给目标添加眩晕效果
							List<PvpBuffData> stopDizzinessList = sourceItem.pvpPlayerBuff.GetListByBuffType(BuffTypeEnum.Stop_Dizziness, sourceItem.pvpPlayerBuff.roundBuffList);
							if(stopDizzinessList.Count == 0)
							{
								// 这儿添加条件达成几率
								if(sourceItem.DizzinessRandom(sourceItem.GameControl.PvpRounds, (int)buffData.valueFloat))
								{
									// 如果是眩晕
									if(buffData.buffType == BuffTypeEnum.Dizziness)
									{
										// 添加眩晕 BUFF
										buffItem.pvpPlayerBuff.InsertBuff(BuffStageTypeEnum.Attack, skillItem.skillData.skillID, buffData);

										// 获取阻止眩晕 Buff 列表
										List<PvpBuffData> skillStopDizzinessList = skillItem.GetBuffList(BuffTypeEnum.Stop_Dizziness);
										if(skillStopDizzinessList != null && skillStopDizzinessList.Count > 0)
										{
											// 遍历列表，
											foreach(PvpBuffData skillBuffData in skillStopDizzinessList)
											{
												sourceItem.pvpPlayerBuff.InsertBuff(BuffStageTypeEnum.Attack, skillItem.skillData.skillID, skillBuffData);
											}
										}
										// 调用眩晕表现
										TriggerBuffItemDizziness(buffData, sourceItem, targetItem, buffItem, skillItem);
									}
								}
							}
						}
					}else
					{
						buffItem.pvpPlayerBuff.InsertBuff(BuffStageTypeEnum.Attack, skillItem.skillData.skillID, buffData);
					}
				}else if(buffData.stageType == BuffStageTypeEnum.Walk)
				{
					buffItem.pvpPlayerBuff.InsertBuff(BuffStageTypeEnum.Walk, skillItem.skillData.skillID, buffData);
				}
			}
		}
	}

	/// <summary>
	/// 眩晕效果
	/// </summary>
	/// <param name="buffData">Buff data.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="buffItem">Buff item.</param>
	/// <param name="skillItem">Skill item.</param>
	private static void TriggerBuffItemDizziness(PvpBuffData buffData, PvpFightUnit sourceItem, PvpFightUnit targetItem, PvpFightUnit buffItem, BaseSkillItem skillItem)
	{
		if(buffItem != null) buffItem.ChangeDizziness();
	}

	/// <summary>
	/// 伤害 hp 效果
	/// </summary>
	/// <param name="buffData">Buff data.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="buffItem">Buff item.</param>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="delayStatus">If set to <c>true</c> delay status.</param>
	private static void TriggerBuffItemAttack(PvpBuffData buffData, PvpFightUnit sourceItem, PvpFightUnit targetItem, PvpFightUnit buffItem, BaseSkillItem skillItem, bool delayStatus)
	{
		float hurtValue = buffData.valueFloat;

		if(buffData.effectType != BuffEffectTypeEnum.DEFAULT)
		{
			int fp = buffItem.ActP(sourceItem);
			int sp = buffItem.StP();
			hurtValue *= buffItem.GetElementParam(fp, sp);
		}
		hurtValue -= buffItem.Def;

		if(hurtValue <= 0) hurtValue = 1;

		Debug.Log ("技能伤害数据为 ：：：：" + hurtValue + ":" + delayStatus);

		if(!delayStatus)
		{
			if(buffItem != null) buffItem.ChangeHp (-hurtValue, true, skillItem.IsMagic);
		}else
		{
			if(buffItem != null) buffItem.ChangeHp (-hurtValue, false, skillItem.IsMagic);
			// 添加延迟展示
			SkillManager.DelayInsert(skillItem.skillData.skillID, new SkillEffectAttack(buffData, buffItem, skillItem, -hurtValue));
		}
	}

	/// <summary>
	/// 恢复 hp 效果
	/// </summary>
	/// <param name="buffData">Buff data.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="buffItem">Buff item.</param>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="delayStatus">If set to <c>true</c> delay status.</param>
	private static void TriggerBuffItemRecoverHp(PvpBuffData buffData, PvpFightUnit sourceItem, PvpFightUnit targetItem, PvpFightUnit buffItem, BaseSkillItem skillItem, bool delayStatus)
	{
		if(buffItem != null)
		{
			if(!delayStatus)
			{
				buffItem.ChangeHp (buffData.valueFloat);
			}else
			{
				buffItem.ChangeHp (buffData.valueFloat, false);
				// 添加延迟展示
				SkillManager.DelayInsert(skillItem.skillData.skillID, new SkillEffectRecoverHp(buffData, buffItem, skillItem, buffData.valueFloat));
			}
		}
	}

	/// <summary>
	/// 换 hp 效果
	/// </summary>
	/// <param name="buffData">Buff data.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="buffItem">Buff item.</param>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="delayStatus">If set to <c>true</c> delay status.</param>
	private static void TriggerBuffItemExchangeHp(PvpBuffData buffData, PvpFightUnit sourceItem, PvpFightUnit targetItem, PvpFightUnit buffItem, BaseSkillItem skillItem, bool delayStatus)
	{
		if(!delayStatus)
		{
			sourceItem.TurnHp (targetItem.CurHp, true, skillItem.IsMagic);
		}else
		{
			sourceItem.TurnHp (targetItem.CurHp, false, skillItem.IsMagic);
			// 添加延迟展示
			SkillManager.DelayInsert(skillItem.skillData.skillID, new SkillEffectExchangeHp(buffData, sourceItem, skillItem, targetItem.CurHp));
		}
	}

	/// <summary>
	/// 攻击转 hp 效果
	/// </summary>
	/// <param name="buffData">Buff data.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="buffItem">Buff item.</param>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="delayStatus">If set to <c>true</c> delay status.</param>
	private static void TriggerBuffItemExchangeAttackHp(PvpBuffData buffData, PvpFightUnit sourceItem, PvpFightUnit targetItem, PvpFightUnit buffItem, BaseSkillItem skillItem, bool delayStatus)
	{
		bool critBool = false;
		bool missBool = false;
		float attak = targetItem.BeHurtValue(buffItem, -1, ref critBool, ref missBool) * (buffData.valueFloat / PvpUserInfo.ODDS_VALUE);

		if(!delayStatus)
		{
			buffItem.ChangeHp (attak);
		}else
		{
			buffItem.ChangeHp (attak, false);
			// 添加延迟展示
			SkillManager.DelayInsert(skillItem.skillData.skillID, new SkillEffectExchangeAttackHp(buffData, buffItem, skillItem, attak));
		}
	}

	/// <summary>
	/// 召唤效果
	/// </summary>
	/// <param name="buffData">Buff data.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="buffItem">Buff item.</param>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="delayStatus">If set to <c>true</c> delay status.</param>
	private static void TriggerBuffItemSummon(PvpBuffData buffData, PvpFightUnit sourceItem, PvpFightUnit targetItem, PvpFightUnit buffItem, BaseSkillItem skillItem, bool delayStatus)
	{
		// 如果是法术触发
		if(skillItem.skillData.triggerType == SkillTriggerTypeEnum.Skill)
		{
			Vector3 position = sourceItem.AreaRandom (sourceItem.GameControl.PvpRounds, new Vector2(sourceItem.XPosition, sourceItem.YPosition));
			// 创建怪物
			buffItem.GameControl.CreateMonster (skillItem, position, buffData.valueString, buffItem, buffData.roundValue, false);
		}
	}

	/// <summary>
	/// 反击效果
	/// </summary>
	/// <param name="buffList">Buff list.</param>
	/// <param name="step">Step.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="buffItem">Buff item.</param>
	/// <param name="strikeCallback">Strike callback.</param>
	private static void TriggerBuffItemStrike(List<PvpBuffData> buffList, int step, PvpFightUnit sourceItem, PvpFightUnit targetItem, PvpFightUnit buffItem, Action<string> strikeCallback = null)
	{
		if(sourceItem.StrickeStatus)
		{
			if(strikeCallback != null) strikeCallback("");
			return;
		}

		PvpBuffData buffData = buffList [0];
		if(targetItem.StrickeRandom(targetItem.GameControl.PvpRounds, step, (int)buffData.valueFloat))
		{
			((PvpCharacter)buffItem).StrickeAttack (sourceItem, strikeCallback);
			return;
		}

		if(strikeCallback != null) strikeCallback("");
	}

	/// <summary>
	/// 多次施法效果
	/// </summary>
	/// <param name="buffData">Buff data.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="buffItem">Buff item.</param>
	/// <param name="delayStatus">If set to <c>true</c> delay status.</param>
	private static void TriggerBuffItemTriggerSkill(PvpBuffData buffData, PvpFightUnit sourceItem, PvpFightUnit targetItem, PvpFightUnit buffItem, BaseSkillItem skillItem, bool delayStatus)
	{
		if(skillItem == null) return;

		SkillData skillData = buffItem.pvpPlayerSkill.GetSkillDataBySkillID(skillItem.configData.Id);
		if(skillData != null)
		{
			for(int index = 0; index < buffData.roundValue; index ++)
			{
				// 设置技能数量
				SkillManager.RepeatMagicCount ++;
				// 使用技能
				sourceItem.GameControl.UseSkill(skillData, sourceItem.GameControl.skillSelfStatus, false);
			}
		}
	}

	/// <summary>
	/// 传送效果
	/// </summary>
	/// <param name="buffData">Buff data.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="buffItem">Buff item.</param>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="delayStatus">If set to <c>true</c> delay status.</param>
	private static void TriggerBuffItemTransmit(PvpBuffData buffData, PvpFightUnit sourceItem, PvpFightUnit targetItem, PvpFightUnit buffItem, BaseSkillItem skillItem, bool delayStatus)
	{
		if(skillItem == null) return;
		
		Vector3 randomPosition = sourceItem.PositionRandom (sourceItem.GameControl.PvpRounds);

		//Debug.Log ("随机传送位置：" + randomPosition.x + ":" + randomPosition.y);

		sourceItem.GameControl.UseSkillTransmit (skillItem, sourceItem, sourceItem.XPosition, sourceItem.YPosition, (int)randomPosition.x, (int)randomPosition.y);
	}

	/// <summary>
	/// 条件几率检查
	/// </summary>
	/// <returns><c>true</c>, if odds check was conditioned, <c>false</c> otherwise.</returns>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="step">Step.</param>
	private static bool ConditionOddsCheck(BaseSkillItem skillItem, PvpFightUnit sourceItem, int step)
	{
		switch(skillItem.skillData.oddsType)
		{
			case SkillOddsTypeEnum.Attack :
					return sourceItem.AttackRandom(sourceItem.GameControl.PvpRounds, step, (int)(skillItem.skillData.conditionData.conditionValue / 100f));
				; break;
			case SkillOddsTypeEnum.Attack_2 :
					return sourceItem.AttackRandom2(sourceItem.GameControl.PvpRounds, step, (int)(skillItem.skillData.conditionData.conditionValue / 100f));
				; break;
			case SkillOddsTypeEnum.Skill :
					return sourceItem.SkillRandom(sourceItem.GameControl.PvpRounds, (int)(skillItem.skillData.conditionData.conditionValue / 100f));
				; break;
		}
		return false;
	}

	/// <summary>
	/// 条件元素检查
	/// </summary>
	/// <returns><c>true</c>, if element check was conditioned, <c>false</c> otherwise.</returns>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="skillData">Skill data.</param>
	private static bool ConditionElementCheck(PvpFightUnit sourceItem, PvpSkillData skillData)
	{
		if((int)skillData.conditionData.conditionValue == (int)sourceItem.GameControl.currentEliminateAttribute) return true;
		return false;
	}

	/// <summary>
	/// 条件几率触发检查
	/// </summary>
	/// <returns><c>true</c>, if odds trigger check was conditioned, <c>false</c> otherwise.</returns>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="buffData">Buff data.</param>
	private static bool ConditionOddsTriggerCheck(PvpFightUnit sourceItem, PvpBuffData buffData)
	{
		bool triggerOdds = sourceItem.SkillRandom(sourceItem.GameControl.PvpRounds, (int)(buffData.valueFloat / 100f));

		Debug.Log ("多重施法效果触发结果：" + triggerOdds);

		return triggerOdds;
	}

	/// <summary>
	/// 每回合开始
	/// </summary>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	public static void ExecuteRoundBegin(PvpFightUnit sourceItem, PvpFightUnit targetItem)
	{
		sourceItem.pvpPlayerBuff.RoundDelay ();
		targetItem.pvpPlayerBuff.RoundDelay ();
	}

	/// <summary>
	/// 回合开始、轮到自己
	/// </summary>
	/// <param name="pvpFightUnit">Pvp fight unit.</param>
	public static void ExecuteRoundBegin(PvpFightUnit pvpFightUnit)
	{
		// 更新 buff 回合
		pvpFightUnit.pvpPlayerBuff.RoundExecute ();
		// 清空行走触发的 buff
		pvpFightUnit.pvpPlayerBuff.WalkExecute();

		// 更新技能 cd
		pvpFightUnit.pvpPlayerSkill.RoundExecute ();

		// 执行回合前 buff
		Trigger (BuffStageTypeEnum.Round_Begin, pvpFightUnit);
	}

	/// <summary>
	/// 技能初始化
	/// </summary>
	/// <param name="pvpUserInfo">Pvp user info.</param>
	/// <param name="dataList">Data list.</param>
	/// <param name="dataCdList">Data cd list.</param>
	public static void SkillInit(PvpUserInfo pvpUserInfo, List<PvpSkillHouseData> dataList, List<PvpSkillCdData> dataCdList)
	{
		pvpUserInfo.pvpPlayerSkill = new PvpPlayerSkill ();
		pvpUserInfo.pvpPlayerSkill.skillCdList = dataCdList;
		pvpUserInfo.pvpPlayerSkill.skillList = GetSkillItemList(dataList);
	}

	/// <summary>
	/// 效果初始化
	/// </summary>
	/// <param name="pvpUserInfo">Pvp user info.</param>
	public static void BuffListInit(PvpUserInfo pvpUserInfo)
	{
		pvpUserInfo.pvpPlayerBuff = new PvpPlayerBuff ();

		pvpUserInfo.pvpPlayerBuff.fixedBuffList = GetBuffDataListByStageType (pvpUserInfo.pvpPlayerSkill.skillList, BuffStageTypeEnum.Fixed);
		pvpUserInfo.pvpPlayerBuff.roundBeginBuffList = GetBuffDataListByStageType (pvpUserInfo.pvpPlayerSkill.skillList, BuffStageTypeEnum.Round_Begin);
		pvpUserInfo.pvpPlayerBuff.roundBuffList = new List<PvpBuffData>();
		pvpUserInfo.pvpPlayerBuff.walkBuffList = new List<PvpBuffData>();
		pvpUserInfo.pvpPlayerBuff.petBuffList = GetBuffDataListByStageType (pvpUserInfo.pvpPlayerSkill.skillList, BuffStageTypeEnum.Fixed_Pet);
		pvpUserInfo.pvpPlayerBuff.triggerSkillBuffList = GetBuffDataListByStageType (pvpUserInfo.pvpPlayerSkill.skillList, BuffStageTypeEnum.Trigger_Skill);
	}

	/// <summary>
	/// 获取技能列表
	/// </summary>
	/// <returns>The skill item list.</returns>
	/// <param name="dataList">Data list.</param>
	private static List<BaseSkillItem> GetSkillItemList(List<PvpSkillHouseData> dataList)
	{
		List<BaseSkillItem> resultList = new List<BaseSkillItem> ();

		BaseSkillItem skillItem = null;
		foreach(PvpSkillHouseData pvpSkillHouseData in dataList)
		{
			if(!ExistsSkill(pvpSkillHouseData.skillData.SkillFX, resultList))
			{
				skillItem = GetSkillItem(pvpSkillHouseData.skillData);
				if(skillItem != null)
				{
					// 设置 houseID
					skillItem.houseID = pvpSkillHouseData.houseID;
					resultList.Add(skillItem);
				}
			}
		}
		return resultList;
	}

	/// <summary>
	/// 技能存在检查
	/// </summary>
	/// <returns><c>true</c>, if skill was existsed, <c>false</c> otherwise.</returns>
	/// <param name="skillID">Skill I.</param>
	/// <param name="skillList">Skill list.</param>
	private static bool ExistsSkill(string skillID, List<BaseSkillItem> skillList)
	{
		foreach(BaseSkillItem skillItem in skillList)
		{
			if(skillItem.skillData.skillID == skillID) return true;
		}
		return false;
	}

	/// <summary>
	/// 根据触发类别检查技能
	/// </summary>
	/// <returns>The skill item list by trigger type.</returns>
	/// <param name="dataList">Data list.</param>
	/// <param name="triggerType">Trigger type.</param>
	private static List<BaseSkillItem> GetSkillItemListByTriggerType(List<BaseSkillItem> dataList, int triggerType)
	{
		List<BaseSkillItem> resultList = new List<BaseSkillItem> ();

		foreach(BaseSkillItem skillItem in dataList)
		{
			if(skillItem.skillData.triggerType == triggerType) resultList.Add(skillItem);
		}

		return resultList;
	}

	/// <summary>
	/// 根据触发阶段查找效果
	/// </summary>
	/// <returns>The buff data list by stage type.</returns>
	/// <param name="dataList">Data list.</param>
	/// <param name="stageType">Stage type.</param>
	private static List<PvpBuffData> GetBuffDataListByStageType(List<BaseSkillItem> dataList, int stageType)
	{
		List<PvpBuffData> resultList = new List<PvpBuffData> ();

		List<PvpBuffData> buffList = null;
		foreach(BaseSkillItem skillItem in dataList)
		{
			buffList = GetBuffDataListByItemAndStageType(skillItem, stageType);
			if(buffList != null && buffList.Count > 0)
			{
				resultList.InsertRange(resultList.Count, buffList);
			}
		}

		return resultList;
	}

	/// <summary>
	/// 根据触发阶段和技能查找效果
	/// </summary>
	/// <returns>The buff data list by item and stage type.</returns>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="stageType">Stage type.</param>
	private static List<PvpBuffData> GetBuffDataListByItemAndStageType(BaseSkillItem skillItem, int stageType)
	{
		List<PvpBuffData> resultList = new List<PvpBuffData> ();

		List<PvpBuffData> buffList = skillItem.skillData.conditionData.buffList;
		foreach(PvpBuffData buffData in buffList)
		{
			if(buffData.stageType == stageType)
			{
				buffData.skillID = skillItem.skillData.skillID;
				resultList.Add(buffData);
			}
		}

		return resultList;
	}

	/// <summary>
	/// 获取技能
	/// </summary>
	/// <returns>The skill item.</returns>
	/// <param name="skillData">Skill data.</param>
	public static BaseSkillItem GetSkillItem(SkillData skillData)
	{
		BaseSkillItem skillItem = null;
		switch(skillData.SkillFX)
		{
			case "SkFX20" : skillItem = new SkillItemFx20(); break;
			case "SkFX21" : skillItem = new SkillItemFx21(); break;
			case "SkFX22" : skillItem = new SkillItemFx22(); break;
			case "SkFX23" : skillItem = new SkillItemFx23(); break;
			case "SkFX24" : skillItem = new SkillItemFx24(); break;
			case "SkFX25" : skillItem = new SkillItemFx25(); break;
			case "SkFX26" : skillItem = new SkillItemFx26(); break;
			case "SkFX27" : skillItem = new SkillItemFx27(); break;
			case "SkFX28" : skillItem = new SkillItemFx28(); break;
			case "SkFX29" : skillItem = new SkillItemFx29(); break;
			case "SkFX31" : skillItem = new SkillItemFx31(); break;
			case "SkFX32" : skillItem = new SkillItemFx32(); break;
			case "SkFX33" : skillItem = new SkillItemFx33(); break;
			case "SkFX34" : skillItem = new SkillItemFx34(); break;
			case "SkFX35" : skillItem = new SkillItemFx35(); break;
			case "SkFX38" : skillItem = new SkillItemFx38(); break;
			case "SkFX39" : skillItem = new SkillItemFx39(); break;
			case "SkFX42" : skillItem = new SkillItemFx42(); break;
			case "SkFX41" : skillItem = new SkillItemFx41(); break;
		}

		if(skillItem != null)
		{
			skillItem.AnalysisSkill(skillData);
		}

		return skillItem;
	}
}
