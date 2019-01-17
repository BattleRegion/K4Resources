using UnityEngine;
using System.Collections;

/// <summary>
/// 在规定参数 A 剩余回合内通关副本（包含 A 回合）
/// </summary>
public class AchievementItem01 : AchievementItem
{
	public AchievementItem01():base(1)
	{
		this.stageType = AchievementStageTypeEnum.Fight_Ing;
	}

	public override void SetValue (int value)
	{
		if(value < int.Parse(this.conditionValue))
		{
			this.intValue = value;
			this.itemStatus = false;
		}
	}
}
