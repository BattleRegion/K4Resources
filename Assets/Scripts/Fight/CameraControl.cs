using UnityEngine;
using System.Collections;

public class CameraControl  {
    public static void ShakeCamera()
    {
        iTween.ShakePosition(GameObject.Find("DungeonCamera"), new Vector3(0.04f, 0.04f, 0), 0.6f);
    }
    public static void BossInShake()
    {
        iTween.ShakePosition(GameObject.Find("DungeonCamera"), new Vector3(0.06f, 0, 0), 0.5f);
    }
}
