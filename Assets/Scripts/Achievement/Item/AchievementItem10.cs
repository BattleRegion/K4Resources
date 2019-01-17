using UnityEngine;
using System.Collections;

/// <summary>
/// 不能让任何怪物逃跑
/// </summary>
public class AchievementItem10 : AchievementItem
{
	public AchievementItem10():base(10)
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
