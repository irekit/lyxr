using UnityEngine;
using System.Collections;
public class enemyscript : MonoBehaviour
{
    private GameObject player;
    private controller troll;
    [SerializeField] private ParticleSystem part;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject deactject;
    [SerializeField] private GameObject splash;
    [SerializeField] private bool turnwhile;
    private Vector3 og_pos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        troll = player.GetComponent<controller>();
        og_pos = transform.position;
    }
    void OnEnable()
    {
        GetComponent<BoxCollider2D>().enabled = !turnwhile;
        splash.SetActive(false);
        splash.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-180, 180));
        if (deactject != null) deactject.SetActive(true);
        anim = GetComponent<Animator>();
        anim.enabled = false;
        GetComponent<SpriteRenderer>().sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
    }
    private Animator anim;
    // Update is called once per frame
    public void MoveRight()
    {
        StartCoroutine(Mov());
    }
    IEnumerator Mov()
    {
        for (int i = 0; i < 8; i++)
        {
            transform.Translate(0.1f, 0, 0);
            yield return null;
            transform.Translate(0.3f, 0, 0);
            yield return null;
            transform.Translate(0.1f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        GetComponent<BoxCollider2D>().enabled = turnwhile;
    }
    void Update()
    {
        if (splash.activeSelf && splash.transform.localScale.x < 5)
        {
            splash.transform.localScale = splash.transform.localScale + new Vector3(Time.deltaTime * 10, Time.deltaTime * 10, 0);
        }
        if (troll.respawning)
        {
            if (deactject != null) deactject.SetActive(true);
            splash.SetActive(false);
            splash.transform.localScale = new Vector3(2, 2, 2);
            anim.enabled = false;
            GetComponent<SpriteRenderer>().sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
        }
    }
    public void Respawn()
    {
        if (deactject != null) deactject.SetActive(false);
        //if(!troll.optimisation) part.Play();
        anim.enabled = true;
        anim.Play("deth");
        GetComponent<BoxCollider2D>().enabled = false;
        splash.SetActive(true);
    }
}
