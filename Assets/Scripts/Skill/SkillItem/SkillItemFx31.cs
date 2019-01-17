using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 释放X技能时有Y%概率再次释放A次
/// </summary>
public class SkillItemFx31 : BaseSkillItem
{
	public override void AnalysisSkill (SkillData configData)
	{
		base.AnalysisSkill (configData);
		
		// 技能类别
		this.skillData.skillType = SkillTypeEnum.Talent;

		// 主角
		this.skillData.skillTarget = SkillTargetTypeEnum.Self_Player;
		// 固定
		this.skillData.triggerType = SkillTriggerTypeEnum.Fixed;
		// 触发几率方式
		this.skillData.oddsType = SkillOddsTypeEnum.Skill;

		// 触发技能
		if(this.ValueStringCheck(this.configData.Yparameter) && this.ValueFloatCheck(this.configData.Aparameter) && this.ValueStringCheck(this.configData.Xparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Trigger_Skill, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Trigger_Skill, BuffEffectTypeEnum.DEFAULT, float.Parse(this.configData.Yparameter), true, (int)(this.configData.Aparameter), true, this.configData.Xparameter));
		}
	}
}
