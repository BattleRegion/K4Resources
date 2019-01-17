using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 对敌方主角发动Y元素属性,X点攻击力的打击.并使其眩晕N回合.眩晕效果的角色无法被连续眩晕
/// </summary>
public class SkillItemFx20 : BaseSkillItem
{
	public override void AnalysisSkill (SkillData configData)
	{
		base.AnalysisSkill (configData);

		// 技能类别
		this.skillData.skillType = SkillTypeEnum.Magic;

		// 敌方主角
		this.skillData.skillTarget = SkillTargetTypeEnum.Enemy_Player;
		// 技能
		this.skillData.triggerType = SkillTriggerTypeEnum.Skill;
		// 触发几率方式
		this.skillData.oddsType = SkillOddsTypeEnum.Default;

		// 元素攻击 固定值
		if(this.ValueStringCheck(this.configData.Xparameter) && this.ValueStringCheck(this.configData.Yparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Attack, BuffTargetTypeEnum.ENEMY, BuffStageTypeEnum.Attack, int.Parse(this.configData.Yparameter), float.Parse(this.configData.Xparameter), false, 0, true));
		}
		// 眩晕 固定值
		if(this.ValueFloatCheck(this.configData.Nparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Dizziness, BuffTargetTypeEnum.ENEMY, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, this.configData.Dparameter, false, (int)(this.configData.Nparameter), false));
		}
		// 无法眩晕 固定值
		if(this.ValueFloatCheck(this.configData.Nparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Stop_Dizziness, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, 0f, false, (int)(this.configData.Nparameter), false));
		}
	}
}
