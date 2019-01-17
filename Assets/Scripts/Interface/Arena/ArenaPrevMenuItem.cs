using UnityEngine;
using System.Collections;

public class ArenaPrevMenuItem : MonoBehaviour, itemInterface 
{
	public delegate void ClickDelegate (ArenaPrevRankInfo arenaPrevRankInfo);
	public delegate void ClickPetDelegate(UserPet userPet);

	public UISprite codeItem;
	public UILabel txtName;
	public PlayerAvata playerAvatar;
	public ItemInterface petItem;
	public UISprite roleItem;

	private ArenaPrevMenuItem.ClickDelegate clickCallback;
	private ArenaPrevMenuItem.ClickPetDelegate clickPetCallback;
	private ArenaPrevRankInfo arenaPrevRankInfo;

	void Awake()
	{
		petItem.itemInter = this;
		UIEventListener.Get (this.roleItem.gameObject).onClick = OnRoleClickHandler;
	}

	public void ChangeData(ArenaPrevRankInfo arenaPrevRankInfo, ArenaPrevMenuItem.ClickDelegate clickCallback, ArenaPrevMenuItem.ClickPetDelegate clickPetCallback)
	{
		this.clickCallback = clickCallback;
		this.clickPetCallback = clickPetCallback;
		this.arenaPrevRankInfo = arenaPrevRankInfo;

		this.codeItem.spriteName = "";
		this.txtName.text = "";

		if(this.playerAvatar != null)
		{
			this.playerAvatar.ClearAvata();
		}

		if(this.arenaPrevRankInfo != null)
		{
			this.codeItem.spriteName = "UInum_" + this.arenaPrevRankInfo.rank;
			this.txtName.text = this.arenaPrevRankInfo.name;

			if (this.arenaPrevRankInfo.weapon != null)
			{
				this.playerAvatar.AddAvataWare(this.arenaPrevRankInfo.weapon.CurHardWareData.SkinId, DungeonEnum.FaceDirection.None);
			}
			if (this.arenaPrevRankInfo.armor != null)
			{
				this.playerAvatar.AddAvataWare(this.arenaPrevRankInfo.armor.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
			}
			if (this.arenaPrevRankInfo.helmet != null)
			{
				this.playerAvatar.AddAvataWare(this.arenaPrevRankInfo.helmet.CurHardWareData.SkinId, DungeonEnum.FaceDirection.LeftDown);
			}
			
			/*if(this.arenaPrevRankInfo.leader != null && this.arenaPrevRankInfo.leader.CurPetData != null)
			{
				this.petItem.SetItem(this.arenaPrevRankInfo.leader.Level, this.arenaPrevRankInfo.leader.CurPetData.PCost, (int)this.arenaPrevRankInfo.leader.CurHp, (int)this.arenaPrevRankInfo.leader.CurAtk, this.arenaPrevRankInfo.leader.CurPetData.PetPro, this.arenaPrevRankInfo.leader.CurPetData.Id, false, this.arenaPrevRankInfo.leader.CurPetData.Rank, this.arenaPrevRankInfo.leader.UserPetId);
			}*/
		}
	}

	public void _OnClickItem(int UserMonsterID)
	{
		//if(this.clickPetCallback != null) this.clickPetCallback(this.arenaPrevRankInfo.leader);
	}

	public void _OnLongPressItem(int UserMonsterID)
	{
		//if(this.clickPetCallback != null) this.clickPetCallback(this.arenaPrevRankInfo.leader);
	}

	private void OnRoleClickHandler(GameObject o)
	{
		if(this.clickCallback != null) this.clickCallback(this.arenaPrevRankInfo);
	}
}
