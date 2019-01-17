using UnityEngine;
using System.Collections;

/// <summary>
/// 不穿戴 A 元素属性的装备通关
/// </summary>
public class AchievementItem08 : AchievementItem
{
	public AchievementItem08():base(8)
	{
		this.stageType = AchievementStageTypeEnum.Fight_Begin;
	}

	public override void SetValue (int value)
	{
		if(value == int.Parse(this.conditionValue))
		{
			this.itemStatus = false;
		}
	}

	public override void Trigger ()
	{
		if(UserManager.CurUserInfo.CurWeapon != null)
		{
			this.SetValue((int)UserManager.CurUserInfo.CurWeapon.CurHardWareData.Element);
		}
		if(UserManager.CurUserInfo.CurHelmet != null)
		{
			this.SetValue((int)UserManager.CurUserInfo.CurHelmet.CurHardWareData.Element);
		}
		if(UserManager.CurUserInfo.CurArmor != null)
		{
			this.SetValue((int)UserManager.CurUserInfo.CurArmor.CurHardWareData.Element);
		}
	}
}
