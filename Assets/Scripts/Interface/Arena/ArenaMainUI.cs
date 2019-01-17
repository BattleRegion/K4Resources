using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;

public class ArenaMainUI : MonoBehaviour
{
	public GameObject partyUI;

	public UILabel txtName;
	public ArenaStarItemList arenaStarItemList;
	public ArenaIconItem arenaIconItem;
	public UILabel txtSeason;
	public UILabel txtDay;
	public UILabel txtRank;
	//public UILabel txtTime;
	public UIButton btnFight;
	public GameObject NoteResultBoard;
	public UILabel NoteInfoLabel;
	public GameObject CostResultBoard;
	public UILabel CostInfoLabel;
	
	private ArenaData arenaData;
	private ParamData paramData;
	
	public GameObject ListMask_1;
	public GameObject ListMask_2;
	
	/// <summary>
	/// 更新界面数据
	/// </summary>
	public void ChangeData()
	{
		this.arenaData = ConfigManager.ArenaConfig.GetArenaByLv(UserManager.CurUserInfo.ArenaStarLevel);
		this.paramData = ConfigManager.ParamConfig.GetParam();
		
		//初始化刷新时间数据
		
		/*if (UserManager.CurUserInfo.ArenaRefreshTime.Year == 1)
        {
            DateTime arenaRefreshTime = ConfigManager.LocalTime.LocalTime;
            string[] arenaReTimeData = paramData.ArenaReTime.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            if (arenaReTimeData.Length == 3)
            {
                arenaRefreshTime = new DateTime(arenaRefreshTime.Year, arenaRefreshTime.Month, arenaRefreshTime.Day, int.Parse(arenaReTimeData[0]), int.Parse(arenaReTimeData[1]), int.Parse(arenaReTimeData[2]));
            }
            UserManager.CurUserInfo.ArenaRefreshTime = arenaRefreshTime;
        }
        // 如果当前时间超过了设置的刷新时间
        if (UserManager.CurUserInfo.ArenaRefreshTime.Subtract(ConfigManager.LocalTime.LocalTime).Ticks < 0)
        {
            // 重置刷新次数
            UserManager.CurUserInfo.ArenaFreeTimes = paramData.ArenaFreeRate;
            // 设置刷新时间为明天
            UserManager.CurUserInfo.ArenaRefreshTime = UserManager.CurUserInfo.ArenaRefreshTime.AddDays(1);
        }

        DateTime beginDateTime = DateTime.Now;
        DateTime endDateTime = DateTime.Now;

        string[] arenaTimeData = paramData.ArenaTime.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
        if (arenaTimeData.Length == 2)
        {
            beginDateTime = DateTime.Parse(arenaTimeData[0]);
            endDateTime = DateTime.Parse(arenaTimeData[1]);
        }*/
		
		if (arenaData != null)
		{
			this.txtName.text = arenaData.RankName;
			if(arenaData.RankStar != 0)
			{
				// 显示状态
				this.arenaStarItemList.ShowData(true, 0);
				// 设置星级
				this.arenaStarItemList.ChangeData(UserManager.CurUserInfo.ArenaStarExp - arenaData.RankExp, arenaData.RankStar);
			}
			else
			{
				// 显示状态
				this.arenaStarItemList.ShowData(false, UserManager.CurUserInfo.ArenaRank);
			}
			// 设置图标
			this.arenaIconItem.ChangeData(UserManager.CurUserInfo.ArenaStarLevel);
		}
		
		// 如果不在赛季日期范围内
		/*
        if (ConfigManager.LocalTime.LocalTime < beginDateTime || ConfigManager.LocalTime.LocalTime > endDateTime)
        {
            this.txtSeason.text = "null";
            this.txtDay.text = "null";
            this.txtTime.text = "null";
        }
        else
        {
            this.txtSeason.text = paramData.ArenaName;
            this.txtDay.text = "剩余 " + endDateTime.Subtract(ConfigManager.LocalTime.LocalTime).Days.ToString() + " 天";
            this.txtTime.text = "剩余 " + UserManager.CurUserInfo.ArenaFreeTimes.ToString() + " 场";
        }*/
	}
	
