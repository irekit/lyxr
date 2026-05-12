using UnityEngine;

public class blackcover : MonoBehaviour
{
    private SpriteRenderer rend;
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        rend.color = new Color(1, 1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        float rend_col = 0;
        if(player.transform.position.y > transform.position.y)
        {
            rend_col = Mathf.Clamp((rend.bounds.max.y - player.transform.position.y) * 0.5f + 2, 0, 1);
        }
        else
        {
            rend_col = Mathf.Clamp((player.transform.position.y - rend.bounds.min.y) * 0.5f + 1, 0, 1);

        }
        rend.color = new Color(rend_col, rend_col, rend_col, 1);
    }
}
