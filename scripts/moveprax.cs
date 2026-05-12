using UnityEngine;

public class moveprax : MonoBehaviour
{
    private GameObject player;
    private controller trol;
    private Vector3 original_position;
    [SerializeField] private float parallax;
    float lastcheck = 0;
    void Start()
    {
        original_position = transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        trol = player.GetComponent<controller>();
        lastcheck = trol.checkpoint;
    }
    void Update()
    {
        if(trol.checkpoint != lastcheck)
        {
            original_position = transform.position;
        }
        lastcheck = trol.checkpoint;
        transform.Translate(0, -Time.deltaTime * trol.scroll * parallax - trol.purescroll * parallax, 0);
        if (trol.respawning)
        {
            transform.position = new Vector3(original_position.x, original_position.y, 0);
        }
    }
}
