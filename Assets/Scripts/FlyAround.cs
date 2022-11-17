using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyAround : MonoBehaviour
{
    [SerializeField] RectTransform parent;
    [SerializeField] RectTransform self;
    Vector3 origin;
    public float speed;
    [SerializeField] Vector3 destination;

    public enum FlyPattern { RANDOM, CIRCLES, ORTHOGONAL};
    //[System.NonSerialized] 
    public FlyPattern pattern = FlyPattern.RANDOM;
    private float radius = -1;
    private float angle;
    private float angleInc;
    private float direction;

    void Awake()
    {
        self = GetComponent<RectTransform>();
        parent =transform.parent.GetComponent<RectTransform>();
        origin = self.localPosition;
    }

    private IEnumerator Disable()
    {
        yield return new WaitForEndOfFrame();
        GetComponentInParent<HorizontalLayoutGroup>().enabled = false;
        
    }

    public void SetDestination()
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

        float time;
    void FixedUpdate()
    {
        // If out of bounds, reset
        if (self.localPosition.x > parent.rect.width / 2 ||
            self.localPosition.x < -parent.rect.width / 2 ||
            self.localPosition.y > parent.rect.height / 2 ||
            self.localPosition.y < -parent.rect.height / 2)
        {
            destination = origin;    // if out of range go back to middle
            Debug.Log(name + " out of bounds!");
            if (pattern == FlyPattern.CIRCLES)
            {
                radius -= 5f;
                if (radius < 5f) radius = 5f;
                SetDestination();
            }

        }
        if (Vector3.Distance(self.localPosition, destination) > 1f)
        {
            self.localPosition = Vector3.MoveTowards(self.localPosition, destination, Time.deltaTime * speed * 10f);
            //self.localPosition = Vector3.Lerp(self.localPosition, destination, time);
            //time += (speed * Time.deltaTime) / Vector3.Distance(self.localPosition, destination);
        }
        else
        {
            SetDestination();
            //origin = self.position;
            time = 0;
        }
    }
}
