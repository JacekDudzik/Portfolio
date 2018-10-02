using UnityEngine;

public class ScoreManager
{
    public static int BestSessionScore { get; private set; }
    public static int BestScore { get; private set; }
    public static bool NewHighscore { get; private set; }

    public static void LoadHighscores()
    {
        BestScore = PlayerPrefs.GetInt("bestScore");
    }

    public static void SubmitNewScore(int score)
    {
        NewHighscore = false;
        if (score > BestScore)
        {
            BestScore = score;
            BestSessionScore = score;
            NewHighscore = true;
        }
        else if (score > BestSessionScore)
        {
            BestSessionScore = score;
        }
    }

    public static void SaveHighscores()
    {
        PlayerPrefs.SetInt("bestScore", BestScore);
    }
}
