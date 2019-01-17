using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 不携带 A 元素属性的英雄通关
/// </summary>
public class AchievementItem07 : AchievementItem
{
	public AchievementItem07():base(7)
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
		List<UserPet> petList = UserManager.CurUserInfo.CurPets;
		foreach(UserPet userPet in petList)
		{
			SetValue((int)userPet.CurPetData.PetPro);
		}
	}
}
