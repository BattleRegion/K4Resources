using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NumTextureLabel : MonoBehaviour
{

    #region 属性
    //所有精灵
    public Sprite[] LabelAllSprites;

    /// <summary>
    /// 数字字符串
    /// </summary>
    public string NumString;   

    public int CountNum;
    /// <summary>
    /// 总长度
    /// </summary>
    public float TotalWidth = 0;

    public List<SpriteRenderer> NumRenders = new List<SpriteRenderer>();
    #endregion

    #region 重写MONO
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
       
    }
    #endregion

    #region 虚方法
    /// <summary>
    /// 设置数字
    /// </summary>
    /// <param name="numString"></param>
    Vector3 lastPosition = new Vector3(0, 0, 0);
    float lastWidth = 0;
	public virtual void SetNum(string numString, int layer = -1, bool oriStatus = false)
    {
        Clear();
        NumString = numString;
        for (int i = 0; i < NumString.Length; i++)
        {
            string s = NumString[i].ToString();
            GameObject numObject = new GameObject();
            numObject.transform.parent = transform;
            numObject.transform.localScale = new Vector3(1, 1, 1);
            numObject.name = NumString[i].ToString();
            SpriteRenderer sr = numObject.AddComponent<SpriteRenderer>();
            NumRenders.Add(sr);
            sr.sprite = LabelAllSprites[NumToSpriteIndex(s)];
            sr.transform.localPosition = new Vector3(lastPosition.x + lastWidth / 2 + sr.sprite.rect.width / 2, lastPosition.y, lastPosition.z);
            lastWidth = sr.sprite.rect.width;
            lastPosition = sr.transform.localPosition;
            TotalWidth = TotalWidth + sr.sprite.rect.width;
        }
		SetLayer(layer);

        foreach (SpriteRenderer sr in NumRenders)
        {
            Vector3 v1 =new Vector3(sr.gameObject.transform.localPosition.x - TotalWidth / 2, sr.gameObject.transform.localPosition.y, sr.gameObject.transform.localPosition.z);
            sr.gameObject.transform.localPosition = v1;
            sr.transform.localPosition = ResetSpecialPosition(sr.gameObject);
        }
    }

    /// <summary>
    /// 清理
    /// </summary>
    public virtual void Clear()
    {
        foreach (SpriteRenderer sr in NumRenders)
        {
			if(sr != null && sr.gameObject != null)
			{
            	Destroy(sr.gameObject);
			}
        }
        NumRenders.Clear();
        TotalWidth = 0;
        lastWidth = 0;
        lastPosition = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// 数字转换索引
    /// </summary>
    public virtual int NumToSpriteIndex(string s)
    {
        return 0;
    }

    /// <summary>
    /// 设置层
    /// </summary>
	public void SetLayer(int layer = -1)
    {
        foreach (Transform child in transform)
        {
			if(layer != -1)
			{
				child.gameObject.layer = layer;
			}else{
				child.gameObject.layer = Layer();
			}
            child.GetComponent<SpriteRenderer>().sortingOrder = Sort();
        }
    }

    /// <summary>
    /// 获取层
    /// </summary>
    /// <returns></returns>
    public virtual int Layer()
    {
        return LayerHelper.Basic;
    }

    public virtual int Sort()
    {
        return 0;
    }

    /// <summary>
    /// 重设特殊字符位置
    /// </summary>
    public virtual Vector3 ResetSpecialPosition(GameObject go)
    {
        return new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, go.transform.localPosition.z);
    }
    #endregion
}
