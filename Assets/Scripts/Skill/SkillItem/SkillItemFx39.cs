using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 普通攻击时有X%概率在自己周围随机位置召唤一个Y参数的怪物,该怪物在主角释放完终结技后行动.只攻击敌方主角并持续N回合.一回合内只能触发1次.
/// </summary>
public class SkillItemFx39 : BaseSkillItem
{
	public override void AnalysisSkill (SkillData configData)
	{
		base.AnalysisSkill (configData);
		
		// 技能类别
		this.skillData.skillType = SkillTypeEnum.Talent;

		// 主角
		this.skillData.skillTarget = SkillTargetTypeEnum.Self;
		// 攻击
		this.skillData.triggerType = SkillTriggerTypeEnum.Attack;
		// 触发几率方式
		this.skillData.oddsType = SkillOddsTypeEnum.Summon;

		// 触发条件
		this.skillData.conditionData.conditionType = ConditionTypeEnum.Odds;
		// 触发几率
		this.skillData.conditionData.conditionValue = float.Parse(this.configData.Yparameter);

		// 召唤
		if(this.ValueFloatCheck(this.configData.Nparameter) && this.ValueStringCheck(this.configData.Xparameter))
		{
			this.skillData.conditionData.buffList.Add(new PvpBuffData (BuffTypeEnum.Summon, BuffTargetTypeEnum.SELF, BuffStageTypeEnum.Attack, BuffEffectTypeEnum.DEFAULT, 0, false, (int)this.configData.Nparameter, false, this.configData.Xparameter));
		}
	}
}
