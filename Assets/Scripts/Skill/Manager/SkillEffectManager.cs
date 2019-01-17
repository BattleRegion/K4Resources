using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class SkillEffectManager
{
	private static List<SkillEffectItem> poolList = new List<SkillEffectItem>();

	/// <summary>
	/// 触发单个表现
	/// </summary>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="targetItemList">Target item list.</param>
	public static void Trigger(SkillEffectParam effectParam, string skillID, PvpFightUnit sourceItem, PvpFightUnit targetItem, List<PvpFightUnit> targetItemList = null, Action<SkillEffectParam> endCallback = null, Action<SkillEffectParam> attackCallback = null)
	{
		BaseSkillItem skillItem = sourceItem.pvpPlayerSkill.GetSkillItemBySkillID(skillID);
		SkillEffectItem effectItem = CreateSkillEffectItem ();
		if(effectItem != null)
		{
			effectItem.Run(effectParam, skillItem, sourceItem, targetItem, targetItemList, endCallback, attackCallback);
		}
	}

	/// <summary>
	/// 触发召唤表现
	/// </summary>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="position">Position.</param>
	/// <param name="endCallback">End callback.</param>
	public static void Trigger(SkillEffectParam effectParam, PvpFightUnit pvpFightUnit, BaseSkillItem skillItem, Vector3 position, Action<SkillEffectParam> endCallback)
	{
		//Debug.Log ("调用 SkillEffectManager 的 Trigger 函数 ！");
		SkillEffectItem effectItem = CreateSkillEffectItem ();
		if(effectItem != null)
		{
			//Debug.Log ("调用 SkillEffectItem 的 Run 函数 ！");
			effectItem.Run(effectParam, pvpFightUnit, skillItem, position, endCallback);
		}
	}

	/// <summary>
	/// 固定表现
	/// </summary>
	/// <param name="pvpFightUnit">Pvp fight unit.</param>
	/// <param name="skillList">Skill list.</param>
	public static void TriggerFixed(PvpFightUnit pvpFightUnit)
	{
		List<string> prefabList = GetPrefabListByEffectType (SkillEffectTypeEnum.FIXED, pvpFightUnit);
		if(prefabList != null && prefabList.Count > 0)
		{
			// 隐藏阴影
			pvpFightUnit.HiddenShadow (false);
			// 遍历效果
			foreach(string prefabItem in prefabList)
			{
				//Debug.Log("创建光圈预设！");
				// 创建预设
				PvpGameObjectManager.Create (DungeonSpritePathManager.SkillBumpFX (prefabItem), (GameObject skillItem)=>
				{
					//Debug.Log("创建光圈预设回调！");
					//GameObject skillItem = GetPrefabItem(prefabItem);
					if (skillItem != null)
					{
						// 设置父对象为角色
						skillItem.transform.parent = pvpFightUnit.transform;
						skillItem.transform.localPosition = new Vector3(0f, 0.04f, 0f);
					}  
				});
			}
		}
	}

	/// <summary>
	/// 创建预设
	/// </summary>
	/// <returns>The prefab item.</returns>
	/// <param name="prefabName">Prefab name.</param>
	public static GameObject GetPrefabItem(string prefabName)
	{
		GameObject prefabItem = Resources.Load(DungeonSpritePathManager.SkillBumpFX(prefabName)) as GameObject;
		GameObject newItem = null;

		if (prefabItem)
		{
			newItem = GameObject.Instantiate(prefabItem) as GameObject;
		}

		return newItem;
	}

	/// <summary>
	/// 获得粒子延迟时间
	/// </summary>
	/// <returns>The prefab destory time item.</returns>
	/// <param name="prefab">Prefab.</param>
	public static float GetPrefabDestoryTimeItem(GameObject prefab)
	{
		if(prefab == null) return 0f;

		Component AnimationJscript = prefab.GetComponent("DeadTime");  //C#访问JS
		if(AnimationJscript == null) return 0f;

		FieldInfo parameter = AnimationJscript.GetType().GetField("deadTime"); //反射，效率可能会有影响
		return float.Parse(parameter.GetValue(AnimationJscript).ToString());
	}

	/// <summary>
	/// 查找角色身上的固定效果
	/// </summary>
	/// <returns>The prefab list by effect type.</returns>
	/// <param name="effectType">Effect type.</param>
	/// <param name="pvpFightUnit">Pvp fight unit.</param>
	private static List<string> GetPrefabListByEffectType(int effectType, PvpFightUnit pvpFightUnit)
	{
		List<string> resultList = new List<string> ();

		List<PvpSkillHouseData> skillItemList = pvpFightUnit.PvpUserInfo.UserSkillList;
		foreach(PvpSkillHouseData skillItem in skillItemList)
		{
			// 如果是固定效果
			if(skillItem.skillData.FXType1 == effectType)
			{
				if(resultList.IndexOf(skillItem.skillData.FXPrefab1) == -1) resultList.Add(skillItem.skillData.FXPrefab1);
			}
			// 如果是固定效果
			if(skillItem.skillData.FXType2 == effectType)
			{
				if(resultList.IndexOf(skillItem.skillData.FXPrefab2) == -1) resultList.Add(skillItem.skillData.FXPrefab2);
			}
			// 如果是固定效果
			if(skillItem.skillData.FXType3 == effectType)
			{
				if(resultList.IndexOf(skillItem.skillData.FXPrefab3) == -1) resultList.Add(skillItem.skillData.FXPrefab3);
			}
		}

		return resultList;
	}

	/// <summary>
	/// 创建池对象
	/// </summary>
	/// <returns>The skill effect item.</returns>
	private static SkillEffectItem CreateSkillEffectItem()
	{
		SkillEffectItem effectItem = GetSkillEffectItem ();
		if(effectItem == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "SkillEffectItemPool";
			effectItem = gameObject.AddComponent<SkillEffectItem>();
			poolList.Add(effectItem);
		}
		return effectItem;
	}

	/// <summary>
	/// 查找池对象
	/// </summary>
	/// <returns>The skill effect item.</returns>
	private static SkillEffectItem GetSkillEffectItem()
	{
		foreach(SkillEffectItem effectItem in poolList)
		{
			if(!effectItem.runStatus) return effectItem;
		}
		return null;
	}
}
