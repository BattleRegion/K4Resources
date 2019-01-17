using UnityEngine;
using System.Collections;
using PomeloSocketCenter.PomeloLib;
using System.Collections.Generic;
using SimpleJson;

public class HardwareSkillEvolution : MonoBehaviour
{
    public GameObject SkillEvolutionButton;
    int CurUid;

    UserWare Curware
    {
        get
        {
            return UserManager.CurUserInfo.FindUserWare(CurUid);
        }
        set
        {
            Curware = value;
        }
    }

    #region NGUI
    public UILabel PreName;
    public UILabel LaterName;
    public UILabel PreLevel;
    public UILabel LaterLevel;
    public UILabel PreChain;
    public UILabel LaterChain;
    public UILabel PreDescription;
    public UILabel LaterDescription;
    public UISprite PreMap;
    public UISprite LaterMap;
    public UILabel Price;
    public UILabel Tips;
    #endregion

    public HardwareSkillMaterial Material_1;
    public HardwareSkillMaterial Material_2;
    public HardwareSkillMaterial Material_3;
    public HardwareSkillMaterial Material_4;

    public void SetSkillEvolution(int UwId)
    {
        CurUid = UwId;
        UserWare uw = UserManager.CurUserInfo.FindUserWare(UwId);
        //SkillData skill1 = ConfigManager.SkillConfig.GetSkillByIdLv(uw.CurHardWareData.SkillAffix1, uw.CurSkillLv);
        //SkillData skill2 = ConfigManager.SkillConfig.GetSkillByIdLv(uw.CurHardWareData.SkillAffix1, uw.CurSkillLv + 1);

        //PreName.text = skill1.Name;
        //LaterName.text = skill2.Name;
        //PreLevel.text = "Lv." + skill1.SkillLv;
        //LaterLevel.text = "Lv." + skill2.SkillLv;
        //PreChain.text = skill1.Bparameter.ToString();
        //LaterChain.text = skill2.Bparameter.ToString();
        //PreDescription.text = skill1.Description;
        //LaterDescription.text = skill2.Description;
        //PreMap.spriteName = skill1.SkillIcon;
        //LaterMap.spriteName = skill2.SkillIcon;

        //Price.text = skill2.UpCoin.ToString();

        //Material_1.SetMaterial(skill1.UpN1, (FindMaterial(skill1.UpN1) - Bool2Int(skill1.UpN1 == Curware.CurHardWareData.Id)));
        //Material_2.SetMaterial(skill1.UpN2, (FindMaterial(skill1.UpN2) - Bool2Int(skill1.UpN2 == Curware.CurHardWareData.Id)));
        //Material_3.SetMaterial(skill1.UpN3, (FindMaterial(skill1.UpN3) - Bool2Int(skill1.UpN3 == Curware.CurHardWareData.Id)));
        //Material_4.SetMaterial(skill1.UpN4, (FindMaterial(skill1.UpN4) - Bool2Int(skill1.UpN4 == Curware.CurHardWareData.Id)));

        //if (ConditionTest(skill1.UpN1, skill1.UpN2, skill1.UpN3, skill1.UpN4))
        //{
        //    Tips.text = "";
        //    SkillEvolutionButton.GetComponent<BoxCollider>().enabled = true;
        //}
        //else
        //{
        //    Tips.text = "素材不足";
        //    SkillEvolutionButton.GetComponent<BoxCollider>().enabled = false;
        //}
    }

    bool ConditionTest(string m1, string m2, string m3, string m4)
    {
        List<string> IdList = new List<string>();
        foreach (UserWare uw in UserManager.CurUserInfo.UserWares)
        {
            IdList.Add(uw.CurHardWareData.Id);
        }
        foreach (UserItem ui in UserManager.CurUserInfo.UserItems)
        {
            IdList.Add(ui.CurItemData.Id);
        }
        IdList.Remove(Curware.CurHardWareData.Id);
        if (!string.IsNullOrEmpty(m1))
        {
            if (IdList.Contains(m1))
            {
                IdList.Remove(m1);
            }
            else return false;
        }
        if (!string.IsNullOrEmpty(m2))
        {
            if (IdList.Contains(m2))
            {
                IdList.Remove(m2);
            }
            else return false;
        }
        if (!string.IsNullOrEmpty(m3))
        {
            if (IdList.Contains(m3))
            {
                IdList.Remove(m3);
            }
            else return false;
        }
        if (!string.IsNullOrEmpty(m4))
        {
            if (IdList.Contains(m4))
            {
                return true;
            }
            else return false;
        }
        return true;
    }

