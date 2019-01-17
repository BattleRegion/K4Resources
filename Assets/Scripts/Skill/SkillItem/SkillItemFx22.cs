using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 在自己周围随机位置召唤一个Y参数的怪物,该怪物在主角释放完终结技后行动.只攻击敌方主角并持续N回合.(敌方会攻击我方召唤物,我方不会攻击自己的召唤物)
/// </summary>
public class SkillItemFx22 : BaseSkillItem
{
	public override void AnalysisSkill (SkillData configData)
	{
		base.AnalysisSkill (configData);
		
		// 技能类别
		this.skillData.skillType = SkillTypeEnum.Magic;
		
		// 已方主角
		this.skillData.skillTarget = SkillTargetTypeEnum.Self_Player;
		// 技能
		this.skillData.triggerType = SkillTriggerTypeEnum.Skill;
		// 触发几率方式
		this.skillData.oddsType = SkillOddsTypeEnum.Default;

		// 召唤
		if(this.ValueFloatCheck(this.configData.Nparameter) && this.ValueStringCheck(this.configData.Xparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Summon, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, 0, false, (int)this.configData.Nparameter, false, this.configData.Xparameter));
		}
	}
}
