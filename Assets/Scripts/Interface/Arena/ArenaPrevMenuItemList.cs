using UnityEngine;
using System.Text;
using System.Collections.Generic;

public class ArenaPrevMenuItemList : MonoBehaviour 
{
	private const int MAX_SIZE = 3;

	public UILabel txtReward;
	public ArenaPrevMenuItem[] itemList;
	
	public GameObject backBar;
	public GameObject mainBoard;

	public GameObject currentView;
	public GameObject targetView;
	public ArenaRoleDetail arenaRoleDetail;
	public SetMonsterDetail monsterDetail;

	public void ChangeData()
	{
		ParamData paramData = ConfigManager.ParamConfig.GetParam ();

		StringBuilder stringBuilder = new StringBuilder ();
		stringBuilder.Append ("第一名：" + paramData.RewardTop1Name + "\n");
		stringBuilder.Append ("第二名：" + paramData.RewardTop2Name + "\n");
		stringBuilder.Append ("第三名：" + paramData.RewardTop3Name);

		this.txtReward.text = stringBuilder.ToString ();

		List<ArenaPrevRankInfo> dataList = UserManager.CurUserInfo.LastRankInfoList;

		for(int index = 0; index < MAX_SIZE; index ++)
		{
			if(index < dataList.Count)
			{
				this.itemList[index].gameObject.SetActive(true);
				this.itemList[index].ChangeData(dataList[index], OnPrevItemClickHandler, OnPrevPetItemClickHandler);
			}else{
				this.itemList[index].gameObject.SetActive(false);
			}
		}
	}

	private void OnPrevItemClickHandler(ArenaPrevRankInfo arenaPrevRankInfo)
	{
		this.arenaRoleDetail.SetRoleDetail (arenaPrevRankInfo);
		
		AnimationHelper.AnimationMoveTo(new Vector3(-800, this.backBar.transform.localPosition.y, this.backBar.transform.localPosition.z), this.backBar, iTween.EaseType.linear, null, null, 0.2f);
		AnimationHelper.AnimationMoveTo(new Vector3(800, this.mainBoard.transform.localPosition.y, this.mainBoard.transform.localPosition.z), this.mainBoard, iTween.EaseType.linear, gameObject, "SceneSwitch", 0.2f);
	}

	private void OnPrevPetItemClickHandler(UserPet userPet)
	{
		this.monsterDetail.gameObject.SetActive (true);
		this.monsterDetail.SetDetail (userPet);
	}
	
	void SceneSwitch()
	{
		this.currentView.SetActive(false);
		if (this.targetView != null)
		{
			this.targetView.SetActive(true);
		}
	}

	void OnEnable()
	{
		if(Time.time - UserManager.CurUserInfo.ArenaLastTime >= UserManager.CurUserInfo.ArenaLastTimeDelay)
		{
			UserManager.CurUserInfo.ArenaLastTime = (int)Time.time;
			ArenaUI.GetLastRankRequest((result) =>
			{
				if (result == ArenaMessageResult.Success)
				{
					Loom.QueueOnMainThread(() =>
					{
						this.ChangeData();
					});
				}
				else
				{
					
				}
			});
		}else{
			this.ChangeData();
		}
	}
	
	void OnDisable()
	{
		if(this.itemList != null)
		{
			foreach(ArenaPrevMenuItem menuItem in this.itemList)
			{
				menuItem.gameObject.SetActive(false);
			}
		}
	}
}
