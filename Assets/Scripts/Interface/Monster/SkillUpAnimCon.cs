using UnityEngine;
using System.Collections;

public class SkillUpAnimCon : MonoBehaviour, itemInterface
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
                SkinConfigData skindata = ConfigManager.SkinConfig.GetSkinDataById(Id_m1);  //素材
                HMTexture_1.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + skindata.IconId + " - s");
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
                SkinConfigData skindata = ConfigManager.SkinConfig.GetSkinDataById(Id_m2);  //素材
                HMTexture_2.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + skindata.IconId + " - s");
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
                SkinConfigData skindata = ConfigManager.SkinConfig.GetSkinDataById(Id_m3);  //素材
                HMTexture_3.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + skindata.IconId + " - s");
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
                SkinConfigData skindata = ConfigManager.SkinConfig.GetSkinDataById(Id_m4);  //素材
                HMTexture_4.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + skindata.IconId + " - s");
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
                SkinConfigData skindata = ConfigManager.SkinConfig.GetSkinDataById(Id_m5);  //素材
                HMTexture_5.sprite = Resources.Load<Sprite>("Atlas/ItemIcons/" + skindata.IconId + " - s");
                HMTexture_5.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            Animation_5.SetActive(false);
        }
        Invoke("PlayAudio", 3f);
        Invoke("SetOver", 4f);
        StartCoroutine(MaterialDelayDisappear());
    }

    void PlayAudio()
    {
        AudioClip Audio = Resources.Load<AudioClip>("Audio/UIAudio/Synthesis");
        audio.clip = Audio;
        audio.Play();
    }

    void SetOver()
    {
        isOver = true;
    }

    IEnumerator MaterialDelayDisappear(GameObject material)
    {
        yield return new WaitForSeconds(1.5f);
        DestroyImmediate(material);
    }

    IEnumerator MaterialDelayDisappear()
    {
        yield return new WaitForSeconds(1.5f);
        HMTexture_1.sprite = null;
        HMTexture_2.sprite = null;
        HMTexture_3.sprite = null;
        HMTexture_4.sprite = null;
        HMTexture_5.sprite = null;
    }


    void SetScale(GameObject g)
    {
        g.transform.localScale = new Vector3(56.8f, 56.8f, 56.8f);
    }

    void OnEnable()
    {
        isOver = false;
        Invoke("SetPowUpInfo", 0.01f);
    }

    void OnDisable()
    {
        Destroy(BasePet);
    }

    /// <summary>
    /// 用于进化
    /// </summary>
    public GameObject CurView;
    public GameObject TargetView;
    public PetSkillEvolution PEvoView;

    public void SetPowUpInfo()
    {
        SetStartInfo();
        UIEventListener.Get(BaseBoard).onClick = (g) =>
        {
            if (isOver)
            {
                gameObject.SetActive(false);
                PEvoView.SetSkillEvolution(Pbase.UserPetId);
                //if (CurView != null && TargetView != null)
                //{
                //    CurView.SetActive(false);
                //    TargetView.SetActive(true);
                //}
            }
        };
    }
    #endregion

    #region NGUI
    public UILabel Name;
    public ItemInterface Icon;
    public UILabel PreSkillLv;
    public UILabel LaterSkillLv;
    public UILabel SkillName;

    public UserPet Pbase;

    void SetStartInfo()
    {
        //Name.text = Pbase.CurPetData.Name;
        //SkillName.text = Pbase.CurPetData.PetSkillData.Name;
        //Icon.SetItem(Pbase.Level, Pbase.CurPetData.PCost, (int)Pbase.CurHp, (int)Pbase.CurAtk, Pbase.CurPetData.PetPro, Pbase.CurPetData.Id, false, Pbase.CurPetData.Rank, Pbase.UserPetId);
        //PreSkillLv.text = (Pbase.CurSkillLv - 1).ToString();
        //LaterSkillLv.text = Pbase.CurSkillLv.ToString();
        //Icon.itemInter = this;
    }

    public void _OnClickItem(int UserMonsterID)
    {

    }

    public void _OnLongPressItem(int UserMonsterID)
    {

    }
    #endregion
}
