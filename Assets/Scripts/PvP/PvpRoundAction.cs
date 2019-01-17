using UnityEngine;
using System.Collections;

public class PvpRoundAction : MonoBehaviour 
{
	public Sprite selfSprite;
	public Sprite enemySprite;

	public SpriteRenderer spriteRender;

	public void ChangeData(bool self)
	{
		this.spriteRender.gameObject.SetActive (true);
		if(self)
		{
			this.spriteRender.sprite = this.selfSprite;
		}else{
			this.spriteRender.sprite = this.enemySprite;
		}
	}
}
