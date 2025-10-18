using UnityEngine;

public class FogScript : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float minimumSpeed = 3; //the lowest speed the fog have to give bottom limit of the lerp
    [SerializeField] private float interpolationRate = 0.5f; //will clamp to range of 0 and 1 by Vector3.Lerp().
    [SerializeField] private float damage = 10f;
    [SerializeField] private float gracePeriod = 10f; //time before fog start moving
    [SerializeField] private float damageRate = 2; //damage trigger persecond
    private float damageTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (target == null)
        {
            target = GameObject.Find("Player");
        }
        damageTimer = 1f / damageRate;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gracePeriod > 0f)
        {//run timer before fog starts chasing
            gracePeriod -= Time.fixedDeltaTime;
        }
        else
        {
            //will lerp toward the player
            movement();
        }
    }

    //im using lerp so that the further the player is, the faster the fog moves
    private void movement()
    {
        if (target != null)
        {
            if (Vector3.Distance(this.transform.position, target.transform.position)>2f)
            { //once player is roughly inside (around 2 meter away from center of the fog), fog will stop
                Vector3 direction = Vector3.Lerp(this.transform.position, target.transform.position, interpolationRate);
                direction = direction - this.transform.position;
                //lerp is basicaly outputs position that are from point a to certain percentage of point b.

                //check speed to apply speed equals to or more than minimum speed.
                if (direction.magnitude < minimumSpeed)
                {
                    direction = direction.normalized * minimumSpeed;
                }

                //apply movement
                this.transform.Translate(direction * Time.fixedDeltaTime, Space.World);
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
}
