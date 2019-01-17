using UnityEngine;
using System.Collections.Generic;

public class PvpPlayerBuff
{
	/// <summary>
	/// 宠物相关的 buff 
	/// </summary>
	public List<PvpBuffData> petBuffList;

	/// <summary>
	/// 固定 只在一开始战斗前进行计算
	/// </summary>
	public List<PvpBuffData> fixedBuffList;

	/// <summary>
	/// 回合开始 只在每个回合开始进行计算
	/// </summary>
	public List<PvpBuffData> roundBeginBuffList;

	/// <summary>
	/// 回合 每个回合结束之后，需要把回合数减 1，如果为 0 则从列表中移除
	/// </summary>
	public List<PvpBuffData> roundBuffList;

	/// <summary>
	/// 行走 每个行走结束之后，触发的 buff，只触发一次，考虑到叠加的原因，单独放一个列表，在每次行走结束之后这个列表清空
	/// </summary>
	public List<PvpBuffData> walkBuffList;

	/// <summary>
	/// 延时 Buff
	/// </summary>
	public List<PvpBuffData> delayBuffList;

	/// <summary>
	/// 触发技能 
	/// </summary>
	public List<PvpBuffData> triggerSkillBuffList;

	public PvpPlayerBuff()
	{
		this.petBuffList = new List<PvpBuffData> ();

		this.fixedBuffList = new List<PvpBuffData> ();
		this.roundBeginBuffList = new List<PvpBuffData> ();
		this.roundBuffList = new List<PvpBuffData> ();
		this.walkBuffList = new List<PvpBuffData> ();
		this.delayBuffList = new List<PvpBuffData> ();
		this.triggerSkillBuffList = new List<PvpBuffData> ();
	}

	/// <summary>
	/// 重置回合 buff
	/// </summary>
	public void ClearBuffList()
	{
		this.roundBuffList.Clear ();
		this.walkBuffList.Clear ();
		this.delayBuffList.Clear ();
	}

	/// <summary>
	/// 获得角色身上所有 Buff，应该只在回合开始前进行计算
	/// </summary>
	/// <returns>The all buff list.</returns>
	public List<PvpBuffData> GetAllBuffList()
	{
		List<PvpBuffData> resultList = new List<PvpBuffData> ();

		resultList.InsertRange (resultList.Count, fixedBuffList);
		resultList.InsertRange (resultList.Count, roundBuffList);
		resultList.InsertRange (resultList.Count, walkBuffList);

		return resultList;
	}

	/// <summary>
	/// 延时处理
	/// </summary>
	public void RoundDelay()
	{
		foreach(PvpBuffData buffData in this.delayBuffList)
		{
			this.InsertBuff(buffData.stageType, buffData.skillID, buffData, false);
		}

		this.delayBuffList.Clear ();
	}

	/// <summary>
	/// 清空延时 buff
	/// </summary>
	public void ClearDelay()
	{
		this.delayBuffList.Clear ();
	}

	/// <summary>
	/// 添加 buff
	/// </summary>
	/// <param name="skillID">Skill I.</param>
	/// <param name="buffData">Buff data.</param>
	public void InsertBuff(string skillID, PvpBuffData buffData, bool delay = true)
	{
		buffData.skillID = skillID;
		if(delay)
		{
			this.delayBuffList.Add (buffData);
		}else{
			this.roundBuffList.Add(buffData);
		}
	}

	/// <summary>
	/// 添加 Buff
	/// </summary>
	/// <param name="stageType">Stage type.</param>
	/// <param name="skillID">Skill I.</param>
	/// <param name="buffData">Buff data.</param>
	public void InsertBuff(int stageType, string skillID, PvpBuffData buffData, bool delay = true)
	{
		if(delay)
		{
			if(buffData.delay) this.InsertBuff(skillID, buffData, true);
		}

		List<PvpBuffData> resultList = null;

		if(stageType == BuffStageTypeEnum.Attack)
		{
			resultList = this.roundBuffList;
		}else if(stageType == BuffStageTypeEnum.Walk)
		{
			resultList = this.walkBuffList;
		}
		if(resultList == null) return;

		int overlayIndex = OverlayCheck (skillID, buffData, resultList);
		// 如果不能叠加，直接替换
		if(overlayIndex != -1)
		{
			buffData.skillID = skillID;
			resultList[overlayIndex] = buffData;
		}else{
			buffData.skillID = skillID;
			resultList.Add(buffData);
		}
	}

