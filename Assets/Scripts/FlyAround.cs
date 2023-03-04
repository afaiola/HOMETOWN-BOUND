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
    Vector3 origin;
    public float speed;
    [SerializeField] Vector3 destination;

    public enum FlyPattern { RANDOM, CIRCLES, ORTHOGONAL };
    //[System.NonSerialized] 
    public FlyPattern pattern = FlyPattern.RANDOM;
    private float radius = -1;
    private float angle;
    private float angleInc;
    private float direction;
    RectTransform[] snaps;
    Vector3[] spawnPoints = new Vector3[8];
    public int cornerIndex = 0;

    void Awake()
    {
        self = GetComponent<RectTransform>();
        parent = transform.parent.GetComponent<RectTransform>();
        FillCornerArray();
        // //GetComponent<RectTransform>().position = GetRandomCorner();
        Vector3 corner = GetPosition();
        self.localPosition = corner;

        // origin = self.localPosition;
        snaps = GetActiveSnaps(parent.parent);

    }

    private void FillCornerArray()
    {
        spawnPoints[0] = new Vector3(-parent.rect.width / 2 + (self.rect.width / 2), parent.rect.height / 2 - (self.rect.height / 2), 0); //top left
        spawnPoints[4] = new Vector3(0, parent.rect.height / 2 - (self.rect.height / 2), 0); // top centre
        spawnPoints[1] = new Vector3(parent.rect.width / 2 - (self.rect.width / 2), parent.rect.height / 2 - (self.rect.height / 2), 0); // top right
        spawnPoints[2] = new Vector3(-parent.rect.width / 2 + (self.rect.width / 2), -parent.rect.height / 2 + (self.rect.height), 0); // bottom left
        spawnPoints[5] = new Vector3(0, -parent.rect.height / 2 + (self.rect.height / 2), 0); // bottom centre
        spawnPoints[3] = new Vector3(parent.rect.width / 2 - (self.rect.width / 2), -parent.rect.height / 2 + (self.rect.height), 0); //bottom right
        spawnPoints[6] = new Vector3(parent.rect.width / 2 - (self.rect.width / 2), 0, 0); //mid right
        spawnPoints[7] = new Vector3(-parent.rect.width / 2 + (self.rect.width / 2), 0, 0); //mid left
    }

    private Vector3 GetPosition()
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

    public void SetDestination(Vector3? dest)
    {
        if (dest != null)
        {
            destination = (Vector3)dest;
        }
        else
        {
            // CARDS ARE ANCHORED TO TOP LEFT
            var x = Random.Range(-parent.rect.width / 2f, parent.rect.width / 2f);
            var y = Random.Range(-parent.rect.height / 2f, parent.rect.height / 2f);
            //Debug.Log(name + " pos: " + x + ", " + y);
            if (pattern == FlyPattern.CIRCLES)
            {
                if (radius == -1)
                {
                    // set up the card.
                    radius = Random.Range(10f, 25f);
                    origin = new Vector3(); // new Vector3(Random.Range(-25f, 25f), Random.Range(-25f, 25f), 0f);
                    Vector3 dir = transform.localPosition - origin;
                    angle = Vector3.SignedAngle(transform.right, dir, Vector3.right);
                    if (transform.localPosition.y < 0) angle *= -1f;
                    if (angle < 0f) angle = 360f + angle;
                    angle = angle * Mathf.PI / 180f;
                    origin = transform.localPosition - new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);

                    direction = Random.Range(-1f, 1f);
                    if (direction < 0) direction = -1f;
                    else direction = 1f;

                    angleInc = Mathf.PI * 5f / 180f;
                }

                angle += angleInc * direction;
                if (angle > 2f * Mathf.PI) angle = 0;
                if (angle < 0) angle = 2f * Mathf.PI;

                x = radius * Mathf.Cos(angle) + origin.x;
                y = radius * Mathf.Sin(angle) + origin.y;
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
            destination = new Vector3(x, y, 0);
            if (name == "Card")
            {
                //Debug.DrawLine(destination, origin, Color.red, 5f);
                //Debug.Log("angle: " + angle);
                //Debug.Log(name + " dest: " + x + ", " + y);
            }
        }
    }

    float time;
    void FixedUpdate()
    {
        if (firstUpdate)
        {
            firstUpdate = false;
            self.localPosition = GetPosition();
            origin = self.localPosition;
            SetDestination(MoveToFarCorner());
        }
        // if (IsWithinDeadZone(self, snaps))
        // {
        //     Bounce();
        // }
        // // If out of bounds, reset
        if (isOutOfBounds())
        {
            SetDestination(MoveToFarCorner());    // if out of range go to furthest corner
            Debug.Log(name + " out of bounds!");
            if (pattern == FlyPattern.CIRCLES)
            {
                radius -= 5f;
                if (radius < 5f) radius = 5f;
                SetDestination(null);
            }

        }
        if (Vector3.Distance(self.localPosition, destination) > 1f)
        {
            self.localPosition = Vector3.MoveTowards(self.localPosition, destination, Time.deltaTime * speed * 10f);
            //transform.localPosition = Vector3.Lerp(transform.localPosition, destination, time);
            //time += (speed * Time.deltaTime) / Vector3.Distance(transform.localPosition, destination);
        }
        else
        {
            SetDestination(null);
            //origin = transform.position;
            time = 0;
        }
        Debug.Log(name + " is at position " + transform.localPosition);
    }

    private void Bounce()
    {
        if (destination != Vector3.zero)
        {
            SetDestination(new Vector3(-destination.x, -destination.y, 0));
        }
        else
        {
            SetDestination(GetPosition());
        }
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
        return (self.localPosition.x > parent.rect.width / 2 ||
            self.localPosition.x < -parent.rect.width / 2 ||
            self.localPosition.y > parent.rect.height / 2 ||
            self.localPosition.y < -parent.rect.height / 2);
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

    private bool IsWithinDeadZone(RectTransform local, RectTransform[] snaps)
    {
        Vector3 loc = local.localPosition;
        Dictionary<RectTransform, bool> retval = new Dictionary<RectTransform, bool>();
        foreach (RectTransform snap in snaps)
        {
            bool overlapping = true;
            Vector3 snapTopLeft = snap.transform.localPosition;
            Vector3 snapBottomRight = new Vector3(snap.localPosition.x + snap.rect.width, snap.localPosition.y - snap.rect.height, 0);

            if (loc.y < snapBottomRight.y || // is transform is below the snap
            loc.y - local.rect.height > snapTopLeft.y)       //if transform is above the snap
            {
                overlapping = false;
            }
            if (loc.x > snapBottomRight.x || // transform is on the right of the snap
                loc.x + snap.rect.width < snapTopLeft.x) // if transform is on the left of the snap
            {
                overlapping = false;
            }
            retval.Add(snap, overlapping);
        }
        return retval.ContainsValue(true);
    }
}
