using UnityEngine;
using System.Collections;

public class BaseSkillEffect
{
	protected PvpBuffData buffData;
	protected PvpFightUnit buffItem;
	protected BaseSkillItem skillItem;

	public BaseSkillEffect(PvpBuffData buffData, PvpFightUnit buffItem, BaseSkillItem skillItem)
	{
		this.buffData = buffData;
		this.buffItem = buffItem;
		this.skillItem = skillItem;
	}

	public virtual void Run()
	{

	}
}
