using UnityEngine;
using System.Collections.Generic;

public class ArenaThisMenuItemList : MonoBehaviour 
{
	public UIGrid uiGrid;
	public UIPanel uiPanel;
	public ListViewAnime listViewAnime;
	public GameObject arenaThisMenuObject;
	public GameObject itemPool;

	public GameObject backBar;
	public GameObject mainBoard;

	public GameObject currentView;
	public GameObject targetView;
	public ArenaRoleDetail arenaRoleDetail;
	public ArenaMainUI arenaMainUI;
	public SetMonsterDetail monsterDetail;

	private IList<ArenaThisMenuItem> itemList;

	public void ChangeData()
	{
		// 更新我的竞技场排名
		this.arenaMainUI.ChangeRankData ();

		if(this.itemList == null) this.itemList = new List<ArenaThisMenuItem>();

		List<ArenaThisRankInfo> dataList = UserManager.CurUserInfo.SeasonRankInfoList;
		int len = dataList.Count;

		GameObject newObjectItem;
		ArenaThisMenuItem newMenuItem;

		List<GameObject> animeList = new List<GameObject>();

		//len = 100; 测试用

		for(int index = 0; index < len; index ++)
		{
			if(index < this.itemList.Count)
			{
				newMenuItem = this.itemList[index];
				newMenuItem.transform.parent = this.uiGrid.transform;
			}else
			{
				newObjectItem = NGUITools.AddChild(this.uiGrid.gameObject, this.arenaThisMenuObject);
				newMenuItem = newObjectItem.GetComponent<ArenaThisMenuItem>();
				this.itemList.Add(newMenuItem);
			}
			if(newMenuItem != null)
			{
				animeList.Add(newMenuItem.gameObject);
				newMenuItem.ChangeData(dataList[index], OnThisItemClickHandler, OnThisPetItemClickHandler);
				newMenuItem.gameObject.SetActive(false);
			}
		}

		for(int index = len; index < this.itemList.Count; index ++)
		{
			this.itemList[index].transform.parent = this.itemPool.transform;
		}

		this.uiGrid.Reposition ();
		this.listViewAnime.cells = animeList;

		this.ResetPosition ();

		this.listViewAnime.enabled = true;
	}

	public void ResetPosition()
	{
		this.uiPanel.transform.localPosition = new Vector3 (0f, -14f, 0f);
		this.uiPanel.clipOffset = new Vector2 (0f, 0f);
	}

	private void OnThisItemClickHandler(ArenaThisRankInfo arenaThisRankInfo)
	{
		this.arenaRoleDetail.SetRoleDetail (arenaThisRankInfo);

		AnimationHelper.AnimationMoveTo(new Vector3(-800, this.backBar.transform.localPosition.y, this.backBar.transform.localPosition.z), this.backBar, iTween.EaseType.linear, null, null, 0.2f);
		AnimationHelper.AnimationMoveTo(new Vector3(800, this.mainBoard.transform.localPosition.y, this.mainBoard.transform.localPosition.z), this.mainBoard, iTween.EaseType.linear, gameObject, "SceneSwitch", 0.2f);
	}
	
	private void OnThisPetItemClickHandler(UserPet userPet)
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
		this.listViewAnime.enabled = false;
		// 如果大于拉取时间，拉取数据
		if(Time.time - UserManager.CurUserInfo.ArenaSeasonTime >= UserManager.CurUserInfo.ArenaSeasonTimeDelay)
		{
			// 设置延迟时间
			UserManager.CurUserInfo.ArenaSeasonTime = (int)Time.time;
			ArenaUI.GetSeasonRankRequest((result) =>
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
		this.listViewAnime.enabled = false;

		if(this.itemList != null)
		{
			foreach(ArenaThisMenuItem menuItem in this.itemList)
			{
				menuItem.gameObject.SetActive(false);
			}
		}
	}
}
