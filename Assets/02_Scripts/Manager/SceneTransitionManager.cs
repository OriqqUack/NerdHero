using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransitionManager
{
    public static string NextSceneName;

    public static void LoadScene(string sceneName)
    {
        NextSceneName = sceneName;
        SceneManager.LoadScene("Scene_LoadingScene");
    }

    public static void LoadSceneInstantly(string sceneName)
    {
        NextSceneName = sceneName;
        SceneManager.LoadScene(sceneName);
    }
}
