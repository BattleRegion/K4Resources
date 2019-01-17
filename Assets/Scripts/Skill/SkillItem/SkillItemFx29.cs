using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 主角的普通攻击产生的伤害X%转换为当前生命力
/// </summary>
public class SkillItemFx29 : BaseSkillItem
{
	public override void AnalysisSkill (SkillData configData)
	{
		base.AnalysisSkill (configData);
		
		// 技能类别
		this.skillData.skillType = SkillTypeEnum.Talent;

		// 主角
		this.skillData.skillTarget = SkillTargetTypeEnum.Self_Player;
		// 普通攻击
		this.skillData.triggerType = SkillTriggerTypeEnum.Attack;
		// 触发几率方式
		this.skillData.oddsType = SkillOddsTypeEnum.Default;

		// 伤害转生命
		if(this.ValueStringCheck(this.configData.Xparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Exchange_Attack_Hp, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, float.Parse(this.configData.Xparameter), true, 0, true));
		}
	}
}
