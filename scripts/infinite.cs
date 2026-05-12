using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class infinite : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private SpawnOrder[] strings;
    [SerializeField] private float gauss_multiplier;
    private float tim = 0;
    private List<GameObject>[] pool;
    [SerializeField] private int[] nums;
    [System.Serializable]
    struct SpawnOrder
    {
        public string order;
        public float difficulty;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    float diffi = 0;
    private GameObject player;
    private controller trol;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        trol = player.GetComponent<controller>();
        pool = new List<GameObject>[prefabs.Length];
        for (int j = 0; j < prefabs.Length; j++)
        {
            pool[j] = new List<GameObject>();
        }
        for(int j = 0; j < prefabs.Length; j++)
        {
            for (int i = 0; i < nums[j]; i++)
            {
                GameObject pol = Instantiate(prefabs[j], Vector3.zero, Quaternion.identity);
                pol.SetActive(false);
                pool[j].Add(pol);
            }
        }
    }
    void Update()
    {
        tim -= trol.scroll * Time.deltaTime + trol.purescroll;
        if(tim > 5)
        {
            tim -= 5;
            Spawn();
        }
        diffi += Time.deltaTime * 0.04f;
        for (int i = 0; i < pool.Length; i++)
        {
            for (int j = 0; j < nums[i]; j++)
            {
                if(pool[i][j].activeSelf && pool[i][j].transform.position.y > 10)
                {
                    pool[i][j].SetActive(false);
                }
            }
        }
        if (trol.respawning)
        {
            diffi = 0;
            for (int i = 0; i < pool.Length; i++)
            {
                for(int j = 0; j < nums[i]; j++)
                {
                    pool[i][j].SetActive(false);
                }
            }
        }
    }
    void Insta(int ind, float xpos)
    {
        GameObject jec = pool[ind][0];
        jec.SetActive(true);
        jec.transform.position = new Vector3(xpos, -10, 0);
        pool[ind].RemoveAt(0);
        pool[ind].Add(jec);
    }
    void Spawn()
    {
        float[] diffs = new float[strings.Length];
        float sum = 0;
        for(int i = 0; i < strings.Length; i++)
        {
            float ggg = strings[i].difficulty - diffi;
            diffs[i] = Mathf.Exp(-gauss_multiplier * ggg * ggg);
            sum += diffs[i];
        }
        float rand = UnityEngine.Random.value;
        float accum = 0;
        for(int i = 0; i < strings.Length; i++)
        {
            float ds = diffs[i] / sum;
            if((rand >= accum && rand < accum + ds) || i == strings.Length - 1)
            {
                string ord = strings[i].order;
                int it = 0;
                for(float j = -1.75f; j <= 1.75f; j += 0.5f)
                {
                    if (ord[it] == 'c')
                    {
                        Insta(0, j);
                    }
                    else if (ord[it] == 'e')
                    {
                        Insta(1, j);
                    }
                    else if (ord[it] == 'l')
                    {
                        Insta(2, j);
                    }
                    else if (ord[it] == 'p')
                    {
                        Insta(3, j);
                    }
                    else if (ord[it] == 'r')
                    {
                        Insta(4, j);
                    }
                    else if (ord[it] == 's')
                    {
                        Insta(5, j);
                    }

                    it++;
                }
                break;
            }
            accum += ds;
        }
    }
}
