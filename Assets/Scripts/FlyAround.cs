using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class FlyAround : MonoBehaviour
{
    private bool firstUpdate = true;
    [SerializeField] RectTransform parent;
    [SerializeField] RectTransform self;
    Vector2 origin;
    public float speed;
    private Vector2 destination;

    public enum FlyPattern { RANDOM, CIRCLES, ORTHOGONAL };
    //[System.NonSerialized] 
    public FlyPattern pattern = FlyPattern.RANDOM;
    private float radius = -1;
    private float angle;
    private float angleInc;
    private float direction;
    RectTransform[] snaps;
    Vector2[] spawnPoints = new Vector2[9];
    public int cornerIndex = 0;

    private float b_height, b_width, p_height, p_width;


    void OnEnable()
    {
        self = GetComponent<RectTransform>();
        // forces to center
        self.anchorMin = new Vector2(0.5f, 0.5f);
        self.anchorMax = new Vector2(0.5f, 0.5f);
        parent = transform.parent.GetComponent<RectTransform>();

        snaps = GetActiveSnaps(parent.parent);
    }

    private void FillCornerArray()
    {
        b_width = self.rect.width / 2f;
        b_height = self.rect.height / 2f;
        p_width = parent.rect.width / 2f;
        p_height = parent.rect.height / 2f;
        float left = b_width - p_width;
        float middle = 0;
        float right = p_width - b_width;
        float bottom = b_height - p_height;
        float center = 0;
        float top = p_height - b_height;

        spawnPoints[0] = new Vector2(left, top); 
        spawnPoints[1] = new Vector2(middle, top); 
        spawnPoints[2] = new Vector2(right, top);
        spawnPoints[3] = new Vector2(left, bottom);
        spawnPoints[4] = new Vector2(middle, bottom); 
        spawnPoints[5] = new Vector2(right, bottom);
        spawnPoints[6] = new Vector2(left, center); 
        spawnPoints[7] = new Vector2(middle, center); 
        spawnPoints[8] = new Vector2(right, center); 
    }

    private Vector2 GetPosition()
    {
        int res = 0;
        if (Regex.Match(gameObject.name, @"\d+").Success)
        {
            res = int.Parse(Regex.Match(gameObject.name, @"\d+").Value);
        }
        int ctr = 0;
        for (int i = 0; i < res; i++)
        {
            if (ctr < spawnPoints.Length)
            {
                ctr++;
            }
            else
            {
                ctr = 0;
            }
        }
        return spawnPoints[ctr];
    }
    private IEnumerator Disable()
    {
        yield return new WaitForEndOfFrame();
        GetComponentInParent<HorizontalLayoutGroup>().enabled = false;

    }

    public void SetDestination(Vector2? dest)
    {
        if (dest != null)
        {
            destination = (Vector2)dest;
        }
        else
        {
            if (b_width == 0) return;   // dimensions not yet initialized
            // CARDS ARE ANCHORED TO TOP LEFT
            var x = Random.Range(b_width - p_width, p_width - b_width);
            var y = Random.Range(b_height - p_height, p_height - b_height);
            //Debug.Log(name + " pos: " + x + ", " + y);
            if (pattern == FlyPattern.CIRCLES)
            {
                if (radius == -1)
                {
                    radius = Random.Range(b_width/2f, p_width)/2f;
                    //radius = Mathf.Clamp(radius, 0, parent.rect.height/2f - y - self.rect.height/2f);

                    //origin = new Vector2(); // new Vector3(Random.Range(-25f, 25f), Random.Range(-25f, 25f), 0f);
                    Vector2 dir = self.anchoredPosition - origin;
                    angle = Vector3.SignedAngle(transform.right, dir, Vector3.right);
                    if (transform.localPosition.y < 0) angle *= -1f;
                    if (angle < 0f) angle = 360f + angle;
                    angle = angle * Mathf.PI / 180f;
                    origin = self.anchoredPosition - new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);

                    direction = Random.Range(-1f, 1f);
                    if (direction < 0) direction = -1f;
                    else direction = 1f;

                    angleInc = Mathf.PI * 5f / 180f;
                }
                if (Vector2.Distance(self.anchoredPosition, origin) <= radius + 1f) // dont start orbiting unless within range
                {
                    angle += angleInc * direction;
                    if (angle > 2f * Mathf.PI) angle = 0;
                    if (angle < 0) angle = 2f * Mathf.PI;

                    x = radius * Mathf.Cos(angle) + origin.x;
                    y = radius * Mathf.Sin(angle) + origin.y;
                }
                else
                {
                    x = origin.x;
                    y = origin.y;
                }
            }
            else if (pattern == FlyPattern.ORTHOGONAL)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    x = transform.localPosition.x;
                }
                else
                {
                    y = transform.localPosition.y;
                }
            }

            x = Mathf.Clamp(x, b_width - p_width, p_width - b_width);
            y = Mathf.Clamp(y, b_height - p_height, p_height - b_height);
            destination = new Vector2(x, y);
        }
    }

    float time;
    void FixedUpdate()
    {
        if (firstUpdate)
        {
            FillCornerArray();
            Vector2 corner = GetPosition();
            self.anchoredPosition = corner;
            //SetDestination(null);
            firstUpdate = false;
            origin = self.anchoredPosition;
            if (pattern == FlyPattern.CIRCLES)
            {
                // go to center
                float x = Random.Range(-p_width/2f, p_width/2f);
                float y = Random.Range(-p_height / 2f, p_height / 2f);
                origin = new Vector2(x, y);
                SetDestination(origin);
            }
        }

        // // If out of bounds, reset
        /*if (isOutOfBounds())
        {
            SetDestination(MoveToFarCorner());    // if out of range go to furthest corner
            Debug.Log(name + " out of bounds!");
            if (pattern == FlyPattern.CIRCLES)
            {
                radius -= 5f;
                if (radius < 5f) radius = 5f;
                SetDestination(null);
            }

        }*/
        if (Vector3.Distance(self.anchoredPosition, destination) > 1f)
        {
            self.anchoredPosition = Vector3.MoveTowards(self.anchoredPosition, destination, Time.deltaTime * speed * 10f);
        }
        else
        {
            SetDestination(null);
            //origin = transform.position;
            time = 0;
        }
       // Debug.Log(name + " is at position " + transform.anchoredPosition);
    }

    private Vector3 MoveToFarCorner()
    {
        Vector3 retVec = new Vector3();
        float prevDist = 0;

        foreach (Vector3 v in spawnPoints)
        {
            float dist = Vector3.Distance(v, self.localPosition);
            if (dist > prevDist)
            {
                prevDist = dist;
                retVec = v;
            }
        }
        return retVec;
    }

    private bool isOutOfBounds()
    {
        Debug.Log($"{name} OUT OF BOUNDS");
        return (self.anchoredPosition.x > parent.rect.width / 2 ||
            self.anchoredPosition.x < -parent.rect.width / 2 ||
            self.anchoredPosition.y > parent.rect.height / 2 ||
            self.anchoredPosition.y < -parent.rect.height / 2);
    }

    private RectTransform[] GetActiveSnaps(Transform ancestor)
    {
        List<RectTransform> retSnaps = new List<RectTransform>();
        RectTransform[] tmpArray = new RectTransform[0];
        foreach (Transform child in ancestor.transform)
        {
            if (child.transform.childCount > 0)
            {
                tmpArray = GetActiveSnaps(child);
            }
            if (child.name.Contains("Snap") && child.gameObject.activeSelf)
            {
                retSnaps.Add(child.GetComponent<RectTransform>());
            }
            if (tmpArray.Length > 0)
            {
                retSnaps.AddRange(tmpArray);
            }
        }
        return (retSnaps.ToArray());
    }
}
