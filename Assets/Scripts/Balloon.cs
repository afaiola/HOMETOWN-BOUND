using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// slowly moves object back to center when nudged
public class Balloon : MonoBehaviour
{
    public Transform anchor;
    private float rotateSpeed = 30f;

    private LineRenderer line;
    private Vector3 center;
    private Quaternion up;
    private float length;

    // Start is called before the first frame update
    void Start()
    {
        center = transform.localPosition;
        up = transform.localRotation;
        length = Vector3.Distance(transform.localPosition, anchor.localPosition);
        Vector3[] positions = new Vector3[2];
        line = anchor.GetComponent<LineRenderer>();
        line.GetPositions(positions);
        positions[1] = transform.localPosition;
        line.SetPositions(positions);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dist = Vector3.Distance(transform.localPosition, center);
        float height = Vector3.Distance(anchor.localPosition, transform.localPosition);
        Vector3 pos = transform.localPosition;

        // move object back to center
        if (dist > 0.05f)
        {
            float angle = Vector3.Angle(anchor.up, (transform.localPosition - anchor.localPosition).normalized);
            height = Mathf.Cos(angle * Mathf.PI / 180f) * length;
            pos = new Vector3(transform.localPosition.x, height, transform.localPosition.z);

            transform.localPosition = Vector3.MoveTowards(pos, center, Time.deltaTime * dist);
            Vector3[] positions = new Vector3[2];
            line.GetPositions(positions);
            positions[1] = transform.localPosition;
            line.SetPositions(positions);
        }
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, up, Time.deltaTime*rotateSpeed);

    }
}
