using UnityEngine;
using System.Collections;

public class AvatarTest : MonoBehaviour
{
    public string WeaponSkinId;
    public string HelmetSkinId;
    public string ArmorSkinId;

    public DungeonEnum.FaceDirection direction;

    public PlayerAvata player;

    void Start()
    {
        if (!string.IsNullOrEmpty(WeaponSkinId))
        {
            Debug.Log(WeaponSkinId);
            player.AddAvataWare(WeaponSkinId, DungeonEnum.FaceDirection.None);
        }
        if (!string.IsNullOrEmpty(HelmetSkinId))
        {
            player.AddAvataWare(HelmetSkinId, direction);
        }
        if (!string.IsNullOrEmpty(ArmorSkinId))
        {
            player.AddAvataWare(ArmorSkinId, direction);
        }
    }
}
