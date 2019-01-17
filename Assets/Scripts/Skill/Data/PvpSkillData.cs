using UnityEngine;
using System.Collections.Generic;

public class PvpSkillData
{
	/// <summary>
	/// 技能 ID
	/// </summary>
	public string skillID;

	/// <summary>
	/// 效果 ID
	/// </summary>
	public string effectID;

	/// <summary>
	/// 技能分类
	/// </summary>
	public int skillType;
	
	/// <summary>
	/// 释放目标
	/// </summary>
	public int skillTarget;

	/// <summary>
	/// 触发类别
	/// </summary>
	public int triggerType;

	/// <summary>
	/// 几率计算类别
	/// </summary>
	public int oddsType;

	/// <summary>
	/// Buff 触发条件
	/// </summary>
	public PvpConditionData conditionData;

	public PvpSkillData(string skillID, string effectID)
	{
		this.skillID = skillID;
		this.effectID = effectID;
	}
}
