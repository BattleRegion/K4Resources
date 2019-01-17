using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 消除X属性方砖时主角额外+Y点能量
/// </summary>
public class SkillItemFx27 : BaseSkillItem
{
	public override void AnalysisSkill (SkillData configData)
	{
		base.AnalysisSkill (configData);
		
		// 技能类别
		this.skillData.skillType = SkillTypeEnum.Talent;

		// 主角
		this.skillData.skillTarget = SkillTargetTypeEnum.Self_Player;
		// 行走
		this.skillData.triggerType = SkillTriggerTypeEnum.Walk;
		// 触发几率方式
		this.skillData.oddsType = SkillOddsTypeEnum.Default;

		// 触发类别
		this.skillData.conditionData.conditionType = ConditionTypeEnum.Element;
		// 元素属性
		this.skillData.conditionData.conditionValue = float.Parse(this.configData.Xparameter);

		// 能量 固定值
		if(this.ValueStringCheck(this.configData.Yparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Energy, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Walk, BuffEffectTypeEnum.DEFAULT, float.Parse(this.configData.Yparameter), false, 1, true));
		}
	}
}
