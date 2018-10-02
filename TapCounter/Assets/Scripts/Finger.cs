using UnityEngine;

public class Finger
{
    private Touch touch;
    public float startTime;
    public Vector2 startPosition;
    #region Properties
    public float LifeTime
    {
        get
        {
            return Time.unscaledTime - startTime;
        }
    }
    public virtual TouchPhase Phase
    {
        get
        {
            return touch.phase;
        }
    }
    public virtual Vector2 Position
    {
        get
        {
            return touch.position;
        }
    }
    public virtual Vector2 DeltaPosition
    {
        get
        {
            return touch.position - startPosition;
        }
    }
    #endregion
    public Finger(Touch _touch)
    {
        touch = _touch;
        startTime = Time.unscaledTime;
        startPosition = touch.position;
    }
    public Finger()
    {
        startTime = Time.unscaledTime;
    }
}