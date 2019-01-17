using UnityEngine;
using System;
using System.Collections;

public class SkillFlyItem : MonoBehaviour 
{
	public UILabel txtName;

	private Action callback;

	public void Run(string text, Action callback)
	{
		this.callback = callback;

		this.txtName.text = text;
		
		this.transform.localPosition = new Vector3 (400f, this.transform.localPosition.y, this.transform.localPosition.z);

		this.gameObject.SetActive (true);

		AnimationHelper.AnimationMoveTo(new Vector3(0, this.transform.localPosition.y, this.transform.localPosition.z), this.gameObject, iTween.EaseType.linear, this.gameObject, "MoveEndCallback", 0.5f);
	}

	private void MoveEndCallback()
	{
		this.StartCoroutine (this.MoveEnd ());
	}

	IEnumerator MoveEnd()
	{
		// 延迟 5 秒
		yield return new WaitForSeconds (1f);

		this.gameObject.SetActive (false);
		
		if(this.callback != null) this.callback();
	}
}
