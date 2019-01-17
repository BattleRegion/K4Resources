using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 对敌方主角发动Y元素属性,X点攻击力的打击.并恢复自身A点生命力
/// </summary>
public class SkillItemFx23 : BaseSkillItem
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

		// 元素攻击 固定
		if(this.ValueStringCheck(this.configData.Xparameter) && this.ValueStringCheck(this.configData.Yparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Attack, BuffTargetTypeEnum.ENEMY, BuffStageTypeEnum.Attack, int.Parse(this.configData.Yparameter), float.Parse(this.configData.Xparameter), false, 0, true));
		}
		// 恢复生命 固定
		if(this.ValueFloatCheck(this.configData.Aparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Recover_Hp, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, this.configData.Aparameter, false, 0, true));
		}
	}
}
