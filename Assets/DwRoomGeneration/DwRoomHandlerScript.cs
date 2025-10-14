using System.Collections.Generic;
using UnityEngine;

public class DwRoomHandlerScript : MonoBehaviour
{
    //developer included attribute
    [SerializeField] List<GameObject> roomVariation;

    //RoomHandler Self attribute (no need to change unless you want to change how this script works)
    private int roomVariaty = 1; //how many room variety exist (prefab)
    private int maxRoom = 5;
    private List<GameObject> listRooms = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roomVariaty = roomVariation.Count;
        listRooms.Add(GameObject.Find("Starting Room 1"));
        listRooms.Add(GameObject.Find("DwRoom1"));
        listRooms.Add(GameObject.Find("DwRoom2"));
    }


    //to get random prefab room
    private GameObject getRoom()
    {
        int index = Random.Range(0, roomVariaty); //get random number according to amount of room prefab
        return roomVariation[index];
    }

    //recieve notification
    public void notifyHandler(GameObject sourceObject)
    {
        //find index of source of the caller
        int srcIndex = listRooms.IndexOf(sourceObject.transform.parent.gameObject); //get room game object then find corresponding index
        int genIndex = srcIndex + 1; //to generate the second room ahead, get exitRoom of the next room
        //get first index ("EnterBox")
        Transform target = listRooms[genIndex].transform.GetChild(0).gameObject.GetComponent<DwRoomEnterScript>().exitRoom;

        //generate room
        GameObject generatedRoom = getRoom();

        //spawning the room and adding it to list for handler
        generatedRoom = Instantiate(
            generatedRoom,
            target.position,
            target.rotation
            );
        listRooms.Add(generatedRoom);

        //destroy further than 2 rooms away
        if (listRooms.Count > maxRoom)
        {
            GameObject room = listRooms[0];
            listRooms.RemoveAt(0);
            Destroy(room);
        }
    }
}
