using UnityEngine;
using System.Collections;

/// <summary>
/// 战力值小等于 A 值的队伍通关
/// </summary>
public class AchievementItem06 : AchievementItem
{
	public AchievementItem06():base(6)
	{
		this.stageType = AchievementStageTypeEnum.Fight_Begin;
	}

	public override void SetValue (int value)
	{
		this.intValue = value;
		if(this.intValue > int.Parse(this.conditionValue))
		{
			this.itemStatus = false;
		}
	}

	public override void Trigger ()
	{
		this.SetValue (UserManager.CurUserInfo.CurWarfare);
	}
}
