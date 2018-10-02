using UnityEngine;

public class MouseFinger : Finger
{
    public bool active;
    public bool lastActive;
    Vector2 lastPosition;
    #region Properties
    public bool Ended
    {
        get
        {
            return lastActive && !active;
        }
    }
    public bool Began
    {
        get
        {
            return !lastActive && active;
        }
    }
    public bool Hold
    {
        get
        {
            return active && lastActive;
        }
    }
    public override TouchPhase Phase
    {
        get
        {
            if (Began) return TouchPhase.Began;
            else if (Ended) return TouchPhase.Ended;
            else if (Hold) return (Position == lastPosition) ? TouchPhase.Stationary : TouchPhase.Moved;
            else return TouchPhase.Canceled;
        }
    }
    public override Vector2 Position
    {
        get
        {
            return Input.mousePosition;
        }
    }
    public override Vector2 DeltaPosition
    {
        get
        {
            return (Vector2)Input.mousePosition - startPosition;
        }
    }
    #endregion
    public MouseFinger() : base()
    {
        startPosition = Position;
    }

    public void Update()        //  called from TouchManager.Update()
    {
        if (Input.GetMouseButton(0) && !active && !lastActive)
        {
            active = true;
        }
        if (!Input.GetMouseButton(0) && active && lastActive)
        {
            active = false;
        }

        if (Began) startTime = Time.unscaledTime;
    }

    public void LateUpdate()    //  called from TouchManager.LateUpdate()
    {
        lastActive = active;
        lastPosition = Position;
    }
}