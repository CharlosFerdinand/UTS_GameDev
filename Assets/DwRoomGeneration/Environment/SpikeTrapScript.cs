using UnityEngine;

public class SpikeTrapScript : MonoBehaviour
{
    [Header("Spike Stats")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private Vector3 attackPosition = Vector3.up * 0.46f; //position of spike when attacking in local position (0f,0.46f,0f)
    private Vector3 rest; //position for spike to rest
    private bool activate = false; //when its alive.
    private bool sleep = true; //when it consume no computing power.
    private Transform spike; //im gonna use this to move the spike, y position will go up to 0.46 local if activated

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spike = transform.GetChild(0); //as it is the only item in TrapBase
        rest = spike.localPosition;
    }

    // used fixed update to control the collision detection. (activate=true when on collision)
    void FixedUpdate()
    {
        if (activate)
        { //wake up when activate
            sleep = false;
        }

        if (!sleep && activate)
        {//when awake and is active, go to attack position
            moveSpike(attackPosition); //will move toward attack position.
        } 
        else if (!sleep)
        { //when awake but not attacking
            sleep = moveSpike(rest); //if it stop moving or if it reach resting zone, sleep.
        }
        activate = false;
    }


    //activation
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "hero")
        {
            activate = true;
        }
    }


    private bool moveSpike(Vector3 targetPos) //move the spike and return true if actively moving
    {
        Vector3 distance = targetPos - spike.localPosition;
        if (distance.magnitude >= 0.05f) //if distance length is not close
        {
            distance = distance * Time.fixedDeltaTime * 8; //within 0/8 second, will reach distance
            spike.localPosition = distance + spike.localPosition;
            return false;
        }
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "hero" && other.gameObject.GetComponent<DwInterfaceDamageAble>() != null)
        {
            other.gameObject.GetComponent<DwInterfaceDamageAble>().takeDamage(damage);
        }
    }
}
