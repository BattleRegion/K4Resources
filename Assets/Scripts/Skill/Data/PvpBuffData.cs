using UnityEngine;
using System.Collections;

public class PvpBuffData
{
	/// <summary>
	/// Buff 类别
	/// </summary>
	public int buffType;

	/// <summary>
	/// 释放目标
	/// </summary>
	public int targetType;

	/// <summary>
	/// 触发阶段类别 添加这个字段目的是为了计算固定属性、回合属性方便，
	/// </summary>
	public int stageType;

	/// <summary>
	/// 目标条件类别 比如 X 元素
	/// </summary>
	public int effectType;

	/// <summary>
	/// Float 类型值
	/// </summary>
	public float valueFloat;
	
	/// <summary>
	/// String 类型值
	/// </summary>
	public string valueString;

	/// <summary>
	/// 值是否是百分比 True 为百分比
	/// </summary>
	public bool valueType;

	/// <summary>
	/// 回合值 大于 0，说明几个回合之后会消失
	/// </summary>
	public int roundValue;

	/// <summary>
	/// 能否叠加
	/// </summary>
	public bool overlay;

	/// <summary>
	/// 延时一回合
	/// </summary>
	public bool delay;

	/// <summary>
	/// 技能 ID 只为了检测 buff 叠加用，唯一值
	/// </summary>
	public string skillID;

	public PvpBuffData(int buffType, int targetType, int stageType, int effectType, float valueFloat, bool valueType, int roundValue, bool overlay = true, string valueString = "", bool delay = false)
	{
		this.buffType = buffType;
		this.targetType = targetType;
		this.stageType = stageType;
		this.effectType = effectType;
		this.valueFloat = valueFloat;
		this.valueType = valueType;
		this.roundValue = roundValue;
		this.overlay = overlay;
		this.valueString = valueString;
		this.delay = delay;
	}
}
