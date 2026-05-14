using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class controller : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputact;
    private InputAction press;
    [SerializeField] private GameObject slash;
    [SerializeField] private GameObject longslash;
    public float justkilledenemy = 0;
    [SerializeField]private ParticleSystem uppart;
    [SerializeField]private ParticleSystem downpart;
    [SerializeField] private Sprite sprdef;
    [SerializeField] private Sprite sprpre;
    [SerializeField] private Sprite slashed;
    [SerializeField] private float[] checkpoints;
    private int checkpoint_index = 0;
    public float checkpoint;
    [SerializeField] private GameObject sprendject;
    private SpriteRenderer sprend;
    [SerializeField] private Camera rendcam;
    [SerializeField] private Material fullmatt;
    [SerializeField]private RenderTexture rend;
    [SerializeField] private GameObject longvfx;
    [SerializeField] private GameObject circlefx;
    private Animator animator;
    private SpriteRenderer circlerenderer;
    private float circletimer = 0;
    [SerializeField] private GameObject maincamera;
    public bool optimisation = false;
    private Vector3 last_pos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        last_pos = transform.position;
        if(Screen.width > Screen.height)
        {
            maincamera.GetComponent<Camera>().orthographicSize = 5;
        }
        else
        {
            maincamera.GetComponent<Camera>().orthographicSize = 6;
        }
            sprend = sprendject.GetComponent<SpriteRenderer>();
        animator = sprendject.GetComponent<Animator>();
        circlerenderer = circlefx.GetComponent<SpriteRenderer>();
        circlefx.SetActive(false);
        rend = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.RHalf);
        rend.Create();
        rendcam.gameObject.SetActive(true);
        //ACTIVATE IT IN GAME AND ASSIGN TEXTURE
        rendcam.targetTexture = rend;
        fullmatt.SetTexture("_tex", rend);
        longslash.SetActive(false);
        initscroll = scroll;
        initpos = transform.position.y;
        press = inputact["press"];
        rights = new List<Collider2D>();
        lefts = new List<Collider2D>();
        sources = GetComponents<AudioSource>();
        las_results = new List<Collider2D>();
        Reset();
        GameObject[] fects = GameObject.FindGameObjectsWithTag("effect");
        if (optimisation)
        {
            for (int i = 0; i < fects.Length; i++)
            {
                fects[i].SetActive(false);
            }
        }
    }
    private AudioSource[] sources;
    public bool right = true;
    [SerializeField] private Material[] scrollmats;
    [SerializeField] private ParticleSystem onepart;
    [SerializeField] private GameObject onepartject;
    [SerializeField] private float speedmultipliermultiplier;
    private float intendedscroll;
    [SerializeField] private float speedloss;
    [SerializeField] private bool infi = false;
    public float speedmultiplier = 1;
    private float longslashtimer = 0;
    public float purescroll = 0;
    bool lastdashing = false;
    private float slashedtimer = 0;
    private float master_pos = 0;
    private IEnumerator ScreenShake(bool rightdir, float force)
    {
        if (rightdir)
        {
            maincamera.transform.Translate(force, 0, 0);
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSecondsRealtime(0.01f);
                maincamera.transform.Translate(-force*0.2f, 0, 0);
            }
        }
        else
        {
            maincamera.transform.Translate(-force, 0, 0);
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSecondsRealtime(0.01f);
                maincamera.transform.Translate(force*0.2f, 0, 0);
            }
        }
    }
    struct Box
    {
        public Vector3 localFrontTopLeft { get; private set; }
        public Vector3 localFrontTopRight { get; private set; }
        public Vector3 localFrontBottomLeft { get; private set; }
        public Vector3 localFrontBottomRight { get; private set; }
        public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
        public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
        public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
        public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

        public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
        public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
        public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
        public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
        public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
        public Vector3 backTopRight { get { return localBackTopRight + origin; } }
        public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
        public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

        public Vector3 origin { get; private set; }

        public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
        {
            Rotate(orientation);
        }
        public Box(Vector3 origin, Vector3 halfExtents)
        {
            this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
            this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

            this.origin = origin;
        }

        Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            Vector3 direction = point - pivot;
            return pivot + rotation * direction;
        }
        public void Rotate(Quaternion orientation)
        {
            localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
            localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
            localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
            localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
        }
    }
    void DrawBox(Box box, Color color)
    {
        Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
        Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
        Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
        Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

        Debug.DrawLine(box.backTopLeft, box.backTopRight, color);
        Debug.DrawLine(box.backTopRight, box.backBottomRight, color);
        Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color);
        Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color);

        Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color);
        Debug.DrawLine(box.frontTopRight, box.backTopRight, color);
        Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
        Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color);
    }
    void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
    {
        DrawBox(new Box(origin, halfExtents, orientation), color);
    }
    //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
    void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color)
    {
        direction.Normalize();
        Box bottomBox = new Box(origin, halfExtents, orientation);
        Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

        Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
        Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
        Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
        Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
        Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
        Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
        Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
        Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

        DrawBox(bottomBox, color);
        DrawBox(topBox, color);
    }
    List<Collider2D> las_results;
    private void Update()
    {
        List<RaycastHit2D> results = new List<RaycastHit2D>();
        DrawBoxCastBox(last_pos, new Vector3(0.25f, 0.25f, 0.25f), Quaternion.identity, transform.position - last_pos, 1, Color.red);
        ContactFilter2D filt = new ContactFilter2D();
        filt.useLayerMask = true;
        filt.layerMask = LayerMask.GetMask("Default", "Ignore Raycast");
        int num_results = Physics2D.BoxCast(new Vector2(last_pos.x, last_pos.y), new Vector2(0.475f, 0.475f), 0, new Vector2(transform.position.x - last_pos.x, transform.position.y - last_pos.y), filt, results, 1);
        List<Collider2D> cur_cols = results.Select(r => r.collider).ToList();
        Debug.Log("colliding with " + cur_cols.Count + " and last colliding with " + las_results.Count);
        
        for (int i = 0; i < results.Count; i++)
        {
            Debug.Log(cur_cols[i].gameObject);
            bool conts = false;
            for(int j = 0; j < las_results.Count; j++)
            {
                if (las_results[j].gameObject == cur_cols[i].gameObject)
                {
                    conts = true;
                    Debug.Log("conts");
                }
            }
            if (!conts)
            {
                Debug.Log("colenter");
                CollisionEnter(results[i]);
            }
            else
            {
                CollisionStay(results[i]);
            }
        }
        for (int i = 0; i < las_results.Count; i++)
        {
            bool cons = false;
            for (int j = 0; j < cur_cols.Count; j++)
            {
                if (cur_cols[j].gameObject == las_results[i].gameObject)
                {
                    cons = true;
                    Debug.Log("cons");
                }
            }
            if (!cons)
            {
                CollisionExit(las_results[i]);
                Debug.Log("colexit");
            }
        }
        las_results = new List<Collider2D>(cur_cols);
        last_pos = transform.position;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -1.8f, 1.8f), transform.position.y, 0);
        if(circletimer > 0)
        {
            circletimer -= Time.deltaTime * 1.5f;
            circlerenderer.color = new Color(circletimer, circletimer, circletimer, 1);
            circlefx.transform.localScale = new Vector3(circlefx.transform.localScale.x + Time.deltaTime * 30, circlefx.transform.localScale.y + Time.deltaTime * 30, 1);
        }
        else
        {
            circlefx.SetActive(false);
        }
        if (longvfx.transform.localScale.y < 150)
        {

            longvfx.transform.Translate(0, -scroll * Time.deltaTime - purescroll, 0);
            longvfx.transform.localScale += new Vector3(Time.deltaTime, longvfx.transform.localScale.y * 5 * Time.deltaTime, 0);
        }
        else
        {
            longvfx.SetActive(false);
        }
        if (grounded)
        {
            sprend.flipX = right;
            slashedtimer = 0;
        }
        else
        {
            sprend.flipX = !right;
        }

        if (slashedtimer > 0)
        {
            slashedtimer -= Time.deltaTime;
        }
        onepartject.transform.Translate(0, -scroll * Time.deltaTime - purescroll, 0);
        if (upjectmoveright)
        {
            uppartject.transform.Translate(20 * Time.deltaTime, -scroll * Time.deltaTime - purescroll, 0);
        }
        else
        {
            uppartject.transform.Translate(-20 * Time.deltaTime, -scroll * Time.deltaTime - purescroll, 0);
        }


        longslash.transform.Translate(0, -scroll * Time.deltaTime - purescroll, 0);
        if (longslashtimer > 0)
        {
            longslashtimer -= Time.deltaTime;
        }
        else
        {
            longslash.SetActive(false);
        }
        if (justkilledenemy > 0)
        {
            justkilledenemy -= Time.deltaTime;
        }
        if (speedmultiplier > 0)
        {
            if (!bossmode)
            {
                speedmultiplier -= Time.deltaTime * speedloss * Mathf.Max(speedmultiplier * speedmultiplier, 0.2f);
                speedmultiplier -= Mathf.Exp(-speedmultiplier * speedmultiplier * 8) * 2 * Time.deltaTime * speedmultiplier;
            }
        }
        else if(!cur_respawning)
        {
            speedmultiplier = 0;
            Respawn();
        }
        if (!cur_respawning)
        {
            if (infi)
            {
                scroll = intendedscroll * Mathf.Clamp(speedmultiplier, 0, 2f) * speedmultipliermultiplier;
            }
            else
                scroll = intendedscroll * Mathf.Clamp(speedmultiplier, 0, 1) * speedmultipliermultiplier;
        }
        else scroll = 0;
        animator.SetFloat("speed", Mathf.Clamp(speedmultiplier, 0, 1));
        if (transform.position.y > initpos + 0.002f || transform.position.y < initpos - 0.002f)
        {
            float nepos = Mathf.Lerp(transform.position.y, initpos, 0.1f);
            purescroll = transform.position.y - nepos;
            transform.position = new Vector3(transform.position.x, nepos, 0);
        }
        else
        {
            purescroll = 0;
        }
        slash.transform.Translate(0, -scroll * Time.deltaTime - purescroll, 0);
        if (slashofftimer <= 0)
        {
            slash.SetActive(false);
        }
        else
        {
            if (right)
            {
                slash.transform.Translate(Time.deltaTime * 8f, 0, 0);

            }
            else
            {

                slash.transform.Translate(-Time.deltaTime * 8f, 0, 0);
            }

            SpriteRenderer spirend = slash.GetComponent<SpriteRenderer>();
            spirend.color = new Color(0.7f + slashofftimer * 1, 0.7f + slashofftimer * 1, 0.7f + slashofftimer * 1, 1);
            slash.transform.localScale = new Vector3(slash.transform.localScale.x + Time.deltaTime * 8, slash.transform.localScale.y - Time.deltaTime * 2, 1);
            slashofftimer -= Time.deltaTime;
        }
        master_pos += scroll * Time.deltaTime + purescroll;
        foreach(Material scrollmat in scrollmats)
        {
            scrollmat.SetFloat("_scroll", master_pos);
        }
        if (bufferleft > 0) bufferleft -= Time.deltaTime;
        if (bufferright > 0) bufferright -= Time.deltaTime;
        
        if (dashing)
        {
            if (!lastdashing)
            {
                circlefx.transform.localScale = new Vector3(5, 5, 0);
                circletimer = 1;
                circlefx.SetActive(true);
                circlefx.transform.position = transform.position;
                StartCoroutine(ScreenShake(right, 0.03f));
                lastdashing = true;
                intendedscroll = initscroll * 2.5f;
            }
        }
        else
        {
            if (lastdashing)
            {
                StartCoroutine(ScreenShake(right, 0.03f));
                lastdashing = false;
                circlefx.transform.localScale = new Vector3(5, 5, 0);
                circletimer = 1;
                circlefx.SetActive(true);
                circlefx.transform.position = transform.position;
            }
            intendedscroll = initscroll;
        }
        bool changed = false;
        if (press.WasPressedThisFrame() || Input.anyKeyDown)
        {
            if (right) changed = changed || ChangeLeft();
            else changed = changed || ChangeRight();
        }
        if(!changed)
        {
            if (bufferleft > 0.001f && right)
            {
                ChangeLeft();
            }
            else if (bufferright > 0.001f && grounded && !right)
            {
                ChangeLeft();
            }
        }
        animator.SetBool("grounded", grounded);
        //check for prepslash
        Vector3 offs_prep = right ? new Vector3(1f, -0.7f, 0) : new Vector3(-1f, -0.7f, 0);
        Vector3 offs_prep_2 = right ? new Vector3(1f, 0, 0) : new Vector3(-1f, 0, 0);
        Collider2D col = Physics2D.OverlapPoint(transform.position + offs_prep);
        Collider2D col_2 = Physics2D.OverlapPoint(transform.position + offs_prep_2);
        if (!cur_respawning && !prep_slashing && !grounded && (col != null && (col.gameObject.CompareTag("cell") || col.gameObject.CompareTag("enemy")) || col_2 != null && (col_2.gameObject.CompareTag("cell") || col_2.gameObject.CompareTag("enemy"))))
        {
            prep_slashing = true;
            prep_slash_timer = 0.1f;
            animator.SetTrigger("prepslash");
            Time.timeScale = 0.5f;
        }
        if(prep_slash_timer > 0)
        {
            prep_slash_timer -= Time.deltaTime;
        }
        else if(prep_slash_timer != -50)
        {
            prep_slash_timer = -50;
            prep_slashing = false;
            Time.timeScale = 1;
        }

        last_position = transform.position;
        for(int i = checkpoint_index; i < checkpoints.Length; i++)
        {
            if (master_pos < checkpoints[i] + 2 && master_pos > checkpoints[i] - 2)
            {
                checkpoint = checkpoints[i];
                checkpoint_index = i;
            }
        }
        if (transform.position.y > 6)
        {
            scroll = 0;
        }
        if (!cur_respawning && !grounded)
        {
            if (right)
            {
                if (infi)
                    transform.Translate(Time.deltaTime * dashspeed * Mathf.Clamp(speedmultiplier, 0, 2) * speedmultipliermultiplier, 0, 0);
                else
                    transform.Translate(Time.deltaTime * dashspeed * Mathf.Clamp(speedmultiplier, 0, 1) * speedmultipliermultiplier, 0, 0);
            }
            else
            {
                if (infi)
                    transform.Translate(-Time.deltaTime * dashspeed * Mathf.Clamp(speedmultiplier, 0, 2f) * speedmultipliermultiplier, 0, 0);
                else
                    transform.Translate(-Time.deltaTime * dashspeed * Mathf.Clamp(speedmultiplier, 0, 1) * speedmultipliermultiplier, 0, 0);
            }
        }
    }
    private bool prep_slashing = false;
    private float prep_slash_timer = 0;
    private float bufferleft = 0;
    private float bufferright = 0;
    public float initpos;
    private bool dashing = false;
    [SerializeField] private GameObject splash_ject;
    private bool grounded = false;
    private float slashofftimer = 0;
    [SerializeField] private float dashspeed = 80f;
    public bool bossmode;
    [SerializeField] private GameObject uppartject;
    bool upjectmoveright;
    void CollisionStay(RaycastHit2D other)
    {
        if (other.collider.gameObject.CompareTag("wall"))
        {
            dashing = false;
            grounded = true;
        }
    }
    void CollisionEnter(RaycastHit2D other)
    {
        if (other.collider.gameObject.CompareTag("spike"))
        {
            Respawn();
        }
        else if (other.collider.gameObject.CompareTag("cell"))
        {
            //non-harmful enemy type
            float las = other.transform.position.x;
            Vector3 othpos = other.transform.position;
            other.collider.gameObject.GetComponent<enemyscript>().Respawn();
            for (int i = 0; i < 10; i++)
            {
                Vector2 poscheck;
                if (right)
                {
                    poscheck = new Vector2(othpos.x + (i + 1) * 0.5f, othpos.y);
                }
                else
                {
                    poscheck = new Vector2(othpos.x - (i + 1) * 0.5f, othpos.y);
                }
                Collider2D newcol = Physics2D.OverlapPoint(poscheck);
                if (newcol && (newcol.gameObject.tag == "cell" || newcol.gameObject.tag == "enemy"))
                {
                    las = newcol.gameObject.transform.position.x;
                    newcol.gameObject.GetComponent<enemyscript>().Respawn();
                }
                else if(!cur_respawning)
                {
                    StartCoroutine(SlashedEnemies(i, other.collider, las));
                    break;
                }
            }

        }
        else if (other.collider.gameObject.CompareTag("enemy"))
        {
            if (Mathf.Abs(other.normal.x) == 1)
            {
                float las = other.transform.position.x;
                for(int i = 0; i < 10; i++)
                {
                    Vector2 poscheck;
                    if (right)
                    {
                        poscheck = new Vector2(other.transform.position.x + (i + 1) * 0.5f, other.transform.position.y);
                    }
                    else
                    {
                         poscheck = new Vector2(other.transform.position.x - (i + 1) * 0.5f, other.transform.position.y);
                    }
                        Collider2D newcol = Physics2D.OverlapPoint(poscheck);
                    if (newcol && newcol.gameObject.tag == "enemy")
                    {
                        las = newcol.gameObject.transform.position.x;
                        newcol.gameObject.GetComponent<enemyscript>().Respawn();
                    }
                    else if (!cur_respawning)
                    {
                        StartCoroutine(SlashedEnemies(i, other.collider, las));
                        break;
                    }
                }
         
                other.collider.gameObject.GetComponent<enemyscript>().Respawn();
            }
            else
            {
                Respawn();
            }
        }
        else if (other.collider.gameObject.CompareTag("wall"))
        {
            grounded = true;
            bool tx = transform.position.x < last_pos.x;
            transform.position = other.point;
            if(tx)
            {
                transform.Translate(0.24f, 0, 0);
            }
            else
            {
                transform.Translate(-0.24f, 0, 0);
            }
        }
        else if (other.collider.gameObject.CompareTag("right"))
        {
            rights.Add(other.collider);
        }
        else if (other.collider.gameObject.CompareTag("left"))
        {
            lefts.Add(other.collider);
        }
        else if (other.collider.gameObject.CompareTag("purple"))
        {
            rights.Add(other.collider);
            lefts.Add(other.collider);
        }
    }
    private Vector3 last_position;
    void Respawn()
    {
        if (!cur_respawning)
        {
            splash_ject.SetActive(true);
            splash_ject.transform.localScale = new Vector3(speedmultiplier * 4, speedmultiplier * 4, 0);
            if (right)
            {
                splash_ject.transform.localPosition = new Vector3(-0.16f, splash_ject.transform.localPosition.y, 0);
            }
            else
            {
                splash_ject.transform.localPosition = new Vector3(-0.16f, splash_ject.transform.localPosition.y, 0);
            }
            cur_respawning = true;
            speedmultiplier = 0.05f;
            StartCoroutine(Respawning());
            animator.SetTrigger("respawn");
            Time.timeScale = 0.5f;
        }
    }
    public bool cur_respawning = false;
    IEnumerator Respawning()
    {
        yield return new WaitForSeconds(1);
        cur_respawning = false;
        Reset();
    }
    void Reset()
    {
        splash_ject.SetActive(false);
        speedmultiplier = 1;
        master_pos = 0;
        rights.Clear();
        lefts.Clear();
        StartCoroutine(DoRespawning());
    }
    IEnumerator DoRespawning()
    {
        respawning = true;
        yield return null;
        respawning = false;
        Time.timeScale = 1;
        transform.position = new Vector3(-1.75f, 3.2f, 0);
    }
    float initscroll;
    public float scroll = -1f;
    private List<Collider2D> rights;
    private List<Collider2D> lefts;
    public bool respawning = false;
    void CollisionExit(Collider2D other)
    {
        if (other.gameObject.CompareTag("wall"))
        {
            grounded = false;
            dashing = true;
        }
        else if (other.gameObject.CompareTag("right"))
        {
            rights.Remove(other);
        }
        else if (other.gameObject.CompareTag("left"))
        {
            lefts.Remove(other);
        }
        else if (other.gameObject.CompareTag("purple"))
        {
            rights.Remove(other);
            lefts.Remove(other);
        }
    }
    bool ChangeRight()
    {
        if (grounded)
        {
            Debug.Log("cr");
            right = true;
            grounded = false;
            dashing = true;
            transform.Translate(0.01f, 0, 0);
            return true;
        }
        else
        {
            if (rights.Count > 0)
            {
                GameObject closject = rights[0].gameObject;
                foreach(Collider2D ot in rights)
                {
                    Vector3 np = ot.gameObject.transform.position;
                    if (Mathf.Abs(np.x - transform.position.x) < Mathf.Abs(closject.transform.position.x - transform.position.x))
                    {
                        closject = ot.gameObject;
                    }
                }
                transform.position = closject.transform.position;
                closject.GetComponent<leftright>().Lr(true);
                right = true;

                return true;
            }
            else
            {
                bufferright = 0.05f;
                return false;
            }
        }
    }
    IEnumerator SlashedEnemies(int i, Collider2D other, float las)
    {
        Debug.Log("I slashed " + (i + 1) + " enemies!");
        transform.position = new Vector3(transform.position.x, other.gameObject.transform.position.y, 0);
        Vector2 newpos = new Vector2(las, other.gameObject.transform.position.y + 0.1f);
        //Debug.Log(i);
        //Debug.Log(newpos);
        Vector2 sub = newpos - new Vector2(transform.position.x, transform.position.y);

        slashedtimer = 0.5f;
        speedmultiplier += i * 0.2f;
        if(infi)
            speedmultiplier -= i * 0.1f;
        StartCoroutine(ScreenShake(right, 0.01f * i));
        
        if (i > 5)
        {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(0.08f);
            Time.timeScale = 1;
            longvfx.SetActive(true);
            longslash.SetActive(true);
            longslashtimer = 0.01f;
            longslash.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(2, -2));
            longslash.transform.position = new Vector3(0, other.gameObject.transform.position.y, 0);

            longvfx.transform.position = longslash.transform.position;
            longvfx.transform.localScale = new Vector3(4.5f, 4.5f, 1);
            if(!optimisation) uppart.Play();
            if(!optimisation) downpart.Play();
            uppartject.transform.rotation = longslash.transform.rotation;
            if (right) uppartject.transform.position = new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, 0);
            else uppartject.transform.position = new Vector3(other.gameObject.transform.position.x + (i + 1) * 0.5f, other.gameObject.transform.position.y, 0);
            upjectmoveright = right;
            sources[2].Play();
        }
        else if(i > 0)
        {
            sources[1].Play();
        }
        else
        {
            sources[0].Play();
        }
        //onevfx, multvfx, longvfx
        if (i > 0)
        {
            Time.timeScale = 0;
            float inx = transform.position.x;
            yield return new WaitForSecondsRealtime(0.1f);
            while (transform.position.x < newpos.x - 2f || transform.position.x > newpos.x + 2f)
            {
                transform.position = new Vector3(transform.position.x + 2 * (right ? 1 : -1), transform.position.y + 0.01f, 0);
                yield return null;
            }
            Time.timeScale = 1;
            float rotslash = Mathf.Atan2(sub.x, sub.y);
            slashofftimer = 0.3f;
            slash.SetActive(true);
            if (right)
            {

                slash.transform.position = new Vector3(other.gameObject.transform.position.x + i * 0.25f, other.gameObject.transform.position.y, 0);
            }
            else
            {

                slash.transform.position = new Vector3(other.gameObject.transform.position.x - i * 0.25f, other.gameObject.transform.position.y, 0);
            }
            slash.transform.localScale = new Vector3(i * 0.2f + 0.3f, 1.5f, 1);

            slash.transform.rotation = Quaternion.Euler(0, 0, rotslash);
        }
        else
        {
            if (!optimisation) 
                onepartject.transform.position = other.gameObject.transform.position;
            if (!optimisation) 
                onepart.Play();
            Time.timeScale = 1;
        }

        transform.position = newpos;
        justkilledenemy = 2;
        prep_slashing = false;
    }
    bool ChangeLeft()
    {
        if (grounded) 
        {
            Debug.Log("cl");
            right = false;
            grounded = false;
            dashing = true;
            transform.Translate(-0.01f, 0, 0);
            return true;
        }
        else
        {
            if (lefts.Count > 0)
            {
                GameObject closject = lefts[0].gameObject;
                foreach (Collider2D ot in lefts)
                {
                    Vector3 np = ot.gameObject.transform.position;
                    if (Mathf.Abs(np.x - transform.position.x) < Mathf.Abs(closject.transform.position.x - transform.position.x))
                    {
                        closject = ot.gameObject;
                    }
                }
                transform.position = closject.transform.position;
                closject.GetComponent<leftright>().Lr(false);
                right = false;
                return true;
            }
            else
            {
                bufferleft = 0.05f;
                return false;
            }
        }
    }
}
