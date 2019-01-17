using UnityEngine;
using System.Collections.Generic;

public class PvpFeelItemList : MonoBehaviour 
{
	public void Show()
	{
		if(this.gameObject.activeSelf)
		{
			this.Hide();
		}else
		{
			this.gameObject.SetActive(true);
			this.transform.localPosition = new Vector3(600f, this.transform.localPosition.y, this.transform.localPosition.z);
			AnimationHelper.AnimationMoveTo(new Vector3(0, this.transform.localPosition.y, this.transform.localPosition.z), this.gameObject, iTween.EaseType.linear, null, null, 0.3f);
		}
	}

	public void Hide()
	{
		AnimationHelper.AnimationMoveTo(new Vector3(600f, this.transform.localPosition.y, this.transform.localPosition.z), this.gameObject, iTween.EaseType.linear, this.gameObject, "HideEndCallback", 0.3f);
	}

	private void HideEndCallback()
	{
		this.Close();
	}

	public void Close()
	{
		this.gameObject.SetActive (false);
	}
}
