using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 将自己的当前生命力变为对手当前生命力.(可超过自己当前生命值上限)
/// </summary>
public class SkillItemFx21 : BaseSkillItem
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

		// 互换生命
		this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Exchange_Hp, BuffTargetTypeEnum.ENEMY, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, 0, false, 0, false));
	}
}
