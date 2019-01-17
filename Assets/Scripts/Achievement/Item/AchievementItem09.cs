using UnityEngine;
using System.Collections;

/// <summary>
/// 使用终结技杀死 BOSS（轻重武器终结技）
/// </summary>
public class AchievementItem09 : AchievementItem
{
	public AchievementItem09():base(9)
	{
		this.itemStatus = false;
		this.stageType = AchievementStageTypeEnum.Fight_Ing;
	}

	public override void SetValue (int value)
	{
		if(value > int.Parse(this.conditionValue))
		{
			this.itemStatus = true;
		}
	}
}
