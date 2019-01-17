using UnityEngine;
using System.Collections.Generic;

public class BaseSkillItem
{
	/// <summary>
	/// 技能
	/// </summary>
	public PvpSkillData skillData;

	/// <summary>
	/// 配置，内容不得修改
	/// </summary>
	public SkillData configData;

	public int houseID;

	/// <summary>
	/// 解析技能
	/// </summary>
	/// <param name="configData">Config data.</param>
	public virtual void AnalysisSkill(SkillData configData)
	{
		this.configData = configData;

		// 初始化技能
		this.skillData = new PvpSkillData (configData.Id, configData.SkillFX);

		// 触发条件
		this.skillData.conditionData = new PvpConditionData ();

		// Buff 
		this.skillData.conditionData.buffList = new List<PvpBuffData> ();
	}

	public bool IsMagic
	{
		get { return this.skillData.skillType == SkillTypeEnum.Magic; }
	}

	protected bool ValueStringCheck(string value)
	{
		// 如果是空
		if(string.IsNullOrEmpty(value)) return false;
		if(value == "0") return false;

		return true;
	}

	protected bool ValueFloatCheck(float value)
	{
		if(value == 0) return false;
		return true;
	}

	public List<PvpBuffData> GetBuffList(int buffType)
	{
		List<PvpBuffData> resultList = new List<PvpBuffData> ();

		List<PvpBuffData> buffList = this.skillData.conditionData.buffList;
		if(buffList == null || buffList.Count == 0) return resultList;

		foreach(PvpBuffData buffData in buffList)
		{
			if(buffData.buffType == buffType) resultList.Add(buffData);
		}
		return resultList;
	}

	/// <summary>
	/// 验证是否是有相应的 buff
	/// </summary>
	/// <returns><c>true</c>, if type check was buffed, <c>false</c> otherwise.</returns>
	/// <param name="buffType">Buff type.</param>
	public bool BuffTypeCheck(int buffType)
	{
		List<PvpBuffData> buffList = this.skillData.conditionData.buffList;
		if(buffList == null || buffList.Count == 0) return false;

		foreach(PvpBuffData buffData in buffList)
		{
			if(buffData.buffType == buffType) return true;
		}
		return false;
	}
}