	/// <summary>
	/// 更新排名
	/// </summary>
	public void ChangeRankData()
	{
		this.txtRank.text = UserManager.CurUserInfo.ArenaRank.ToString();
		this.arenaStarItemList.InitRankData (UserManager.CurUserInfo.ArenaRank);
	}
	
	void OnEnable()
	{
		ListMask_1.transform.localPosition = new Vector3 (0.2f, 0.5813999f, -0.5f);
		ListMask_2.transform.localPosition = new Vector3 (0.2f, -1.203503f, -0.5f);
		ListMask_1.SetActive(true);
		ListMask_2.SetActive(true);

		if(this.partyUI != null) this.partyUI.gameObject.SetActive(false);
		
		this.ChangeData ();
	}
	
	void OnDisable()
	{
		if(ListMask_1 != null) ListMask_1.SetActive(false);
		if(ListMask_2 != null) ListMask_2.SetActive(false);
	}
	
	/// <summary>
	/// 花费面板点击报名
	/// </summary>
	public void SubmitGetTicketRequestClickHandler()
	{
		/*ArenaUI.GetTicketRequest((r) =>
		{
			this.TicketRequestProgress(r);
		});
		this.CostResultBoard.gameObject.SetActive(false);*/
	}
	
	/// <summary>
	/// 花费面板取消报名
	/// </summary>
	/// <returns><c>true</c> if this instance cancel get ticket request click handler; otherwise, <c>false</c>.</returns>
	public void CancelGetTicketRequestClickHandler()
	{
		this.CostResultBoard.gameObject.SetActive(false);
	}
	
	/// <summary>
	/// 隐藏等级不足提示
	/// </summary>
	public void HiddenLevelErrorBoardResult()
	{
		this.NoteResultBoard.gameObject.SetActive(false);
	}
	
	private void TicketRequestProgress(ArenaMessageResult r)
	{
		if (r == ArenaMessageResult.Success)
		{
			// 扣除消耗
			UserManager.CurUserInfo.AddUserElements(UserManager.CurUserInfo.SeasonTicketInfo.consumeList);
			// 竞技场剩余场次
			UserManager.CurUserInfo.ArenaFreeTimes--;
			//this.txtTime.text = "剩余 " + UserManager.CurUserInfo.ArenaFreeTimes.ToString() + " 场";
			this.btnFight.isEnabled = false;
			this.NoteInfoLabel.text = "报名成功！";
			
			// 转入到新场景
			Loom.QueueOnMainThread(() =>
			                       {
				// 设置选择状态
				UserManager.CurView = ViewControl.Views.Social;
				Application.LoadLevel("PvP");
				ApplicationControl.CurApp.StopLoading();
			});
		}
		else
		{
			Loom.QueueOnMainThread(() =>
			                       {
				this.NoteInfoLabel.text = "报名失败！";
			});
		}
	}
	
	/// <summary>
	/// 点击报名
	/// </summary>
	public void GetTicketRequestClickHandler()
	{
		// 如果没有达到开启等级，提示等级不足的提示！
		if (UserManager.CurUserInfo.Level < paramData.ArenaOpenLv)
		{
			this.NoteInfoLabel.text = "需要等级达到 " + this.paramData.ArenaOpenLv + " 级！";
			this.NoteResultBoard.gameObject.SetActive(true);
			return;
		}
		// 如果还有免费刷新次数
		if (UserManager.CurUserInfo.ArenaFreeTimes > 0 || true)
		{
			this.gameObject.SetActive(false);
			if(this.partyUI != null) this.partyUI.SetActive(true);

			/*ArenaUI.GetTicketRequest((r) =>
			{
				this.TicketRequestProgress(r);
			});*/
			//this.NoteResultBoard.gameObject.SetActive(true);
		}
		else
		{
			this.CostInfoLabel.text = "免费次数已用完，确认花费 " + this.arenaData.CoinRate + " 金币报名？";
			this.CostResultBoard.gameObject.SetActive(true);
		}
	}
}
