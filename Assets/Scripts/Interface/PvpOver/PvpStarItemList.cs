using UnityEngine;
using System;
using System.Collections.Generic;

public class PvpStarItemList : MonoBehaviour 
{
	public PvpStarItem[] itemList;
	public UISprite[] backItemList;
	
	private PvpStarItem[] arenaItemList;
	private UISprite[] arenaBackItemList;

	/// <summary>
	/// 设置星级数
	/// </summary>
	/// <param name="level">Level.</param>
	public void InitItemListData(int level, bool lightStatus = false)
	{
		int maxStar = 0;
		ArenaData arenaData = ConfigManager.ArenaConfig.GetArenaByLv (level);
		if(arenaData != null) maxStar = arenaData.RankStar;

		if(maxStar == 0)
		{
			this.ShowData(false);
		}else{
			this.ShowData(true);
		}

		for(int index = 0; index < this.itemList.Length; index ++)
		{
			this.itemList[index].gameObject.SetActive(false);
			this.backItemList[index].gameObject.SetActive(false);
		}
		if(maxStar == 3)
		{
			this.arenaItemList = new PvpStarItem[] {this.itemList[1], this.itemList[2], this.itemList[3]};
			this.arenaBackItemList = new UISprite[] {this.backItemList[1], this.backItemList[2], this.backItemList[3]};
		}else if(maxStar == 5)
		{
			this.arenaItemList = new PvpStarItem[] {this.itemList[0], this.itemList[1], this.itemList[2], this.itemList[3], this.itemList[4]};
			this.arenaBackItemList = new UISprite[] {this.backItemList[0], this.backItemList[1], this.backItemList[2], this.backItemList[3], this.backItemList[4]};
		}
		
		for(int index = 0; index < maxStar; index ++)
		{
			this.arenaItemList[index].star = index + 1;
			this.arenaItemList[index].gameObject.SetActive(true);
			this.arenaBackItemList[index].gameObject.SetActive(true);
			// 初始化数据为 0
			if(!lightStatus)
			{
				this.arenaItemList[index].InitData(0);
			}else
			{
				Debug.Log(maxStar);
				this.arenaItemList[index].InitData(maxStar);
			}
		}
	}

	public void InitData(int oriExp, int oriLevel, int newLevel)
	{
		if(this.arenaItemList == null) return;

		ArenaData oriArenaData = ConfigManager.ArenaConfig.GetArenaByLv (oriLevel);

		int oriStar = 0;
		if(oriArenaData != null) oriStar = oriExp - oriArenaData.RankExp;

		foreach(PvpStarItem pvpStarItem in this.arenaItemList)
		{
			if(pvpStarItem != null)
			{
				pvpStarItem.InitData(oriStar);
			}
		}
	}

	/// <summary>
	/// 更新等级
	/// </summary>
	/// <param name="star">Star.</param>

	private List<PvpStarItem> starIemList;

	private Action changeCallback;
	private Action endCallback;

	public void ChangeData(int oriExp, int oriLevel, int newExp, int newLevel, Action changeCallback, Action endCallback)
	{
		this.changeCallback = changeCallback;
		this.endCallback = endCallback;

		if(oriExp == newExp && oriLevel == newLevel)
		{
			if(this.endCallback != null) this.endCallback();
			return;
		}

		int effectType = PvpStarItem.ZERO;
		int star = 0;

		this.starIemList = this.GetItemList (oriExp, oriLevel, newExp, newLevel, false, ref effectType, ref star);

		// 处理效果
		this.ProgressOriStar (effectType, star, oriExp, oriLevel, newExp, newLevel);
	}

	private void ProgressOriStar(int effectType, int star, int oriExp, int oriLevel, int newExp, int newLevel)
	{
		if(this.starIemList.Count > 0)
		{
			PvpStarItem pvpStarItem = this.starIemList[0];
			pvpStarItem.ChangeData(effectType, star, ()=>
			{
				// 移除位置 0 处的数据
				this.starIemList.RemoveAt(0);
				this.ProgressOriStar(effectType, star, oriExp, oriLevel, newExp, newLevel);
			});
		}else
		{
			if(oriLevel != newLevel)
			{
				if(this.changeCallback != null) this.changeCallback();

				if(effectType == PvpStarItem.SUB)
				{
					this.InitItemListData(newLevel, true);
				}else{
					this.InitItemListData(newLevel, false);
				}

				int newEffectType = PvpStarItem.ZERO;
				int newStar = 0;

				this.starIemList = this.GetItemList(oriExp, oriLevel, newExp, newLevel, true, ref newEffectType, ref newStar);

				this.ProgressNewStar(newEffectType, newStar);
			}else{
				if(this.endCallback != null) this.endCallback();
			}
		}
	}

