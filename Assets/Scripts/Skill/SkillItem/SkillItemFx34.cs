using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 消除X元素属性格时,发动的普通攻击伤害*X%,暴击率+Y%,暴击伤害+A%(参数允许负值)
/// </summary>
public class SkillItemFx34 : BaseSkillItem
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
		// 元素
		this.skillData.conditionData.conditionValue = int.Parse(this.configData.Xparameter);

		// 普通攻击 百分比
		if(this.ValueStringCheck(this.configData.Yparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Attack, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Walk, BuffEffectTypeEnum.DEFAULT, float.Parse(this.configData.Yparameter), true, 1, true));
		}
		// 暴击 百分比
		if(this.ValueFloatCheck(this.configData.Aparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Crit, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Walk, BuffEffectTypeEnum.DEFAULT, this.configData.Aparameter, true, 1, true));
		}
		// 暴击伤害 百分比
		if(this.ValueFloatCheck(this.configData.Bparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Crit_Hurt, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Walk, BuffEffectTypeEnum.DEFAULT, this.configData.Bparameter, true, 1, true));
		}
	}
}
