using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PomeloSocketCenter.PomeloLib;
using System;
using SimpleJson;

public class DiamondLottery : MonoBehaviour
{
    #region 引用
    public enum LotteryAnimationState
    {
        Default = 0,
        Click = 1,
        Over = 2,
        refresh = 3
    }

    public Animator LotteryAnimator;

    public GameObject ClickButton;
    public GameObject RefreshButton;

    public UILabel LotteryConsume;
    public UILabel RefreshConsume;

    public List<SpriteRenderer> Items;

    public List<GameObject> ItemButtons;

    public SpriteRenderer CenterItem;

    public GameObject FlashPrefab;

    public SetUser User;

    bool wait = false; //动画中 屏蔽点击

    public GameObject Cover;


    public GameObject PetDetail;

    public GameObject WeaponDetail;

    public GameObject ArmorDetail;

    public GameObject ItemDetail;
    #endregion

    #region 通信
    string RewardId;
    bool isNew;
    LotteryEnum.AwardType RewardType;
    /// <summary>
    /// 刷新奖池请求
    /// </summary>
    void RefreshTurnTabelRequest(Action callback)
    {
        wait = true;
        SocketCenter.Request(GameRouteConfig.ResetLotteryTurntable, null, (result) =>
        {
            Loom.QueueOnMainThread(() =>
            {
                if (result.Code == SocketResult.ResultCode.Success)
                {
                    UserManager.CurUserInfo.AddElements((JsonArray)result.Data["consumes"]);
                    UserManager.CurUserInfo.UserLotteryItems.Clear();
                    foreach (JsonObject lotteryData in (JsonArray)result.Data["pool_items"])
                    {
                        LotteryItemData lo = new LotteryItemData(lotteryData);
                        UserManager.CurUserInfo.UserLotteryItems.Add(lo);
                    }
                    User.SetInfo();
                    callback();
                }
                else
                {
                    wait = false;
                }
            });
        }, null, true, true);
    }

    /// <summary>
    /// 抽奖请求
    /// </summary>
    void LotteryRequest(Action<int> callback)
    {
        wait = true;
        SocketCenter.Request(GameRouteConfig.DiamondLottery, null, (result) =>
        {
            Loom.QueueOnMainThread(() =>
            {
                if (result.Code == SocketResult.ResultCode.Success)
                {
                    foreach(JsonObject data in (JsonArray)result.Data["elements"])
                    {
                        if (data["type"].ToString() == "5")
                        {
                            isNew = ConfigManager.PetConfig.IsNew(data["id"].ToString());
                            break;
                        }
                        else if(data["type"].ToString() == "4")
                        {
                            isNew = ConfigManager.HardWareConfig.IsNew(data["id"].ToString());
                            break;
                        }
                        else if(data["type"].ToString() == "2")
                        {
                            isNew = ConfigManager.ItemConfig.IsNew(data["id"].ToString());
                            break;
                        }
                        else isNew = false;
                    }
                    UserManager.CurUserInfo.AddElements((JsonArray)result.Data["elements"]);
                    User.SetInfo();
                    int index = int.Parse(result.Data["pool_index"].ToString());
                    RewardId = UserManager.CurUserInfo.UserLotteryItems[index].Id;
                    RewardType = UserManager.CurUserInfo.UserLotteryItems[index].AwardType;
                    UserManager.CurUserInfo.UserLotteryItems.Clear();
                    foreach (JsonObject lotteryData in (JsonArray)result.Data["pool_items"])
                    {
                        LotteryItemData lo = new LotteryItemData(lotteryData);
                        UserManager.CurUserInfo.UserLotteryItems.Add(lo);
                    }
                    callback(index);
                }
                else
                {
                    wait = false;
                }
            });
        }, null, true, true);
    }
    #endregion

    #region Action
    /// <summary>
    /// 切换animator状态
    /// </summary>
    void ChangeAnimatorController(LotteryAnimationState state)
    {
        LotteryAnimator.SetInteger("ActionState", (int)state);
    }

    /// <summary>
    /// 刷新Icon闪烁
    /// </summary>
    void RefreshFlash()
    {
        foreach (SpriteRenderer s in Items)
        {
            GameObject flash = Instantiate(FlashPrefab, s.transform.position, s.transform.rotation) as GameObject;
        }
    }

    /// <summary>
    /// 重置抽奖界面
    /// </summary>
    void Reset()
    {
        if (TempObject != null)
        {
            Destroy(TempObject);
        }
        LotteryAnimator.SetInteger("ActionState", 0);
    }

