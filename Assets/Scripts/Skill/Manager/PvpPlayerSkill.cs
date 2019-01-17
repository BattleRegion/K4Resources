using UnityEngine;
using System.Collections.Generic;

public class PvpPlayerSkill
{
	/// <summary>
	/// 所携带的技能
	/// </summary>
	public List<BaseSkillItem> skillList;
	public List<PvpSkillCdData> skillCdList;

	public PvpPlayerSkill()
	{
		this.skillList = new List<BaseSkillItem> ();
		this.skillCdList = new List<PvpSkillCdData> ();
	}

	/// <summary>
	/// 刷新 Cd
	/// </summary>
	/// <param name="skillID">Skill I.</param>
	public void RefreshCd(string skillID, int petID)
	{
		PvpSkillCdData pvpSkillCdData = this.GetSkillCdDataBySkillID (skillID, petID);
		if(pvpSkillCdData != null) pvpSkillCdData.cd = pvpSkillCdData.skillData.SkillCd;
	}

	/// <summary>
	/// 清空 Cd
	/// </summary>
	public void ClearCd()
	{
		for(int index = 0; index < this.skillCdList.Count; index ++)
		{
			this.skillCdList[index].cd = 0;
		}
	}

	/// <summary>
	/// 根据技能
	/// </summary>
	/// <returns>The skill data by skill type and odds type.</returns>
	/// <param name="skillType">Skill type.</param>
	/// <param name="oddsType">Odds type.</param>
	public List<BaseSkillItem> GetSkillDataBySkillTypeAndOddsType(int skillType, int oddsType)
	{
		List<BaseSkillItem> resultList = new List<BaseSkillItem> ();
		foreach(BaseSkillItem skillItem in this.skillList)
		{
			if(skillItem.skillData.skillType == skillType && skillItem.skillData.oddsType == oddsType)
			{
				resultList.Add(skillItem);
			}
		}
		return resultList;
	}

	/// <summary>
	/// 根据技能 ID 获取 SkillData
	/// </summary>
	/// <returns>The skill data by skill I.</returns>
	/// <param name="skillID">Skill I.</param>
	public SkillData GetSkillDataBySkillID(string skillID)
	{
		BaseSkillItem skillItem = this.GetSkillItemBySkillID (skillID);
		if(skillItem == null) return null;

		return skillItem.configData;
	}

	/// <summary>
	/// 根据技能 ID，宠物 ID 获取 PvpSkillCdData
	/// </summary>
	/// <returns>The skill cd data by skill I.</returns>
	/// <param name="skillID">Skill I.</param>
	/// <param name="petID">Pet I.</param>
	public PvpSkillCdData GetSkillCdDataBySkillID(string skillID, int petID)
	{
		foreach(PvpSkillCdData skillCdData in this.skillCdList)
		{
			if(skillCdData.skillData.Id == skillID && skillCdData.petID == petID) return skillCdData;
		}
		return null;
	}

	/// <summary>
	/// 根据技能 ID 获取 BaseSkillItem
	/// </summary>
	/// <returns>The skill item by skill I.</returns>
	/// <param name="skillID">Skill I.</param>
	public BaseSkillItem GetSkillItemBySkillID(string skillID)
	{
		if(this.skillList == null || this.skillList.Count == 0) return null;

		foreach(BaseSkillItem skillItem in this.skillList)
		{
			if(skillItem.skillData.skillID == skillID) return skillItem;
		}
		return null;
	}

	/// <summary>
	/// 技能 CD 检测
	/// </summary>
	/// <returns><c>true</c>, if cd check was skilled, <c>false</c> otherwise.</returns>
	/// <param name="skillID">Skill I.</param>
	public bool SkillCdCheck(string skillID, int petID)
	{
		PvpSkillCdData pvpSkillCdData = this.GetSkillCdDataBySkillID (skillID, petID);
		if(pvpSkillCdData == null || pvpSkillCdData.cd > 0) return false;

		return true;
	}

	/// <summary>
	/// 执行 CD
	/// </summary>
	public void RoundExecute()
	{
		for(int index = 0; index < this.skillCdList.Count; index ++)
		{
			this.skillCdList[index].cd --;
		}
	}
}
