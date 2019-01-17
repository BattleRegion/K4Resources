using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BSGridControl : MonoBehaviour 
{
    public UIGrid itemList;
    public GameObject weaponItem;
    List<GameObject> weapons = new List<GameObject>();

    public void AddItem(
        int ATK,
        int CriticalRate,
        string Cskill_1,
        string Cskill_2,
        string Cskill_3,
        string Name,
        int Num,
        int StarsNum,
        string WeaponSpriteName,
        int ItemTag  //标识该item的唯一tag，传给RemoveItem()可以移除该item
        )
    {
        GameObject iTem;
        iTem = NGUITools.AddChild(itemList.gameObject, weaponItem);
        weapons.Add(iTem);
        iTem.transform.FindChild("ATK").GetComponent<UILabel>().text = ATK.ToString();
        iTem.transform.FindChild("CriticalRate").GetComponent<UILabel>().text = CriticalRate.ToString() + "%";
        iTem.transform.FindChild("Cskill_1").GetComponent<UISprite>().spriteName = Cskill_1;
        iTem.transform.FindChild("Cskill_2").GetComponent<UISprite>().spriteName = Cskill_2;
        iTem.transform.FindChild("Cskill_3").GetComponent<UISprite>().spriteName = Cskill_3;
        iTem.transform.FindChild("Name").GetComponent<UILabel>().text = Name;
        iTem.transform.FindChild("Num").GetComponent<UILabel>().text = Num.ToString();
        iTem.transform.FindChild("WeaponSprite").GetComponent<UISprite>().spriteName = WeaponSpriteName;

        SetStars set = iTem.transform.FindChild("Stars").GetComponent<SetStars>();

        set.SetStar(StarsNum);

        itemList.repositionNow = true;
        iTem.name = ItemTag.ToString(); 
    }

    public void ClearList()  //清空武器列表
    {
        for (int i = weapons.Count - 1; i >= 0; i--)
        {
            GameObject rgo = weapons[i];
            weapons.Remove(weapons[i]);
            Destroy(rgo);
        }
        itemList.repositionNow = true;
    }

    public void RemoveItem(int ItemTag) 
    {
        foreach (GameObject go in weapons)
        {
            if (go.name == ItemTag.ToString())
            {
                GameObject rgo = go;
                weapons.Remove(go);
                Destroy(rgo);
                break;
            }
        }
        itemList.repositionNow = true;
    }

/*    void Start()
    {
        AddItem(9999, 99, "", "", "", "xilidejian", 99, 5, ""); 
    }*/

}