    int Bool2Int(bool b)
    {
        if (b) return 1;
        else return 0;
    }

    public int FindMaterial(string Id)
    {
        int count = 0;
        if (ConfigManager.ItemConfig.GetItemById(Id) != null)
        {
            foreach (UserItem ui in UserManager.CurUserInfo.UserItems)
            {
                if (ui.CurItemData.Id == Id)
                {
                    count++;
                }
            }
        }
        else
        {
            foreach (UserWare uw in UserManager.CurUserInfo.UserWares)
            {
                if (uw.CurHardWareData.Id == Id)
                {
                    count++;
                }
            }
        }
        return count;
    }

    void OnEnable()
    {
        UIEventListener.Get(SkillEvolutionButton).onClick = (g) =>
        {
            SkillEvolutionRequest();
        };
    }

    void OnDisable()
    {
        Material_1.SetMaterial(null, 0);
        Material_2.SetMaterial(null, 0);
        Material_3.SetMaterial(null, 0);
        Material_4.SetMaterial(null, 0);
    }

    public HSkillUpAnimCon SkillUpAnimation;

    void SkillEvolutionRequest()
    {
        JsonObject args = new JsonObject();
        args.Add("house_id", CurUid);
        SocketCenter.Request(GameRouteConfig.WeaponSkill, args, (r) =>
        {
            if (r.Code == SocketResult.ResultCode.Success)
            {
                Loom.QueueOnMainThread(() =>
                {
                    UserManager.CurUserInfo.AddElements((JsonArray)r.Data["consumes"]);
                    UserWare NewWare = new UserWare((JsonObject)r.Data["weapon"]);
                    UserManager.CurUserInfo.UserWares.Remove(Curware);
                    UserManager.CurUserInfo.UserWares.Add(NewWare);
                    JsonArray ItemMaterials = (JsonArray)r.Data["item_materials"];
                    string[] MaterialIds = new string[5];
                    for (int i = 0; i < ItemMaterials.Count; i++ )
                    {
                        int Uid = int.Parse(ItemMaterials[i].ToString());
                        if (UserManager.CurUserInfo.FindItemById(Uid) != null)
                        {
                            UserItem ui = UserManager.CurUserInfo.FindItemById(Uid);
                            UserManager.CurUserInfo.UserItems.Remove(ui);
                            MaterialIds[i] = ui.CurItemData.SkinId;
                        }
                        else
                        {
                            UserWare uw = UserManager.CurUserInfo.FindUserWare(Uid);
                            UserManager.CurUserInfo.UserWares.Remove(uw);
                            MaterialIds[i] = uw.CurHardWareData.SkinId;
                        }
                    }

                    //Animation
                    SkillUpAnimation.Hbase = NewWare;
                    SkillUpAnimation.gameObject.SetActive(true);
                    SkillUpAnimation.SetAnimation(NewWare.CurHardWareData.SkinId, MaterialIds[0], MaterialIds[1], MaterialIds[2], MaterialIds[3], MaterialIds[4]);
                });
            }
        }, null, true, true);

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

    void SwitchView()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(-800, BackBar.transform.localPosition.y, BackBar.transform.localPosition.z), BackBar, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(800, MainBoard.transform.localPosition.y, MainBoard.transform.localPosition.z), MainBoard, iTween.EaseType.linear, gameObject, "SceneSwitch", 0.2f);
    }

    public void SceneSwitch()
    {
        CurView.SetActive(false);
        if (TargetView != null)
        {
            TargetView.SetActive(true);
        }
    }
}
