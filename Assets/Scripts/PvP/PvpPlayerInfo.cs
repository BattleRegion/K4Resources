using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;
using SimpleJson;

public class PvpPlayerInfo : MonoBehaviour
{
    #region 属性

	public PvpGameControl gameControl;
	public PvpPlayerInfoItem TopItem;
	public PvpPlayerInfoItem BottomItem;

	public CoolingTimeUI coolingTimeUI;
	public GameObject SurrenderBtn;
	public PvpCoolingTime pvpCoolingTime;

    #endregion

	void Awake()
	{
		UIEventListener.Get(this.SurrenderBtn).onClick = (go) => 
		{
			// 如果战斗已经结束！
			if(gameControl.messageFightEndStatus) return;

			gameControl.PvpSurrender.SetActive(true);
		};
	}

	/// <summary>
	/// 刷新技能 CD
	/// </summary>
	public void RefreshSkillCd(PvpFightUnit selfItem, PvpFightUnit targetItem)
	{
		this.BottomItem.RefreshCd(selfItem);
	}

	/// <summary>
	/// 设置面板显示状态
	/// </summary>
	/// <param name="active">If set to <c>true</c> active.</param>
	public void ChangeActiveStatus(bool active, bool all = false)
	{
		if(this.TopItem != null) this.TopItem.gameObject.SetActive(active);
		if(this.BottomItem != null) this.BottomItem.gameObject.SetActive(active);
		if(all)
		{
			if(this.coolingTimeUI != null) this.coolingTimeUI.gameObject.SetActive(active);
			if(this.SurrenderBtn != null) this.SurrenderBtn.gameObject.SetActive(active);
		}
	}

	public void ChangeDelayActiveStatus(bool active)
	{
		if(this.coolingTimeUI != null) this.coolingTimeUI.gameObject.SetActive(active);
		if(this.SurrenderBtn != null) this.SurrenderBtn.gameObject.SetActive(active);
	}

	public void ChangeCoolingTime(float time, bool self, int roundTime)
	{
		if(roundTime != -1) time = roundTime / 1000f;

		if(this.pvpCoolingTime != null) this.pvpCoolingTime.Run(time, self, this.gameControl);
	}

	public void StopCoolingTime()
	{
		if(this.pvpCoolingTime != null) this.pvpCoolingTime.Stop();
	}

	public void SetPowerData()
	{
		this.TopItem.SetPowerData ();
		this.BottomItem.SetPowerData ();
	}

	/// <summary>
	/// 显示模型
	/// </summary>
	/// <param name="pvpUserInfo">Pvp user info.</param>
	/// <param name="status">If set to <c>true</c> status.</param>
	public void ShowAvatar(PvpUserInfo pvpUserInfo, bool status = false)
	{
		if(!status)
		{
			this.BottomItem.ShowCharCard(pvpUserInfo, false);
		}else{
			this.TopItem.ShowCharCard(pvpUserInfo, true);
		}
	}

	public void ResetFocusPetAvata()
	{
		if(this.BottomItem != null) this.BottomItem.ResetFocusPetAvata();
	}
}
