using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 主角生命力上限+X,攻击力+X,暴击率+A%,闪躲率+B%,暴击伤害+C%,每回合开始恢复D生命力(参数允许负值)
/// </summary>
public class SkillItemFx28 : BaseSkillItem
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
		this.skillData.oddsType = SkillOddsTypeEnum.Default;

		// 防御 固定
		if(this.ValueStringCheck(this.configData.Xparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Denfense, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Fixed, BuffEffectTypeEnum.DEFAULT, float.Parse(this.configData.Xparameter), false, 0, true));
		}
		// 攻击力 固定
		if(this.ValueStringCheck(this.configData.Yparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Attack, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Fixed, BuffEffectTypeEnum.DEFAULT, float.Parse(this.configData.Yparameter), false, 0, true));
		}
		// 暴击 百分比
		if(this.ValueFloatCheck(this.configData.Aparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Crit, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Fixed, BuffEffectTypeEnum.DEFAULT, this.configData.Aparameter, true, 0, true));
		}
		// 闪避 百分比
		if(this.ValueFloatCheck(this.configData.Bparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Avoid, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Fixed, BuffEffectTypeEnum.DEFAULT, this.configData.Bparameter, true, 0, true));
		}
		// 暴击伤害 百分比
		if(this.ValueFloatCheck(this.configData.Cparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Crit_Hurt, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Fixed, BuffEffectTypeEnum.DEFAULT, this.configData.Cparameter, true, 0, true));
		}
		// 生命 固定
		if(this.ValueFloatCheck(this.configData.Dparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Hp, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Round_Begin, BuffEffectTypeEnum.DEFAULT, this.configData.Dparameter, true, 0, true));
		}
	}
}
