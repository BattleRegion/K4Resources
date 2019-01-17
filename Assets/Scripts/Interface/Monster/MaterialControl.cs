using UnityEngine;
using System.Collections;

public class MaterialControl : MonoBehaviour {

    public UserPet CurMaterialPet;

    public UITexture UpPetAvata;

    public UITexture UpPetFrame;

    public Texture baseFrame;

    public GameObject whiteFrame;

    public void ShowWhite(bool show)
    {
        whiteFrame.SetActive(show);
    }

    public void SetPet(UserPet up)
    {
        CurMaterialPet = up;
        SkinConfigData skinData = ConfigManager.SkinConfig.GetSkinDataById(up.CurPetData.SkinId);
        string frame = Tools.GetRankFrame(up.CurPetData.Rank);
        Texture ft = Resources.Load<Texture>("UI/UI_Assets/others/" + frame);
        Texture at = Resources.Load<Texture>("Atlas/PetAvatars/" + skinData.IconId);
        UpPetFrame.mainTexture = ft;
        UpPetAvata.mainTexture = at;
    }

    public void RemovePet()
    {
        UpPetAvata.mainTexture = null;
        UpPetFrame.mainTexture = baseFrame;
        CurMaterialPet = null;
    }
}
