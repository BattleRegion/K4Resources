using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialDetail : MonoBehaviour
{
    /// <summary>
    /// 查看详情时关闭unit层摄像机
    /// </summary>
    public GameObject UnitCamera;

    public UILabel Name;
    public UILabel Count;
    public UILabel Price;
    public UITexture MaterialTexture;
    public UIGrid stars;

    public GameObject StarPrefab;
    public GameObject StarOutline;

    public ShowNewObject NewObjectController;
    public GameObject ShowNewButton;

    string CurItemId;

    List<GameObject> TempStars = new List<GameObject>();
    List<Transform> StarOutlineTransforms = new List<Transform>();

    public List<equipmentItemInterface> TargetEquipments = new List<equipmentItemInterface>();
    public List<UISprite> SourceSprites = new List<UISprite>();
    public List<UILabel> SourceTips = new List<UILabel>();

    public void SetDetail(int UserMaterialId)
    {
        UserItem u = UserManager.CurUserInfo.FindItemById(UserMaterialId);

        CurItemId = u.CurItemData.Id;

        SetDetail(u.CurItemData.Id);
    }

    public void SetDetail(string Id)
    {
        UnitCamera.SetActive(false);

        CurItemId = Id;

        ItemData Idata = ConfigManager.ItemConfig.GetItemById(Id);
        Texture t = Resources.Load<Texture>("Atlas/ItemIcons/" + Idata.SkinId);
        MaterialTexture.mainTexture = t;
        Name.text = Idata.Description;
        int MaterialCount = 0;
        foreach (UserItem ui in UserManager.CurUserInfo.UserItems)
        {
            if (ui.CurItemData.Id == Idata.Id)
            {
                MaterialCount++;
            }
        }
        Count.text = MaterialCount.ToString();
        Price.text = Idata.Price.ToString();

        int starCount = Idata.Rank;
        while (starCount > 0)
        {
            GameObject go = NGUITools.AddChild(stars.gameObject, StarOutline);
            StarOutlineTransforms.Add(go.transform);
            starCount--;
        }
        stars.Reposition();
        StartCoroutine(AddStars(0.2f, Idata.Rank));

        List<string> targetEquipmentIds = ConfigManager.HardwareMaterialConfig.GetMaterialTargetIds(Idata.Id);
        for (int i = 0; i < targetEquipmentIds.Count; i++)
        {
            HardWareData h = ConfigManager.HardWareConfig.GetHardWareById(targetEquipmentIds[i]);
            if (i < TargetEquipments.Count)
            {
                TargetEquipments[i].gameObject.SetActive(true);
                UserWare uw = new UserWare(h.Id, 1);
                TargetEquipments[i].SetItem(uw);
            }
        }
        for (int i = 0; i < SourceSprites.Count; i++)
        {
            SetSourceInfo(Id, i);
        }
    }

    bool newStart = false;
    IEnumerator AddStars(float time, int rank)
    {
        yield return new WaitForSeconds(time);
        for (int i = 0; i < rank; i++)
        {
            StartCoroutine(AddStarAnimation(i));
        }
        Invoke("SetNewStart", rank * 0.2f);
    }

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

    void SetSourceInfo(string Id, int index)
    {
        ItemData id = ConfigManager.ItemConfig.GetItemById(Id);
        if(id.Icon == "1")
        {
            SourceSprites[index].spriteName = "bottom_bg_quest";
        }
        else if (id.Icon == "2")
        {
            SourceSprites[index].spriteName = "bottom_bg_gacha";
        }
        SourceTips[index].text = id.Tips;
    }

    void OnEnable()
    {
        UIEventListener.Get(ShowNewButton).onClick = (g) =>
        {
            NewObjectController.ShowNew(0, CurItemId);
        };
    }

    void OnDisable()
    {
        if (!UnitCamera.activeSelf)
            UnitCamera.SetActive(true);
        foreach (equipmentItemInterface e in TargetEquipments)
        {
            e.gameObject.SetActive(false);
        }

        foreach (Transform t in stars.transform)
        {
            Destroy(t.gameObject);
        }
        foreach (GameObject g in TempStars)
        {
            Destroy(g);
        }
        TempStars.Clear();
        StarOutlineTransforms.Clear();
    }
}
