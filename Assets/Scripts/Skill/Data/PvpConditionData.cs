using UnityEngine;
using System.Collections.Generic;

public class PvpConditionData
{
	/// <summary>
	/// 触发条件 包括 元素、几率、武器
	/// </summary>
	public int conditionType;

	/// <summary>
	/// 触发值
	/// </summary>
	public float conditionValue;
	
	/// <summary>
	/// Buff 类别
	/// </summary>
	public List<PvpBuffData> buffList;

	/// <summary>
	/// 参数都为 0 表示没有触发条件，直接执行 Buff 效果
	/// </summary>
	/// <param name="conditionType">Condition type.</param>
	/// <param name="conditionValue">Condition value.</param>
	public PvpConditionData(int conditionType = 0, float conditionValue = 0f)
	{
		this.conditionType = conditionType;
		this.conditionValue = conditionValue;
	}
}
