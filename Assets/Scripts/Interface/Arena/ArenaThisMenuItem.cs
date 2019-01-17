using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaThisMenuItem : MonoBehaviour, itemInterface  
{
	public delegate void ClickDelegate (ArenaThisRankInfo arenaThisRankInfo);
	public delegate void ClickPetDelegate(UserPet userPet);

	public List<UISprite> codeItemList;
	public UILabel txtName;
	public UILabel txtLevel;
	public PlayerAvata playerAvatar;
	public ItemInterface petItem;

	private ArenaThisMenuItem.ClickDelegate clickCallback;
	private ArenaThisMenuItem.ClickPetDelegate clickPetCallback;
	private ArenaThisRankInfo arenaThisRankInfo;

	void Awake()
	{
		petItem.itemInter = this;
	}

	public void ChangeData(ArenaThisRankInfo arenaThisRankInfo, ArenaThisMenuItem.ClickDelegate clickCallback, ArenaThisMenuItem.ClickPetDelegate clickPetCallback)
	{
		this.clickCallback = clickCallback;
		this.clickPetCallback = clickPetCallback;
		this.arenaThisRankInfo = arenaThisRankInfo;

		this.SetRankLevel (0);
		this.txtName.text = "";
		this.txtLevel.text = "";

		if(this.playerAvatar != null)
		{
			this.playerAvatar.ClearAvata();
		}

		if(this.arenaThisRankInfo != null)
		{
			this.SetRankLevel(this.arenaThisRankInfo.rank);
			this.txtName.text = this.arenaThisRankInfo.name;
			this.txtLevel.text = "lv." + this.arenaThisRankInfo.level;

			if (this.arenaThisRankInfo.weapon != null)
			{
				this.playerAvatar.AddAvataWare(this.arenaThisRankInfo.weapon.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
			}
			if (this.arenaThisRankInfo.armor != null)
			{
				this.playerAvatar.AddAvataWare(this.arenaThisRankInfo.armor.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
			}
			if (this.arenaThisRankInfo.helmet != null)
			{
				this.playerAvatar.AddAvataWare(this.arenaThisRankInfo.helmet.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
			}

			if(this.arenaThisRankInfo.leader != null && this.arenaThisRankInfo.leader.CurPetData != null)
			{
				this.petItem.SetItem(this.arenaThisRankInfo.leader);
			}
		}
	}

	private void SetRankLevel(int num)
	{
		string numText = num.ToString ();
		for(int index = 0; index < this.codeItemList.Count; index ++)
		{
			if(index < numText.Length)
			{
				this.codeItemList[index].spriteName = "UInum_" + numText[index];
				this.codeItemList[index].gameObject.SetActive(true);
			}else{
				this.codeItemList[index].gameObject.SetActive(false);
			}
		}
	}

	public void _OnClickItem(int UserMonsterID)
	{
		//if(this.clickPetCallback != null) this.clickPetCallback(this.arenaThisRankInfo.leader);
	}
	
	public void _OnLongPressItem(int UserMonsterID)
	{
		//if(this.clickPetCallback != null) this.clickPetCallback(this.arenaThisRankInfo.leader);
	}

	public void ItemClickHandler()
	{
		if(this.clickCallback != null) this.clickCallback(this.arenaThisRankInfo);
	}
}
