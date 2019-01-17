using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;

public class OverControl : MonoBehaviour, _MaterialItemInterface, itemInterface, EquipmentiIemInterface
{
    public GameObject backBar;
    public UILabel goldGot;
    public UILabel expGot;
    public AlphaMaskBar expBar;
    public UILabel expLeft;
    public GameObject petPrefab;
    public GameObject materialPrefab;
    public GameObject hardwarePrefab;
    public GameObject title_materialGot;
    public GameObject title_petGot;
    public UIGrid materialGrid;
    public UIGrid petGrid;

    public UISprite achievementDiamond;
    public GameObject AchieveBoard;
    public UILabel AchieveLabel;

    public List<ItemInterface> materials = new List<ItemInterface>();
    public List<MaterialItemInterface> pets = new List<MaterialItemInterface>();

    /// <summary>
    /// 箱子和蛋动画的预设
    /// </summary>
    public List<GameObject> Eggs_s = new List<GameObject>();
    public List<GameObject> Chests_s = new List<GameObject>();
    public List<GameObject> Eggs_b = new List<GameObject>();
    public List<GameObject> Chests_b = new List<GameObject>();

    public GameObject dungeonView;
    public GameObject partyView;

    public GameObject MaterialDetail;
    public GameObject PetDetail;
    public GameObject WeaponDetail;
    public GameObject ArmorDetail;


    /// <summary>
    /// 获得金币
    /// </summary>
    public static int gold = 0;

    /// <summary>
    /// 获得经验
    /// </summary>
    public static int exp = 0;

    /// <summary>
    /// 前总经验
    /// </summary>
    public static int lvTotalExp = 0;

    /// <summary>
    /// 前经验
    /// </summary>
    public static int lvCurExp = 0;

    /// <summary>
    /// 升级数
    /// </summary>
    public static int lvAddition = 0;

    /// <summary>
    /// 获得素材ID数组
    /// </summary>
    public static List<string> materialIds = new List<string>();
    public static List<int> materialUids = new List<int>();
    public static List<bool> materialNewTag = new List<bool>();

    /// <summary>
    /// 获得宠物ID数组
    /// </summary>
    public static List<string> petIds = new List<string>();
    public static List<int> petUids = new List<int>();
    public static List<bool> petNewTag = new List<bool>();

    /// <summary>
    /// 是否需要进入结算
    /// </summary>
    public static bool isOver = false;

    /// <summary>
    /// 结算是否结束
    /// </summary>
    public static bool overDone = false;

    #region 动画参数
    int BackBarInitX = -800;
    int ListInitX = 800;
    int backBarX = 0;
    int goldGotX = 170;
    int expGotX = 170;
    int materialGotX = 0;
    int petGotX = 0;
    int materialGridX = -210;
    int petGridX = -210;
    float animeTime = 1f;
    #endregion




    /// <summary>
    /// 播放数字滚动音效
    /// </summary>
    //void PlayScrollAudio()
    //{
    //    AudioClip Ac = Resources.Load<AudioClip>("Audio/UIAudio/Score");
    //    audio.clip = Ac;
    //    audio.loop = true;
    //    Invoke("StopScrollAudio", 2f);
    //}

    void StopScrollAudio()
    {
        audio.Stop();
    }

    public void CloseThisUI()
    {
        //Debug.Log("curGuideID= " + UserManager.CurUserInfo.GuideId);
        //if (UserManager.CurUserInfo.GuideId == 41)
        //{
        //    GameObject gp = GameObject.Find("Camera");
        //    GuiderLocal gl= gp.transform.Find("GuiderPanel").GetComponent<GuiderLocal>();
        //    gl.BackPveAction();
        //}
        GuiderLocal.TriggerGuideWithOut("overWar");

        DestroyVictorMusic();
        gameObject.SetActive(false);
        dungeonView.SetActive(true);
        partyView.SetActive(false);
        OverControl.overDone = false;

        PveGameControl.CurFriend = null;
        HelpFriend = null;
    }

    void DestroyVictorMusic()
    {
        GameObject gb = GameObject.Find("gm_victor_music");
        if (gb)
        {
            Destroy(gb); 
            AudioSource am = GameObject.Find("UI Root").GetComponent<AudioSource>();
            am.Play() ; 
        }
    }
    
    /// <summary>
    /// 箱子和蛋
    /// </summary>
    List<GameObject> Chests = new List<GameObject>();
    List<GameObject> Eggs = new List<GameObject>();
    List<MaterialItemInterface> materialIcons = new List<MaterialItemInterface>();
    List<equipmentItemInterface> hardwareIcons = new List<equipmentItemInterface>();
    List<ItemInterface> petIcons = new List<ItemInterface>();

