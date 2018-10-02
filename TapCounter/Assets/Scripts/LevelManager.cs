using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelUI UI;

    float levelTime;
    bool levelStarted;

    #region Properties

    float timer;
    float Timer
    {
        get { return timer; }
        set
        {
            timer = value;
            UI.SetTimerBar(timer / levelTime);
        }
    }

    private int score;
    public int Score
    {
        set
        {
            score = value;
            //  Update score text
            UI.SetScoreText(score);
        }
        get { return score; }
    }
    #endregion

    void Start()
    {
        StartLevel(10f);
    }

    void Update()
    {
        if (levelStarted)
        {
            if (Timer <= 0)
            { EndLevel(); }
            else
            { Timer -= Time.deltaTime; }
        }
    }

    void OnFingerTap(Finger finger)
    {
        Score++;
    }

    public void StartLevel(float _levelTime)
    {
        levelTime = _levelTime;
        Timer = levelTime;
        if (!levelStarted)
        {
            UI.StartLevelCountDown();
        }
    }

    public void StartLevelCallBack()
    {
        TouchManager.OnFingerTap += OnFingerTap;
        levelStarted = true;
    }

    public void EndLevel()
    {
        levelStarted = false;
        TouchManager.OnFingerTap -= OnFingerTap;
        ScoreManager.SubmitNewScore(Score);
        UI.ShowEndPanel();
    }
}
