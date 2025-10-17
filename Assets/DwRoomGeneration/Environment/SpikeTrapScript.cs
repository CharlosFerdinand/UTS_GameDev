using UnityEngine;

public class SpikeTrapScript : MonoBehaviour
{
    public Transform spike; //im gonna use this to move the spike, y position will go up to 0.46 local if activated
    public Vector3 rest; //position for spike to rest
    public Vector3 attackPosition = Vector3.up * 0.46f; //position of spike when attacking in local position (0f,0.46f,0f)
    public bool activate = false; //when its alive.
    public bool sleep = true; //when it consume no computing power.

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
            Debug.Log("moving");
            distance = distance * Time.deltaTime * 8; //within 0/8 second, will reach distance
            spike.localPosition = distance + spike.localPosition;
            return false;
        }
        return true;
    }
}
