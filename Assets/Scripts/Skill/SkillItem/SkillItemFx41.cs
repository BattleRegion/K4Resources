using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 对X参数范围内的敌方单位进行Y属性的A点打击,并使其眩晕N回合.
/// </summary>
public class SkillItemFx41 : BaseSkillItem 
{
	public override void AnalysisSkill (SkillData configData)
	{
		base.AnalysisSkill (configData);
		
		// 技能类别
		this.skillData.skillType = SkillTypeEnum.Magic;

		// 范围
		this.skillData.skillTarget = SkillTargetTypeEnum.Range;
		// 攻击
		this.skillData.triggerType = SkillTriggerTypeEnum.Skill;
		// 触发几率方式
		this.skillData.oddsType = SkillOddsTypeEnum.Default;
		
		// 伤害 固定值
		if(this.ValueFloatCheck(this.configData.Aparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Attack, BuffTargetTypeEnum.ENEMY, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, this.configData.Aparameter, false, 0, true));
		}
		// 眩晕
		if(this.ValueFloatCheck(this.configData.Nparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Dizziness, BuffTargetTypeEnum.ENEMY, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, this.configData.Dparameter, false, (int)this.configData.Nparameter, false));
		}
		// 无法眩晕 固定值
		if(this.ValueFloatCheck(this.configData.Nparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Stop_Dizziness, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, 0f, false, (int)(this.configData.Nparameter), false));
		}
	}
}
