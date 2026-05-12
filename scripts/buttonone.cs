using UnityEngine;
using UnityEngine.Events;
public class buttonone : MonoBehaviour
{
    private GameObject player;
    private controller trol;
    private Collider2D col;
    private bool righthere;
    [SerializeField] private Sprite buttonsprite1;
    [SerializeField] private Sprite buttonsprite2;
    private SpriteRenderer sprend;
    [SerializeField] private UnityEvent triggervent; 
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        trol = player.GetComponent<controller>();
        col = GetComponent<BoxCollider2D>();
        righthere = transform.position.x > 0;
        sprend = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (trol.respawning)
        {
            col.enabled = true;
            sprend.sprite = buttonsprite1;
        }
    }
    void OnTriggerEnter2D(Collider2D play)
    {
        if (play.gameObject.CompareTag("Player") && trol.right == righthere)
        {
            if (righthere)
            {
                if (player.transform.position.x < transform.position.x - 0.25f)
                {
                    Triggered();
                }
            }
            else
            {
                if(player.transform.position.x > transform.position.x + 0.25f)
                {
                    Triggered();
                }
            }
        }
    }
    void Triggered()
    {
        sprend.sprite = buttonsprite2;
        triggervent.Invoke();
    }
}
