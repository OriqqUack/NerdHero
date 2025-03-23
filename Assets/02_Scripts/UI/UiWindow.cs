using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//이 클래스를 상속받는 UI를 만들어야함.
public class UiWindow : MonoBehaviour, IPointerDownHandler
{
    public KeyCode CloseKey = KeyCode.Escape;
    public bool FixedDefaultPosition = false;
    public Vector2 DefaultPosition;
    [HideInInspector] public bool ChildWindow = false;
    [SerializeField] protected AudioClip clickSound;
    [SerializeField] protected AudioClip closeSound;
    
    public delegate void OnWindowClose(WindowHolder holder);
    
    private OnWindowClose _closeCallback;
    private WindowHolder _windowHolder;

    protected virtual void Start()
    {
        transform.Find("Dimed")?.GetComponent<Button>().onClick.AddListener(() => Close());
    }

    public void RegisterCloseCallback(OnWindowClose callback, WindowHolder holder)
    {
        _windowHolder = holder;
        _closeCallback = callback;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ActiveWindow();
    }

    public void ActiveWindow()
    {
        if (!ChildWindow)
        {
            transform.SetAsLastSibling();
            WindowManager.ActiveWindow = this;
        }
    }

    private void OnEnable()
    {
        if(FixedDefaultPosition) GetComponent<RectTransform>().anchoredPosition = DefaultPosition;
    }

    public virtual void Update()
    {
        if (Input.GetKeyDown(CloseKey) && WindowManager.ActiveWindow == this)
        {
            Close();
        }
    }

    public virtual void Close()
    {
        if(_closeCallback != null) _closeCallback(_windowHolder);
        //SoundManager.Instance.Play(closeSound);
        Destroy(gameObject);
    }

    public virtual void Initialize(WindowHolder holder, string name = "")
    {
        
    }
}
