using UnityEngine;
using System.Collections;

public class PowerFlyTool
{
	private static float beginX = -232f;
	private static float endX = 165f;

    public static void PowerEffect(Vector3 itemPosition, GameObject endObject, float currentValue, float totalValue)
	{
		PvpGameObjectManager.Create ("PreFabs/FX/UiFx_5", (GameObject power) =>
		{
			//GameObject power = GameObject.Instantiate(Resources.Load("PreFabs/FX/UiFx_5")) as GameObject;
			power.gameObject.SetActive (false);
			
			power.transform.parent = endObject.transform.parent;
			power.transform.localPosition = itemPosition;
			power.transform.localScale = new Vector3 (1f, 1f, 1f);
			
			if(totalValue == 0f) totalValue = 1f;
			
			float powerX = (currentValue / totalValue) * (Mathf.Abs (beginX) + Mathf.Abs (endX)) + beginX;
			
			PowerFlyItem powerFlyItem = power.AddComponent<PowerFlyItem>();
			powerFlyItem.Run(new Vector3(powerX, endObject.transform.localPosition.y, endObject.transform.localPosition.z));
		});
	}
}

class PowerFlyItem : MonoBehaviour
{
	public void Run(Vector3 endPoint)
	{
		this.gameObject.SetActive (true);
		AnimationHelper.AnimationMoveTo(endPoint, this.gameObject, iTween.EaseType.linear, this.gameObject, "MoveEndCallback", 0.5f);
	}

	private void MoveEndCallback()
	{
		GameObject.Destroy(this.gameObject);
	}
}