    /// <summary>
    /// 添加素材
    /// </summary>
    /// <param name="m"></param>
    void AddMaterial(List<string> m)
    {
        foreach (string s in m)
        {
            GameObject Anime;
            ItemData Idata = ConfigManager.ItemConfig.GetItemById(s);
            if(Idata != null)
            {
                Anime = NGUITools.AddChild(materialGrid.gameObject, Chests_s[Idata.Rank - 1]);
            }
            else
            {
                HardWareData hd = ConfigManager.HardWareConfig.GetHardWareById(s);
                Anime = NGUITools.AddChild(materialGrid.gameObject, Chests_s[hd.Rank - 1]);
            }
            Anime.transform.localScale = new Vector3(500, 500, 1);
            PetUIController.SetLayer(Anime.transform, LayerHelper.Unit);
            Chests.Add(Anime);

            materialGrid.Reposition();
        }
    }

    /// <summary>
    /// 添加宠物
    /// </summary>
    /// <param name="p"></param>
    void AddPet(List<string> p)
    {
        foreach (string s in p)
        {
            PetData Pdata = ConfigManager.PetConfig.GetPetById(s);
            GameObject Anime = NGUITools.AddChild(petGrid.gameObject, Eggs_s[Pdata.Rank - 1]);
            Anime.transform.localScale = new Vector3(568, 568, 1);
            PetUIController.SetLayer(Anime.transform, LayerHelper.Unit);
            Eggs.Add(Anime);

            petGrid.Reposition();
        }
    }

    /// <summary>
    /// 弹开动画
    /// </summary>
    /// <param name="a"></param>
    void SetAction(Animator a)
    {
        a.SetBool("Action", true);
    }

    /// <summary>
    /// 依次弹开，如果是新的会显示独立动画
    /// </summary>
    IEnumerator AnimationOver(GameObject g, int index, string Id, int type)
    {
        yield return new WaitForSeconds(0.5f);
        curType = type;
        curIndex = index;

        switch (type)
        {
            case 0:
                {
                    if (materialNewTag[index - 1])
                    {
                        StartCoroutine(ReplaceAnimation(g, index, Id, type, false)); //不自动进行下去，独立动画显示完之后点击再开始
                        ShowNew(type, Id, index);
                    }
                    else
                    {
                        StartCoroutine(ReplaceAnimation(g, index, Id, type, true));
                        SetAction(g.GetComponent<Animator>());
                    }
                    break;
                }
            case 1:
                {
                    if (petNewTag[index - 1])
                    {
                        StartCoroutine(ReplaceAnimation(g, index, Id, type, false));
                        ShowNew(type, Id, index);
                    }
                    else
                    {
                        StartCoroutine(ReplaceAnimation(g, index, Id, type, true));
                        SetAction(g.GetComponent<Animator>());
                    }
                    break;
                }
            default: break;
        }
        SetAction(g.GetComponent<Animator>());
    }

    /// <summary>
    /// 弹开动画，0是素材，1是宠物
    /// </summary>
    /// <param name="g"></param>
    /// <param name="Id"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    IEnumerator ReplaceAnimation(GameObject g, int index, string Id, int type, bool autoNext)
    {
        yield return new WaitForSeconds(0.25f);
        switch(type)
        {
            case 0:
                {
                    UserWare uw = UserManager.CurUserInfo.FindUserWare(materialUids[index - 1]);
                    if (uw != null)
                    {
                        equipmentItemInterface e = NGUITools.AddChild(materialGrid.gameObject, hardwarePrefab).GetComponent<equipmentItemInterface>();
                        e.SetItem(uw.Level, uw.CurAtk, uw.CurHardWareData.Element, uw.CurHardWareData.SkinId, materialNewTag[index - 1], uw.CurHardWareData.Rank, uw.UserWareId);
                        e.transform.localPosition = g.transform.localPosition;
                        hardwareIcons.Add(e);
                    }
                    else
                    {
                        MaterialItemInterface m = NGUITools.AddChild(materialGrid.gameObject, materialPrefab).GetComponent<MaterialItemInterface>();
                        m.SetMaterialItem(Id);
                        m.transform.localPosition = g.transform.localPosition;
                        m.userMaterialId = materialUids[index - 1];
                        materialIcons.Add(m);

                        if (materialNewTag[index - 1])
                        {
                            m.IsNew(true);
                        }
                    }
                    break;
                }
            case 1:
                {
                    ItemInterface i = NGUITools.AddChild(petGrid.gameObject, petPrefab).GetComponent<ItemInterface>();
                    PetData p = ConfigManager.PetConfig.GetPetById(Id);
                    i.SetItem(1, -1, -1, -1, p.PetPro, Id, false, p.Rank, petUids[index - 1]);
                    i.transform.localPosition = g.transform.localPosition;
                    petIcons.Add(i);

                    if (petNewTag[index - 1])
                    {
                        i.IsNew(true);
                    }
                    break;
                }
            default: break;
        }
        Destroy(g);

        if (autoNext)
        {
            switch (type)
            {
                case 0:
                    {
                        if (index == Chests.Count) //如果是最后一个素材，导入宠物面板
                        {
                            PetMoveIn();
                        }
                        else
                        {
                            StartCoroutine(AnimationOver(Chests[index], index + 1, materialIds[index], 0));
                        }
                        break;
                    }
                case 1:
                    {
                        if (index == Eggs.Count)
                        {
                            StartCoroutine(DelayControl());
                        }
                        else
                        {
                            StartCoroutine(AnimationOver(Eggs[index], index + 1, petIds[index], 1));
                        }
                        break;
                    }
                default: break;
            }

        }
    }

