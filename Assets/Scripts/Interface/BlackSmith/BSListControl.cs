using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BSListControl : MonoBehaviour, EquipmentMenuItemInter
{
    public enum HardwareType
    {
        LightWeapon = 0,
        HeavyWeapon = 1,
        RemoteWeapon = 2,
        Helmet = 3,
        Armor = 4
    }


    /// <summary>
    /// 选择是哪种类型装备的List
    /// </summary>
    public HardwareType type;

    public UIGrid itemList;
    public GameObject equipmentMenuItem;
    public ListViewAnime ListAnime;
    List<GameObject> menuItems = new List<GameObject>();

    /// <summary>
    /// 显示给用户的轻武器条目的ID List 
    /// </summary>
    List<string> ShowLightWeaponIds = new List<string>();

    /// <summary>
    /// 显示给用户的重武器条目的ID List 
    /// </summary>
    List<string> ShowHeavyWeaponIds = new List<string>();

    /// <summary>
    /// 显示给用户的远程武器条目的ID List 
    /// </summary>
    List<string> ShowRemoteWeaponIds = new List<string>();

    /// <summary>
    /// 显示给用户的头盔条目ID List
    /// </summary>
    List<string> ShowHelmetIds = new List<string>();

    /// <summary>
    /// 显示给用户的护甲条目ID List
    /// </summary>
    List<string> ShowArmorIds = new List<string>();

    /// <summary>
    /// 数量
    /// </summary>
    int Count;

    public SetWeaponDetail makeWeapon;
    public SetArmorDetail makeArmor;

    public void AddItem(
        string EquipmentID,
        string Name,
        DungeonEnum.ElementAttributes ItemElementType,
        int StarsNum,
        int Atk,
        int Def,
        int Cskill_Num_1,
        int Cskill_Num_2,
        int Cskill_Num_3,
        int HaveNum,
        bool CouldMake
        )
    {
        GameObject menuItem;
        menuItem = NGUITools.AddChild(itemList.gameObject, equipmentMenuItem);
        menuItems.Add(menuItem);

        EquipmentMenuItemInterface e = menuItem.GetComponent<EquipmentMenuItemInterface>();
        e.SetMenuItem(EquipmentID, Name, ItemElementType, StarsNum, Atk, Def, Cskill_Num_1, Cskill_Num_2, Cskill_Num_3, HaveNum, CouldMake);

        menuItem.name = Count.ToString();
        Count++;

        ListAnime.cells.Add(menuItem);
        menuItem.SetActive(false);

        menuItem.GetComponent<EquipmentMenuItemInterface>().equipInter = this;
    }

    public void ClearList()  //清空列表
    {
        for (int i = menuItems.Count - 1; i >= 0; i--)
        {
            GameObject rgo = menuItems[i];
            menuItems.Remove(menuItems[i]);
            Destroy(rgo);
        }
        itemList.repositionNow = true;
    }

    int GetHaveNumById(string Id)
    {
        int count = 0;
        foreach (UserWare uw in UserManager.CurUserInfo.UserWares)
        {
            if (uw.CurHardWareData.Id == Id)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// 返回是否可以制作该武器
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    static bool GetAvailableById(string Id)
    {
        List<int> haveNumList = new List<int>();   //用于记录所有需要各素材Item数量
        List<int> needNumList = new List<int>();
        int tag = 0;
        HardwareMaterialData hwd = ConfigManager.HardwareMaterialConfig.GetHardwareMaterialById(Id);
        if (hwd.Rate_1 > 0) {haveNumList.Add(0); needNumList.Add(hwd.Rate_1);}
        if (hwd.Rate_2 > 0) {haveNumList.Add(0); needNumList.Add(hwd.Rate_2);}
        if (hwd.Rate_3 > 0) {haveNumList.Add(0); needNumList.Add(hwd.Rate_3);}
        if (hwd.Rate_4 > 0) {haveNumList.Add(0); needNumList.Add(hwd.Rate_4);}
        if (hwd.Rate_5 > 0) {haveNumList.Add(0); needNumList.Add(hwd.Rate_5);}
        foreach (UserItem ui in UserManager.CurUserInfo.UserItems)
        {
            if (ui.CurItemData.Id == hwd.MaterialId_1 && haveNumList.Count > 0) haveNumList[0]++;
            if (ui.CurItemData.Id == hwd.MaterialId_2 && haveNumList.Count > 1) haveNumList[1]++;
            if (ui.CurItemData.Id == hwd.MaterialId_3 && haveNumList.Count > 2) haveNumList[2]++;
            if (ui.CurItemData.Id == hwd.MaterialId_4 && haveNumList.Count > 3) haveNumList[3]++;
            if (ui.CurItemData.Id == hwd.MaterialId_5 && haveNumList.Count > 4) haveNumList[4]++;
        }
        for (int i = 0; i < haveNumList.Count; i++)
        {
            if (haveNumList[i] >= needNumList[i])
            {
                tag++;
            }
        }
        if (tag >= needNumList.Count) return true;
        else return false;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void OnEnable()
    {
        switch ((int)type)
        {
            case 0:
                {
                    ShowLightWeaponIds.Clear();
                    ShowLightWeaponIds = ConfigManager.HardwareMaterialConfig.GetShowHardwareList(type);
                    foreach (string str in ShowLightWeaponIds)
                    {
                        HardWareData hd = ConfigManager.HardWareConfig.GetHardWareById(str);
                        AddItem(hd.Id, hd.Name, hd.Element, hd.Rank, hd.Attack, hd.Defend, hd.SkillAffix1Amount, hd.SkillAffix2Amount, hd.SkillAffix3Amount, GetHaveNumById(hd.Id), GetAvailableById(hd.Id));
                    }
                    break;
                }
            case 1:
                {
                    ShowHeavyWeaponIds.Clear();
                    ShowHeavyWeaponIds = ConfigManager.HardwareMaterialConfig.GetShowHardwareList(type);
                    foreach (string str in ShowHeavyWeaponIds)
                    {
                        HardWareData hd = ConfigManager.HardWareConfig.GetHardWareById(str);
                        AddItem(hd.Id, hd.Name, hd.Element, hd.Rank, hd.Attack, hd.Defend, hd.SkillAffix1Amount, hd.SkillAffix2Amount, hd.SkillAffix3Amount, GetHaveNumById(hd.Id), GetAvailableById(hd.Id));
                    }
                    break;
                }
            case 2:
                {
                    ShowRemoteWeaponIds.Clear();
                    ShowRemoteWeaponIds = ConfigManager.HardwareMaterialConfig.GetShowHardwareList(type);
                    foreach (string str in ShowRemoteWeaponIds)
                    {
                        HardWareData hd = ConfigManager.HardWareConfig.GetHardWareById(str);
                        AddItem(hd.Id, hd.Name, hd.Element, hd.Rank, hd.Attack, hd.Defend, hd.SkillAffix1Amount, hd.SkillAffix2Amount, hd.SkillAffix3Amount, GetHaveNumById(hd.Id), GetAvailableById(hd.Id));
                    }
                    break;
                }
            case 3:
                {
                    ShowHelmetIds.Clear();
                    ShowHelmetIds = ConfigManager.HardwareMaterialConfig.GetShowHardwareList(type);
                    foreach (string str in ShowHelmetIds)
                    {
                        HardWareData hd = ConfigManager.HardWareConfig.GetHardWareById(str);
                        AddItem(hd.Id, hd.Name, hd.Element, hd.Rank, hd.Attack, hd.Defend, hd.SkillAffix1Amount, hd.SkillAffix2Amount, hd.SkillAffix3Amount, GetHaveNumById(hd.Id), GetAvailableById(hd.Id));
                    }
                    break;
                }
            case 4:
                {
                    ShowArmorIds.Clear();
                    ShowArmorIds = ConfigManager.HardwareMaterialConfig.GetShowHardwareList(type);
                    foreach (string str in ShowArmorIds)
                    {
                        HardWareData hd = ConfigManager.HardWareConfig.GetHardWareById(str);
                        AddItem(hd.Id, hd.Name, hd.Element, hd.Rank, hd.Attack, hd.Defend, hd.SkillAffix1Amount, hd.SkillAffix2Amount, hd.SkillAffix3Amount, GetHaveNumById(hd.Id), GetAvailableById(hd.Id));
                    }
                    break;
                }
            default: break;
        }
        itemList.Reposition();
        ListAnime.enabled = true;
    }


    void OnDisable()
    {
        Count = 0;
        ListAnime.cells.Clear(); //清空用于动画的List
        ListAnime.enabled = false;
        ClearList();
    }
    
    public void _OnClickMenuItem(string HardwareID)
    {
        HardWareData h = ConfigManager.HardWareConfig.GetHardWareById(HardwareID);
        HardwareMaterialData hd = ConfigManager.HardwareMaterialConfig.GetHardwareMaterialById(HardwareID);
        if ((int)h.Style < 5 && makeWeapon != null) //武器
        {
            makeWeapon.SetWeaponInfo(
                HardwareID,
                "CskillName",
                "CskillMap_1",
                "CskillMap_2",
                "CskillMap_3",
                h.SkillAffix1Amount,
                h.SkillAffix2Amount,
                h.SkillAffix3Amount,
                "CskillEffect",
                hd.MaterialId_1,
                hd.Rate_1,
                GetMaterialNum(hd.MaterialId_1),
                hd.MaterialId_2,
                hd.Rate_2,
                GetMaterialNum(hd.MaterialId_2),
                hd.MaterialId_3,
                hd.Rate_3,
                GetMaterialNum(hd.MaterialId_3),
                hd.MaterialId_4,
                hd.Rate_4,
                GetMaterialNum(hd.MaterialId_4),
                hd.MaterialId_5,
                hd.Rate_5,
                GetMaterialNum(hd.MaterialId_5),
                hd.Price);
            SwitchMakeView();
        }
        else if((int)h.Style > 5 && makeArmor != null)   //护甲
        {
            makeArmor.SetWeaponInfo(
                HardwareID,
                hd.MaterialId_1,
                hd.Rate_1,
                GetMaterialNum(hd.MaterialId_1),
                hd.MaterialId_2,
                hd.Rate_2,
                GetMaterialNum(hd.MaterialId_2),
                hd.MaterialId_3,
                hd.Rate_3,
                GetMaterialNum(hd.MaterialId_3),
                hd.MaterialId_4,
                hd.Rate_4,
                GetMaterialNum(hd.MaterialId_4),
                hd.MaterialId_5,
                hd.Rate_5,
                GetMaterialNum(hd.MaterialId_5),
                hd.Price);
            SwitchMakeView();
        }
    }

    int GetMaterialNum(string MaterialID)
    {
        int count = 0;
        foreach (UserItem ui in UserManager.CurUserInfo.UserItems)
        {
            if (ui.CurItemData.Id == MaterialID)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// 当前场景
    /// </summary>
    public GameObject CurView;

    /// <summary>
    /// 目标场景
    /// </summary>
    public GameObject TargetView;

    /// <summary>
    /// 返回条（用于动画退出）
    /// </summary>
    public GameObject BackBar;

    /// <summary>
    /// 主要界面（动画退出）
    /// </summary>
    public GameObject MainBoard;

    public GameObject Bubble;

    public GameObject Avatar;

    public GameObject InfoText;

    void SwitchMakeView()
    {

        AnimationHelper.AnimationMoveTo(new Vector3(-800, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(800, MainBoard.transform.localPosition.y, MainBoard.transform.localPosition.z), MainBoard, iTween.EaseType.linear, gameObject, "SceneSwitch", 0.2f);
        Bubble.SetActive(false);
        Avatar.SetActive(false);
    }

    void SceneSwitch()
    {
        CurView.SetActive(false);
        if (TargetView != null)
        {
            TargetView.SetActive(true);
        }
    }
}
