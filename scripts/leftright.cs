using UnityEngine;
using System.Collections;
public class leftright : MonoBehaviour
{
    [SerializeField] private ParticleSystem lef;
    [SerializeField] private ParticleSystem rig;
    private GameObject player;
    public void Lr(bool right)
    {
        if (!player.GetComponent<controller>().optimisation)
        {
            StartCoroutine(Lefr());
            if (right)
            {
                rig.Play();
            }
            else
            {
                lef.Play();
            }
        }
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private IEnumerator Lefr()
    {
        Time.timeScale = 0.01f;
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1;
    }
}
