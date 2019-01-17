using UnityEngine;
using System.Collections;

public class CoolingTimeUI : MonoBehaviour 
{
	public InfoLabel HpLabel;
	public GameObject HpValue;

	public void ChangeData(float currentSecond, float totalSecond, bool selfStatus)
	{
		if (currentSecond < 0) currentSecond = 0;

		HpLabel.SetNum(((int)currentSecond).ToString());
		HpLabel.transform.localPosition = new Vector3(150 - HpLabel.TotalWidth/2, 6, 0);
	}

	public void ChangeProgress(float progress)
	{
		HpValue.renderer.material.SetFloat("_Progress", progress);
	}
}
