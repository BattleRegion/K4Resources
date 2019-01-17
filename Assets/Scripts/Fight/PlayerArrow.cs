using UnityEngine;
using System.Collections;

public class PlayerArrow : MonoBehaviour {

    /// <summary>
    /// 所有箭头
    /// </summary>
    public GameObject[] Arrows;

	public Sprite arrowSprite;
	public Material arrowMaterial;
	
	public void InitArrow()
	{
		for(int index = 0; index < this.Arrows.Length; index ++)
		{
			GameObject objectItem = this.Arrows[index];
			SpriteRenderer spriteRender = objectItem.AddComponent<SpriteRenderer>();
			if(spriteRender != null)
			{
				spriteRender.sprite = this.arrowSprite; 
			}
		}
	}

    void ArrowAnimationPlay()
    {
        MoveAnimation(new Vector3(0, 0.08f, 0), Arrows[0]);
        MoveAnimation(new Vector3(0, 0.05f, 0), Arrows[1]);
        MoveAnimation(new Vector3(0, 0.06f, 0), Arrows[2]);
        MoveAnimation(new Vector3(0, 0.05f, 0), Arrows[3]);
        MoveAnimation(new Vector3(0, 0.08f, 0), Arrows[4]);
        MoveAnimation(new Vector3(0, 0.05f, 0), Arrows[5]);
        MoveAnimation(new Vector3(0, 0.06f, 0), Arrows[6]);
        MoveAnimation(new Vector3(0, 0.05f, 0), Arrows[7]);
    }

    void MoveAnimation(Vector3 moveBy, GameObject arrow)
    {
        Hashtable args = new Hashtable();
		args.Add ("name", "PlayerArrowTweenItem");
        args.Add("amount", moveBy);
        args.Add("time", 0.3f);
        args.Add("looptype", iTween.LoopType.pingPong);
        args.Add("easetype", iTween.EaseType.easeInOutQuad);
        iTween.MoveBy(arrow, args);
    }

	bool enabledStatus = false;

	void OnEnable()
	{
		if(!this.enabledStatus)
		{
			this.InitArrow ();
			this.enabledStatus = true;
			this.ArrowAnimationPlay();
		}
	}
}
