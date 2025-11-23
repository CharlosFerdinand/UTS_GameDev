using System.Collections.Generic;
using UnityEngine;

public class DwRoomEnterScript : MonoBehaviour
{
    public Transform exitRoom;
    private DwRoomHandlerScript RoomHandler;
    private bool triggered = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RoomHandler =
            GameObject.Find("RoomHandler").GetComponent<DwRoomHandlerScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "hero" && !triggered)
        {
            triggered = true;
            //instantiate next room at "ExitRoom"
            updateHandler();
        }
    }

    private void updateHandler() //notify the RoomHandler on trigger
    {
        RoomHandler.notifyHandler(this.gameObject);//notify room handler from this object
        //deactivate trigger, Room handler will handle the deletion of Room
        this.gameObject.SetActive(false);
    }

    //get a list of path for fog in the room
    public List<Vector3> getFogPathing()
    {
        List<Vector3> positions = new List<Vector3>();
        if (this.transform.Find("FogPathing") == null)
        { //Default: use entry door and exit door
            positions.Add(this.transform.parent.position);
            positions.Add(exitRoom.position);
        }
        else
        { //use fog pathing's list of position
            foreach (Transform transform in this.transform.Find("FogPathing"))
            {
                positions.Add(transform.position);
            }
        }
        return positions;
    }
}
