using AllIn1SpriteShader;
using UnityEngine;
using UnityEngine.UI;

public class IslandController : MonoBehaviour
{
    [SerializeField] private CircleExpositor expositors = null;
    public float expositorDistance;

    private int currExpositor;

    void Start()
    {
        currExpositor = 0;

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

    public int GetCurrExpositor() { return currExpositor; }
}
