using UnityEngine;
using System.Collections;

public class PvpEgg : PvpFightUnit 
{
	public int eggType;

	public string eggID;

	public string summonMonsterID;

	public InfoLabel hpInfoItem;

	public override void SetName()
	{
		name = "Egg:" + XPosition + "," + YPosition;
	}

	public override void RefreshHp(bool refresh = true)
	{
		base.RefreshHp (refresh);
		if(this.CurHp < 0) this.CurHp = 0;

		if(hpInfoItem != null) this.hpInfoItem.SetNum(this.CurHp + "/" + this.Hp, -1, true);
	}
}
