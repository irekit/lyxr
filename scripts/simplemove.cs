using UnityEngine;

public class simplemove : MonoBehaviour
{
    private GameObject player;
    private controller trol;
    private Vector3 original_position;
    [SerializeField] private bool start_on = true;
    bool hascol = false;
    void Start()
    {
        //Debug.Log(start_on);
        original_position = transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        trol = player.GetComponent<controller>();
        if(GetComponent<BoxCollider2D>() != null)
        {
            hascol = true;
            GetComponent<BoxCollider2D>().enabled = start_on;
        }
    }
    void Update()
    {
        transform.Translate(0, -Time.deltaTime * trol.scroll - trol.purescroll, 0);
        if (trol.respawning)
        {
            if(hascol) GetComponent<BoxCollider2D>().enabled = start_on;
            transform.position = new Vector3(original_position.x, original_position.y - trol.checkpoint, 0);
        }
    }
}
