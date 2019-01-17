using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 初始战斗能量提高X点,能量上限提高Y点
/// </summary>
public class SkillItemFx38 : BaseSkillItem
{
	public override void AnalysisSkill (SkillData configData)
	{
		base.AnalysisSkill (configData);
		
		// 技能类别
		this.skillData.skillType = SkillTypeEnum.Talent;

		// 自己
		this.skillData.skillTarget = SkillTargetTypeEnum.Self;
		// 固定
		this.skillData.triggerType = SkillTriggerTypeEnum.Fixed;
		// 触发几率方式
		this.skillData.oddsType = SkillOddsTypeEnum.Default;

		// 能量 固定
		if(this.ValueStringCheck(this.configData.Xparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Energy, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Fixed_Pet, BuffEffectTypeEnum.DEFAULT, float.Parse(this.configData.Xparameter), false, 0, true));
		}
		// 能量上限 固定
		if(this.ValueStringCheck(this.configData.Yparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Energy_Limit, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Fixed_Pet, BuffEffectTypeEnum.DEFAULT, float.Parse(this.configData.Yparameter), false, 0, true));
		}
	}
}
