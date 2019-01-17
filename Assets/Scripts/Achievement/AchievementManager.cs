using UnityEngine;
using SimpleJson;
using System.Collections.Generic;

public class AchievementManager
{
	public static List<AchievementItem> itemList;
	private static PveGameControl pveGameControl;

	/// <summary>
	/// 获取 JSON 数据
	/// </summary>
	/// <returns>The json.</returns>
	public static JsonArray GetJson(int result)
	{
		JsonArray pathJsonList = new JsonArray();
		if(result == 0) return pathJsonList;

		foreach(AchievementItem achievementItem in itemList)
		{
			// 如果条件达成
			if(achievementItem.itemStatus)
			{
				pathJsonList.Add(achievementItem.itemType);
			}
		}

		return pathJsonList;
	}

	/// <summary>
	/// 初始化副本数据
	/// </summary>
	/// <param name="dungeonData">Dungeon data.</param>
	public static void Init(DungeonData dungeonData, PveGameControl pveGameControl)
	{
		if(dungeonData == null) return;

		AchievementManager.pveGameControl = pveGameControl;

		if(itemList == null) itemList = new List<AchievementItem>();
		itemList.Clear ();
		// 如果有成就达成条件
		if(!string.IsNullOrEmpty(dungeonData.Quest) && !string.IsNullOrEmpty(dungeonData.QuestParameter))
		{
			string[] questIdList = dungeonData.Quest.Split(new string[]{","}, System.StringSplitOptions.RemoveEmptyEntries);
			string[] questParamList = dungeonData.QuestParameter.Split(new string[]{","}, System.StringSplitOptions.RemoveEmptyEntries);

			if(questIdList.Length == questParamList.Length)
			{
				for(int index = 0; index < questParamList.Length; index ++)
				{
					AchievementItem achievementItem = GetItem(int.Parse(questIdList[index]), questParamList[index]);
					itemList.Add(achievementItem);
				}
			}
		}

		// 触发固定成就
		TriggerFixed ();
	}

	/// <summary>
	/// 触发固定成就
	/// </summary>
	public static void TriggerFixed()
	{
		List<AchievementItem> resultList = GetItemListByStageType (AchievementStageTypeEnum.Fight_Begin);
		if(resultList != null && resultList.Count > 0)
		{
			foreach(AchievementItem achievementItem in resultList)
			{
				achievementItem.Trigger();
			}
		}
	}

	/// <summary>
	/// 触发成就
	/// </summary>
	/// <param name="achievementType">Achievement type.</param>
	/// <param name="itemValue">Item value.</param>
	public static void Trigger(int achievementType, int itemValue)
	{
		// 如果未初始化完成
		if(!AchievementManager.pveGameControl.initStatus) return;

		List<AchievementItem> resultList = GetItemListByType (achievementType);
		if(resultList != null && resultList.Count > 0)
		{
			foreach(AchievementItem achievementItem in resultList)
			{
				achievementItem.SetValue(itemValue);
			}
		}
	}

	/// <summary>
	/// 根据触发阶段查找成就
	/// </summary>
	/// <returns>The item list by stage type.</returns>
	/// <param name="stageType">Stage type.</param>
	private static List<AchievementItem> GetItemListByStageType(int stageType)
	{
		if(itemList == null) return null;

		List<AchievementItem> resultList = new List<AchievementItem> ();
		foreach(AchievementItem achievementItem in itemList)
		{
			if(achievementItem.stageType == stageType) resultList.Add(achievementItem);
		}
		return resultList;
	}

	/// <summary>
	/// 根据成就类别查找成就
	/// </summary>
	/// <returns>The item list by type.</returns>
	/// <param name="achievementType">Achievement type.</param>
	private static List<AchievementItem> GetItemListByType(int achievementType)
	{
		if(itemList == null) return null;

		List<AchievementItem> resultList = new List<AchievementItem> ();
		foreach(AchievementItem achievementItem in itemList)
		{
			if(achievementItem.itemType == achievementType) resultList.Add(achievementItem);
		}
		return resultList;
	}

	/// <summary>
	/// 初始化成就
	/// </summary>
	/// <returns>The item.</returns>
	/// <param name="itemType">Item type.</param>
	/// <param name="conditionValue">Condition value.</param>
	private static AchievementItem GetItem(int itemType, string conditionValue)
	{
		AchievementItem achievementItem = null;
		switch(itemType)
		{
			case 1 : achievementItem = new AchievementItem01(); break;
			case 2 : achievementItem = new AchievementItem02(); break;
			case 3 : achievementItem = new AchievementItem03(); break;
			case 4 : achievementItem = new AchievementItem04(); break;
			case 5 : achievementItem = new AchievementItem05(); break;
			case 6 : achievementItem = new AchievementItem06(); break;
			case 7 : achievementItem = new AchievementItem07(); break;
			case 8 : achievementItem = new AchievementItem08(); break;
			case 9 : achievementItem = new AchievementItem09(); break;
			case 10 : achievementItem = new AchievementItem10(); break;
		}
		achievementItem.Init(conditionValue);
		return achievementItem;
	}
}
