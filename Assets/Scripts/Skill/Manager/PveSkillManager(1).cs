using UnityEngine;
using System;
using System.Collections.Generic;

public class PveSkillManager
{
    /// <summary>
    /// 技能触发，不需要打击目标
    /// </summary>
    /// <param name="skillData">Skill data.</param>
    /// <param name="step">Step.</param>
    /// <param name="sourceItem">Source item.</param>
    /// <param name="cd">If set to <c>true</c> cd.</param>
    public static void Trigger(SkillData skillData, PveFightUnit sourceItem, bool cd = true, Action callback = null)
	{
        // 如果需要检测 CD
        if (cd)
        {
            // 设置技能数量
            SkillManager.RepeatMagicCount++;
        }

		List<PveFightUnit> resultList = sourceItem.GameControl.GetSkillFightUnitTarget (skillData, sourceItem);

        BaseSkillItem skillItem = sourceItem.pvpPlayerSkill.GetSkillItemBySkillID(skillData.Id);
        if (skillItem == null) return;
        if (resultList == null || resultList.Count == 0)
        {
            sourceItem.pvpPlayerSkill.RefreshCd (skillData.Id, sourceItem.GameControl.GetHouseID());
            PveSkillEffectManager.Trigger(skillItem, sourceItem,  null, () =>
            {
                // 如果需要检测 CD
				if(cd)
				{
					// 设置技能数量
					SkillManager.RepeatMagicCount --;
				}
                if (callback != null) callback();
            }, null);
            return;
        }


        Debug.Log("----- "+resultList.Count);
        List<PveEnemyUnit> resultListpve = new List<PveEnemyUnit>();
        foreach (PveFightUnit pe in resultList)
        {
            Debug.Log("----- " + pe.name);
            if (pe.name.IndexOf("Character") == -1)
            {
                PveEnemyUnit re = (PveEnemyUnit)pe;
                resultListpve.Add(re);
            }           
        }

		if(skillItem.skillData.skillTarget != SkillTargetTypeEnum.Range)
		{
            Trigger(skillData, sourceItem, null, cd, callback);
		}else{                                  
            Trigger(skillData, sourceItem, resultListpve, cd, callback);
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
    public static void Trigger(SkillData skillData, PveFightUnit sourceItem, List<PveEnemyUnit> targetItemList = null, bool cd = true, Action callback = null)
	{

        sourceItem.pvpPlayerSkill.RefreshCd(skillData.Id, sourceItem.GameControl.GetHouseID());
		//sourceItem.pvpPlayerSkill.RefreshCd (skillData.Id, 0);
        TriggerSkillItem(GetSkillItem(skillData), sourceItem, targetItemList, callback);

		if(cd)
		{
			List<PvpBuffData> triggerBuffList = sourceItem.pvpPlayerBuff.TriggerSkillCheck (skillData.Id);
			if(triggerBuffList != null && triggerBuffList.Count > 0)
			{
				foreach(PvpBuffData buffData in triggerBuffList)
				{
                    foreach (PveEnemyUnit targetItem in targetItemList)
                    {
                        if(ConditionOddsTriggerCheck(sourceItem, buffData)) TriggerBuffItem(null, buffData, sourceItem, targetItem);
                    }					
				}
			}
		}
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
    public static void Trigger(int triggerType, PveFightUnit sourceItem, List<PveEnemyUnit> targetItemList = null, Action<string> strikeCallback = null)
	{
		List<BaseSkillItem> resultList = GetSkillItemListByTriggerType (sourceItem.pvpPlayerSkill.skillList, triggerType);
		if(resultList != null && resultList.Count > 0)
		{
			foreach(BaseSkillItem skillItem in resultList)
			{
				TriggerSkillItem(GetSkillItem(skillItem.configData), sourceItem, targetItemList);
			}
		}

		if(triggerType == SkillTriggerTypeEnum.Attack )
		{
            if (targetItemList != null)
            {
                foreach (PveFightUnit targetItem in targetItemList)
                {
                    //strike 反击
                    //-- pve  怪物无pvpplayerbuff  不在此处反击

                    //List<PvpBuffData> strikeList = targetItem.pvpPlayerBuff.GetListByBuffType(BuffTypeEnum.Strike, targetItem.pvpPlayerBuff.fixedBuffList);
                    //if (strikeList.Count > 0)
                    //{
                    //    TriggerBuffItemStrike(strikeList, sourceItem, targetItem, targetItem, strikeCallback);
                    //    return;
                    //}
                }
            }
			if(strikeCallback != null) strikeCallback("");
		}
    }
    /// <summary>
    /// 普通攻击召唤怪物触发，特殊、单独处理
    /// </summary>
    /// <param name="step">Step.</param>
    /// <param name="sourceItem">Source item.</param>
    /// <param name="targetItem">Target item.</param>
    /// <param name="position">Position.</param>
	public static void Trigger(PveFightUnit sourceItem, PveFightUnit targetItem, Vector2 position)
	{
		Debug.Log ("行走的终点位置 ：：：：：：：：：：：：：：：：： " + position.x + ":" + position.y);
		List<BaseSkillItem> skillItemList = sourceItem.pvpPlayerSkill.GetSkillDataBySkillTypeAndOddsType (SkillTypeEnum.Talent, SkillOddsTypeEnum.Summon);
		if(skillItemList != null && skillItemList.Count > 0)
		{
			foreach(BaseSkillItem skillItem in skillItemList)
			{
				bool randomValue = sourceItem.SummonRandom( (int)(skillItem.skillData.conditionData.conditionValue / 100f));
				if(randomValue)
				{
					List<PvpBuffData> buffList = skillItem.skillData.conditionData.buffList;
					foreach(PvpBuffData buffData in buffList)
					{
						// 如果不是召唤
						if(buffData.buffType != BuffTypeEnum.Summon)
						{
							TriggerBuffItem(skillItem, buffData, sourceItem, targetItem);
						}else{
							Vector3 areaPosition = sourceItem.AreaRandom (position);
                            sourceItem.GameControl.CreateMonster(skillItem,areaPosition, buffData.valueString, sourceItem, buffData.roundValue, true);
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
	public static void Trigger2(int stageType, PveFightUnit PveFightUnit)
	{
		List<PvpBuffData> buffList = null;

		if(stageType == BuffStageTypeEnum.Round_Begin)
		{
			buffList = PveFightUnit.pvpPlayerBuff.roundBeginBuffList;
		}
		if(buffList == null || buffList.Count == 0) return;

		foreach(PvpBuffData buffData in buffList)
		{
			TriggerBuffItem(null, buffData, PveFightUnit, null);
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
    public static void TriggerSkillItem(BaseSkillItem skillItem, PveFightUnit sourceItem, List<PveEnemyUnit> targetItemList = null, Action callback = null)
	{
        // 触发效果
        //PveSkillEffectManager.Trigger (skillItem, sourceItem, targetItem, targetItemList);
		bool triggerBool = true;

		if(skillItem.skillData.conditionData.conditionType != ConditionTypeEnum.Default)
		{
			switch(skillItem.skillData.conditionData.conditionType)
			{
				case ConditionTypeEnum.Odds :
						if(!ConditionOddsCheck(skillItem, sourceItem)) triggerBool = false ;
					;break;
				case ConditionTypeEnum.Element :
						if(!ConditionElementCheck(sourceItem, skillItem.skillData)) triggerBool = false ; 
					;break;
			}

            if (callback != null) callback();
		}
		if(triggerBool)
		{
            // 触发效果
            PveSkillEffectManager.Trigger(skillItem, sourceItem, targetItemList, () =>
            {
                if (callback != null) callback();
            }, () =>
            {
                // 触发攻击回调
                List<PvpBuffData> buffList = skillItem.skillData.conditionData.buffList;
                if (buffList != null)
                {
                    foreach (PvpBuffData buffData in buffList)
                    {
						if(targetItemList != null)
						{
	                        foreach (PveEnemyUnit targetFightUnit in targetItemList)
	                        {
	                            TriggerBuffItem(skillItem, buffData, sourceItem, (PveFightUnit)targetFightUnit);
	                        }
						}else{
							TriggerBuffItem(skillItem, buffData, sourceItem, sourceItem);
						}
                    }
                }
                
            });            
		}
	}



	private static void TriggerBuffItem(BaseSkillItem skillItem, PvpBuffData buffData, PveFightUnit sourceItem, PveFightUnit targetItem)
	{
		PveFightUnit buffItem = null;
        //Debug.Log("----0  " + buffData.targetType);
		if(buffData.targetType == BuffTargetTypeEnum.ENEMY)
		{
			buffItem = targetItem;
		}else{
			buffItem = sourceItem;
		}
		if(buffData.roundValue == 0 || buffData.buffType == BuffTypeEnum.Trigger_Skill || buffData.buffType == BuffTypeEnum.Summon) //这儿处理立刻触发的 Buff
		{
			switch(buffData.buffType)
			{
				case BuffTypeEnum.Attack :
						TriggerBuffItemAttack(buffData, sourceItem, targetItem, buffItem);
					; break;
				case BuffTypeEnum.Recover_Hp :
						TriggerBuffItemRecoverHp(buffData, sourceItem, targetItem, buffItem);
					; break;
				case BuffTypeEnum.Exchange_Hp :
						TriggerBuffItemExchangeHp(buffData, sourceItem, targetItem, buffItem);
					; break;
				case BuffTypeEnum.Exchange_Attack_Hp :
						TriggerBuffItemExchangeAttackHp(buffData, sourceItem, targetItem, buffItem);
					; break;
				case BuffTypeEnum.Summon :
						TriggerBuffItemSummon(buffData, sourceItem, targetItem, buffItem, skillItem);
					; break;
				case BuffTypeEnum.Trigger_Skill :
						TriggerBuffItemTriggerSkill(buffData, sourceItem, targetItem, buffItem);
					; break;
                case BuffTypeEnum.Transmit:
                    TriggerBuffItemTransmit(sourceItem);
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
					if(buffData.buffType == BuffTypeEnum.Dizziness || buffData.buffType == BuffTypeEnum.Stop_Dizziness)
					{
						if(sourceItem.GetType() == typeof(PvpCharacter) && targetItem.GetType() == typeof(PvpCharacter))
						{
							if(buffData.buffType == BuffTypeEnum.Dizziness)
							{
								List<PvpBuffData> dizzinessList = sourceItem.pvpPlayerBuff.GetListByBuffType(BuffTypeEnum.Dizziness, sourceItem.pvpPlayerBuff.roundBuffList);
								if(dizzinessList.Count == 0)
								{
									buffItem.pvpPlayerBuff.InsertBuff(BuffStageTypeEnum.Attack, skillItem.skillData.skillID, buffData);
								}
							}else if(buffData.buffType == BuffTypeEnum.Stop_Dizziness)
							{
								List<PvpBuffData> stopDizzinessList = sourceItem.pvpPlayerBuff.GetListByBuffType(BuffTypeEnum.Stop_Dizziness, sourceItem.pvpPlayerBuff.roundBuffList);
								if(stopDizzinessList.Count == 0)
								{
									sourceItem.pvpPlayerBuff.InsertBuff(BuffStageTypeEnum.Attack, skillItem.skillData.skillID, buffData);
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

	private static void TriggerBuffItemAttack(PvpBuffData buffData, PveFightUnit sourceItem, PveFightUnit targetItem, PveFightUnit buffItem)
	{
		float hurtValue = buffData.valueFloat;

        

		if(buffData.effectType != BuffEffectTypeEnum.DEFAULT)
		{
			int fp = buffItem.ActP(sourceItem);
			int sp = buffItem.StP();
			hurtValue *= buffItem.GetElementParam(fp, sp);
		}
		hurtValue -= buffItem.Def;
        Debug.Log("---HP opre- " + hurtValue);
		if(hurtValue <= 0) hurtValue = 1;
		if(buffItem != null) buffItem.ChangeHp (-hurtValue);
	}

	private static void TriggerBuffItemRecoverHp(PvpBuffData buffData, PveFightUnit sourceItem, PveFightUnit targetItem, PveFightUnit buffItem)
	{
		if(buffItem != null) buffItem.ChangeHp (buffData.valueFloat);
	}

	private static void TriggerBuffItemExchangeHp(PvpBuffData buffData, PveFightUnit sourceItem, PveFightUnit targetItem, PveFightUnit buffItem)
	{
		sourceItem.TurnHp (targetItem.CurHp);
	}

	private static void TriggerBuffItemExchangeAttackHp(PvpBuffData buffData, PveFightUnit sourceItem, PveFightUnit targetItem, PveFightUnit buffItem)
	{
		bool critBool = false;
		bool missBool = false;
		float attak = targetItem.BeHurtValue(buffItem, ref critBool, ref missBool) * (buffData.valueFloat / PvpUserInfo.ODDS_VALUE);
		buffItem.ChangeHp (attak);
	}

	private static void TriggerBuffItemSummon(PvpBuffData buffData, PveFightUnit sourceItem, PveFightUnit targetItem, PveFightUnit buffItem, BaseSkillItem skillItem)
	{
		if(skillItem.skillData.triggerType == SkillTriggerTypeEnum.Skill)
		{
			Vector3 position = sourceItem.AreaRandom (new Vector2(sourceItem.XPosition, sourceItem.YPosition));
			//buffItem.GameControl.CreateMonster (position, buffData.valueString, buffItem, buffData.roundValue, false);
		}
	}

	private static void TriggerBuffItemStrike(List<PvpBuffData> buffList, PveFightUnit sourceItem, PveFightUnit targetItem, PveFightUnit buffItem, Action<string> strikeCallback = null)
	{
		if(sourceItem.StrickeStatus) return;

        HardWareData.HardWareType ht = UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style;
		if (ht == HardWareData.HardWareType.Light || ht == HardWareData.HardWareType.Heavy)
		{
			PvpBuffData buffData = buffList [0];
			if(targetItem.StrickeRandom((int)buffData.valueFloat))
			{
				((PveCharacter)buffItem).StrickeAttack (sourceItem, strikeCallback);
				return;
			}
		}

		if(strikeCallback != null) strikeCallback("");
	}

	private static void TriggerBuffItemTriggerSkill(PvpBuffData buffData, PveFightUnit sourceItem, PveFightUnit targetItem, PveFightUnit buffItem)
	{
		SkillData skillData = buffItem.pvpPlayerSkill.GetSkillDataBySkillID(buffData.valueString);
		if(skillData != null)
		{
			for(int index = 0; index < buffData.roundValue; index ++)
			{
                //Debug.Log("---------"+buffData.roundValue);
				sourceItem.GameControl.UseSkill(skillData, sourceItem.GameControl.skillSelfStatus, false);
			}
		}
	}

	private static bool ConditionOddsCheck(BaseSkillItem skillItem, PveFightUnit sourceItem)
	{
		switch(skillItem.skillData.oddsType)
		{
			case SkillOddsTypeEnum.Attack :
					return sourceItem.AttackRandom( (int)(skillItem.skillData.conditionData.conditionValue / 100f));
				; break;
			case SkillOddsTypeEnum.Attack_2 :
					return sourceItem.AttackRandom2( (int)(skillItem.skillData.conditionData.conditionValue / 100f));
				; break;
			case SkillOddsTypeEnum.Skill :
					return sourceItem.SkillRandom( (int)(skillItem.skillData.conditionData.conditionValue / 100f));
				; break;
			/*case SkillOddsTypeEnum.Summon :
					return sourceItem.SummonRandom(sourceItem.GameControl.PvpRounds, step, (int)(skillItem.skillData.conditionData.conditionValue / 100f));
				; break;*/
		}
		return false;
	}

	private static bool ConditionElementCheck(PveFightUnit sourceItem, PvpSkillData skillData)
	{
		//if((int)skillData.conditionData.conditionValue == (int)sourceItem.GameControl.currentEliminateAttribute) return true;
		return false;
	}

	private static bool ConditionOddsTriggerCheck(PveFightUnit sourceItem, PvpBuffData buffData)
	{
		return sourceItem.SkillRandom((int)(buffData.valueFloat / 100f));
	}
    /// <summary>
    /// 传送效果
    /// </summary>   
    private static void TriggerBuffItemTransmit(PveFightUnit sourceItem)
    {

        sourceItem.GameControl.UseSkillTransmit();
    }
    /// <summary>
    /// 每回合开始
    /// </summary>
    /// <param name="sourceItem">Source item.</param>
    /// <param name="targetItem">Target item.</param>
	public static void ExecuteRoundBegin(PveFightUnit sourceItem, PveFightUnit targetItem=null)
	{
		sourceItem.pvpPlayerBuff.RoundDelay ();
		if(targetItem!=null)targetItem.pvpPlayerBuff.RoundDelay ();
	}
    /// <summary>
    /// 回合开始、轮到自己
    /// </summary>
    /// <param name="pvpFightUnit">Pvp fight unit.</param>
	public static void ExecuteRoundBegin1(PveFightUnit PveFightUnit)
	{
		// 更新技能 cd
		PveFightUnit.pvpPlayerSkill.RoundExecute ();
		// 更新 buff 回合
		PveFightUnit.pvpPlayerBuff.RoundExecute ();
		// 清空行走触发的 buff
		PveFightUnit.pvpPlayerBuff.WalkExecute();
		// 执行回合前 buff
		Trigger2 (BuffStageTypeEnum.Round_Begin, PveFightUnit);
	}

    public static void SkillInit(PveUserInfo pveUserInfo, List<PvpSkillHouseData> dataList, List<PvpSkillCdData> dataCdList=null)
	{
		pveUserInfo.pvpPlayerSkill = new PvpPlayerSkill ();
        pveUserInfo.pvpPlayerSkill.skillCdList = dataCdList;
        pveUserInfo.pvpPlayerSkill.skillList = GetSkillItemList(dataList);
	}

	public static void BuffListInit(PveUserInfo pvpUserInfo)
	{
		pvpUserInfo.pvpPlayerBuff = new PvpPlayerBuff ();

		pvpUserInfo.pvpPlayerBuff.fixedBuffList = GetBuffDataListByStageType (pvpUserInfo.pvpPlayerSkill.skillList, BuffStageTypeEnum.Fixed);
		pvpUserInfo.pvpPlayerBuff.roundBeginBuffList = GetBuffDataListByStageType (pvpUserInfo.pvpPlayerSkill.skillList, BuffStageTypeEnum.Round_Begin);
		pvpUserInfo.pvpPlayerBuff.roundBuffList = new List<PvpBuffData>();
		pvpUserInfo.pvpPlayerBuff.walkBuffList = new List<PvpBuffData>();
		pvpUserInfo.pvpPlayerBuff.petBuffList = GetBuffDataListByStageType (pvpUserInfo.pvpPlayerSkill.skillList, BuffStageTypeEnum.Fixed_Pet);
		pvpUserInfo.pvpPlayerBuff.triggerSkillBuffList = GetBuffDataListByStageType (pvpUserInfo.pvpPlayerSkill.skillList, BuffStageTypeEnum.Trigger_Skill);
	}

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

	private static bool ExistsSkill(string skillID, List<BaseSkillItem> skillList)
	{
		foreach(BaseSkillItem skillItem in skillList)
		{
			if(skillItem.skillData.skillID == skillID) return true;
		}
		return false;
	}

	private static List<BaseSkillItem> GetSkillItemListByTriggerType(List<BaseSkillItem> dataList, int triggerType)
	{
		List<BaseSkillItem> resultList = new List<BaseSkillItem> ();

		foreach(BaseSkillItem skillItem in dataList)
		{
			if(skillItem.skillData.triggerType == triggerType) resultList.Add(skillItem);
		}

		return resultList;
	}

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

	private static List<PvpBuffData> GetBuffDataListByItemAndStageType(BaseSkillItem skillItem, int stageType)
	{
		List<PvpBuffData> resultList = new List<PvpBuffData> ();

		List<PvpBuffData> buffList = skillItem.skillData.conditionData.buffList;
		foreach(PvpBuffData buffData in buffList)
		{
			if(buffData.stageType == stageType) resultList.Add(buffData);
		}

		return resultList;
	}

	private static BaseSkillItem GetSkillItem(SkillData skillData)
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