	private void ProgressNewStar(int effectType, int star)
	{
		if(this.starIemList.Count > 0)
		{
			PvpStarItem pvpStarItem = this.starIemList[0];
			pvpStarItem.ChangeData(effectType, star, ()=>
			{
				// 移除位置 0 处的数据
				this.starIemList.RemoveAt(0);
				this.ProgressNewStar(effectType, star);
			});
		}else
		{
			if(this.endCallback != null) this.endCallback();
		}
	}

	private List<PvpStarItem> GetItemList(int oriExp, int oriLevel, int newExp, int newLevel, bool newStatus, ref int effectType, ref int star)
	{
		List<PvpStarItem> resultList = new List<PvpStarItem>();

		ArenaData oriArenaData = ConfigManager.ArenaConfig.GetArenaByLv (oriLevel);
		ArenaData newArenaData = ConfigManager.ArenaConfig.GetArenaByLv (newLevel);
		// 如果数据不正确
		if(oriArenaData == null || newArenaData == null || this.arenaItemList == null) return resultList;
		
		int oriStar = oriExp - oriArenaData.RankExp;
		int newStar = newExp - newArenaData.RankExp;
		
		if(oriStar < 0) oriStar = 0;
		if(newStar < 0) newStar = 0;

		Debug.Log("ori Star : " + oriStar + ", new Star : " + newStar);
		
		// 如果等级降低
		if(oriLevel > newLevel)
		{
			if(!newStatus)
			{
				effectType = PvpStarItem.SUB;
				star = 0;

				int beginStar = oriStar - 1;
				if(beginStar < 0) beginStar = 0;
				
				for(int index = beginStar; index >= 0; index --)
				{
					resultList.Add(this.arenaItemList[index]);
				}
			}else
			{
				effectType = PvpStarItem.SUB;
				star = newStar;
				
				for(int index = newArenaData.RankStar - 1; index >= newStar; index --)
				{
					resultList.Add(this.arenaItemList[index]);
				}
			}
		}
		else if(oriLevel < newLevel)
		{
			if(!newStatus)
			{
				// 如果等级升高
				effectType = PvpStarItem.ADD;
				star = oriArenaData.RankStar;
				
				for(int index = oriStar; index < oriArenaData.RankStar; index ++)
				{
					resultList.Add(this.arenaItemList[index]);
				}
			}
			else
			{
				// 如果等级升高
				effectType = PvpStarItem.ADD;
				star = newStar;
				
				for(int index = 0; index < newStar; index ++)
				{
					resultList.Add(this.arenaItemList[index]);
				}
			}
		}
		else
		{
			// 如果星级升高
			if(oriStar < newStar) // 1, 2
			{
				effectType = PvpStarItem.ADD;
				star = newStar;

				if(newStar > oriArenaData.RankStar) newStar = oriArenaData.RankStar;

				for(int index = oriStar; index < newStar; index ++)
				{
					resultList.Add(this.arenaItemList[index]);
				}
			}
			else if(oriStar > newStar) // 2, 1
			{
				effectType = PvpStarItem.SUB;
				star = newStar; 

				int beginStar = oriStar - 1;
				if(beginStar < 0) beginStar = 0;

				int endStar = newStar - 1;

				for(int index = beginStar; index > endStar; index --)
				{
					resultList.Add(this.arenaItemList[index]);
				}
			}
		}
		
		return resultList;
	}

	public void ShowData(bool active)
	{
		for(int index = 0; index < this.itemList.Length; index ++)
		{
			this.itemList[index].gameObject.SetActive(true);
			this.backItemList[index].gameObject.SetActive(true);
		}
	}
}
