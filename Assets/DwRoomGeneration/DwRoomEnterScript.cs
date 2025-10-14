using System.Collections.Generic;
using UnityEngine;

public class DwRoomEnterScript : MonoBehaviour
{
    private DwRoomHandlerScript RoomHandler;
    public Transform exitRoom;


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
        if (other.gameObject.tag == "hero")
        {
            //instantiate next room at "ExitRoom"
            updateHandler();
        }
    }

    private void updateHandler()
    {
        RoomHandler.notifyHandler(this.gameObject);//notify room handler from this object
        Destroy(this.gameObject);
    }
}
