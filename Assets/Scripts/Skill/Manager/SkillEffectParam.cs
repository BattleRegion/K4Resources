using UnityEngine;
using System.Collections.Generic;

public class SkillEffectParam
{
	public PvpBuffData buffData;

	public BaseSkillItem skillItem;

	public PvpFightUnit sourceItem;

	public PvpFightUnit targetItem;

	public List<PvpFightUnit> targetItemList;

	public SkillEffectParam(PvpBuffData buffData, BaseSkillItem skillItem, PvpFightUnit sourceItem = null, PvpFightUnit targetItem = null, List<PvpFightUnit> targetItemList = null)
	{
		this.buffData = buffData;
		this.skillItem = skillItem;
		this.sourceItem = sourceItem;
		this.targetItem = targetItem;
		this.targetItemList = targetItemList;
	}
}
