using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 对敌方主角发动Y元素属性X点攻击力的打击.并使自身攻击力*(1+A)%,防御力*(1+B)%,闪躲率+C%,暴击概率+D%持续N参数回合.(效果顶替)
/// </summary>
public class SkillItemFx24 : BaseSkillItem
{
	public override void AnalysisSkill (SkillData configData)
	{
		base.AnalysisSkill (configData);
		
		// 技能类别
		this.skillData.skillType = SkillTypeEnum.Magic;

		// 敌方
		this.skillData.skillTarget = SkillTargetTypeEnum.Enemy_Player;
		// 技能
		this.skillData.triggerType = SkillTriggerTypeEnum.Skill;
		// 触发几率方式
		this.skillData.oddsType = SkillOddsTypeEnum.Default;

		// 元素攻击
		if(this.ValueStringCheck(this.configData.Yparameter) && this.ValueStringCheck(this.configData.Xparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Attack, BuffTargetTypeEnum.ENEMY, BuffStageTypeEnum.Attack, int.Parse(this.configData.Yparameter), float.Parse(this.configData.Xparameter), false, 0, true));
		}
		// 攻击力 百分比
		if(this.ValueFloatCheck(this.configData.Aparameter) && this.ValueFloatCheck(this.configData.Nparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Attack, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, this.configData.Aparameter, true, (int)(this.configData.Nparameter), false));
		}
		// 防御力 百分比
		if(this.ValueFloatCheck(this.configData.Bparameter) && this.ValueFloatCheck(this.configData.Nparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Denfense, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, this.configData.Bparameter, true, (int)(this.configData.Nparameter), false));
		}
		// 闪避 百分比
		if(this.ValueFloatCheck(this.configData.Cparameter) && this.ValueFloatCheck(this.configData.Nparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Avoid, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, this.configData.Cparameter, true, (int)(this.configData.Nparameter), false));
		}
		// 暴击 百分比
		if(this.ValueFloatCheck(this.configData.Dparameter) && this.ValueFloatCheck(this.configData.Nparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Crit, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, this.configData.Dparameter, true, (int)(this.configData.Nparameter), false));
		}
	}
}