    void ToDetail(int i)
    {
        if (i >= UserManager.CurUserInfo.UserLotteryItems.Count) return;

        switch (UserManager.CurUserInfo.UserLotteryItems[i].AwardType)
        {
            case LotteryEnum.AwardType.None: break;
            case LotteryEnum.AwardType.Hardware:
                {
                    if ((int)ConfigManager.HardWareConfig.GetHardWareById(UserManager.CurUserInfo.UserLotteryItems[i].Id).Style < 5)
                    {
                        WeaponDetail.SetActive(true);
                        UserWare weapon = new UserWare(UserManager.CurUserInfo.UserLotteryItems[i].Id, 1);
                        WeaponDetail.GetComponent<WeaponDetail>().SetDetail(weapon);
                    }
                    else
                    {
                        ArmorDetail.SetActive(true);
                        UserWare armor = new UserWare(UserManager.CurUserInfo.UserLotteryItems[i].Id, 1);
                        ArmorDetail.GetComponent<ArmorDetail>().SetDetail(armor);
                    }
                    break;
                }
            case LotteryEnum.AwardType.Item:
                {
                    ItemDetail.SetActive(true);
                    ItemDetail.GetComponent<MaterialDetail>().SetDetail(UserManager.CurUserInfo.UserLotteryItems[i].Id);
                    break;
                }
            case LotteryEnum.AwardType.Pet:
                {
                    PetDetail.SetActive(true);
                    UserPet pet = new UserPet(UserManager.CurUserInfo.UserLotteryItems[i].Id, 1);
                    PetDetail.GetComponent<SetMonsterDetail>().SetDetail(pet);
                    break;
                }
            default: break;
        }
    }
    #endregion

    #region 抽奖
    /// <summary>
    /// 切换至抽奖动画
    /// </summary>
    Action LotteryOverAction;

    void LotteryAnimation(int index, Action callback)
    {
        LotteryOverAction = callback;
        ChangeAnimatorController(LotteryAnimationState.Click);
        Invoke("DelayAction", 3f); // 7.5
        StartCoroutine(DelayFlash(index, 2.5f));
        StartCoroutine(DelayFlash(index, 3f));
        switch (RewardType)
        {
            case LotteryEnum.AwardType.None: break;
            case LotteryEnum.AwardType.Pet:
                {
                    StartCoroutine(PetLotteryAction(index, 2f)); //7
                    break;
                }
            case LotteryEnum.AwardType.Hardware:
                {
                    StartCoroutine(ItemLotteryAction(index, 2f));
                    break;
                }
            case LotteryEnum.AwardType.Item:
                {
                    StartCoroutine(ItemLotteryAction(index, 2f));
                    break;
                }
            default: break;
        }
    }

    /// <summary>
    /// 动画结束的回调
    /// </summary>
    void DelayAction()
    {
        LotteryOverAction();
        ChangeAnimatorController(LotteryAnimationState.Over);
    }

    GameObject TempObject;
    /// <summary>
    /// 抽一次的prefab飞入动画
    /// </summary>
    IEnumerator PetLotteryAction(int index, float delay)
    {
        if (TempObject != null) Destroy(TempObject);
        yield return new WaitForSeconds(delay);

        if (isNew)
        {
            ShowNew(1, RewardId);
        }


        GameObject CenterObject = Instantiate(Resources.Load<GameObject>("PreFabs/Characters/" + Tools.GetSkinIdById(RewardId) + "60")) as GameObject;
        TempObject = CenterObject;
        CenterObject.transform.localScale = Vector3.zero;
        Tools.SetLayer(CenterObject.transform, LayerHelper.Top);
        Items[index].color = Color.black;
        CenterObject.transform.position = new Vector3(0f, -0.21f, 0f);
        AnimationHelper.AnimationScaleTo(new Vector3(0.11f, 0.11f, 1f), CenterObject, iTween.EaseType.easeOutElastic, null, null, 1f);
    }

    /// <summary>
    /// 装备素材飞入
    /// </summary>
    IEnumerator ItemLotteryAction(int index, float delay)
    {

        if (TempObject != null) Destroy(TempObject);
        yield return new WaitForSeconds(delay);

        if (isNew)
        {
            ShowNew(0, RewardId);
        }

        GameObject CenterObject = NGUITools.AddChild(gameObject, Resources.Load<GameObject>("PreFabs/UI/ItemSkin"));
        CenterObject.GetComponent<SkinPrefabControl>().SetSkin(RewardId);
        TempObject = CenterObject;
        CenterObject.transform.localScale = Vector3.zero;
        Tools.SetLayer(CenterObject.transform, LayerHelper.Top);
        Items[index].color = Color.black;
        CenterObject.transform.localPosition = new Vector3(0f, -40f, 0f);
        AnimationHelper.AnimationScaleTo(new Vector3(70f, 70f, 1f), CenterObject, iTween.EaseType.easeOutElastic, null, null, 1f);
    }

    /// <summary>
    /// 抽中的icon闪烁
    /// </summary>
    IEnumerator DelayFlash(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpriteRenderer s = Items[index];
        GameObject flash = Instantiate(FlashPrefab, s.transform.position, s.transform.rotation) as GameObject;
    }

    #endregion

