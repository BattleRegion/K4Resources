using UnityEngine;
using System.Collections;

/// <summary>
/// 在消除 A 格内通关副本（包含 A 回合）
/// </summary>
public class AchievementItem03 : AchievementItem
{
	public AchievementItem03():base(3)
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
