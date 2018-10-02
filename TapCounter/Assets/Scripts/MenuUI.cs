using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public GameObject[] uiPanels;
    [Space]
    public Text gameBestScoreText;
    public Text sessionBestScoreText;

    private void Start()
    {
        gameBestScoreText.text = ScoreManager.BestScore.ToString("000");
        sessionBestScoreText.text = ScoreManager.BestSessionScore.ToString("000");
    }
    public void ShowPanel(GameObject panel)
    {
        foreach (GameObject uiPanel in uiPanels)
        {
            uiPanel.SetActive(false);
        }
        panel.SetActive(true);
    }
}
