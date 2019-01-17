using UnityEngine;
using System.Collections;

public enum InputState
{
    None,
    Start,
    Hold,
    End,
    Move
}

/// <summary>
/// 鼠标输入
/// </summary>
public class K4Input : MonoBehaviour
{
    public static bool inputLock = false;

    public InputState CurInput = InputState.None;

    public Vector3 StartPosition = Vector3.zero;

    public Vector3 EndPosition = Vector3.zero;

    public Vector3 CurrentPosition = Vector3.zero;

    public Vector3 LastPosition = Vector3.zero;

    public Vector3 direction
    {
        get
        {
            if ((CurrentPosition - StartPosition) == Vector3.zero) return Vector3.zero;
            else return (CurrentPosition - StartPosition) / (CurrentPosition - StartPosition).magnitude;
        }
        set
        {
            //direction = value;
        }
    }

    public int frameCount;

    public float inputTime;

    void SetVectors()
    {
        switch(CurInput)
        {
            case InputState.Start:
                {
                    StartPosition = ScreenPositionToLocalPosition(Input.mousePosition);
                    CurrentPosition = ScreenPositionToLocalPosition(Input.mousePosition);
                    LastPosition = ScreenPositionToLocalPosition(Input.mousePosition);
                    return;
                }
            case InputState.End:
                {
                    EndPosition = ScreenPositionToLocalPosition(Input.mousePosition);
                    CurrentPosition = ScreenPositionToLocalPosition(Input.mousePosition);
                    LastPosition = ScreenPositionToLocalPosition(Input.mousePosition);
                    return;
                }
        }
        LastPosition = CurrentPosition;
        CurrentPosition = ScreenPositionToLocalPosition(Input.mousePosition);
    }

    void SetInputTime()
    {
        inputTime = (float)frameCount * Time.deltaTime;
    }

    Vector3 ScreenPositionToLocalPosition(Vector3 input)
    {
        Vector3 temp = Camera.main.ScreenToWorldPoint(input);
        return 568f * temp;
    }

    void OnEnable()
    {
        CurInput = InputState.None;
        frameCount = 0;
    }

    void Update()
    {
        if(!inputLock)
        {
            //set frame count
            if (CurInput != InputState.None)
            {
                frameCount++;
            }

            if (Input.GetMouseButtonDown(0))
            {
                CurInput = InputState.Start;
                frameCount = 1;
            }
            else if (CurInput != InputState.End && Input.GetMouseButtonUp(0))
            {
                CurInput = InputState.End;
                SetVectors();
                return;
            }
            else if (CurInput == InputState.Start && (CurrentPosition == LastPosition))
            {
                CurInput = InputState.Hold;
            }
            else if ((CurInput == InputState.Start || CurInput == InputState.Hold) && (CurrentPosition != LastPosition))
            {
                CurInput = InputState.Move;
            }
            else if (CurInput == InputState.End)
            {
                CurInput = InputState.None;
            }

            //set input position
            if (CurInput != InputState.End && CurInput != InputState.None)
            {
                SetVectors();
            }

            SetInputTime();
        }
    }
}
