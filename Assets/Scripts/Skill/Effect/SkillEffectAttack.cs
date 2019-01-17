using UnityEngine;
using System.Collections;

public class SkillEffectAttack : BaseSkillEffect
{
	protected float hurtValue;

	public SkillEffectAttack(PvpBuffData buffData, PvpFightUnit buffItem, BaseSkillItem skillItem, float hurtValue):base(buffData, buffItem, skillItem)
	{
		this.hurtValue = hurtValue;
	}
	
	public override void Run()
	{
		if(buffItem != null) buffItem.ChangeHpShow(this.hurtValue);
	}
}
