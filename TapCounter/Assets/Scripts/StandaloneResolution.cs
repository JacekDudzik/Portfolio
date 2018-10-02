using UnityEngine;

public class StandaloneResolution : MonoBehaviour
{
    public bool fullScreen;
    [Space]
    public bool useRealScreenSize;
    public float realScreenRatio;
    public float gameScreenToRealScreenRatio;
    [Space]
    public bool verticalScreen;
    [Space]
    public int width;
    public int height;

    private void Awake()
    {
        //
        if (useRealScreenSize)
        {
            Resolution[] resolutions = Screen.resolutions;
            Resolution highestRes = resolutions[0];
            for (int i = 1; i < resolutions.Length; i++)
            {
                if (resolutions[i].height > highestRes.height)
                {
                    highestRes = resolutions[i];
                }
            }
            height = (verticalScreen) ? highestRes.width : highestRes.height;
            height = (int)(height * gameScreenToRealScreenRatio);
            width = (int)(height * realScreenRatio);
        }

        //Set screen size for Standalone
#if UNITY_STANDALONE
        Screen.SetResolution(width, height, fullScreen);
        Screen.fullScreen = fullScreen;
#endif
    }
}
