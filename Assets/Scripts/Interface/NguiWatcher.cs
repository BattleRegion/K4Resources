using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class NguiWatcher : MonoBehaviour
{
    public GameObject WatchOn;

    void Update()
    {
        if (WatchOn.layer != LayerHelper.Unit)
        {
            Debug.LogError("The layer has been changed!");
        }
    }
}
