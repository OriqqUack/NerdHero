using UnityEngine;

public abstract class UI_Popup : MonoBehaviour
{
    [SerializeField] protected AudioClip clickSound;
    [SerializeField] protected AudioClip closeSound;
    public abstract void Close();
}
