using UnityEngine;
using System.Collections;
using System;

public class K4Animation : MonoBehaviour
{
    public bool moveEnd = false;

    bool start = false;

    Vector3 targetPosition;

    Vector3 route;

    //Vector3 direction;

    float time;

    float frames;

    //float speed;

    float distance;

    Action MoveEndAction;

    public void MoveTo(Vector3 targetPosition, float time, Action callback)
    {
        this.targetPosition = targetPosition;
        this.time = time;
        this.frames = time / Time.deltaTime;
        this.route = (targetPosition - transform.localPosition);
        this.distance = (transform.localPosition - targetPosition).magnitude;
        MoveEndAction = callback;
        start = true;
    }

    void Move()
    {
        transform.localPosition += (route / frames);

        if (distance < (transform.localPosition - targetPosition).magnitude)
        {
            transform.localPosition = targetPosition;
            moveEnd = true;
        }

        distance = (transform.localPosition - targetPosition).magnitude;
    }

    void AnimationUpdate()
    {
        if(moveEnd)
        {
            if (MoveEndAction != null) MoveEndAction();
            Destroy(this);
        }
        if(start)
        {
            Move();
        }
    }

    void Update()
    {
        AnimationUpdate();
    }
}

public class K4MoveHelper
{
    public static void MoveTo(GameObject moveObject, Vector3 targetPosition, float time, Action callback = null)
    {
        K4Animation animation = moveObject.AddComponent<K4Animation>();
        animation.MoveTo(targetPosition, time, callback);
    }
}
