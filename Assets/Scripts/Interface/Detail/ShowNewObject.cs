using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowNewObject : MonoBehaviour
{
    void OnEnable()
    {
        UIEventListener.Get(NewBg).onClick = (go) =>
        {
            if (newStart)
            {
                newStart = false;

                mTexture.mainTexture = null;
                if (GameObject.Find("ChestAnimation") != null)
                    Destroy(GameObject.Find("ChestAnimation"));

                if (NewBoard.transform.FindChild("PetAnimation") != null)
                    Destroy(NewBoard.transform.FindChild("PetAnimation").gameObject);
                if (GameObject.Find("EggAnimation") != null)
                    Destroy(GameObject.Find("EggAnimation"));


                foreach (Transform t in StarGrid.transform)
                {
                    Destroy(t.gameObject);
                }
                foreach (GameObject g in TempStars)
                {
                    Destroy(g);
                }
                TempStars.Clear();
                StarOutlineTransforms.Clear();

                NewBoard.SetActive(false);
                NewBg.SetActive(false);
            }
        };
    }

    #region NEW ITEM
    /// <summary>
    /// 新的材料或者宠物独立显示
    /// </summary>
    public GameObject NewBoard;
    public GameObject NewBg;
    public UITexture mTexture;

    public GameObject InfoBoard;
    public UILabel NameLabel;
    public GameObject StarPrefab;
    public GameObject StarOutline;
    public UIGrid StarGrid;

    public Vector3 Name_StartPosition = new Vector3(0f, -730f, 0f);
    public Vector3 Name_TargetPosition = new Vector3(0f, -330f, 0f);

    /// <summary>
    /// 箱子和蛋动画的预设
    /// </summary>
    public List<GameObject> Eggs_s = new List<GameObject>();
    public List<GameObject> Chests_s = new List<GameObject>();
    public List<GameObject> Eggs_b = new List<GameObject>();
    public List<GameObject> Chests_b = new List<GameObject>();

    bool newStart = false;

    //0 装备素材  1 宠物
    public void ShowNew(int type, string Id)
    {
        NewBoard.SetActive(true);
        NewBg.SetActive(true);

        SetInfoBoard(Id, type);
        switch (type)
        {
            case 0:
                {
                    ItemData i = ConfigManager.ItemConfig.GetItemById(Id);
                    if (i != null)
                    {
                        GameObject g = Instantiate(Chests_b[i.Rank - 1], Vector3.zero, Quaternion.identity) as GameObject;
                        g.name = "ChestAnimation";
                        PetUIController.SetLayer(g.transform, LayerHelper.UIFX);
                        StartCoroutine(ShowMaterialTexture(i.SkinId, 1f));
                        StartCoroutine(InfoBoardFlyIn(i.Rank, 1.5f, 0));
                    }
                    else
                    {
                        HardWareData hd = ConfigManager.HardWareConfig.GetHardWareById(Id);
                        GameObject g = Instantiate(Chests_b[hd.Rank - 1], Vector3.zero, Quaternion.identity) as GameObject;
                        g.name = "ChestAnimation";
                        PetUIController.SetLayer(g.transform, LayerHelper.UIFX);
                        StartCoroutine(ShowMaterialTexture(hd.SkinId, 1f));
                        StartCoroutine(InfoBoardFlyIn(hd.Rank, 1.5f, 0));
                    }
                    break;
                }
            case 1:
                {
                    PetData p = ConfigManager.PetConfig.GetPetById(Id);
                    GameObject g = Instantiate(Eggs_b[p.Rank - 1], Vector3.zero, Quaternion.identity) as GameObject;
                    g.name = "EggAnimation";
                    PetUIController.SetLayer(g.transform, LayerHelper.UIFX);
                    StartCoroutine(ShowPetAnimation(p.SkinId, 1.25f));
                    StartCoroutine(InfoBoardFlyIn(p.Rank, 1.75f, 1));
                    break;
                }
            default: break;
        }
    }

    void SetInfoBoard(string Id, int type)
    {
        InfoBoard.transform.localPosition = Name_StartPosition;
        switch (type)
        {
            case 0:
                {
                    ItemData i = ConfigManager.ItemConfig.GetItemById(Id);
                    if (i != null)
                    {
                        NameLabel.text = i.Description;
                        int count = i.Rank;
                        while (count > 0)
                        {
                            GameObject g = NGUITools.AddChild(StarGrid.gameObject, StarOutline);
                            StarOutlineTransforms.Add(g.transform);
                            count--;
                        }
                        StarGrid.Reposition();
                    }
                    else
                    {
                        HardWareData hd = ConfigManager.HardWareConfig.GetHardWareById(Id);
                        NameLabel.text = hd.Name;
                        int count = hd.Rank;
                        while (count > 0)
                        {
                            GameObject g = NGUITools.AddChild(StarGrid.gameObject, StarOutline);
                            StarOutlineTransforms.Add(g.transform);
                            count--;
                        }
                        StarGrid.Reposition();
                    }
                    break;
                }
            case 1:
                {
                    PetData p = ConfigManager.PetConfig.GetPetById(Id);
                    NameLabel.text = p.Name;
                    int count = p.MaxRank;
                    while (count > 0)
                    {
                        GameObject g = NGUITools.AddChild(StarGrid.gameObject, StarOutline);
                        StarOutlineTransforms.Add(g.transform);
                        count--;
                    }
                    StarGrid.Reposition();
                    break;
                }
            default: break;
        }
    }

    /// <summary>
    /// 弹出动画
    /// </summary>
    void SetScale(GameObject g, Vector3 scale)
    {
        g.transform.localScale = Vector3.zero;
        AnimationHelper.AnimationScaleTo(scale, g, iTween.EaseType.spring, null, null, 0.5f);
    }

    IEnumerator ShowMaterialTexture(string SkinId, float time)
    {
        yield return new WaitForSeconds(time);
        mTexture.mainTexture = Resources.Load<Texture>("Atlas/ItemIcons/" + SkinId);
        if (mTexture.mainTexture == null)
        {
            mTexture.mainTexture = Resources.Load<Sprite>("Atlas/ItemIcons/" + SkinId).texture;
        }
        SetScale(mTexture.gameObject, Vector3.one);
    }

    IEnumerator ShowPetAnimation(string SkinId, float time)
    {
        yield return new WaitForSeconds(time);

        GameObject PetAnimation = NGUITools.AddChild(NewBoard, Resources.Load<GameObject>("PreFabs/Characters/" + SkinId + "60"));
        PetAnimation.name = "PetAnimation";
        PetUIController.SetLayer(PetAnimation.transform, LayerHelper.Cover);
        PetAnimation.transform.localPosition = Vector3.zero;
        SetScale(PetAnimation, new Vector3(100f, 100f, 1f));
    }

    IEnumerator InfoBoardFlyIn(int rank, float time, int type)
    {
        yield return new WaitForSeconds(time);
        AnimationHelper.AnimationMoveTo(Name_TargetPosition, InfoBoard, iTween.EaseType.linear, null, null, 0.2f);
        StartCoroutine(AddStars(0.4f, rank));
    }

    IEnumerator AddStars(float time, int rank)
    {
        yield return new WaitForSeconds(time);
        for (int i = 0; i < rank; i++)
        {
            StartCoroutine(AddStarAnimation(i));
        }
        Invoke("SetNewStart", rank * 0.2f);
    }

    List<GameObject> TempStars = new List<GameObject>();
    List<Transform> StarOutlineTransforms = new List<Transform>();
    IEnumerator AddStarAnimation(int index)
    {
        yield return new WaitForSeconds(index * 0.2f);
        GameObject g = NGUITools.AddChild(StarOutlineTransforms[index].gameObject, StarPrefab);
        TempStars.Add(g);
        g.transform.localPosition = new Vector3(-0.5f, 0.5f, 0f);
        g.transform.localScale *= 3f;
        AnimationHelper.AnimationScaleTo(Vector3.one, g, iTween.EaseType.linear, null, null, 0.2f);
    }

    void SetNewStart()
    {
        newStart = true;
    }
    #endregion
}