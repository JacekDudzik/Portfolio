using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [Header("Game panel")]
    public LevelManager LM;
    [Space]
    public GameObject counter;
    [Space]
    public Text scoreText;
    public Text countDownText;
    [Space]
    public Image TimerBar;
    [Space]
    [Header("End panel")]
    public GameObject endPanel;
    public Text bestGameScoreText;
    public Text newHighscoreText;

    public void StartLevelCountDown()
    {
        StartCoroutine(StartLevelSequence());
    }

    public void SetScoreText(int value)
    {
        scoreText.text = value.ToString("000");
    }

    public void SetTimerBar(float value)
    {
        TimerBar.fillAmount = value;
    }

    private IEnumerator StartLevelSequence()
    {
        Animator countDownAnimator = countDownText.GetComponent<Animator>();

        countDownText.text = "3";
        countDownAnimator.SetTrigger("Show");
        yield return new WaitForSeconds(1f);
        countDownText.text = "2";
        countDownAnimator.SetTrigger("Show");
        yield return new WaitForSeconds(1f);
        countDownText.text = "1";
        countDownAnimator.SetTrigger("Show");
        yield return new WaitForSeconds(1f);
        countDownText.text = "START";
        countDownAnimator.SetTrigger("Show");
        yield return new WaitForSeconds(1f);
        //  show counter
        counter.SetActive(true);
        LM.StartLevelCallBack();
    }

    public void ShowEndPanel()
    {
        bestGameScoreText.text = ScoreManager.BestScore.ToString("000");
        endPanel.SetActive(true);
        newHighscoreText.gameObject.SetActive(ScoreManager.NewHighscore);
    }
}
