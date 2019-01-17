using UnityEngine;
using System.Collections.Generic;

public class AchievementItem
{
	/// <summary>
	/// 类别
	/// </summary>
	public int itemType;

	/// <summary>
	/// 阶段
	/// </summary>
	public int stageType;

	/// <summary>
	/// 条件值
	/// </summary>
	public string conditionValue;

	/// <summary>
	/// 数据值
	/// </summary>
	public int intValue;
	public string stringValue;
	public float floatValue;

	/// <summary>
	/// 达成条件
	/// </summary>
	public bool itemStatus;

	public AchievementItem(int itemType)
	{
		this.itemStatus = true;
		this.itemType = itemType;
	}

	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="itemType">Item type.</param>
	public void Init(string conditionValue)
	{
		this.conditionValue = conditionValue;
	}

	public virtual void Trigger()
	{

	}

	public virtual void SetValue(int value)
	{
		this.intValue = value;
	}

	public virtual void SetValue(string value)
	{
		this.stringValue = value;
	}

	public virtual void SetValue(float value)
	{
		this.floatValue = value;
	}
}