	/// <summary>
	/// 检测是否能触发相应的技能
	/// </summary>
	/// <returns><c>true</c>, if skill check was triggered, <c>false</c> otherwise.</returns>
	/// <param name="skillID">Skill I.</param>
	public List<PvpBuffData> TriggerSkillCheck(string skillID)
	{
		List<PvpBuffData> resultList = new List<PvpBuffData> ();

		if(this.triggerSkillBuffList == null || this.triggerSkillBuffList.Count == 0) return resultList;

		foreach(PvpBuffData buffData in this.triggerSkillBuffList)
		{
			if(this.TriggerSkillCheckItem(skillID, buffData.valueString)) resultList.Add(buffData);
		}

		return resultList;
	}

	/// <summary>
	/// 验证是否是触发的技能
	/// </summary>
	/// <returns><c>true</c>, if skill check item was triggered, <c>false</c> otherwise.</returns>
	/// <param name="skillID">Skill I.</param>
	/// <param name="valueString">Value string.</param>
	private bool TriggerSkillCheckItem(string skillID, string valueString)
	{
		if(valueString.IndexOf(",") != -1)
		{
			string[] valueStringList = valueString.Split(new string[]{","}, System.StringSplitOptions.RemoveEmptyEntries);
			foreach(string valueItem in valueStringList)
			{
				if(valueItem.Trim() == skillID) return true;
			}
			return false;
		}else{
			if(skillID.Trim() == valueString.Trim()) return true;
			return false;
		}
	}

	/// <summary>
	/// 根据 buff 类别获取 buff 累积值
	/// </summary>
	/// <returns>The value by buff type to float.</returns>
	/// <param name="buffType">Buff type.</param>
	/// <param name="odds">If set to <c>true</c> odds.</param>
	public float GetValueByBuffTypeToFloat(int buffType, bool odds = true)
	{
		List<PvpBuffData> resultList = this.GetAllBuffList ();

		if(resultList == null || resultList.Count == 0) return 0;

		return this.GetValueByBuffListAndBuffType (buffType, resultList, odds);
	}

	/// <summary>
	/// 获取列表中所有的 buff 数据值
	/// </summary>
	/// <returns>The value by buff type to class.</returns>
	/// <param name="petStatus">If set to <c>true</c> pet status.</param>
	public PvpBuffValueData GetValueByBuffTypeToClass(bool petStatus = false)
	{
		List<PvpBuffData> resultList = null;

		if(!petStatus)
		{
			resultList = this.GetAllBuffList ();
		}else{
			resultList = this.petBuffList;
		}

		PvpBuffValueData resultData = new PvpBuffValueData ();
		if(resultList == null || resultList.Count == 0) return resultData;

		if(!petStatus)
		{
			// 防御 固定值
			resultData.defense = this.GetValueByBuffListAndBuffType(BuffTypeEnum.Recover_Denfense, resultList, false);
			// 攻击力 固定值
			resultData.attack = this.GetValueByBuffListAndBuffType (BuffTypeEnum.Recover_Attack, resultList, false);
			// 攻击力 百分比
			resultData.attack_odds = this.GetValueByBuffListAndBuffType (BuffTypeEnum.Recover_Attack, resultList);
			// 防御力 百分比
			resultData.defense_odds = this.GetValueByBuffListAndBuffType (BuffTypeEnum.Recover_Denfense, resultList);
			// 闪避 百分比
			resultData.avoid_odds = this.GetValueByBuffListAndBuffType (BuffTypeEnum.Recover_Avoid, resultList);
			// 暴击 百分比
			resultData.crit_odds = this.GetValueByBuffListAndBuffType (BuffTypeEnum.Recover_Crit, resultList);
			// 暴击伤害 百分比
			resultData.crit_hurt_odds = this.GetValueByBuffListAndBuffType (BuffTypeEnum.Recover_Crit_Hurt, resultList);
		}else{
			// 攻击力 百分比
			resultData.attack_odds = this.GetValueByBuffListAndBuffType(BuffTypeEnum.Recover_Attack, resultList);
			// 生命上限 百分比
			resultData.hp_limit_odds = this.GetValueByBuffListAndBuffType(BuffTypeEnum.Recover_Hp_Limit, resultList);
			// 能量上限 固定值
			resultData.energy_limit = this.GetValueByBuffListAndBuffType(BuffTypeEnum.Recover_Energy_Limit, resultList, false);
			// 初始能量
			resultData.energy = this.GetValueByBuffListAndBuffType(BuffTypeEnum.Recover_Energy, resultList, false);
		}

		return resultData;
	}

