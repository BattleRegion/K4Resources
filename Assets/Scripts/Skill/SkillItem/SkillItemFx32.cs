using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 普通攻击时有X%,对敌方主角发动Y属性的A点攻击.并使其攻击力*(1+B)%,防御力*(1+C)%,持续N参数回合.
/// </summary>
public class SkillItemFx32 : BaseSkillItem
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
		this.skillData.oddsType = SkillOddsTypeEnum.Attack;

		// 触发类别
		this.skillData.conditionData.conditionType = ConditionTypeEnum.Odds;
		// 触发几率
		this.skillData.conditionData.conditionValue = float.Parse(this.configData.Xparameter);

		// 元素攻击 固定值
		if(this.ValueStringCheck(this.configData.Yparameter) && this.ValueFloatCheck(this.configData.Aparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Attack, BuffTargetTypeEnum.ENEMY, BuffStageTypeEnum.Attack, int.Parse(this.configData.Yparameter), this.configData.Aparameter, false, 0, true));
		}
		// 攻击力 百分比
		if(this.ValueFloatCheck(this.configData.Bparameter) && this.ValueFloatCheck(this.configData.Nparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Attack, BuffTargetTypeEnum.ENEMY, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, this.configData.Bparameter, true, (int)(this.configData.Nparameter), false, "", true));
		}
		// 防御力 百分比
		if(this.ValueFloatCheck(this.configData.Cparameter) && this.ValueFloatCheck(this.configData.Nparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Denfense, BuffTargetTypeEnum.ENEMY, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, this.configData.Cparameter, true, (int)(this.configData.Nparameter), false, "", true));
		}
	}
}
