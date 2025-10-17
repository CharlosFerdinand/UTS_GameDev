using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    [Header("Movement Information")]
    [SerializeField] private List<Transform> pathPoints;
    [SerializeField] private float speed = 1;
    private Vector3 target;
    private int index;
    private bool moving = false;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameObject.AddComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
        rb.mass = 99999;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        if (pathPoints.Count > 1)
        {//if there is more than 1 move point
            moving = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (moving)
        {//when there is movement (or if pathPoints is not empty)
            target = pathPoints[index].position;
            if (Vector3.Distance(this.transform.position, target) > 0.1f)
            {
                Vector3 direction = Vector3.Lerp(this.transform.position, target, 0.7f);
                direction = (direction - this.transform.position) * speed * Time.deltaTime;
                rb.AddForce(direction, ForceMode.VelocityChange);
            }
            else
            {
                index += 1;
                if (index >= pathPoints.Count)
                {//loop
                    index = 0;
                }
            }
        }
    }
}
