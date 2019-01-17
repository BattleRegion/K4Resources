using UnityEngine;
using System.Collections;

public class ArenaSetParty : SetParty 
{
	public GameObject btnFight;

	protected override void StartOperater ()
	{
		base.StartOperater ();

		if(this.btnFight != null)
		{
			UIEventListener.Get(this.btnFight).onClick = (GameObject o)=>
			{
				ArenaUI.GetTicketRequest((r) =>
				{
					this.TicketRequestProgress(r);
				}, this.selfIndex);
			};
		}
	}

	private void TicketRequestProgress(ArenaMessageResult r)
	{
		if (r == ArenaMessageResult.Success)
		{
			// 扣除消耗
			UserManager.CurUserInfo.AddUserElements(UserManager.CurUserInfo.SeasonTicketInfo.consumeList);
			// 竞技场剩余场次
			UserManager.CurUserInfo.ArenaFreeTimes--;
			// 转入到新场景
			Loom.QueueOnMainThread(() =>
			{
				ApplicationControl.CurApp.StopLoading();
				// 设置选择状态
				UserManager.CurView = ViewControl.Views.Social;
				Application.LoadLevel("PvP");
			});
		}else{
			Debug.Log("错误：" + r.ToString());
		}
	}
}
