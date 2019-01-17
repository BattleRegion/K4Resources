using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface materialBagInter
{
    void _OnClickMaterialInter(int Id);
    void _OnLongPressMaterialInter(int Id);
}

public class MaterialBagControl : MonoBehaviour, _MaterialItemInterface
{
    public UIGrid materialBag;
    public GameObject materialItem;
    public UILabel Count;
    public List<GameObject> materialItems = new List<GameObject>();

    public materialBagInter inter;

    public GameObject eSortButton;

    /// <summary>
    /// 添加材料Item
    /// </summary>
    /// <param name="MaterialId"></param>
    public void AddMaterialItem(string MaterialId, int UserMaterialId)
    {
        GameObject iTem;
        iTem = NGUITools.AddChild(materialBag.gameObject, materialItem);
        iTem.transform.localScale = new Vector3(0.95f, 0.95f, 1f);
        materialBag.repositionNow = true;
        materialItems.Add(iTem);
        iTem.name = MaterialId;
        MaterialItemInterface mi;
        mi = iTem.GetComponent<MaterialItemInterface>();
        mi.SetMaterialItem(MaterialId);
        mi.userMaterialId = UserMaterialId;
        mi.materialItemInter = this;
        materialBag.Reposition();

        SetCount();
    }

    IEnumerator DelayAddMaterial(string MaterialId, int UserMaterialId, float time, bool closeCover = false)
    {
        yield return new WaitForSeconds(time);
        AddMaterialItem(MaterialId, UserMaterialId);
        if (closeCover)
        {
            ViewControl.SetCover(false);
        }
    }

    public void SetCount()
    {
        Count.text = (UserManager.CurUserInfo.UserItems.Count + UserManager.CurUserInfo.UserWares.Count).ToString() + "/" + UserManager.CurUserInfo.WareLimit.ToString();
    }

    /// <summary>
    /// 清空材料背包
    /// </summary>
    public void ClearMaterialBag()
    {
        for (int i = materialItems.Count - 1; i >= 0; i--)
        {
            GameObject rgo = materialItems[i];
            materialItems.Remove(materialItems[i]);
            Destroy(rgo);
        }
        materialBag.repositionNow = true;
    }

    public MaterialItemInterface GetItemById(int Id)
    {
        foreach (GameObject g in materialItems)
        {
            MaterialItemInterface m = g.GetComponent<MaterialItemInterface>();
            if (m.userMaterialId == Id)
            {
                return m;
            }
        }
        return null;
    }


    public GameObject CurPanel;
    public GameObject DetailPanel;

    public void RemoveItemById(int UserItemId)
    {
        foreach (GameObject go in materialItems)
        {
            if (go.GetComponent<MaterialItemInterface>().userMaterialId == UserItemId)
            {
                GameObject rgo = go;
                materialItems.Remove(go);
                Destroy(rgo);
                break;
            }
        }
        materialBag.repositionNow = true;
    }

    /// <summary>
    /// 默认星级排序
    /// </summary>
    List<UserItem> SortResult = new List<UserItem>();

    public void DelaySortMaterial()
    {
        Invoke("SortMaterial", 0.23f);
    }
    public void SortMaterial()
    {
        //ViewControl.SetCover(true);
        foreach (UserItem ui in UserManager.CurUserInfo.UserItems)
        {
            SortResult.Add(ui);
        }
        RankSort(0, SortResult.Count - 1);
        if (gameObject.activeSelf && CurPanel.name != "Sprite-Powerup")
        {
            for (int i = 0; i < SortResult.Count; i++)
            {
                StartCoroutine(DelayAddMaterial(SortResult[i].CurItemData.Id, SortResult[i].UserItemId, 0.005f * i, i == SortResult.Count - 1));
            }
        }
        else
        {
            for (int i = 0; i < SortResult.Count; i++)
            {
                AddMaterialItem(SortResult[i].CurItemData.Id, SortResult[i].UserItemId);
            }
        }
    }



    void ExchangeItem(int index1, int index2)
    {
        UserItem ui = SortResult[index1];
        SortResult[index1] = SortResult[index2];
        SortResult[index2] = ui;
    }

    void RankSort(int left, int right)
    {
        if(left < right)
        {
            int middle = RankPartition(left, right);
            RankSort(left, middle - 1);
            RankSort(middle + 1, right);
        }
    }

    int RankPartition(int left, int right)
    {
        while(left < right)
        {
            while(SortResult[right].CurItemData.Rank < SortResult[left].CurItemData.Rank)
            {
                right--;
            }
            if(left < right)
            {
                ExchangeItem(left, right);
                left++;
            }
            while(SortResult[left].CurItemData.Rank > SortResult[right].CurItemData.Rank)
            {
                left++;
            }
            if(left < right)
            {
                ExchangeItem(left, right);
                right--;
            }
        }
        return left;
    }

    public void SwitchDetail()
    {
        if (DetailPanel != null)
        {
            DetailPanel.SetActive(true);
        }
    }

    void OnEnable()
    {
        if(eSortButton != null)
        {
            eSortButton.SetActive(false);
        }
        materialBag.Reposition();
    }

    void OnDisable()
    {
        SortResult.Clear();
        if(eSortButton != null)
        {
            eSortButton.SetActive(true);
        }
    }

    public void DelayReposition()
    {
        materialBag.Reposition();
    }

    public void _OnClickMaterial(int Id)
    {
        if (inter != null)
        {
            inter._OnClickMaterialInter(Id);
        }
    }

    public void _OnLongPressMaterial(int Id)
    {
        if (inter != null)
        {
            inter._OnLongPressMaterialInter(Id);
            SwitchDetail();
            DetailPanel.GetComponent<MaterialDetail>().SetDetail(Id);
        }
    }
}
