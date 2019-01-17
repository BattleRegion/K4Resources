using UnityEngine;
using System.Collections;

public class PvpMonster : PvpOwnUnit
{
	private static int VarFightIndex = 0;
    #region MONO

	/// <summary>
	/// 战斗顺序
	/// </summary>
	public int FightIndex;

	/// <summary>
	/// 战斗回合
	/// </summary>
	public int FightRound;

	/// <summary>
	/// 战斗行走数量
	/// </summary>
	public int FightRunPower;

	/// <summary>
	/// 怪物 ID
	/// </summary>
	public string MonsterID;

	/// <summary>
	/// 断线重连状态
	/// </summary>
	public bool MonsterReboundStatus;
	
	public GameObject Element_monster;

    #endregion

    #region 重写父类
    public override void SetName()
    {
        name = "Monster:" + XPosition + "," + YPosition;
    }

	public static int GetMonsterIndex()
	{
		return VarFightIndex ++;
	}

	public void ShowElement()
	{
		if(!Element_monster)
		{
			SpriteRenderer sr= Element_monster.transform.GetComponent<SpriteRenderer>();
			sr.sprite = Resources.Load<Sprite>("Atlas/Fight/pveNewCell/" + (int)Element);
		}
	}

    #endregion
}
