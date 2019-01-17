using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 普通攻击有X%概率恢复自身Y点生命力,并使自身攻击力*(1+A)%,防御力*(1+B)%,闪躲率+C%,暴击概率+D%,持续N参数回合.
/// </summary>
public class SkillItemFx33 : BaseSkillItem 
{
	public override void AnalysisSkill (SkillData configData)
	{
		base.AnalysisSkill (configData);
		
		// 技能类别
		this.skillData.skillType = SkillTypeEnum.Talent;

		// 主角
		this.skillData.skillTarget = SkillTargetTypeEnum.Self_Player;
		// 攻击
		this.skillData.triggerType = SkillTriggerTypeEnum.Attack;
		// 触发几率方式
		this.skillData.oddsType = SkillOddsTypeEnum.Attack_2;

		// 触发类别
		this.skillData.conditionData.conditionType = ConditionTypeEnum.Odds;
		// 触发几率
		this.skillData.conditionData.conditionValue = float.Parse(this.configData.Xparameter);

		// 恢复生命 固定值
		if(this.ValueStringCheck(this.configData.Yparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Hp, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, float.Parse(this.configData.Yparameter), false, 0, true));
		}
		// 攻击力 百分比
		if(this.ValueFloatCheck(this.configData.Aparameter) && this.ValueFloatCheck(this.configData.Nparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Attack, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, this.configData.Aparameter, true, (int)(this.configData.Nparameter), false, "", true));
		}
		// 防御力 百分比
		if(this.ValueFloatCheck(this.configData.Bparameter) && this.ValueFloatCheck(this.configData.Nparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Denfense, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, this.configData.Bparameter, true, (int)(this.configData.Nparameter), false, "", true));
		}
		// 暴击 百分比
		if(this.ValueFloatCheck(this.configData.Cparameter) && this.ValueFloatCheck(this.configData.Nparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Crit, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, this.configData.Cparameter, true, (int)(this.configData.Nparameter), false, "", true));
		}
		// 闪避 百分比
		if(this.ValueFloatCheck(this.configData.Dparameter) && this.ValueFloatCheck(this.configData.Nparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Avoid, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, this.configData.Dparameter, true, (int)(this.configData.Nparameter), false, "", true));
		}
	}
}
