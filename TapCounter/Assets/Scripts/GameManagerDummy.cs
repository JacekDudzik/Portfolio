using UnityEngine;

public class GameManagerDummy : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        GameManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        GameManager.ExitGame();
    }
}
