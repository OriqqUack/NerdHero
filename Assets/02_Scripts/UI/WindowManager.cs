using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WindowManager : MonoSingleton<WindowManager>
{
    public static bool anyWindowExists
    {
        get
        {
            return windowsDic.Keys.Count > 0;
        }
    }

    public static UiWindow ActiveWindow;
    private static Dictionary<WindowHolder, UiWindow> windowsDic = new();
    private string sceneName;
    void Awake()
    {
        windowsDic.Clear();
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;
    }

    IEnumerator ActiveWindowLater(UiWindow window)
    {
        yield return 1;
        window.ActiveWindow();
    }

    public UiWindow OpenWindow(string prefabName, WindowHolder holder, bool bringToFront = true)
    {
        if (windowsDic.ContainsKey(holder))
        {
            return windowsDic[holder];
        }
        
        GameObject newWindow = Instantiate(Resources.Load<GameObject>($"Prefabs/UI/{sceneName}/UI_" + prefabName), transform);
        if (!newWindow.GetComponent<UiWindow>().FixedDefaultPosition)
        {
            float width = newWindow.GetComponent<RectTransform>().sizeDelta.x * 0.5f;
            float screenWidth = newWindow.GetComponentInParent<CanvasScaler>().GetComponent<RectTransform>().sizeDelta.x * 0.5f;
            //윈도우가 왼쪽과 오른쪽 모니터로 넘어가는것을 방지
            newWindow.transform.localPosition = new Vector3(Mathf.Clamp(-450f + (windowsDic.Count + 1) * 50f, width - screenWidth + 270f, screenWidth - width)
            , newWindow.transform.localPosition.y - windowsDic.Count * 50F, 0);
        }
        windowsDic.Add(holder, newWindow.GetComponent<UiWindow>());
        newWindow.GetComponent<UiWindow>().RegisterCloseCallback(OnWindowClose, holder);
        if (bringToFront) newWindow.GetComponent<UiWindow>().ActiveWindow();
        return newWindow.GetComponent<UiWindow>();
    }

    public void OnWindowClose(WindowHolder holder)
    {
        if(windowsDic.ContainsKey(holder)) windowsDic.Remove(holder);
        WindowHolder lastHolder = null;
        foreach (var key in windowsDic.Keys)
        {
            if(windowsDic[key] != null) lastHolder = key;
        }
        if(lastHolder != null) StartCoroutine(ActiveWindowLater(windowsDic[lastHolder]));
    }

    public static void CloseAllWindows()
    {
        List<UiWindow> windows = new List<UiWindow>();
        foreach(var obj in windowsDic.Keys) windows.Add(windowsDic[obj]);
        windowsDic.Clear();
        foreach (var obj in windows) obj.Close();
    }

    public static UiWindow GetWindow(string prefabName, WindowHolder holder, bool bringToFront = true)
    {
        if (Instance == null)
        {
            GameObject newObj = Instantiate(Resources.Load<GameObject>($"Prefabs/UI/WindowsManager"), FindObjectOfType<CanvasScaler>().transform);
            newObj.transform.SetAsLastSibling();
            newObj.transform.localPosition = Vector3.zero;
            newObj.transform.localScale = Vector3.one;
            instance = newObj.GetComponent<WindowManager>();
        }
        return instance.OpenWindow(prefabName, holder, bringToFront);
    }

    public static bool isWindowOpen(WindowHolder holder)
    {
        if(holder == null) return false;
        return windowsDic.ContainsKey(holder);
    }

    public static UiWindow GetOpenedWindow(WindowHolder holder)
    {
        if(holder == null) return null;
        if(windowsDic.ContainsKey(holder))
            return windowsDic[holder];
        else
            return null;
    }
}
