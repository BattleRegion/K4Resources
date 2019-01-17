using UnityEngine;
using System.Collections.Generic;

public class PvpFaceManager
{
	public static void ShowFace(PvpCharacter pvpCharacter, string faceName)
	{
		if(pvpCharacter == null || string.IsNullOrEmpty(faceName)) return;
		if (pvpCharacter.faceList == null) pvpCharacter.faceList = new List<string> ();

		// 添加表情名称
		pvpCharacter.faceList.Add (faceName);

		// 如果表情数量为 1
		if(pvpCharacter.faceList.Count == 1)
		{
			ShowFaceItem(pvpCharacter);
		}
	}

	private static void ShowFaceItem(PvpCharacter pvpCharacter)
	{
		if(pvpCharacter.faceList != null && pvpCharacter.faceList.Count > 0)
		{
			string faceName = pvpCharacter.faceList[0];

			GameObject faceItem = GameObject.Instantiate(Resources.Load("Expression/face_" + faceName)) as GameObject;
			if(faceItem == null)
			{
				FaceEnd(pvpCharacter);
			}else
			{
				// 设置层级
				//faceItem.layer = LayerHelper.UI;
				// 设置父类别
				faceItem.transform.parent = pvpCharacter.transform;
				// 设置缩放
				faceItem.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
				// 设置位置
				faceItem.transform.localPosition = new Vector3(0f, 0.25f, 0f);

				FaceItem pvpFaceItem = faceItem.GetComponent<FaceItem>();
				if(pvpFaceItem == null)
				{
					// 销毁表情
					GameObject.Destroy(faceItem);
					FaceEnd(pvpCharacter);
				}
				else
				{
					pvpFaceItem.Init(()=>
					{
						FaceEnd(pvpCharacter);
					});
				}
			}
		}
	}

	private static void FaceEnd(PvpCharacter pvpCharacter)
	{
		// 移除数据
		pvpCharacter.faceList.RemoveAt(0);
		// 继续显示下一个表情
		ShowFaceItem(pvpCharacter);
	}
}
