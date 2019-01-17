using UnityEngine;
using System.Collections;

public class PvpSkillCdData
{
	public SkillData skillData;

	public int petID;

	public int cd;

	public PvpSkillCdData(SkillData skillData, int petID, int cd = 0)
	{
		this.skillData = skillData;
		this.petID = petID;
		this.cd = cd;
	}
}
