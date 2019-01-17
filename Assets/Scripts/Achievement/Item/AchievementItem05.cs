using UnityEngine;
using System.Collections;

/// <summary>
/// 不使用任何主动技能通关
/// </summary>
public class AchievementItem05 : AchievementItem
{
	public AchievementItem05():base(5)
	{
		this.stageType = AchievementStageTypeEnum.Fight_Ing;
	}

	public override void SetValue (int value)
	{
		this.intValue += value;
		if(this.intValue > int.Parse(this.conditionValue))
		{
			this.itemStatus = false;
		}
	}
}
