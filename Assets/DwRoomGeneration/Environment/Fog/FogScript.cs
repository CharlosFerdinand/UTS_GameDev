using System.Collections.Generic;
using UnityEngine;

public class FogScript : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float minimumSpeed = 4; //the lowest speed the fog have to give bottom limit of the lerp
    [SerializeField] private float interpolationRate = 0.5f; //will clamp to range of 0 and 1 by Vector3.Lerp().
    [SerializeField] private float damage = 10f;
    [SerializeField] private float gracePeriod = 10f; //time before fog start moving
    [SerializeField] private float damageRate = 2; //damage trigger persecond
    private float damageTimer = 0f;
    private float speed = 0f;
    private bool start = false;
    private List<Vector3> pathPositions = new List<Vector3>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (target == null)
        {
            target = GameObject.Find("Player");
        }
        damageTimer = 1f / damageRate;
        Invoke("startFog", gracePeriod);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (start)
        {
            //will lerp toward the player
            movement();
        }
    }

    //im using linear interpolation so that the further the player is, the faster the fog moves
    private void movement()
    {
        //determine the speed of the fog.
        if (target != null)
        {
            float interpolationSpeed = Vector3.Distance(this.transform.position, target.transform.position) * interpolationRate;
            if (interpolationSpeed <= minimumSpeed)
            { //if interpolation speed is smaller or equal to minimal speed, use minimal speed
                speed = minimumSpeed;
            }
            else
            {
                speed = interpolationSpeed;
            }
        }

        //moving the fog
        if (pathPositions.Count > 0)
        {
            if (Vector3.Distance(this.transform.position, pathPositions[0]) < 0.1f)
            { //if fog is close enough to the path position.
                pathPositions.RemoveAt(0);
            }
            else
            { //move the fog to path position
                Vector3 direction = (pathPositions[0] - this.transform.position).normalized;
                this.transform.Translate(direction * speed * Time.fixedDeltaTime, Space.World);
                //rotate the fog to face the path position
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), 0.05f);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "hero")
        {
            damageCheck(other); //run damage check
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "hero")
        { //reset timer when exitting the fog.
            damageTimer = 1f / damageRate;
        }
    }

    private void damageCheck(Collider other)
    {
        if (damageTimer > 0f) //run timer down
        {
            damageTimer -= Time.fixedDeltaTime;
        }
        else
        { //deal damage and reset damageTimer
            if (other.GetComponent<DwInterfaceDamageAble>() != null) //check if interface exist
            {
                other.GetComponent<DwInterfaceDamageAble>().takeDamage(damage);
                damageTimer = 1f / damageRate;
            }
        }
    }

    private void startFog()
    {
        start = true;
    }

    public void addPath(List<Vector3> positions)
    {
        for(int i = 0; i < positions.Count; i++)
        {
            pathPositions.Add(positions[i]);
        }
    }
}
