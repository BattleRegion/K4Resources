using UnityEngine;
using System.Collections;

public class NewFloorNotice : MonoBehaviour {

    public InfoLabel FloorLabel;

    void Start()
    {
      
    }

    public void ShowNewFloor(int curFloor, int totalFloor)
    {
        gameObject.SetActive(true);
        FloorLabel.SetNum(curFloor.ToString() + "/" + totalFloor.ToString());
        foreach (Transform t in FloorLabel.transform)
        {
            t.gameObject.layer = LayerHelper.Top;
        }
        transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        AnimationHelper.AnimationMoveTo(new Vector3(20, 80, 0), gameObject, iTween.EaseType.easeInOutExpo, null, null, 0.5f);
    }

    public void MoveOut()
    {
        AnimationHelper.AnimationMoveTo(new Vector3(-500, 80, 0), gameObject, iTween.EaseType.easeInOutExpo, gameObject, "OutEnd", 0.5f);
    }

    void OutEnd()
    {
        transform.localPosition = new Vector3(500, 80, 0);
        gameObject.SetActive(false);
    }
}
