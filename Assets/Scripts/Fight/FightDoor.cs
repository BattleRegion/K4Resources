using UnityEngine;
using System.Collections;

public class FightDoor : MonoBehaviour
{
    public Animator DoorAnimator;
    public GameObject OpenDoorFX;
    public bool HasOpen;
	// Use this for initialization
	void Start ()
    {
        UIEventListener.Get(gameObject).onClick = (go) =>
        {
            //GameObject d = GameObject.Find("DungeonScene");
            //DungeonScene dc = d.GetComponent<DungeonScene>();
            //dc.TryGoToNextFloor();
            //if(DoorAnimator.GetBool("Open"))
            //{
            //    CloseDoor();
            //}
            //else
            //{
            //    OpenDoor();
            //}
        };
	}


    /// <summary>
    /// 开门
    /// </summary>
    public void OpenDoor()
    {
        //OpenDoorFX.SetActive(true);
        DoorAnimator.SetBool("Open", true);
        HasOpen = true;
    }

    public void CloseDoor()
    {
        HasOpen = false;
        //OpenDoorFX.SetActive(false);
        DoorAnimator.SetBool("Open", false);
    }
}
