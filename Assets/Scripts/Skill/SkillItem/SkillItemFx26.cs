using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 当主角被敌方主角重武器或轻武器攻击时,有+X%自动反击对手(普通攻击都计算暴击闪躲及可被普通攻击触发的技能).可造成对手在攻击过程中死亡.反击攻击力为主角0消除格子的时候的普通伤害会产生暴击情况.(反击概率可被叠加)
/// </summary>
public class SkillItemFx26 : BaseSkillItem
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
		this.skillData.oddsType = SkillOddsTypeEnum.Default;

		// 轻武器或者重武器攻击
		//this.skillData.conditionData.conditionType = ConditionTypeEnum.Weapon;
		//this.skillData.conditionData.conditionValue = int.Parse (this.configData.Xparameter);
		// 为了统一，把检测技能转为检测 Buff，反击的概率也转到 buff 中计算，特殊处理这个技能。

		// 反击
		if(this.ValueStringCheck(this.configData.Xparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Strike, BuffTargetTypeEnum.ENEMY, BuffStageTypeEnum.Fixed, BuffEffectTypeEnum.DEFAULT, int.Parse (this.configData.Xparameter), false, 0, true));
		}
	}
}
