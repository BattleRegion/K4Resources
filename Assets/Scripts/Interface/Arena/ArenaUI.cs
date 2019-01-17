using System;
using UnityEngine;
using SimpleJson;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;

public class ArenaUI : MonoBehaviour 
{
	public GameObject backBar;
	public GameObject mainBoard;

	public GameObject mainUI;
	public GameObject summaryUI;
	public PartyInfo PartyControl;

	public ArenaThisMenuItemList thisMenuItemList;
	public ArenaPrevMenuItemList prevMenuItemList;

	public void ShowSummaryEvent()
	{
		AnimationHelper.AnimationMoveTo(new Vector3(800, mainBoard.transform.localPosition.y, 0), mainBoard, iTween.EaseType.linear, null, null, 0.2f);
		AnimationHelper.AnimationMoveTo(new Vector3(-800, backBar.transform.localPosition.y, 0), backBar, iTween.EaseType.linear, gameObject, "ShowEvent", 0.2f);
	}
	
	public void RefreshParty(int index = -1)
	{
		if(index == -1)
		{
			for (int i = 0; i < 5; i++)
			{
				StartCoroutine(PartyControl.refreshParty(i, false));
			}
		}
	}

	public void ShowEvent()
	{
		this.mainUI.SetActive (false);
		this.summaryUI.SetActive(true);
	}

	/// <summary>
	/// 竞技场报名
	/// </summary>
	/// <param name="callback">Callback.</param>
	public static void GetTicketRequest(Action<ArenaMessageResult> callback, int index)
	{
		JsonObject args = new JsonObject();
		args.Add("queue_id", index);
		SocketCenter.Request(GameRouteConfig.GetTicket, args, (r) =>
		{
			if (r.Code == SocketResult.ResultCode.Success)
			{
				Loom.QueueOnMainThread(() =>
				{
					UserManager.CurUserInfo.ArenaPvpTicket = int.Parse (r.Data ["ticket_id"].ToString ());
					UserManager.CurUserInfo.AddElements((JsonArray)r.Data["consumes"]);
					//UserManager.CurUserInfo.SeasonTicketInfo.AnalysisData(r.Data);
					callback(ArenaMessageResult.Success);
				});
			}
			else
			{
				Debug.Log("竞技场报名失败！");
				callback(ArenaMessageResult.Fail);
			}
		}, null, true, true);
	}

	/// <summary>
	/// 获取本赛季排名请求
	/// </summary>
	/// <param name="callback">Callback.</param>
	public static void GetSeasonRankRequest(Action<ArenaMessageResult> callback)
	{
		SocketCenter.Request(GameRouteConfig.GetSeasonRank, null, (r) =>
		{
			if (r.Code == SocketResult.ResultCode.Success)
			{
				Loom.QueueOnMainThread(() =>
				{
					// 重置我的竞技场排名
					UserManager.CurUserInfo.ArenaRank = 0;

					JsonArray dataList = (JsonArray)r.Data["ranks"];
					List<ArenaThisRankInfo> infoList = new List<ArenaThisRankInfo>();

					int index = 0;
					foreach (JsonObject dataItem in dataList)
					{
						index ++;
						ArenaThisRankInfo arenaThisRankInfo = new ArenaThisRankInfo(dataItem, index);
						infoList.Add(arenaThisRankInfo);

						// 我的竞技场排名
						if(arenaThisRankInfo.id == UserManager.CurUserInfo.UserId)
						{
							UserManager.CurUserInfo.ArenaRank = index;
						}
					}
					
					UserManager.CurUserInfo.SeasonRankInfoList = infoList;
					callback(ArenaMessageResult.Success);
				});
			}
			else
			{
				Debug.Log("获取当前赛季排名失败！");
				callback(ArenaMessageResult.Fail);
			}
		}, null, true, true);
	} 

	/// <summary>
	/// 获取上赛季排名请求
	/// </summary>
	/// <param name="callback">Callback.</param>
	public static void GetLastRankRequest(Action<ArenaMessageResult> callback)
	{
		SocketCenter.Request(GameRouteConfig.GetLastRank, null, (r) =>
		{
			if (r.Code == SocketResult.ResultCode.Success)
			{
				Loom.QueueOnMainThread(() =>
				{
					JsonArray dataList = (JsonArray)r.Data["ranks"];
					List<ArenaPrevRankInfo> infoList = new List<ArenaPrevRankInfo>();

					int index = 0;
					foreach (JsonObject dataItem in dataList)
					{
						index ++;
						ArenaPrevRankInfo arenaPrevRankInfo = new ArenaPrevRankInfo(dataItem, index);
						infoList.Add(arenaPrevRankInfo);
					}
					
					UserManager.CurUserInfo.LastRankInfoList = infoList;
					callback(ArenaMessageResult.Success);
				});
			}
			else
			{
				Debug.Log("获取上赛季排名失败！");
				callback(ArenaMessageResult.Fail);
			}
		}, null, true, true);
	} 

	void OnEnable()
	{
		this.RefreshParty ();

		this.StartCoroutine ("OnShowMenuItemList");
	}

	private IEnumerator OnShowMenuItemList()
	{
		yield return new WaitForSeconds (0.3f);
		this.thisMenuItemList.enabled = true;
		this.prevMenuItemList.enabled = true;

        GuiderLocal.TriggerGuideWithOut("openArena");
       
	}

	void OnDisable()
	{
		if(this.thisMenuItemList != null) this.thisMenuItemList.enabled = false;
		if(this.prevMenuItemList != null) this.prevMenuItemList.enabled = false;
	}
}