    int curType = 0;
    int curIndex = 0;
    bool newStart = false;

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

    void ShowNew(int type, string Id, int index)
    {
        NewBoard.SetActive(true);
        NewBg.SetActive(true);
        UnitCamera.SetActive(false);

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
                        PetUIController.SetLayer(g.transform, LayerHelper.UnitFX);
                        StartCoroutine(ShowMaterialTexture(i.SkinId, 1f));
                        StartCoroutine(InfoBoardFlyIn(i.Rank, 1.5f, 0));
                    }
                    else
                    {
                        HardWareData hd = ConfigManager.HardWareConfig.GetHardWareById(Id);
                        GameObject g = Instantiate(Chests_b[hd.Rank - 1], Vector3.zero, Quaternion.identity) as GameObject;
                        g.name = "ChestAnimation";
                        PetUIController.SetLayer(g.transform, LayerHelper.UnitFX);
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
                    PetUIController.SetLayer(g.transform, LayerHelper.UnitFX);
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
                        while(count > 0)
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
                        while(count > 0)
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
                    while(count > 0)
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
        for(int i = 0; i < rank; i++)
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

    /// <summary>
    /// 接口
    /// </summary>
    public void _OnClickItem(int Uid)
    {
        PetDetail.SetActive(true);
        PetDetail.GetComponent<SetMonsterDetail>().SetDetail(Uid);
    }

    public void _OnLongPressItem(int Uid)
    { }

    public void _OnClickMaterial(int Uid)
    {
        MaterialDetail.SetActive(true);
        MaterialDetail.GetComponent<MaterialDetail>().SetDetail(Uid);
    }

    public void _OnLongPressMaterial(int Uid)
    { }

    public void _OnClickEquipmentItem(int Uid)
    {
        if((int)UserManager.CurUserInfo.FindUserWare(Uid).CurHardWareData.Style < 5)
        {
            WeaponDetail.SetActive(true);
            WeaponDetail.GetComponent<WeaponDetail>().SetDetail(Uid);
        }
        else
        {
            ArmorDetail.SetActive(true);
            ArmorDetail.GetComponent<ArmorDetail>().SetDetail(Uid);
        }
    }

    public void _OnLongPressEquipmentItem(int Uid)
    { }

    /// <summary>
    /// 接收数据
    /// </summary>
    /// <param name="Elements"></param>
    public static void AddElement(JsonArray Elements)
    {
		materialIds.Clear();
        materialUids.Clear();
        materialNewTag.Clear();
		petIds.Clear();
        petUids.Clear();
        petNewTag.Clear();

        //lvTotalExp = ConfigManager.HeroConfig.GetHeroByLvl(UserManager.CurUserInfo.Level + 1).Exp - UserManager.CurUserInfo.CurHero.Exp;
        //lvCurExp = UserManager.CurUserInfo.Exp - UserManager.CurUserInfo.CurHero.Exp;
        foreach (JsonObject data in Elements)
        {
            int type = int.Parse(data["type"].ToString());
            switch (type)
            {
                case 0: break;
                case 1:
                    {
                        string id = data["id"].ToString();
                        int count = int.Parse(data["count"].ToString());
                        if (id == "currency2")
                        {
                            gold += count;
                        }
                        else if (id == "exp")
                        {
                            //exp += count;
                            //while(UserManager.CurUserInfo.Exp + exp >= ConfigManager.HeroConfig.GetHeroByLvl(UserManager.CurUserInfo.Level + 1 + lvAddition).Exp)
                            //{
                            //    lvAddition++;
                            //}
                        }
                        break;
                    }
                case 2:
                    {
                        int count = 0;
                        UserItem newUI = new UserItem(data);
                        if(!ConfigManager.ItemConfig.IsNew(newUI.CurItemData.Id))
                        {
                            count++;
                        }
                        if (materialIds.Count != 0)
                        {
                            for (int i = 0; i < materialIds.Count; i++)
                            {
                                if (newUI.CurItemData.Id == materialIds[i])
                                {
                                    count++;
                                }
                            }
                        }
                        if (count == 0) materialNewTag.Add(true);
                        else materialNewTag.Add(false);
                        materialIds.Add(newUI.CurItemData.Id);
                        materialUids.Add(newUI.UserItemId);                
                        break;
                    }
                case 3: break;
                case 4:
                    {
                        if (data.ContainsKey("count"))
                        {
                            //前
                            //materialIds.Add(data["id"].ToString());
                        }
                        else
                        {
                            //后
                            int count = 0;
                            UserWare newW = new UserWare(data);
                            if(!ConfigManager.HardWareConfig.IsNew(newW.CurHardWareData.Id))
                            {
                                count++;
                            }
                            if(materialIds.Count != 0)
                            {
                                for (int i = 0; i < materialIds.Count; i++)
                                {
                                    if(newW.CurHardWareData.Id == materialIds[i])
                                    {
                                        count++;
                                    }
                                }
                            }
                            if (count == 0) materialNewTag.Add(true);
                            else materialNewTag.Add(false);
                            materialIds.Add(newW.CurHardWareData.Id);
                            materialUids.Add(newW.UserWareId);
                        }
                        break;
                    }
                case 5:
                    {
                        if (data.ContainsKey("count"))
                        {
                            //前
					         petIds.Add(data["id"].ToString());
                        }
                        else
                        {
                             //后
                            int count = 0;
                            UserPet newP = new UserPet(data);
                            if(!ConfigManager.PetConfig.IsNew(newP.CurPetData.Id))
                            {
                                count++;
                            }
                            if(petIds.Count != 0)
                            {
                                for(int i = 0; i < petIds.Count; i++)
                                {
                                    if(newP.CurPetData.Id == petIds[i])
                                    {
                                        count++;
                                    }
                                }
                            }
                            if (count == 0) petNewTag.Add(true);
                            else petNewTag.Add(false);
                            petIds.Add(newP.CurPetData.Id);
                            petUids.Add(newP.UserPetId);
                        }
                        break;
                    }
                case 6: break;
                case 7: break;
                case 8: break;
                case 9: break;
                case 10: break;
                case 11: break;
                case 12: break;
                case 13: break;
                case 14: break;
                case 15: break;
                case 16: break;
                case 17: break;
                default: Debug.Log("No fit type!"); break;
            }
        }
    }

    #region 流程
    /// <summary>
    /// 金币跳动
    /// </summary>
    void GoldIncrease()
    {
        //PlayScrollAudio();
        AnimationHelper.LabelUpdate(goldGot, 0, gold, animeTime, "GoldUpdate", gameObject, "AchieveMentMoveIn", gameObject);
    }

    void GoldUpdate(int value)
    {
        goldGot.text = value.ToString();
    }

    void AchieveMentMoveIn()
    {
        DungeonData d = ConfigManager.DungeonConfig.GetDungeonById(PveGameControl.CurDungeonId);
        AchieveLabel.text = d.QuestTips;
        if (UserManager.CurUserInfo.HasAchievedDungeon(d.Id))
        {
            achievementDiamond.spriteName = "level_diamond";
        }
        else
        {
            achievementDiamond.spriteName = "level_diamond_back";
        }

        AnimationHelper.AnimationMoveTo(new Vector3(0f, AchieveBoard.transform.localPosition.y, AchieveBoard.transform.localPosition.z), AchieveBoard, iTween.EaseType.linear, gameObject, "MaterialMoveIn", 0.2f);
    }

    /// <summary>
    /// 经验条
    /// </summary>
    void ExpMoveIn()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(expGotX, expGot.transform.localPosition.y, expGot.transform.localPosition.z), expGot.gameObject, iTween.EaseType.linear, gameObject, "ExpIncrease", 0.2f);
    }

    void ExpIncrease()
    {
        //PlayScrollAudio();
        AnimationHelper.LabelUpdate(expGot, 0, exp, animeTime, "ExpUpdate", gameObject, null, null);
        Debug.Log(lvAddition);
        ExpBarUpdate(UserManager.CurUserInfo.Level - lvAddition, UserManager.CurUserInfo.Level);
        ExpLeftUpdate(UserManager.CurUserInfo.Level - lvAddition, UserManager.CurUserInfo.Level);
    }

    void ExpUpdate(int value)
    {
        expGot.text = value.ToString();
    }

    /// <summary>
    /// 剩余经验值
    /// </summary>
    void ExpLeftUpdate(int fromLv, int toLv)
    {
        if(fromLv < toLv)
        {
            AnimationHelper.AnimationValueTo(expLeft.gameObject, (lvTotalExp - lvCurExp), 0, animeTime, "LeftUpdate", gameObject, "LeftUpdateOver", gameObject, toLv - fromLv - 1);
        }
        else
        {
            AnimationHelper.AnimationValueTo(expLeft.gameObject, (lvTotalExp - lvCurExp), (UserManager.CurUserInfo.CurLevelExp - UserManager.CurUserInfo.CurExp), animeTime, "LeftUpdate", gameObject, null, null, null);
        }
    }

    void LeftUpdate(int value)
    {
        expLeft.text = value.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag">升级次数</param>
    void LeftUpdateOver(int tag)
    {
        if(tag > 0)
        {
            AnimationHelper.AnimationValueTo(expLeft.gameObject, GetLvUpExp(UserManager.CurUserInfo.Level - tag), 0, animeTime, "LeftUpdate", gameObject, "LeftUpdateOver", gameObject, tag - 1);
        }
        else
        {
            AnimationHelper.AnimationValueTo(expLeft.gameObject, UserManager.CurUserInfo.CurLevelExp, (UserManager.CurUserInfo.CurLevelExp - UserManager.CurUserInfo.CurExp), animeTime, "LeftUpdate", gameObject, null, null, null);
        }
    }

    /// <summary>
    /// 经验条
    /// </summary>
    void ExpBarUpdate(int fromLv, int toLv)
    {
        if (fromLv < toLv)
        {
            AnimationHelper.AnimationValueTo(expBar.gameObject, (float)lvCurExp / (float)lvTotalExp, 1f, 0.5f, "BarUpdate", gameObject, "UpdateOver", gameObject, toLv - fromLv - 1);
        }
        else
        {
            AnimationHelper.AnimationValueTo(expBar.gameObject, (float)lvCurExp / (float)lvTotalExp, (float)UserManager.CurUserInfo.CurExp / (float)UserManager.CurUserInfo.CurLevelExp, 0.5f, "BarUpdate", gameObject, "MaterialMoveIn", gameObject, toLv - fromLv - 1);
        }
    }

    void BarUpdate(float value)
    {
        expBar.value = value;
    }

    void UpdateOver(int tag)
    {
        if (tag > 0)
        {
            AnimationHelper.AnimationValueTo(expBar.gameObject, 0f, 1f, 0.5f, "BarUpdate", gameObject, "UpdateOver", gameObject, tag - 1);
        }
        else
        {
            AnimationHelper.AnimationValueTo(expBar.gameObject, 0f, (float)UserManager.CurUserInfo.CurExp / (float)UserManager.CurUserInfo.CurLevelExp, 0.5f, "BarUpdate", gameObject, "SetLevelUp", gameObject, null);
        }
    }

    #region 升级动画
    public GameObject LevelUpBoard;
    public GameObject LvUpInfoBoard;
    public Animator LevelUpAnimation;
    public UILabel Hp_Pre;
    public UILabel Hp_Later;
    public UILabel Atk_Pre;
    public UILabel Atk_Later;
    public UILabel Def_Pre;
    public UILabel Def_Later;
    public UILabel Cost_Pre;
    public UILabel Cost_Later;
    public Vector3 DefaultPosition;
    public Vector3 TarPosition;

    /// <summary>
    /// 恢复升级面板到默认状态
    /// </summary>
    void ResetLvUpBoard()
    {
        LvUpInfoBoard.transform.localPosition = DefaultPosition;
        LvUpInfoBoard.SetActive(false);
        LevelUpBoard.SetActive(false);
    }

    void SetLevelUp()
    {
        LevelUpBoard.SetActive(true);
        int fromLv = UserManager.CurUserInfo.Level - lvAddition;
        int toLv = UserManager.CurUserInfo.Level;
        int PreHp = ConfigManager.HeroConfig.GetHeroByLvl(fromLv).Hp;
        int LaterHp = ConfigManager.HeroConfig.GetHeroByLvl(toLv).Hp;
        int PreAtk = ConfigManager.HeroConfig.GetHeroByLvl(fromLv).Attack;
        int LaterAtk = ConfigManager.HeroConfig.GetHeroByLvl(toLv).Attack;
        int PreDef = ConfigManager.HeroConfig.GetHeroByLvl(fromLv).Def;
        int LaterDef = ConfigManager.HeroConfig.GetHeroByLvl(toLv).Def;
        int PreCost = ConfigManager.HeroConfig.GetHeroByLvl(fromLv).Hcost;
        int LaterCost = ConfigManager.HeroConfig.GetHeroByLvl(toLv).Hcost;
        StartCoroutine(ShowLvUpBoard(PreHp, LaterHp, PreAtk, LaterAtk, PreDef, LaterDef, PreCost, LaterCost));
    }

    IEnumerator ShowLvUpBoard(int PreHp, int LaterHp, int PreAtk, int LaterAtk, int PreDef, int LaterDef, int PreCost, int LaterCost)
    {
        yield return new WaitForSeconds(0f);
        LvUpInfoBoard.SetActive(true);
        Hp_Pre.text = PreHp.ToString();
        Atk_Pre.text = PreAtk.ToString();
        Def_Pre.text = PreDef.ToString();
        Cost_Pre.text = PreCost.ToString();
        Hp_Later.text = LaterHp.ToString();
        Atk_Later.text = LaterAtk.ToString();
        Def_Later.text = LaterDef.ToString();
        Cost_Later.text = LaterCost.ToString();
        AnimationHelper.AnimationValueTo(LvUpInfoBoard, 0, 1, 0.3f, "LvUpBoardFade", gameObject, null, null, null);
        AnimationHelper.AnimationMoveTo(TarPosition, LvUpInfoBoard, iTween.EaseType.linear, null, null, 0.3f);
        StartCoroutine(LevelUp(PreHp, LaterHp, PreAtk, LaterAtk, PreDef, LaterDef, PreCost, LaterCost));
    }

    void LvUpBoardFade(float value)
    {
        LvUpInfoBoard.GetComponent<UIWidget>().alpha = value;
    }

    IEnumerator LevelUp(int PreHp, int LaterHp, int PreAtk, int LaterAtk, int PreDef, int LaterDef, int PreCost, int LaterCost)
    {
        yield return new WaitForSeconds(0.5f);
        AnimationHelper.LabelUpdate(Hp_Later, PreHp, LaterHp, 2f, "LaterHpUpdate", gameObject, null, null);
        AnimationHelper.LabelUpdate(Atk_Later, PreAtk, LaterAtk, 2f, "LaterAtkUpdate", gameObject, null, null);
        AnimationHelper.LabelUpdate(Def_Later, PreDef, LaterDef, 2f, "LaterDefUpdate", gameObject, null, null);
        AnimationHelper.LabelUpdate(Cost_Later, PreCost, LaterCost, 2f, "LaterCostUpdate", gameObject, null, null);
    }

    void LaterHpUpdate(int value)
    {
        Hp_Later.text = value.ToString();
    }

    void LaterAtkUpdate(int value)
    {
        Atk_Later.text = value.ToString();
    }

    void LaterDefUpdate(int value)
    {
        Def_Later.text = value.ToString();
    }

    void LaterCostUpdate(int value)
    {
        Cost_Later.text = value.ToString();
    }

    #endregion

    /// <summary>
    /// 获得当前等级升级经验
    /// </summary>
    int GetLvUpExp(int Level)
    {
        HeroData hd1 = ConfigManager.HeroConfig.GetHeroByLvl(Level);
        HeroData hd2 = ConfigManager.HeroConfig.GetHeroByLvl(Level + 1);
        return hd2.Exp - hd1.Exp;
    }


    /// <summary>
    /// 素材进入场景
    /// </summary>
    void MaterialMoveIn()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(materialGotX, title_materialGot.transform.localPosition.y, title_materialGot.transform.localPosition.z), title_materialGot.gameObject, iTween.EaseType.linear, gameObject, "MaterialGridMoveIn", 0.2f);
    }

    void MaterialGridMoveIn()
    {
        if (Chests.Count != 0)
        {
            AnimationHelper.AnimationMoveTo(new Vector3(materialGridX, materialGrid.transform.localPosition.y, materialGrid.transform.localPosition.z), materialGrid.gameObject, iTween.EaseType.linear, gameObject, "ShowItemIcons", 0.2f);
        }
        else
        {
            AnimationHelper.AnimationMoveTo(new Vector3(materialGridX, materialGrid.transform.localPosition.y, materialGrid.transform.localPosition.z), materialGrid.gameObject, iTween.EaseType.linear, gameObject, "PetMoveIn", 0.2f);
        }
    }

    /// <summary>
    /// 箱子动画开始
    /// </summary>
    void ShowItemIcons()
    {
        if (Chests.Count > 0)
        {
            StartCoroutine(AnimationOver(Chests[0], 1, materialIds[0], 0));
        }
        else
        {
            PetMoveIn();
        }
    }

    /// <summary>
    /// 宠物进入场景
    /// </summary>
    void PetMoveIn()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(petGotX, title_petGot.transform.localPosition.y, title_petGot.transform.localPosition.z), title_petGot.gameObject, iTween.EaseType.linear, gameObject, "PetGridMoveIn", 0.2f);
    }

    void PetGridMoveIn()
    {
        if (Eggs.Count != 0)
        {
            AnimationHelper.AnimationMoveTo(new Vector3(petGridX, petGrid.transform.localPosition.y, petGrid.transform.localPosition.z), petGrid.gameObject, iTween.EaseType.linear, gameObject, "ShowPetIcons", 0.2f);
        }
        else
        {
            AnimationHelper.AnimationMoveTo(new Vector3(petGridX, petGrid.transform.localPosition.y, petGrid.transform.localPosition.z), petGrid.gameObject, iTween.EaseType.linear, gameObject, "DelayControl", 0.2f);
        }
    }

    /// <summary>
    /// 蛋动画开始
    /// </summary>
    void ShowPetIcons()
    {
        if (Eggs.Count > 0)
        {
            StartCoroutine(AnimationOver(Eggs[0], 1, petIds[0], 1));
        }
        else
        {
            Debug.Log("no egg over");
            StartCoroutine(DelayControl());
        }
    }
    IEnumerator DelayControl()
    {
        foreach (MaterialItemInterface m in materialIcons) //最后一个蛋弹开之后指针赋值，可以响应点击事件
        {
            m.materialItemInter = this;
        }
        foreach (ItemInterface i in petIcons)
        {
            i.itemInter = this;
        }
        foreach(equipmentItemInterface e in hardwareIcons)
        {
            e.equipmentItemInter = this;
        }
        yield return new WaitForSeconds(0.5f);
        overDone = true;
    }
    #endregion

    #region 好友
    /// <summary>
    /// 添加好友信息
    /// </summary>
    public GameObject RequestBoard;
    public UILabel AddInfo;
    public GameObject AddButton;
    public GameObject CancelButton;
    public GameObject ReturnButton;
    public GameObject BackBoard;

    /// <summary>
    /// 助战好友ID
    /// </summary>
    public static FriendInfo HelpFriend;
    #endregion


    #region 任务奖励
    public GameObject taskBoard;
    public UILabel TaskName;
    public UILabel Reward;
    public GameObject ConfirmButton;
    public int taskIndex = 0;

    static List<MissionData> completedMissions = new List<MissionData>();

    public static void TaskProgressUpdate(JsonObject data)
    {
        completedMissions.Clear();
        if(data != null)
        {
            JsonArray tasks = (JsonArray)data["daily_tasks"];
            foreach (JsonObject task in tasks)
            {
                bool complete = (int.Parse(task["status"].ToString()) > 0);
                if (complete)
                {
                    completedMissions.Add(UserManager.CurUserInfo.FindUserTask(int.Parse(task["daily_id"].ToString())).CurMission);
                }
            }
        }
    }

	public static bool TaskProgressCheck(JsonObject data)
	{
		if(data == null) return false;

		JsonArray tasks = (JsonArray)data["daily_tasks"];
		foreach (JsonObject task in tasks)
		{
			if(int.Parse(task["status"].ToString()) > 0) return true;
		}
		return false;
	}

    void showCompletedTasks(int index)
    {
        taskIndex = index;
        if(index >= completedMissions.Count)
        {
            //windowAnimaiton(taskBoard);
            return;
        }
        if (taskBoard.activeSelf == false) taskBoard.SetActive(true);
        MissionData m = completedMissions[index];
        TaskName.text = m.MissionName;
        if(ConfigManager.PetConfig.GetPetById(m.RewardId) != null)
        {
            Reward.text = ConfigManager.PetConfig.GetPetById(m.RewardId).Name + "*" + m.RewardRate;
        }
        else if(ConfigManager.ItemConfig.GetItemById(m.RewardId) != null)
        {
            Reward.text = ConfigManager.ItemConfig.GetItemById(m.RewardId).Description + "*" + m.RewardRate;
        }
        else if(ConfigManager.HardWareConfig.GetHardWareById(m.RewardId) != null)
        {
            Reward.text = ConfigManager.HardWareConfig.GetHardWareById(m.RewardId).Name + "*" + m.RewardRate;
        }
        else if (m.RewardId == "Currency1")
        {
            Reward.text = "钻石*" + m.RewardRate;
        }
        windowAnimaiton(taskBoard);
    }

    void windowAnimaiton(GameObject g)
    {
        g.transform.localScale = Vector3.zero;
        AnimationHelper.AnimationScaleTo(Vector3.one, g, iTween.EaseType.easeOutElastic, null, null, 0.5f);
    }

    #endregion

    #region Mono函数
    /// <summary>
    /// 入场动画
    /// </summary>
    void Start()
    {

        GameObject gb = GameObject.Find("gm_victor_music");
        if (gb)
        {
            AudioSource am = GameObject.Find("UI Root").GetComponent<AudioSource>();
            am.Stop();
        }

        //expBar.value = (float)lvCurExp / (float)lvTotalExp;
        goldGot.text = "0";

        AddMaterial(materialIds);
        AddPet(petIds);

        AnimationHelper.AnimationMoveTo(new Vector3(backBarX, backBar.transform.localPosition.y, backBar.transform.localPosition.z), backBar, iTween.EaseType.linear, null, null, 0.2f);
        AnimationHelper.AnimationMoveTo(new Vector3(goldGotX, goldGot.transform.localPosition.y, goldGot.transform.localPosition.z), goldGot.gameObject, iTween.EaseType.linear, gameObject, "GoldIncrease", 0.2f);

        UIEventListener.Get(BackBoard).onClick = (go) =>
        {
            Debug.Log(overDone);
            if (overDone)
            {
                if(HelpFriend != null)
                {
                    RequestBoard.SetActive(true);
                    AddInfo.text = "要向" + HelpFriend.NickName + "发送好友请求吗？";
                    AddButton.SetActive(true);
                    CancelButton.SetActive(true);
                    ReturnButton.SetActive(false);
                }
                else
                {
                    if (completedMissions.Count != 0)
                    {
                        showCompletedTasks(0);
                    }
                    else CloseThisUI();
                }
            }
        };

        //好友
        UIEventListener.Get(AddButton).onClick = (go) =>
        {
            FriendControl.SendFriendRequest(HelpFriend.FriendId, (r) =>
            {
                //if (r == FriendControl.FriendMessageResult.Success)
                //{
                //    Loom.QueueOnMainThread(() =>
                //    {
                //        AddInfo.text = "请求发送成功";
                //        AddButton.SetActive(false);
                //        CancelButton.SetActive(false);
                //        ReturnButton.SetActive(true);
                //    });
                //}
                //else
                //{
                //    Loom.QueueOnMainThread(() =>
                //    {
                //        AddInfo.text = "请求发送失败，要再次向" + HelpFriend.NickName + "发送好友请求吗？";
                //        AddButton.SetActive(true);
                //        CancelButton.SetActive(true);
                //        ReturnButton.SetActive(false);
                //    });
                //}
                if (OverControl.overDone)
                {
                    RequestBoard.SetActive(false);
                    if (completedMissions.Count != 0)
                    {
                        Debug.Log(completedMissions.Count);
                        showCompletedTasks(0);
                    }
                    else CloseThisUI();
                }
            });
        };

        UIEventListener.Get(CancelButton).onClick = (go) =>
        {
            if (OverControl.overDone)
            {
                RequestBoard.SetActive(false);
                if (completedMissions.Count != 0)
                {
                    Debug.Log(completedMissions.Count);
                    showCompletedTasks(0);
                }
                else CloseThisUI();
            }
        };

        UIEventListener.Get(ReturnButton).onClick = (go) =>
        {
            if (OverControl.overDone)
            {
                RequestBoard.SetActive(false);
                if (completedMissions.Count != 0)
                {
                    showCompletedTasks(0);
                }
                else CloseThisUI();
            }
        };

        //任务
        UIEventListener.Get(ConfirmButton).onClick = (go) =>
        {
            if(taskIndex + 1 < completedMissions.Count)
            {
                showCompletedTasks(taskIndex + 1);
            }
            else
            {
                //showCompletedTasks(taskIndex + 1);
                CloseThisUI();
            }
        };

        UIEventListener.Get(NewBg).onClick = (go) =>
        {
            if (newStart)
            {
                newStart = false;
                switch (curType)
                {
                    case 0:
                        {
                            mTexture.mainTexture = null;
                            Destroy(GameObject.Find("ChestAnimation"));
                            if (curIndex == Chests.Count) //如果是最后一个素材，导入宠物面板
                            {
                                PetMoveIn();
                            }
                            else
                            {
                                StartCoroutine(AnimationOver(Chests[curIndex], curIndex + 1, materialIds[curIndex], 0));
                            }
                            break;
                        }
                    case 1:
                        {
                            Destroy(NewBoard.transform.FindChild("PetAnimation").gameObject);
                            Destroy(GameObject.Find("EggAnimation"));
                            if (curIndex == Eggs.Count)
                            {
                                StartCoroutine(DelayControl());
                            }
                            else
                            {
                                StartCoroutine(AnimationOver(Eggs[curIndex], curIndex + 1, petIds[curIndex], 1));
                            }
                            break;
                        }
                    default: break;
                }

                foreach (Transform t in StarGrid.transform)
                {
                    Destroy(t.gameObject);
                }
                foreach(GameObject g in TempStars)
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

        UIEventListener.Get(LevelUpBoard).onClick = (g) =>
        {
            MaterialMoveIn();
            ResetLvUpBoard();
        };
    }


    void OnEnable()
    {
        overDone = false;
        newStart = false;
        Chests.Clear();
        Eggs.Clear();
        materialIcons.Clear();
        hardwareIcons.Clear();
        petIcons.Clear();
    }

    void OnDisable()
    {

        isOver = false;
        overDone = false;

        exp = 0;
        gold = 0;
        lvTotalExp = 0;
        lvCurExp = 0;
        lvAddition = 0;

        RequestBoard.SetActive(false);
    }
    #endregion
}
