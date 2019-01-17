using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

public class PveSkillEffectManager
{
    private static List<PveSkillEffectItem> poolList = new List<PveSkillEffectItem>();

	/// <summary>
	/// 触发单个表现
	/// </summary>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="sourceItem">Source item.</param>
	/// <param name="targetItem">Target item.</param>
	/// <param name="targetItemList">Target item list.</param>
    public static void Trigger(BaseSkillItem skillItem, PveFightUnit sourceItem, List<PveEnemyUnit> targetItemList = null, Action endCallback = null, Action attackCallback = null)
	{
		Debug.Log ("调用技能表现效果");
        PveSkillEffectItem effectItem = CreateSkillEffectItem();
		if(effectItem != null )
		{
            if (targetItemList != null)
            {
                if (skillItem.skillData.skillTarget != SkillTargetTypeEnum.Range)
                {
                    foreach (PveEnemyUnit pe in targetItemList)
                    {
                        Debug.Log("-------------v---" + pe.name);
                        effectItem.Run1(skillItem, sourceItem, pe, targetItemList, endCallback, attackCallback);
                    }
                }
                else
                {
                    effectItem.Run1(skillItem, sourceItem, null, targetItemList, endCallback, attackCallback);
                }
               
            }
            else
            {
                effectItem.Run1(skillItem, sourceItem, null, null, endCallback, attackCallback);
            }
		}
	}

	/// <summary>
	/// 触发召唤表现
	/// </summary>
	/// <param name="skillItem">Skill item.</param>
	/// <param name="position">Position.</param>
	/// <param name="endCallback">End callback.</param>
	public static void Trigger(PveFightUnit PveFightUnit, BaseSkillItem skillItem, Vector3 position, Action endCallback)
	{
        PveSkillEffectItem effectItem = CreateSkillEffectItem();
		if(effectItem != null)
		{
            effectItem.Run(PveFightUnit, skillItem, position, endCallback);
		}
	}

	/// <summary>
	/// 固定表现
	/// </summary>
	/// <param name="PveFightUnit">Pvp fight unit.</param>
	/// <param name="skillList">Skill list.</param>
	public static void TriggerFixed(PveFightUnit PveFightUnit,bool show=true)
	{
        //List<string> prefabList = GetPrefabListByEffectType (SkillEffectTypeEnum.FIXED, PveFightUnit);
        //if(prefabList != null && prefabList.Count > 0)
        //{
        //    ChangeShadow(PveFightUnit, false);
        //    foreach(string prefabItem in prefabList)
        //    {
        //        GameObject skillItem = GetPrefabItem(prefabItem);
        //        if (skillItem != null)
        //        {
        //            // 设置父对象为角色
        //            skillItem.transform.parent = PveFightUnit.transform;
        //            skillItem.transform.localPosition = new Vector3(0f, 0.04f, 0f);
        //        }   
        //    }
        //}else{
        //    ChangeShadow(PveFightUnit, true);
        //}
       

        List<string> prefabList = GetPrefabListByEffectType(SkillEffectTypeEnum.FIXED, PveFightUnit);
        //Debug.Log(PveFightUnit + " --- ");
        if (prefabList != null && prefabList.Count > 0)
        {
            // 隐藏阴影
            PveFightUnit.HiddenShadow(false);

            // 遍历效果
            foreach (string prefabItem in prefabList)
            {
                Transform tr = PveFightUnit.transform.FindChild(prefabItem);             
                if (show)
                {
                    if (tr != null) GameObject.Destroy(tr.gameObject);
                    tr = null;
                    GameObject skillItem = GetPrefabItem(prefabItem);
                    if (skillItem != null)
                    {
                        // 设置父对象为角色
                        skillItem.transform.parent = PveFightUnit.transform;
                        skillItem.name = prefabItem;
                        skillItem.transform.localPosition = new Vector3(0f, 0.04f, 0f);
                    }
                }
                else
                {
                    //if (tr != null) Debug.Log(tr.gameObject + " --- ");
                    if (tr != null) GameObject.DestroyImmediate(tr.gameObject);
                    //if (tr != null) Debug.Log(tr.gameObject + " --- ");
                    tr = null;
                }
            }
        }
	}