	/// <summary>
	/// 根据 buff 类别 获取固定值 或者 百分比形式的数据
	/// </summary>
	/// <returns>The value by buff list and effect type.</returns>
	/// <param name="buffType">Buff type.</param>
	/// <param name="dataList">Data list.</param>
	/// <param name="odds">If set to <c>true</c> odds.</param>
	public float GetValueByBuffListAndBuffType(int buffType, List<PvpBuffData> dataList, bool odds = true)
	{
		float result = 0f;

		foreach(PvpBuffData pvpBuffData in dataList)
		{
			if(pvpBuffData.buffType == buffType)
			{
				if(odds) // 如果是计算百分比形式
				{
					if(pvpBuffData.valueType) result += pvpBuffData.valueFloat;
				}else{ // 如果是计算固定值形式
					if(!pvpBuffData.valueType) result += pvpBuffData.valueFloat;
				}
			}
		}

		return result;
	}

	/// <summary>
	/// 根据 Buff 查询列表
	/// </summary>
	/// <returns>The list by buff type.</returns>
	/// <param name="buffType">Buff type.</param>
	/// <param name="dataList">Data list.</param>
	public List<PvpBuffData> GetListByBuffType(int buffType, List<PvpBuffData> dataList)
	{
		List<PvpBuffData> resultList = new List<PvpBuffData> ();

		foreach(PvpBuffData buffData in dataList)
		{
			if(buffData.buffType == buffType) resultList.Add(buffData);
		}
		return resultList;
	}

	/// <summary>
	/// 能否
	/// </summary>
	/// <returns><c>true</c>, if check was buffed, <c>false</c> otherwise.</returns>
	/// <param name="skillID">Skill I.</param>
	/// <param name="buffData">Buff data.</param>
	public int OverlayCheck(string skillID, PvpBuffData buffData, List<PvpBuffData> dataList)
	{
		if(buffData.overlay) return -1;

		int index = 0;
		foreach(PvpBuffData pvpBuffData in dataList)
		{
			// 如果 Buff 相同，并且所属技能也相同
			if(pvpBuffData.buffType == buffData.buffType && pvpBuffData.skillID == skillID) return index;
			index ++;
		}

		return -1;
	}

	/// <summary>
	/// buff 状态检查
	/// </summary>
	/// <returns><c>true</c> if this instance can or not transmit check; otherwise, <c>false</c>.</returns>
	public bool CanOrNotCheck(int buffType, int round = 0)
	{
		List<PvpBuffData> resultList = this.GetListByBuffType (buffType, this.roundBuffList);
		if(resultList == null || resultList.Count == 0) return false;
		
		foreach(PvpBuffData buffData in resultList)
		{
			if(buffData.roundValue >= round) return true;
		}
		
		return false;
	}

	/// <summary>
	/// 能否行走检查
	/// </summary>
	/// <returns><c>true</c> if this instance can or not move check; otherwise, <c>false</c>.</returns>
	public bool CanOrNotMoveCheck()
	{
		List<PvpBuffData> resultList = this.GetListByBuffType (BuffTypeEnum.Dizziness, this.roundBuffList);
		if(resultList == null || resultList.Count == 0) return true;

		foreach(PvpBuffData buffData in resultList)
		{
			if(buffData.roundValue >= 1) return false;
		}

		return true;
	}

	/// <summary>
	/// 下一次能否行走检查
	/// </summary>
	/// <returns><c>true</c>, if can or not move check was nexted, <c>false</c> otherwise.</returns>
	public bool NextCanOrNotMoveCheck()
	{
		List<PvpBuffData> resultList = this.GetListByBuffType (BuffTypeEnum.Dizziness, this.roundBuffList);
		if(resultList == null || resultList.Count == 0) return true;
		
		foreach(PvpBuffData buffData in resultList)
		{
			if(buffData.roundValue - 1 >= 0) return false;
		}
		
		return true;
	}

	/// <summary>
	/// 执行回合
	/// </summary>
	public void RoundExecute()
	{
		int index = 0;
		while(index < this.roundBuffList.Count)
		{
			PvpBuffData buffData = this.roundBuffList[index];
			buffData.roundValue --;

			if(buffData.roundValue <= 0)
			{
				this.roundBuffList.RemoveAt(index);
			}else{
				index ++;
			}
		}
	}

	/// <summary>
	/// 执行行走
	/// </summary>
	public void WalkExecute()
	{
		this.walkBuffList.Clear ();
	}
}
