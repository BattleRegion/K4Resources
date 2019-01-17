using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface EquipmentForgeInter
{
    void OnFinish();
}

public class EquipmentForgeController : MonoBehaviour
{
    public EquipmentForgeInter animationInter;

    #region Sprite
    public List<SpriteRenderer> materials;

    public SpriteRenderer target;

    public SpriteRenderer baseSprite;

    public void SetAnimation(List<string> Ids, string TarId, string baseId)
    {
        if(Ids.Count > 5)
        {
            Debug.LogError("unexpected materials!");
            return;
        }

        for(int i = 0; i < 5; i++)
        {
            if (string.IsNullOrEmpty(Ids[i])) materials[i].sprite = null;
            else materials[i].sprite = Resources.Load<Sprite>(Tools.GetIconBySkinId(Ids[i]));
        }

        if (string.IsNullOrEmpty(TarId)) target.sprite = null;
        else target.sprite = Resources.Load<Sprite>(Tools.GetSkinPathBySkinId(TarId));

        if (string.IsNullOrEmpty(baseId)) baseSprite.sprite = null;
        else baseSprite.sprite = Resources.Load<Sprite>(Tools.GetIconBySkinId(baseId));
    }
    #endregion

    #region Animator
    public Animator ForgeAnimation;

    public GameObject ForgeFlash;

    void SwitchFlyInAction()
    {
        ForgeAnimation.SetInteger("Action", 1);
    }

    void SwitchComAction()
    {
        int count = 0;
        foreach(SpriteRenderer s in materials)
        {
            if (s.sprite != null) count++;
        }
        Invoke("SwitchCom", 1f / 9f + (float)count * (5f / 9f));
    }

    void SwitchCom()
    {
        ForgeAnimation.SetInteger("Action", 2);
    }

    void SwitchDefaultAction()
    {
        ForgeAnimation.SetInteger("Action", 0);
    }

    void OnFinish()
    {
        if(animationInter != null)
            animationInter.OnFinish();
    }
    #endregion

    #region mono
    void OnEnable()
    {
        SwitchDefaultAction();
        Invoke("SwitchFlyInAction", 1f);
    }

    void OnDisable()
    {
        target.transform.parent.gameObject.SetActive(false);
        ForgeFlash.SetActive(false);
        baseSprite.gameObject.SetActive(true);
    }
    #endregion
}
