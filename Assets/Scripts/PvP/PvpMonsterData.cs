using UnityEngine;
using System.Collections;

public class PvpMonsterData
{
	public string monsterID;

	public int monsterRound;

	public PvpFightUnit pvpFightUnit;

	public PvpMonsterData(string monsterID, int monsterRound, PvpFightUnit pvpFightUnit)
	{
		this.monsterID = monsterID;
		this.monsterRound = monsterRound;
		this.pvpFightUnit = pvpFightUnit;
	}
}
