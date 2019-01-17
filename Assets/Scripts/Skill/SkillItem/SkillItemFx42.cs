using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 主角传送至随机区域
/// </summary>
public class SkillItemFx42 : BaseSkillItem 
{
	public override void AnalysisSkill (SkillData configData)
	{
		base.AnalysisSkill (configData);
		
		// 技能类别
		this.skillData.skillType = SkillTypeEnum.Magic;

		// 主角
		this.skillData.skillTarget = SkillTargetTypeEnum.Self_Player;
		// 攻击
		this.skillData.triggerType = SkillTriggerTypeEnum.Skill;
		// 触发几率方式
		this.skillData.oddsType = SkillOddsTypeEnum.Default;

		// 添加效果
		this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Transmit, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, 0, false, 0, false));
	}
}
