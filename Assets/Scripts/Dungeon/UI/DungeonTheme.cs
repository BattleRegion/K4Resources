using UnityEngine;
using System.Collections;

public class DungeonTheme : MonoBehaviour {

    public SpriteRenderer Bottom;

    public SpriteRenderer BackGround;

    public void SetRender(string Id,DungeonScene scene)
    {
        string basicPath = "OrginPic/FightBackGround/";
        basicPath = basicPath + Id + "/";
        string bottomPath = basicPath + "bottom"+Id;
        string backGround = basicPath + "background"+Id;
        string door = basicPath + "Door" + Id;
        Bottom.sprite = Resources.Load<Sprite>(bottomPath);
        BackGround.sprite = Resources.Load<Sprite>(backGround);
        GameObject doorObj = Instantiate(Resources.Load(door)) as GameObject;
        doorObj.transform.parent = transform;
        scene.CurDoor = doorObj.GetComponent<FightDoor>();
    }
}
