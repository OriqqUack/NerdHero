using AllIn1SpriteShader;
using UnityEngine;
using UnityEngine.UI;

public class IslandController : MonoBehaviour
{
    [SerializeField] private CircleExpositor expositors = null;
    [SerializeField] private float expositorDistance;

    private int _currExpositor;

    void Start()
    {
        _currExpositor = 0;

        expositors.transform.position = new Vector3(0, expositorDistance, 0);
    }

    void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            expositors.ChangeTarget(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            expositors.ChangeTarget(1);
        }
    }

    public int GetCurrExpositor() { return _currExpositor; }
}
