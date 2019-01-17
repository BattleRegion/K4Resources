using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 使所有携带的宠物攻击力提高*(1+X)%,生命力上限*(1+Y)%.(参数允许负值)
/// </summary>
public class SkillItemFx35 : BaseSkillItem
{
	public override void AnalysisSkill (SkillData configData)
	{
		base.AnalysisSkill (configData);
		
		// 技能类别
		this.skillData.skillType = SkillTypeEnum.Talent;

		// 已方宠物
		this.skillData.skillTarget = SkillTargetTypeEnum.Self;
		// 固定
		this.skillData.triggerType = SkillTriggerTypeEnum.Fixed;
		// 触发几率方式
		this.skillData.oddsType = SkillOddsTypeEnum.Default;

		// 攻击力 百分比
		if(this.ValueStringCheck(this.configData.Xparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Attack, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Fixed_Pet, BuffEffectTypeEnum.DEFAULT, float.Parse(this.configData.Xparameter), true, 0, true));
		}
		// 生命 百分比
		if(this.ValueStringCheck(this.configData.Yparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Hp_Limit, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Fixed_Pet, BuffEffectTypeEnum.DEFAULT, float.Parse(this.configData.Yparameter), true, 0, true));
		}
	}
}
