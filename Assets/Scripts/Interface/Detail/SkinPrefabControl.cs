using UnityEngine;
using System.Collections;

public class SkinPrefabControl : MonoBehaviour 
{
    public UITexture ItemTexture;
    public SpriteRenderer HardwareSprite;

    /// <summary>
    /// 装备或者素材skin
    /// </summary>
    public void SetSkin(string Id)
    {
        if(ConfigManager.ItemConfig.GetItemById(Id) != null)
        {
            HardwareSprite.gameObject.SetActive(false);
            ItemTexture.gameObject.SetActive(true);

            ItemTexture.mainTexture = Resources.Load<Texture>(Tools.IconPath + ConfigManager.ItemConfig.GetItemById(Id).SkinId);
        }
        else if(ConfigManager.HardWareConfig.GetHardWareById(Id) != null)
        {
            HardwareSprite.gameObject.SetActive(true);
            ItemTexture.gameObject.SetActive(false);

            HardwareSprite.sprite = Resources.Load<Sprite>(Tools.IconPath + ConfigManager.HardWareConfig.GetHardWareById(Id).SkinId);
        }
    }
}
