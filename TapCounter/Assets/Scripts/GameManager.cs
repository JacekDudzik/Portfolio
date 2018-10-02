using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TouchManager))]
public class GameManager : MonoBehaviour
{
    static MonoBehaviour MB;
    static bool loadingLevel;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        MB = GetComponent<MonoBehaviour>();
        GameInitialization();
    }

    public static void LoadScene(string sceneName)
    {
        LoadScene(sceneName, .2f);
    }
    public static void LoadScene(string sceneName, float delay)
    {
        try
        {
            MB.StartCoroutine(LoadSceneRoutine(sceneName, delay));
        }
        catch (System.Exception)
        {
            Debug.Log("You need to have GameManger instance in your scene.");
            throw;
        }
    }

    void GameInitialization()
    {
        ScoreManager.LoadHighscores();
        LoadScene("MenuScene", 0f);
    }

    public static void ExitGame()
    {
        try
        {
            if (!loadingLevel) MB.StartCoroutine(ExitGameRoutine());
        }
        catch (System.Exception)
        {
            Debug.Log("You need to have GameManger instance in your scene.");
            throw;
        }
    }

    private void OnLevelWasLoaded(int sceneIndex)
    {
        loadingLevel = false;
    }

    private void OnApplicationQuit()
    {
        ScoreManager.SaveHighscores();
    }

    static IEnumerator ExitGameRoutine()
    {
        loadingLevel = true;
        yield return new WaitForSeconds(.2f);
        Application.Quit();
    }

    static IEnumerator LoadSceneRoutine(string sceneName, float delay)
    {
        if (!loadingLevel)
        {
            loadingLevel = true;
            AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneName);
            yield return new WaitForSeconds(delay);   //  ensure to wait at least .2 seconds (time for animations)
            while (!sceneLoad.isDone) yield return new WaitForEndOfFrame();
        }
    }
}
