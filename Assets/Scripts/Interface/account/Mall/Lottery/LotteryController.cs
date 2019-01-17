using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using PomeloSocketCenter.PomeloLib;

public class LotteryController : MonoBehaviour, itemInterface
{
    #region Animation
    public enum LotteryType
    {
        diamond = 0,
        friend = 1,
        diamond_10 = 2,
        friend_10 = 3
    }

    public LotteryType type;

    public List<GameObject> Eggs = new List<GameObject>();

    public List<Texture> LotteryStones = new List<Texture>();

    public List<GameObject> StoneColliders;

    public GameObject HandAnimation;

    public GameObject LotteryAnimation;

    int ChainNumber = 0;

    public GameObject mainBoard;
    public GameObject mainButton;

    public UIGrid ResultBoard;
    public GameObject PetPrefab;

    public GameObject BackGroundButton;
    public GameObject LotteryList;

    public GameObject DetailView;

    public void SetType0()
    {
        type = LotteryType.diamond;
    }
    public void SetType1()
    {
        type = LotteryType.diamond_10;
    }
    public void SetType2()
    {
        type = LotteryType.friend;
    }
    public void SetType3()
    {
        type = LotteryType.friend_10;
    }

    bool GetLink(GameObject g, int index)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (hit.collider.gameObject.name == ("StoneCollider_" + index.ToString()))
            {
                return true;
            }
        }
        return false;
    }

    void OnEnable()
    {
        mainBoard.SetActive(false);
        mainButton.SetActive(false);
        ChainNumber = 0;

        UIEventListener.Get(StoneColliders[0]).onPress = (g, press) =>
        {
            if (ChainNumber == 0)
            {
                ChainNumber++;
                HandAnimation.SetActive(false);
                AddStoneSprite(StoneColliders[0], type);
            }
        };

        UIEventListener.Get(BackGroundButton).onClick = (g) =>
        {
            if (AnimHasOver)
            {
                gameObject.SetActive(false);
                LotteryList.SetActive(true);
            }
        };

        UIEventListener.Get(NewBg).onClick = (go) =>
        {
            if (newStart)
            {
                newStart = false;
                Destroy(NewBoard.transform.FindChild("PetAnimation").gameObject);
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


                if (curIndex == EggPrefabs.Count)
                {
                    StartCoroutine(DelayControl());
                }
                else
                {
                    StartCoroutine(AnimationOver(EggPrefabs[curIndex], curIndex + 1, petIds[curIndex]));
                }
                UnitCamera.SetActive(true);
                NewBoard.SetActive(false);
                NewBg.SetActive(false);
            }
        };

    }

    void OnDisable()
    {
        mainBoard.SetActive(true);
        mainButton.SetActive(true);
        ChainNumber = 0;
        HandAnimation.SetActive(true);
        ClearAllTexture();
        ClearAllSprite();

        foreach (Transform t in ResultBoard.transform)
        {
            Destroy(t.gameObject);
        }
        ResultBoard.transform.parent.gameObject.SetActive(false);
        ResultBoard.Reposition();

        ClearAnimation();
    }

    /// <summary>
    /// 播放声音
    /// </summary>
    void SetStoneVoice()
    {
        AudioClip Ac = Resources.Load<AudioClip>("Audio/UIAudio/Setstone");
        audio.clip = Ac;
        audio.Play();
    }

    void SetGachaVoice()
    {
        AudioClip Ac = Resources.Load<AudioClip>("Audio/UIAudio/Gacha");
        audio.clip = Ac;
        audio.Play();
    }

    /// <summary>
    /// 石头颜色
    /// </summary>
    /// <param name="g"></param>
    /// <param name="Ltype"></param>
    void AddStoneSprite(GameObject g, LotteryType Ltype)
    {
        int type = (int)Ltype % 2;
        g.GetComponent<UITexture>().mainTexture = LotteryStones[type];
        g.transform.localScale = g.transform.localScale * 1.5f;
        AnimationHelper.AnimationScaleTo(new Vector3(1, 1, 1), g, iTween.EaseType.linear, null, null, 0.1f);
    }

    /// <summary>
    /// 蛋颜色
    /// </summary>
    /// <param name="egg"></param>
    /// <param name="rank"></param>
    void SetEggSprite(GameObject egg, int rank)
    {
        Sprite[] eggsprites = Resources.LoadAll<Sprite>("Sprites/_Props/egg_" + (rank - 1).ToString());
        foreach (Sprite s in eggsprites)
        {
            if (s.name == "egg_2")
            {
                egg.GetComponent<SpriteRenderer>().sprite = s;
            }
        }
    }

    /// <summary>
    /// 清除石头
    /// </summary>
    void ClearAllTexture()
    {
        foreach (GameObject g in StoneColliders)
        {
            g.GetComponent<UITexture>().mainTexture = null;
        }
    }

    /// <summary>
    /// 清除蛋
    /// </summary>
    void ClearAllSprite()
    {
        foreach (GameObject g in Eggs)
        {
            g.GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    void DelayOver()
    {
        ResultBoard.transform.parent.gameObject.SetActive(true);
        ClearAllSprite();

        ShowPetIcons();
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (ChainNumber == 5)
            {
                JsonObject args = new JsonObject();
                args.Add("type", (int)type + 1);
                SocketCenter.Request(GameRouteConfig.Lottery, args, (r) =>
                {
                    if (r.Code == SocketResult.ResultCode.Success)
                    {
                        Loom.QueueOnMainThread(() =>
                        {
                            SetGachaVoice();
                            ChainNumber = -1;
                            JsonArray elements = (JsonArray)r.Data["elements"];
                            mainBoard.GetComponent<SetUser>().SetInfo();
                            List<UserPet> newPets = new List<UserPet>();
                            foreach (JsonObject data in elements)
                            {
                                if (int.Parse(data["type"].ToString()) == 5)
                                {
                                    UserPet u = new UserPet(data);
                                    newPets.Add(u);
                                }
                            }
                            for (int i = 0; i < newPets.Count; i++)
                            {
                                SetEggSprite(Eggs[i], newPets[i].CurPetData.Rank);
                            }

                            ShowEggs(newPets);//放置动画预设

                            ResultBoard.repositionNow = true;
                            LotteryAnimation.GetComponent<Animator>().SetInteger("SwitchState", 1);

                            Invoke("DelayOver", 2f); //动画开始

                            UserManager.CurUserInfo.AddElements(elements);
                        });
                    }
                    else
                    {
                        Loom.QueueOnMainThread(() =>
                        {
                            gameObject.SetActive(false);
                            LotteryList.SetActive(true);
                            Debug.Log("抽奖失败!");
                        });
                    }
                }, null, true, true);

            }
            else if(ChainNumber >= 0)
            {
                ChainNumber = 0;
                HandAnimation.SetActive(true);
                ClearAllTexture();
            }
        }
        for(int i = 1; i < StoneColliders.Count; i++)
        {
            if (GetLink(StoneColliders[i], i))
            {
                if (ChainNumber == i)
                {
                    AddStoneSprite(StoneColliders[i], type);
                    SetStoneVoice();
                    ChainNumber++;
                }
            }
        }
    }
    #endregion

    public void _OnClickItem(int Uid)
    {

    }

    public void _OnLongPressItem(int Uid)
    {
        DetailView.SetActive(true);
        DetailView.GetComponent<SetMonsterDetail>().SetDetail(Uid);
    }


    #region 动画
    bool AnimHasOver = false;

    public List<GameObject> Eggs_s = new List<GameObject>();
    public List<GameObject> Eggs_b = new List<GameObject>();

    List<GameObject> EggPrefabs = new List<GameObject>();
    List<ItemInterface> petIcons = new List<ItemInterface>();

    /// <summary>
    /// 获得宠物ID数组
    /// </summary>
    List<string> petIds = new List<string>();
    List<int> petUids = new List<int>();
    List<bool> petNewTag = new List<bool>();

    int curIndex = 0;
    bool newStart = false;

    void ClearAnimation()
    {
        EggPrefabs.Clear();
        petIcons.Clear();
        petIds.Clear();
        petUids.Clear();
        petNewTag.Clear();
    }

    void ShowEggs(List<UserPet> newPets)
    {
        SetIds(newPets);
        AddPet(petIds);
    }

    void SetIds(List<UserPet> Pets)
    {
        foreach(UserPet u in Pets)
        {
            int count = 0;
            if (!ConfigManager.PetConfig.IsNew(u.CurPetData.Id))
            {
                count++;
            }
            if (petIds.Count != 0)
            {
                for (int i = 0; i < petIds.Count; i++)
                {
                    if (u.CurPetData.Id == petIds[i])
                    {
                        count++;
                    }
                }
            }
            if (count == 0) petNewTag.Add(true);
            else petNewTag.Add(false);
            petIds.Add(u.CurPetData.Id);
            petUids.Add(u.UserPetId);
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
            GameObject Anime = NGUITools.AddChild(ResultBoard.gameObject, Eggs_s[Pdata.Rank - 1]);
            Anime.transform.localScale = new Vector3(568, 568, 1);
            PetUIController.SetLayer(Anime.transform, LayerHelper.Unit);
            EggPrefabs.Add(Anime);

            ResultBoard.Reposition();
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
    IEnumerator AnimationOver(GameObject g, int index, string Id)
    {
        yield return new WaitForSeconds(0.5f);
        SetAction(g.GetComponent<Animator>());
        curIndex = index;
        if (petNewTag[index - 1])
        {
            StartCoroutine(ReplaceAnimation(g, index, Id, false));
            ShowNew(Id, index);
        }
        else
        {
            StartCoroutine(ReplaceAnimation(g, index, Id, true));
        }
    }

    /// <summary>
    /// 弹开动画，0是素材，1是宠物
    /// </summary>
    /// <param name="g"></param>
    /// <param name="Id"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    IEnumerator ReplaceAnimation(GameObject g, int index, string Id, bool autoNext)
    {
        yield return new WaitForSeconds(0.25f);

        ItemInterface i = NGUITools.AddChild(ResultBoard.gameObject, PetPrefab).GetComponent<ItemInterface>();
        PetData p = ConfigManager.PetConfig.GetPetById(Id);
        i.SetItem(1, -1, -1, -1, p.PetPro, Id, false, p.Rank, petUids[index - 1]);
        i.transform.localPosition = g.transform.localPosition;
        petIcons.Add(i);

        if (petNewTag[index - 1])
        {
            i.IsNew(true);
        }
        Destroy(g);

        if (autoNext)
        {
            if (index == EggPrefabs.Count)
            {
                StartCoroutine(DelayControl());
            }
            else
            {
                StartCoroutine(AnimationOver(EggPrefabs[index], index + 1, petIds[index]));
            }
        }
    }

    void ShowPetIcons()
    {
        if (EggPrefabs.Count > 0)
        {
            StartCoroutine(AnimationOver(EggPrefabs[0], 1, petIds[0]));
        }
        else
        {
            StartCoroutine(DelayControl());
        }
    }
    IEnumerator DelayControl()
    {
        foreach (ItemInterface i in petIcons)
        {
            i.itemInter = this;
        }
        yield return new WaitForSeconds(0.5f);
        AnimHasOver = true;
    }


    /// <summary>
    /// 新的宠物独立显示
    /// </summary>
    public GameObject NewBoard;
    public GameObject NewBg;
    public GameObject UnitCamera;

    public GameObject InfoBoard;
    public UILabel NameLabel;
    public GameObject StarPrefab;
    public GameObject StarOutline;
    public UIGrid StarGrid;

    public Vector3 Name_StartPosition = new Vector3(0f, -730f, 0f);
    public Vector3 Name_TargetPosition = new Vector3(0f, -330f, 0f);


    void ShowNew(string Id, int index)
    {
        NewBoard.SetActive(true);
        NewBg.SetActive(true);
        UnitCamera.SetActive(false);

        SetInfoBoard(Id, 1);

        PetData p = ConfigManager.PetConfig.GetPetById(Id);
        GameObject g = Instantiate(Eggs_b[p.Rank - 1], Vector3.zero, Quaternion.identity) as GameObject;
        g.name = "EggAnimation";
        PetUIController.SetLayer(g.transform, LayerHelper.UnitFX);
        StartCoroutine(ShowPetAnimation(p.SkinId, 1.25f));
        StartCoroutine(InfoBoardFlyIn(p.Rank, 1.75f, 1));
    }

    IEnumerator ShowPetAnimation(string SkinId, float time)
    {
        yield return new WaitForSeconds(time);

        GameObject PetAnimation = NGUITools.AddChild(NewBoard, Resources.Load<GameObject>("PreFabs/Characters/" + SkinId + "60"));
        PetAnimation.name = "PetAnimation";
        PetUIController.SetLayer(PetAnimation.transform, LayerHelper.Top);
        PetAnimation.transform.localPosition = Vector3.zero;
        PetAnimation.transform.localScale = new Vector3(100f, 100f, 1f);

        newStart = true;
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
