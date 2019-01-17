using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;

public class PvpPlayerInfoItem : MonoBehaviour 
{
	public HpUI HpUI;
	public InfoLabel powerInfo;
	public GameObject PowerValue;
	public InfoLabel  BoutLab;
    public string PositionType;
	public List<PetAvata> UserPetAvatas = new List<PetAvata>();
	public GameObject PartyBoard;

	public void RefreshPlayerInfo(PvpUserInfo pvpUserInfo)
	{
		//刷新怪物信息
		//排序怪物
		/*pvpUserInfo.UserPets.Sort (delegate(UserPet x, UserPet y) {
			if(x.MapIndex < y.MapIndex)
			{
				return -1;
			}else if(x.MapIndex > y.MapIndex)
			{
				return 1;
			}
			return 0;
		});*/
		
		int index = 0;
		foreach (UserPet pet in pvpUserInfo.UserPets)
		{
			//if (pet.MapIndex != -1)
			//{
				UserPetAvatas[index].SetUserPet(pet);
				index++;
			//}
		}
		
	}

	public void ShowCharCard(PvpUserInfo pvpUserInfo, bool status = false)
	{
		return;
		GameObject playerAnime = null;
		if(!status)
		{
			playerAnime = Resources.Load("PreFabs/Characters/S10060") as GameObject;         
		}else{
			playerAnime = Resources.Load("PreFabs/Characters/S10040") as GameObject;         
		}
		GameObject temp = NGUITools.AddChild(PartyBoard, playerAnime);
		PlayerAvata CurPlayerAvata = temp.GetComponent<PlayerAvata>();
		
		PetUIController.SetLayer(temp.transform, LayerHelper.BasicFX);
		if(!status)
		{
			temp.transform.localScale = new Vector3(90f, 90f, 1f);
		}else{
			temp.transform.localScale = new Vector3(-90f, 90f, 1f);
		}
		temp.transform.localPosition = new Vector3(-8f, -80f, -1);

		if (pvpUserInfo.CurWeapon != null)
		{
			string curWeaponSkinId = pvpUserInfo.CurWeapon.CurHardWareData.SkinId;
			CurPlayerAvata.AddAvataWare(curWeaponSkinId, DungeonEnum.FaceDirection.None);
		}
		if (pvpUserInfo.CurHelmet != null)
		{
			string curHelmetSkinId = pvpUserInfo.CurHelmet.CurHardWareData.SkinId;
			CurPlayerAvata.AddAvataWare(curHelmetSkinId, DungeonEnum.FaceDirection.LeftDown);
		}
		if (pvpUserInfo.CurArmor != null)
		{
			string curArmorSkinId = pvpUserInfo.CurArmor.CurHardWareData.SkinId;
			CurPlayerAvata.AddAvataWare(curArmorSkinId, DungeonEnum.FaceDirection.LeftDown);
		}

		CurPlayerAvata.WeaponEffectShow();

		Animator ac = temp.GetComponent<Animator>();
		   
		ac.speed = 0;
	}

	public void SetPowerData()
	{
		Debug.Log ("设置宠物数据 ： xx");
		foreach(PetAvata petAvata in this.UserPetAvatas)
		{
			petAvata.SetPowerData();
		}
	}

	public void RefreshCd(PvpFightUnit pvpFightUnit)
	{
		foreach(PetAvata petAvata in this.UserPetAvatas)
		{
			petAvata.RefreshCd(pvpFightUnit);
		}
	}

	public void SetBoutLabNum(int n)
	{
		if(n < 0) n = 0;

		//回合数设置
		if(n.ToString().Length == 1)
		{
			BoutLab.BeginX = BoutLab.transform.localPosition.x - 10;
		}else{
			BoutLab.BeginX = BoutLab.transform.localPosition.x - 18;
		}
		BoutLab.SetNum(n.ToString());
	}

	public void SetPowerValue(float _pro, float currentPower)
	{        
		string hpString = ((int)(currentPower / 10f)).ToString();
		if(powerInfo != null)
		{
			powerInfo.SetNum(hpString);
			powerInfo.transform.localPosition = new Vector3(210 - powerInfo.TotalWidth / 2, 1, 0);
		}
		if(PowerValue != null)
		{
			PowerValue.renderer.material.SetFloat("_Progress", _pro);
		}
	}

	public void FocusPetAvata(DungeonEnum.ElementAttributes attribute, int count)
	{
		List<PetAvata> attributePa = FindSameAttributePetAvata(attribute);
		
		
		int totalCount = count;
		if (count > attributePa.Count)
		{
			totalCount = attributePa.Count;
		}
		for (int i = 0; i < totalCount; i++)
		{
			PetAvata pa = attributePa[i];
			pa.AvatarFocus(true);
		}
	}
	
	List<PetAvata> FindSameAttributePetAvata(DungeonEnum.ElementAttributes attribute)
	{
		List<PetAvata> attributePa = new List<PetAvata>();
		foreach (PetAvata pa in UserPetAvatas)
		{            
			if (pa.CurPet != null)
			{
				if (pa.CurPet.CurPetData.PetPro == attribute || pa.CurPet.CurPetData.PetPro == DungeonEnum.ElementAttributes.None)
				{
					attributePa.Add(pa);
				}
			}
		}
		return attributePa;
	}

	public void ResetFocusPetAvata()
	{
		foreach (PetAvata pa in this.UserPetAvatas)
		{            
			if (pa.CurPet != null)
			{
				pa.AvatarFocus(false);
			}
		}
	}
	
	public void DisFocusPetAvata(DungeonEnum.ElementAttributes attribute, int count)
	{
		List<PetAvata> attributePa = FindSameAttributePetAvata(attribute);
		//取消全部
		if (count == -1)
		{
			foreach (PetAvata pa in attributePa)
			{
				pa.AvatarFocus(false);
			}
		}
		else
		{
			List<PetAvata> curFocusPetAvatas = new List<PetAvata>();
			int focusCount = 0;
			foreach (PetAvata pa in attributePa)
			{
				if (pa.hasFocus == true)
				{
					curFocusPetAvatas.Add(pa);
					focusCount++;
				}
			}
			if (count < focusCount)
			{
				curFocusPetAvatas[curFocusPetAvatas.Count - 1].AvatarFocus(false);
			}
		}
	}
	
	/// <summary>
	/// 获取已经焦点的宠物头像
	/// </summary>
	/// <returns></returns>
	public List<PetAvata> GetFocusPetAvata()
	{
		List<PetAvata> focusPa = new List<PetAvata>();
		foreach (PetAvata pa in UserPetAvatas)
		{
			if (pa.hasFocus == true)
			{
				focusPa.Add(pa);
			}
		}
		return focusPa;
	}
	
	
	public PetAvata FindPetAvata(UserPet userPet)
	{
		foreach (PetAvata pa in UserPetAvatas)
		{
			if (pa.CurPet == userPet)
			{
				return pa;
			}
		}
		return null;
	}
	
	public void AvataEffectRender(bool render, UserPet userPet)
	{
		PetAvata petAvata = FindPetAvata (userPet);
		if(petAvata != null) petAvata.AvataEffectiveRender(render);
	}

	public void AvataProgress(float _pro, UserPet userPet)
	{
		PetAvata petAvata = FindPetAvata (userPet);
		if(petAvata != null) petAvata.AvataProgress(_pro);
	}
}