	/// <summary>
	/// 设置阴影
	/// </summary>
	/// <param name="PveFightUnit">Pvp fight unit.</param>
	/// <param name="activeStatus">If set to <c>true</c> active status.</param>
	private static void ChangeShadow(PveFightUnit PveFightUnit, bool activeStatus)
	{
		PlayerAvata playerAvata = PveFightUnit.GetComponentInChildren<PlayerAvata> ();
		if(playerAvata == null) return;

		GameObject playerShadow = playerAvata.transform.FindChild ("Shadow").gameObject;
		if(playerShadow == null) return;

		playerShadow.SetActive (activeStatus);
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
	/// <param name="PveFightUnit">Pvp fight unit.</param>
	private static List<string> GetPrefabListByEffectType(int effectType, PveFightUnit PveFightUnit)
	{
        //List<string> resultList = new List<string> ();

        //List<BaseSkillItem> skillItemList = PveFightUnit.pvpPlayerSkill.skillList;
        //foreach(BaseSkillItem skillItem in skillItemList)
        //{
        //    // 如果是固定效果
        //    if(skillItem.configData.FXType1 == SkillEffectTypeEnum.FIXED)
        //    {
        //        if(resultList.IndexOf(skillItem.configData.FXPrefab1) == -1) resultList.Add(skillItem.configData.FXPrefab1);
        //    }
        //    // 如果是固定效果
        //    if(skillItem.configData.FXType2 == SkillEffectTypeEnum.FIXED)
        //    {
        //        if(resultList.IndexOf(skillItem.configData.FXPrefab2) == -1) resultList.Add(skillItem.configData.FXPrefab2);
        //    }
        //    // 如果是固定效果
        //    if(skillItem.configData.FXType3 == SkillEffectTypeEnum.FIXED)
        //    {
        //        if(resultList.IndexOf(skillItem.configData.FXPrefab3) == -1) resultList.Add(skillItem.configData.FXPrefab3);
        //    }
        //}


        List<string> resultList = new List<string>();

        List<PvpSkillHouseData> skillItemList = UserManager.pveUserInfo.UserSkillList;
        foreach (PvpSkillHouseData skillItem in skillItemList)
        {
            // 如果是固定效果
            if (skillItem.skillData.FXType1 == effectType)
            {
                if (resultList.IndexOf(skillItem.skillData.FXPrefab1) == -1) resultList.Add(skillItem.skillData.FXPrefab1);
            }
            // 如果是固定效果
            if (skillItem.skillData.FXType2 == effectType)
            {
                if (resultList.IndexOf(skillItem.skillData.FXPrefab2) == -1) resultList.Add(skillItem.skillData.FXPrefab2);
            }
            // 如果是固定效果
            if (skillItem.skillData.FXType3 == effectType)
            {
                if (resultList.IndexOf(skillItem.skillData.FXPrefab3) == -1) resultList.Add(skillItem.skillData.FXPrefab3);
            }
        }

		return resultList;
	}

	/// <summary>
	/// 创建池对象
	/// </summary>
	/// <returns>The skill effect item.</returns>
    private static PveSkillEffectItem CreateSkillEffectItem()
	{
        PveSkillEffectItem effectItem = GetSkillEffectItem();
		if(effectItem == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "SkillEffectItemPool";
            effectItem = gameObject.AddComponent<PveSkillEffectItem>();
			poolList.Add(effectItem);
		}
		return effectItem;
	}

	/// <summary>
	/// 查找池对象
	/// </summary>
	/// <returns>The skill effect item.</returns>
    private static PveSkillEffectItem GetSkillEffectItem()
	{
        foreach (PveSkillEffectItem effectItem in poolList)
		{
			if(!effectItem.runStatus) return effectItem;
		}
		return null;
	}
}
