using UnityEngine;
using System.Collections;

public class PvpSkillHouseData
{
	public int houseID;

	public SkillData skillData;

	public PvpSkillHouseData(SkillData skillData, int houseID)
	{
		this.skillData = skillData;
		this.houseID = houseID;
	}
}
