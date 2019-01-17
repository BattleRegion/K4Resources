using UnityEngine;
using System.Collections;

public class ArenaStarItemList : MonoBehaviour 
{
	public UILabel txtRank;
	public UISprite[] itemList;
	public UISprite[] backItemList;

	private UISprite[] arenaItemList;
	private UISprite[] arenaBackItemList;

	public void ChangeData(int star, int maxStar)
	{
		this.InitItemListData (maxStar);

		for(int index = 0; index < maxStar; index ++)
		{
			if(index + 1 <= star)
			{
				this.arenaItemList[index].spriteName = "pvp_star";
			}else{
				this.arenaItemList[index].spriteName = "";
			}
		}
	}

	private void InitItemListData(int maxStar)
	{
		for(int index = 0; index < this.itemList.Length; index ++)
		{
			this.itemList[index].gameObject.SetActive(false);
			this.backItemList[index].gameObject.SetActive(false);
		}
		if(maxStar == 3)
		{
			this.arenaItemList = new UISprite[] {this.itemList[1], this.itemList[2], this.itemList[3]};
			this.arenaBackItemList = new UISprite[] {this.backItemList[1], this.backItemList[2], this.backItemList[3]};
		}else if(maxStar == 5)
		{
			this.arenaItemList = new UISprite[] {this.itemList[0], this.itemList[1], this.itemList[2], this.itemList[3], this.itemList[4]};
			this.arenaBackItemList = new UISprite[] {this.backItemList[0], this.backItemList[1], this.backItemList[2], this.backItemList[3], this.backItemList[4]};
		}

		for(int index = 0; index < maxStar; index ++)
		{
			this.arenaItemList[index].gameObject.SetActive(true);
			this.arenaBackItemList[index].gameObject.SetActive(true);
		}
	}

	public void InitRankData(int rank)
	{
		if(rank <= 0) rank = 100;
		this.txtRank.text = "排名：" + rank;
	}

	public void ShowData(bool active, int rank)
	{
		this.InitRankData (rank);

		this.txtRank.gameObject.SetActive (!active);

		for(int index = 0; index < this.itemList.Length; index ++)
		{
			this.itemList[index].gameObject.SetActive(active);
			this.backItemList[index].gameObject.SetActive(active);
		}
	}
}
