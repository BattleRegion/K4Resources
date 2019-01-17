using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class PvpOverUI : MonoBehaviour 
{
	public MainButtons ArenaButton;
	public PvpIconItem iconItem;
	public PvpStarItemList starItemList;
	public UILabel txtName;

	/// <summary>
	/// 战斗数据
	/// </summary>
	public static int starLevel;
	public static int starExp;

	// 任务数据
	public static JsonObject taskData;

	private bool completeStatus;

	void Awake()
	{
		UIEventListener.Get (this.gameObject).onClick = (g) => 
		{
			if(!completeStatus) return;

			UserManager.CurUserInfo.ArenaStarLevel = PvpOverUI.starLevel;
			UserManager.CurUserInfo.ArenaStarExp = PvpOverUI.starExp;
			
			this.gameObject.SetActive(false);

			// 如果有完成的任务
			if(OverControl.TaskProgressCheck(PvpOverUI.taskData))
			{
                OverControl.TaskProgressUpdate(PvpOverUI.taskData);
				UserManager.CurUserInfo.RefreashDailyTasks(PvpOverUI.taskData);
			}else
			{
				if(this.ArenaButton != null) this.ArenaButton.TriggerOnClick();
			}
		};
	}

	/*void Start()
	{
		UserManager.CurUserInfo.ArenaStarExp = 5;
		UserManager.CurUserInfo.ArenaStarLevel = 2;

		PvpOverUI.starExp = 2;
		PvpOverUI.starLevel = 1;

		this.ShowData ();
	}*/

	/// <summary>
	/// 初始化军衔名称
	/// </summary>
	/// <param name="level">Level.</param>
	private void InitData(int level)
	{
		ArenaData arenaData = ConfigManager.ArenaConfig.GetArenaByLv (level);
		if(arenaData != null)
		{
			this.txtName.text = arenaData.RankName;
		}else
		{
			this.txtName.text = "";
		}
	}

	/// <summary>
	/// 显示数据
	/// </summary>
	public void ShowData()
	{
		this.completeStatus = false;

		// 显示军衔名称
		this.InitData (UserManager.CurUserInfo.ArenaStarLevel);
		// 显示军衔图标
		this.iconItem.InitData (UserManager.CurUserInfo.ArenaStarLevel);
		// 显示军衔星级数量
		this.starItemList.InitItemListData (UserManager.CurUserInfo.ArenaStarLevel);
		// 显示军衔星级
		this.starItemList.InitData (UserManager.CurUserInfo.ArenaStarExp, UserManager.CurUserInfo.ArenaStarLevel, PvpOverUI.starLevel);

		this.StartCoroutine (this.ShowDataEnumerator());
	}

	/// <summary>
	/// 动画展示
	/// </summary>
	/// <returns>The data enumerator.</returns>
	private IEnumerator ShowDataEnumerator()
	{
		yield return new WaitForSeconds (0.1f);

		this.starItemList.ChangeData (UserManager.CurUserInfo.ArenaStarExp, UserManager.CurUserInfo.ArenaStarLevel, PvpOverUI.starExp, PvpOverUI.starLevel,
		()=>
		{
			this.InitData (PvpOverUI.starLevel);
			this.iconItem.ChangeData(UserManager.CurUserInfo.ArenaStarLevel, PvpOverUI.starLevel);
		},
		()=>
		{
			this.completeStatus = true;
		});
	}

	public static void ChangeData(int starLevel, int starExp)
	{
		PvpOverUI.starLevel = starLevel;
		PvpOverUI.starExp = starExp;
	}
}
