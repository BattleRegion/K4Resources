using UnityEngine;
using System.Collections.Generic;

public class PveFaceManager
{
	public static List<string> monsterFaceList = new List<string> ();

	public static void Clear()
	{
		if(monsterFaceList != null) monsterFaceList.Clear();
	}

	/// <summary>
	/// 触发表情 列表
	/// </summary>
	/// <param name="fightUnitList">Fight unit list.</param>
	/// <param name="stageType">Stage type.</param>
	public static void Trigger(List<PveFightUnit> fightUnitList, int stageType)
	{
		List<PveFaceData> pveFaceList = ConfigManager.PveFaceConfig.GetFaceListByType (stageType);
		string faceName = "";

		if(pveFaceList != null && pveFaceList.Count > 0)
		{
			foreach(PveFaceData pveFaceData in pveFaceList)
			{
				int randomValue = Tools.GetRandom_n(10000);
				if(randomValue <= pveFaceData.Chance)
				{
					faceName = pveFaceData.FaceId;
					break;
				}
			}
		}

		if(!string.IsNullOrEmpty(faceName))
		{
			foreach(PveFightUnit pveFightUnit in fightUnitList)
			{
				ShowFace(pveFightUnit, faceName, stageType);
			}
		}
	}

	/// <summary>
	/// 触发表情单个 
	/// </summary>
	/// <param name="fightUnit">Fight unit.</param>
	/// <param name="stageType">Stage type.</param>
	public static void Trigger(PveFightUnit fightUnit, int stageType)
	{
		Trigger (new List<PveFightUnit>()
		{
			fightUnit
		}, stageType);
	}

	public static void ShowFace(PveFightUnit pveFightUnit, string faceName, int stageType)
	{
		if(pveFightUnit == null || string.IsNullOrEmpty(faceName)) return;
		if (pveFightUnit.faceList == null) pveFightUnit.faceList = new List<string> ();

		string monsterID = pveFightUnit.GetHashCode () + "_" + stageType;
		// 如果没有表现过
		if(monsterFaceList.IndexOf(monsterID) != -1)
		{
			return;
		}
		// 添加表情
		monsterFaceList.Add (monsterID);

		// 添加表情名称
		pveFightUnit.faceList.Add (faceName);

		// 如果表情数量为 1
		if(pveFightUnit.faceList.Count == 1)
		{
			ShowFaceItem(pveFightUnit);
		}
	}

	private static void ShowFaceItem(PveFightUnit pveFightUnit)
	{
		if(pveFightUnit.faceList != null && pveFightUnit.faceList.Count > 0)
		{
			string faceName = pveFightUnit.faceList[0];

			Object faceSourceItem = Resources.Load("Expression/face_" + faceName);
			if(faceSourceItem == null)
			{
				FaceEnd(pveFightUnit);
			}else
			{
				GameObject faceItem = GameObject.Instantiate(faceSourceItem) as GameObject;
				if(faceItem == null)
				{
					FaceEnd(pveFightUnit);
				}else
				{
					// 设置层级
					//faceItem.layer = LayerHelper.UI;
					// 设置父类别
					faceItem.transform.parent = pveFightUnit.transform;
					// 设置缩放
					faceItem.transform.localScale = new Vector3(0.1f, 0.1f, 1f);

					if(pveFightUnit.GetType() == typeof(PveMonster))
					{
						faceItem.transform.localPosition = new Vector3(0f, 0.25f, 0f);
					}
					else
					{
						faceItem.transform.localPosition = new Vector3(0f, 0.36f, 0f);
					}

					FaceItem pvpFaceItem = faceItem.GetComponent<FaceItem>();
					if(pvpFaceItem == null)
					{
						// 销毁表情
						GameObject.Destroy(faceItem);

						FaceEnd(pveFightUnit);
					}
					else
					{
						pvpFaceItem.Init(()=>
						{
							FaceEnd(pveFightUnit);
						});
					}
				}
			}
		}
	}

	private static void FaceEnd(PveFightUnit pveFightUnit)
	{
		// 移除数据
		pveFightUnit.faceList.RemoveAt(0);
		// 继续显示下一个表情
		ShowFaceItem(pveFightUnit);
	}
}