    #region 刷新奖池
    /// <summary>
    /// 刷新
    /// </summary>
    void RefreshTurnTable()
    {
        RefreshFlash();
        for (int i = 0; i < UserManager.CurUserInfo.UserLotteryItems.Count; i++)
        {
            if (UserManager.CurUserInfo.UserLotteryItems[i].AwardType == LotteryEnum.AwardType.Pet)
            {
                Items[i].transform.localScale = Vector3.one;
                Items[i].sprite = Resources.Load<Sprite>(Tools.GetLotteryIconPath(UserManager.CurUserInfo.UserLotteryItems[i].Id));
            }
            else if (UserManager.CurUserInfo.UserLotteryItems[i].AwardType == LotteryEnum.AwardType.Item || UserManager.CurUserInfo.UserLotteryItems[i].AwardType == LotteryEnum.AwardType.Hardware)
            {
                Items[i].transform.localScale = 0.007f * Vector3.one;
                Items[i].sprite = Resources.Load<Sprite>(Tools.GetIconPath(UserManager.CurUserInfo.UserLotteryItems[i].Id));
            }
            if (!UserManager.CurUserInfo.UserLotteryItems[i].Status)
            {
                Items[i].color = Color.black;
            }
            else
            {
                Items[i].color = Color.white;
            }
        }
    }

    /// <summary>
    /// 刷新动画
    /// </summary>
    Action RefreshOverAction;

    void RefreshAnimation(Action callback)
    {
        RefreshOverAction = callback;
        ChangeAnimatorController(LotteryAnimationState.refresh);
        Invoke("DelayRefresh", 2.5f);
    }

    void DelayRefresh()
    {
        RefreshOverAction();
    }
    #endregion

    #region mono
    void OnEnable()
    {
        LotteryConsume.text = ConfigManager.ParamConfig.GetParam().WheelPrice.ToString();
        RefreshConsume.text = ConfigManager.ParamConfig.GetParam().WheelResetPrice.ToString();
        RefreshTurnTable();
        UIEventListener.Get(ClickButton).onClick = (g) => 
        {
            if(!wait)
            {
                LotteryRequest((index) =>
                {
                    Cover.SetActive(true);
                    LotteryAnimation(index, () =>
                    {
                        wait = false;
                    });
                });
            }
        };

        UIEventListener.Get(RefreshButton).onClick = (g) =>
        {
            if(!wait)
            {
                RefreshTurnTabelRequest(() =>
                {
                    Cover.SetActive(true);
                    RefreshAnimation(() =>
                    {
                        ChangeAnimatorController(LotteryAnimationState.Default);
                        RefreshTurnTable();
                        Cover.SetActive(false);
                        wait = false;
                    });
                });
            }
        };

        UIEventListener.Get(Cover).onClick = (g) =>
        {
            if(!wait)
            {
                Reset();
                int count = 0;
                foreach (LotteryItemData item in UserManager.CurUserInfo.UserLotteryItems)
                {
                    if (item.Status) count++;
                }
                if (count == UserManager.CurUserInfo.UserLotteryItems.Count)
                {
                    RefreshAnimation(() => 
                    {
                        ChangeAnimatorController(LotteryAnimationState.Default);
                        RefreshTurnTable();
                        Cover.SetActive(false);
                    });
                }
                else
                {
                    Cover.SetActive(false);
                }
            }
        };

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

                UnitCamera.SetActive(true);
                NewBoard.SetActive(false);
                NewBg.SetActive(false);
            }
        };


        #region 详情按钮
        UIEventListener.Get(ItemButtons[0]).onClick = (go) =>
        {
            ToDetail(0);
        };

        UIEventListener.Get(ItemButtons[1]).onClick = (go) =>
        {
            ToDetail(1);
        };

        UIEventListener.Get(ItemButtons[2]).onClick = (go) =>
        {
            ToDetail(2);
        };

        UIEventListener.Get(ItemButtons[3]).onClick = (go) =>
        {
            ToDetail(3);
        };

        UIEventListener.Get(ItemButtons[4]).onClick = (go) =>
        {
            ToDetail(4);
        };

        UIEventListener.Get(ItemButtons[5]).onClick = (go) =>
        {
            ToDetail(5);
        };

        UIEventListener.Get(ItemButtons[6]).onClick = (go) =>
        {
            ToDetail(6);
        };

        UIEventListener.Get(ItemButtons[7]).onClick = (go) =>
        {
            ToDetail(7);
        };

        UIEventListener.Get(ItemButtons[8]).onClick = (go) =>
        {
            ToDetail(8);
        };

        UIEventListener.Get(ItemButtons[9]).onClick = (go) =>
        {
            ToDetail(9);
        };
        #endregion
    }
    #endregion

    #region NEW ITEM
    /// <summary>
    /// 新的材料或者宠物独立显示
    /// </summary>
    public GameObject NewBoard;
    public GameObject NewBg;
    public UITexture mTexture;
    public GameObject UnitCamera;

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