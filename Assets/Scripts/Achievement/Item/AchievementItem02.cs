using UnityEngine;
using System.Collections;

/// <summary>
/// 生命力不曾低于 A%，通关
/// </summary>
public class AchievementItem02 : AchievementItem
{
	public AchievementItem02():base(2)
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
