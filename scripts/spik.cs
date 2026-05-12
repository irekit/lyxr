using UnityEngine;
using System.Collections;
public class spik : MonoBehaviour
{
    [SerializeField]private Transform render;
    [SerializeField] private SpriteRenderer ren;
    [SerializeField] private Sprite[] sprs;
    [SerializeField] private bool turnwhile = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        render.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-360, 360));
        ren.sprite = sprs[UnityEngine.Random.Range(0, sprs.Length)];
    }
    public void MoveRight()
    {
        StartCoroutine(Mov());
    }
    void OnEnable()
    {
        GetComponent<BoxCollider2D>().enabled = !turnwhile;
    }
    IEnumerator Mov()
    {
        for (int i = 0; i < 10; i++)
        {
            transform.Translate(0.4f, 0, 0);
            yield return new WaitForSeconds(0.03f);
        }
        GetComponent<BoxCollider2D>().enabled = turnwhile;
    }
}
