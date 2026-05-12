using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class absorbscript : MonoBehaviour
{
    [SerializeField] private Animator nukeanimator;
    private GameObject player;
    [SerializeField] private Vector3 ogscale;
    public bool instance = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        nukeanimator.SetFloat("offs", UnityEngine.Random.value);
        sjc.transform.localScale = ogscale + new Vector3(UnityEngine.Random.value * 0.5f - 0.25f, UnityEngine.Random.value * 0.5f - 0.25f, 0);
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        con = player.GetComponent<controller>();
    }
    private controller con;
    [SerializeField]private ParticleSystem part;
    [SerializeField] public GameObject sjc;
    [SerializeField] private GameObject this_fab;
    void Split()
    {
        if(!Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y) + Vector2.right * 0.5f))
        {
            StartCoroutine(Gen(Vector2.right));
        }
        else if(!Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y) + Vector2.left * 0.5f))
        {
            StartCoroutine(Gen(Vector2.left));
        }
        else if(!Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y) + Vector2.down * 0.5f))
        {
            StartCoroutine(Gen(Vector2.down));
        }
        else if(!Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y) + Vector2.up * 0.5f))
        {
            StartCoroutine(Gen(Vector2.up));
        }
    }
    IEnumerator Gen(Vector2 pos)
    {
        GameObject insta = Instantiate(this_fab, transform.position, Quaternion.identity);
        insta.GetComponent<absorbscript>().instance = true;
        
        if (player.GetComponent<controller>().respawning) yield break;
        insta.transform.Translate(pos.x * 0.25f, pos.y * 0.25f, 0);
        for (int i = 0; i < 2; i++)
        {
            insta.transform.Translate(pos.x * 0.125f, pos.y * 0.125f, 0);
            yield return null;
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        if(instance && transform.position.y > 10)
        {
            Destroy(gameObject);
        }
        if(Mathf.Abs(player.transform.position.y - transform.position.y) < 4 && con.justkilledenemy > 0 && GetComponent<BoxCollider2D>().enabled)
        {
            if(!part.isPlaying) part.Play();
            sjc.transform.localScale = new Vector3(sjc.transform.localScale.y + Time.deltaTime * 0.7f, sjc.transform.localScale.y + Time.deltaTime * 0.7f, 1);
            if(sjc.transform.localScale.x > ogscale.x * 1.7f)
            {
                sjc.transform.localScale = ogscale + new Vector3(UnityEngine.Random.value * 0.5f - 0.25f, UnityEngine.Random.value * 0.5f - 0.25f, 0);
                if(transform.position.y < 8) Split();
            }
        }
        else
        {
            part.Stop();
        }
        if (player.GetComponent<controller>().respawning)
        {
            sjc.transform.localScale = ogscale + new Vector3(UnityEngine.Random.value * 0.5f - 0.25f, UnityEngine.Random.value * 0.5f - 0.25f, 0);
            if (instance) Destroy(gameObject);
        }
        if (player.GetComponent<controller>().cur_respawning && GetComponent<BoxCollider2D>().enabled)
        {
            sjc.transform.localScale = new Vector3(sjc.transform.localScale.y + Time.deltaTime * 5f, sjc.transform.localScale.y + Time.deltaTime * 5f, 1);
            if (sjc.transform.localScale.x > ogscale.x * 1.7f)
            {
                sjc.transform.localScale = ogscale + new Vector3(UnityEngine.Random.value * 0.5f - 0.25f, UnityEngine.Random.value * 0.5f - 0.25f, 0);
                if (transform.position.y < 8) Split();
            }
        }
    }
}
