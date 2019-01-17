using UnityEngine;
using System.Collections;

/// <summary>
/// 使用 A 武器通关副本
/// </summary>
public class AchievementItem04 : AchievementItem
{
	public AchievementItem04():base(4)
	{
		this.stageType = AchievementStageTypeEnum.Fight_Begin;
	}

	public override void SetValue (int value)
	{
		if(value != int.Parse(this.conditionValue))
		{
			this.intValue = value;
			this.itemStatus = false;
		}
	}

	public override void Trigger ()
	{
		// 设置值
		this.SetValue ((int)UserManager.CurUserInfo.CurWeapon.CurHardWareData.Style);
	}
}
