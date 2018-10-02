using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public bool simulateTouchWithMouse;
    public float tapTimeThreshold;

    public static MouseFinger mouseFinger;
    public static List<Finger> fingers = new List<Finger>();

    #region Events
    public delegate void TouchEvent(Finger finger);
    public static TouchEvent OnFingerTap;
    public static TouchEvent OnFingerDown;
    public static TouchEvent OnFingerHold;
    public static TouchEvent OnFingerUp;
    #endregion

    private void Start()
    {
        mouseFinger = new MouseFinger();
    }

    private void Update()
    {
        mouseFinger.Update();

        fingers.AddRange(GetNewFingers());
        //  finger events
        for (int i = fingers.Count - 1; i >= 0; i--)
        {
            switch (fingers[i].Phase)
            {
                case TouchPhase.Began:
                    if (OnFingerDown != null) OnFingerDown(fingers[i]);
                    break;
                case TouchPhase.Stationary:
                    if (OnFingerHold != null) OnFingerHold(fingers[i]);
                    break;
                case TouchPhase.Moved:
                    if (OnFingerHold != null) OnFingerHold(fingers[i]);
                    break;
                case TouchPhase.Ended:
                    if (OnFingerUp != null) OnFingerUp(fingers[i]);

                    if (fingers[i].LifeTime <= tapTimeThreshold)
                        if (OnFingerTap != null) OnFingerTap(fingers[i]);

                    fingers.RemoveAt(i);
                    break;
            }
        }
    }
    private void LateUpdate()
    {
        mouseFinger.LateUpdate();
    }

    List<Finger> GetNewFingers()
    {
        Touch[] allTouches = Input.touches;
        List<Finger> newFingers = new List<Finger>();

        //  get active fingers
        for (int i = 0; i < allTouches.Length; i++)
        {
            if (allTouches[i].phase == TouchPhase.Began)
            {
                newFingers.Add(new Finger(allTouches[i]));
            }
        }
        if (simulateTouchWithMouse && mouseFinger.Began)
        {
            newFingers.Add(mouseFinger);
        }
        return newFingers;
    }
}

