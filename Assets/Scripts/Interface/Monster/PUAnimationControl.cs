using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PUAnimationControl : MonoBehaviour
{
    #region Animation
    public GameObject Animation_1;
    public GameObject Animation_2;
    public GameObject Animation_3;
    public GameObject Animation_4;
    public GameObject Animation_5;

    public SpriteRenderer HMTexture_1;
    public SpriteRenderer HMTexture_2;
    public SpriteRenderer HMTexture_3;
    public SpriteRenderer HMTexture_4;
    public SpriteRenderer HMTexture_5;

    Vector3 BasePosition = new Vector3(0f, -22f, 0f);
    Vector3 Position_1 = new Vector3(-230f, -60f, 0f);
    Vector3 Position_2 = new Vector3(-145f, 75f, 0f);
    Vector3 Position_3 = new Vector3(145f, 75f, 0f);
    Vector3 Position_4 = new Vector3(230f, -60f, 0f);
    Vector3 Position_5 = new Vector3(0f, -150f, 0f);

    GameObject BasePet;

    public GameObject BaseBoard;

    bool isOver = false;

    public void SetAnimation(string Id_base, string Id_m1, string Id_m2, string Id_m3, string Id_m4, string Id_m5)
    {
        BasePet = NGUITools.AddChild(gameObject, Resources.Load<GameObject>("PreFabs/Characters/" + Id_base + "60"));
        BasePet.transform.localPosition = BasePosition;
        PetUIController.SetLayer(BasePet.transform, LayerHelper.UnitFX);
        SetScale(BasePet);
        if (!string.IsNullOrEmpty(Id_m1))
        {
            Animation_1.SetActive(true);
            if (Resources.Load<GameObject>("PreFabs/Characters/" + Id_m1 + "60") != null)
            {
                Animation_1.SetActive(true);
                GameObject Material_1 = NGUITools.AddChild(gameObject, Resources.Load<GameObject>("PreFabs/Characters/" + Id_m1 + "60"));
                PetUIController.SetLayer(Material_1.transform, LayerHelper.UnitFX);
                StartCoroutine(MaterialDelayDisappear(Material_1));
                SetScale(Material_1);
                Material_1.transform.localPosition = Position_1;
            }
            else
            {
                HMTexture_1.sprite = Resources.Load<Sprite>(Tools.GetIconBySkinId(Id_m1));
                HMTexture_1.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            Animation_1.SetActive(false);
        }
        if (!string.IsNullOrEmpty(Id_m2))
        {
            Animation_2.SetActive(true);
            if (Resources.Load<GameObject>("PreFabs/Characters/" + Id_m2 + "60") != null)
            {
                Animation_2.SetActive(true);
                GameObject Material_2 = NGUITools.AddChild(gameObject, Resources.Load<GameObject>("PreFabs/Characters/" + Id_m2 + "60"));
                PetUIController.SetLayer(Material_2.transform, LayerHelper.UnitFX);
                StartCoroutine(MaterialDelayDisappear(Material_2));
                SetScale(Material_2);
                Material_2.transform.localPosition = Position_2;
            }
            else
            {
                HMTexture_2.sprite = Resources.Load<Sprite>(Tools.GetIconBySkinId(Id_m1));
                HMTexture_2.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            Animation_2.SetActive(false);
        }
        if (!string.IsNullOrEmpty(Id_m3))
        {
            Animation_3.SetActive(true);
            if (Resources.Load<GameObject>("PreFabs/Characters/" + Id_m3 + "60") != null)
            {
                Animation_3.SetActive(true);
                GameObject Material_3 = NGUITools.AddChild(gameObject, Resources.Load<GameObject>("PreFabs/Characters/" + Id_m3 + "60"));
                PetUIController.SetLayer(Material_3.transform, LayerHelper.UnitFX);
                StartCoroutine(MaterialDelayDisappear(Material_3));
                SetScale(Material_3);
                Material_3.transform.localPosition = Position_3;
            }
            else
            {
                HMTexture_3.sprite = Resources.Load<Sprite>(Tools.GetIconBySkinId(Id_m1));
                HMTexture_3.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            Animation_3.SetActive(false);
        }
        if (!string.IsNullOrEmpty(Id_m4))
        {
            Animation_4.SetActive(true);
            if (Resources.Load<GameObject>("PreFabs/Characters/" + Id_m4 + "60") != null)
            {
                Animation_4.SetActive(true);
                GameObject Material_4 = NGUITools.AddChild(gameObject, Resources.Load<GameObject>("PreFabs/Characters/" + Id_m4 + "60"));
                PetUIController.SetLayer(Material_4.transform, LayerHelper.UnitFX);
                StartCoroutine(MaterialDelayDisappear(Material_4));
                SetScale(Material_4);
                Material_4.transform.localPosition = Position_4;
            }
            else
            {
                HMTexture_4.sprite = Resources.Load<Sprite>(Tools.GetIconBySkinId(Id_m1));
                HMTexture_4.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            Animation_4.SetActive(false);
        }
        if (!string.IsNullOrEmpty(Id_m5))
        {
            Animation_5.SetActive(true);
            if (Resources.Load<GameObject>("PreFabs/Characters/" + Id_m5 + "60"))
            {
                Animation_5.SetActive(true);
                GameObject Material_5 = NGUITools.AddChild(gameObject, Resources.Load<GameObject>("PreFabs/Characters/" + Id_m5 + "60"));
                PetUIController.SetLayer(Material_5.transform, LayerHelper.UnitFX);
                StartCoroutine(MaterialDelayDisappear(Material_5));
                SetScale(Material_5);
                Material_5.transform.localPosition = Position_5;
            }
            else
            {
                HMTexture_5.sprite = Resources.Load<Sprite>(Tools.GetIconBySkinId(Id_m1));
                HMTexture_5.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            Animation_5.SetActive(false);
        }
        Invoke("PlayAudio", 3f);
        StartCoroutine(MaterialDelayDisappear());
    }

    void PlayAudio()
    {
        AudioClip Audio = Resources.Load<AudioClip>("Audio/UIAudio/Synthesis");
        audio.clip = Audio;
        audio.Play();
    }

    IEnumerator MaterialDelayDisappear(GameObject material)
    {
        yield return new WaitForSeconds(1.5f);
        DestroyImmediate(material);
    }

    IEnumerator MaterialDelayDisappear()
    {
        yield return new WaitForSeconds(1.5f);
        if(HMTexture_1 != null)
        {
            HMTexture_1.sprite = null;
            HMTexture_2.sprite = null;
            HMTexture_3.sprite = null;
            HMTexture_4.sprite = null;
            HMTexture_5.sprite = null;
        }
    }


    void SetScale(GameObject g)
    {
        g.transform.localScale = new Vector3(56.8f, 56.8f, 56.8f);
    }
    #endregion

    public List<GameObject> HiddenObjects;

    void OnEnable()
    {
        isOver = false;
        foreach(GameObject g in HiddenObjects)
        {
            g.SetActive(false);
        }
        if(HMTexture_1 == null)
        {
            Invoke("SetPowUpInfo", 0.01f);
        }
    }

    void OnDisable()
    {
        foreach (GameObject g in HiddenObjects)
        {
            g.SetActive(true);
        }
        HiddenObjects.Clear();
        Destroy(BasePet);
    }

    /// <summary>
    /// 用于进化
    /// </summary>
    public GameObject CurView;
    public GameObject TargetView;

    public void SetEvolutionInfo(UserPet Pre, UserPet Later)
    {
        SetEvolutionIcon(Pre, Later);
        UIEventListener.Get(BaseBoard).onClick = (g) =>
        {
            if (isOver)
            {
                gameObject.SetActive(false);
                if (CurView != null && TargetView != null)
                {
                    CurView.SetActive(false);
                    TargetView.SetActive(true);
                }
            }
        };
        StartCoroutine(ChangePet(Later));
    }

    public void SetPowUpInfo()
    {
        SetStartInfo();
        Invoke("SetBuffInfo", 4f);
        UIEventListener.Get(BaseBoard).onClick = (g) =>
        {
            if (isOver)
            {
                gameObject.SetActive(false);
                if (CurView != null && TargetView != null)
                {
                    CurView.SetActive(false);
                    TargetView.SetActive(true);
                }
            }
        };
    }

    /// <summary>
    /// 设置升级动画结束标志
    /// </summary>
    void SetOver()
    {
        isOver = true;
    }

    #region NGUI/进化
    public ItemInterface PreIcon;
    public ItemInterface LaterIcon;
    public UILabel PreName;
    public UILabel LaterName;

    void SetEvolutionIcon(UserPet Pre, UserPet Later)
    {
        PreIcon.SetItem(Pre);
        LaterIcon.SetItem(Later);
        PreName.text = Pre.CurPetData.Name;
        LaterName.text = Later.CurPetData.Name;
        Invoke("SetOver", 4f);
    }

    IEnumerator ChangePet(UserPet later)
    {
        yield return new WaitForSeconds(3f);

        GameObject temp = BasePet;

        Destroy(temp);

        BasePet = NGUITools.AddChild(gameObject, Resources.Load<GameObject>("PreFabs/Characters/" + later.CurPetData.SkinId + "60"));
        BasePet.transform.localPosition = BasePosition;
        PetUIController.SetLayer(BasePet.transform, LayerHelper.UnitFX);
        SetScale(BasePet);
    }
    #endregion


    #region NGUI/升级
    public UILabel Name;
    public ItemInterface Icon;
    public UILabel PreLv;
    public UILabel PreHp;
    public UILabel PreAtk;

    public UILabel LaterLv;
    public UILabel LaterHp;
    public UILabel LaterAtk;
    public UILabel LvBuff;
    public UILabel HpBuff;
    public UILabel AtkBuff;

    public AlphaMaskBar ExpBar;
    public UILabel ExpLeft;
    public UILabel MaxLevelTips;

    public GameObject CurBoard;
    public GameObject AfterBoard;

    public UserPet Pbase = null;
    public UserPet PLvUp = null;
    public List<UserPet> Pmaterials = new List<UserPet>();

    void SetStartInfo()
    {
        AfterBoard.SetActive(false);
        Name.text = Pbase.CurPetData.Name;
        Icon.SetItem(Pbase.Level, Pbase.CurPetData.PCost, (int)Pbase.CurHp, (int)Pbase.CurAtk, Pbase.CurPetData.PetPro, Pbase.CurPetData.Id, false, Pbase.CurPetData.Rank, Pbase.UserPetId);
        PreLv.text = Pbase.Level.ToString();
        PreHp.text = Pbase.CurHp.ToString();
        PreAtk.text = Pbase.CurAtk.ToString();
        ExpBar.value = (float)Pbase.CurrentExp / (float)Pbase.CurLvlExp;
        ExpLeft.gameObject.SetActive(true);
        ExpLeft.text = (Pbase.CurLvlExp - Pbase.CurrentExp).ToString();
        MaxLevelTips.text = "";
    }

    void SetBuffInfo()
    {
        AfterBoard.SetActive(true);
        //LaterLv.text = PLvUp.Level.ToString();
        //LaterHp.text = PLvUp.CurHp.ToString();
        //LaterAtk.text = PLvUp.CurAtk.ToString();
        AnimationHelper.LabelUpdate(LaterLv, Pbase.Level, PLvUp.Level, 1f, "LaterLvUpdate", gameObject, null, null);
        AnimationHelper.LabelUpdate(LaterHp, (int)Pbase.CurHp, (int)PLvUp.CurHp, 1f, "LaterHpUpdate", gameObject, null, null);
        AnimationHelper.LabelUpdate(LaterAtk, (int)Pbase.CurAtk, (int)PLvUp.CurAtk, 1f, "LaterAtkUpdate", gameObject, null, null);
        AnimationHelper.LabelUpdate(LvBuff, 0, (PLvUp.Level - Pbase.Level), 1f, "LvBuffUpdate", gameObject, null, null);
        AnimationHelper.LabelUpdate(HpBuff, 0, (int)(PLvUp.CurHp - Pbase.CurHp), 1f, "HpBuffUpdate", gameObject, null, null);
        AnimationHelper.LabelUpdate(AtkBuff, 0, (int)(PLvUp.CurAtk - Pbase.CurAtk), 1f, "AtkBuffUpdate", gameObject, null, null);

        //LvBuff.text = "+" + (PLvUp.Level - Pbase.Level).ToString();
        //HpBuff.text = "+" + (PLvUp.CurHp - Pbase.CurHp).ToString();
        //AtkBuff.text = "+" + (PLvUp.CurAtk - Pbase.CurAtk).ToString();
        //ExpBar.value = (float)PLvUp.CurrentExp / (float)PLvUp.CurLvlExp;
        ExpBarUpdate(Pbase.Level, PLvUp.Level);
        if(PLvUp.Level < PLvUp.CurPetData.MaxLevel)
        {
            ExpLeft.text = (PLvUp.CurLvlExp - PLvUp.CurrentExp).ToString();
        }
        else if(PLvUp.CurPetData.Rank < PLvUp.CurPetData.MaxRank)
        {
            ExpLeft.gameObject.SetActive(false);
            MaxLevelTips.text = "已满级，请前去进化";
        }
        else
        {
            ExpLeft.gameObject.SetActive(false);
            MaxLevelTips.text = "已达终极状态！";
        }
        Icon.SetItem(PLvUp.Level, PLvUp.CurPetData.PCost, (int)PLvUp.CurHp, (int)PLvUp.CurAtk, PLvUp.CurPetData.PetPro, PLvUp.CurPetData.Id, false, PLvUp.CurPetData.Rank, PLvUp.UserPetId);
    }

    /// <summary>
    /// label动画
    /// </summary>
    void LaterLvUpdate(int value)
    {
        LaterLv.text = value.ToString();
    }

    void LaterHpUpdate(int value)
    {
        LaterHp.text = value.ToString();
    }

    void LaterAtkUpdate(int value)
    {
        LaterAtk.text = value.ToString();
    }

    void LvBuffUpdate(int value)
    {
        LvBuff.text = "+" + value.ToString();
    }

    void HpBuffUpdate(int value)
    {
        HpBuff.text = "+" + value.ToString();
    }

    void AtkBuffUpdate(int value)
    {
        AtkBuff.text = "+" + value.ToString();
    }

    /// <summary>
    /// 经验条动画
    /// </summary>
    void ExpBarUpdate(int fromLv, int toLv)
    {
        if (fromLv < toLv)
        {
            AnimationHelper.AnimationValueTo(ExpBar.gameObject, ((float)Pbase.CurrentExp/(float)Pbase.CurLvlExp), 1f, 0.5f, "ExpUpdate", gameObject, "UpdateOver", gameObject, toLv - fromLv - 1);
        }
        else
        {
            AnimationHelper.AnimationValueTo(ExpBar.gameObject, ((float)Pbase.CurrentExp / (float)Pbase.CurLvlExp), ((float)PLvUp.CurrentExp / (float)PLvUp.CurLvlExp), 0.5f, "ExpUpdate", gameObject, null, null, null);
        }
    }

    void UpdateOver(int tag)
    {
        if (tag > 0)
        {
            AnimationHelper.AnimationValueTo(ExpBar.gameObject, 0f, 1f, 0.5f, "ExpUpdate", gameObject, "UpdateOver", gameObject, tag - 1);
        }
        else if(PLvUp.Level < PLvUp.CurPetData.MaxLevel)
        {
            AnimationHelper.AnimationValueTo(ExpBar.gameObject, 0f, (float)PLvUp.CurrentExp/(float)PLvUp.CurLvlExp, 0.5f, "ExpUpdate", gameObject, "SetOver", gameObject, null);
        }
        else
        {
            AnimationHelper.AnimationValueTo(ExpBar.gameObject, 0f, 1f, 0.5f, "ExpUpdate", gameObject, "SetOver", gameObject, null);
        }
    }

    void ExpUpdate(float value)
    {
        ExpBar.value = value;
    }
    #endregion
}
