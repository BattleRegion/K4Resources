using UnityEngine;
using System.Collections;

public class PvpRoundSignItem : MonoBehaviour 
{
	public Sprite selfSprite;
	public Sprite enemySprite;

	public SpriteRenderer spriteRender;

	public void Run(bool self)
	{
		iTween iTweenItem = this.gameObject.GetComponent<iTween> ();
		if(iTweenItem != null)
		{
			GameObject.Destroy(iTweenItem);
		}

		Vector3 initPosition = new Vector3 (0f, this.transform.localPosition.y, this.transform.localPosition.z);
		if(self)
		{
			this.spriteRender.sprite = this.selfSprite;
			initPosition.x = -1000f;
		}else{
			this.spriteRender.sprite = this.enemySprite;
			initPosition.x = 1000f;
		}
		this.transform.localPosition = initPosition;
		
		this.gameObject.SetActive (true);

		AnimationHelper.AnimationMoveTo (Vector3.zero, this.gameObject, iTween.EaseType.linear, this.gameObject, "RunEndCallback", 0.5f, "PvpRoundSignItemMoveToTween");
	}

	public void Stop()
	{
		this.gameObject.SetActive (false);
	}

	private void RunEndCallback()
	{
		this.StartCoroutine (this.RunEndEnumerator ());
	}

	private IEnumerator RunEndEnumerator()
	{
		yield return new WaitForSeconds(2f);
		this.gameObject.SetActive (false);
	}

	private void ValueToCompleteCallback()
	{
		this.gameObject.SetActive (false);
	}
}
