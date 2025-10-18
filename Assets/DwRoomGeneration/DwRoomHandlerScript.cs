using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DwRoomHandlerScript : MonoBehaviour
{
    //developer included attribute
    [Header("List of Variety")]
    [SerializeField] List<GameObject> roomVariation;

    //RoomHandler Self attribute (no need to change unless you want to change how this script works)
    private int roomVariaty = 1; //how many room variety exist (prefab)
    private int maxRoom = 5;
    private List<GameObject> listRooms = new List<GameObject>();
    private int score;


    [Header("UI")]
    [SerializeField] private TMP_Text scoreAmount;
    [SerializeField] private TMP_Text scoreResult;
    [SerializeField] private TMP_Text scoreCurrently;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
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

        //Sealing Furthest door
        if (srcIndex == 3) //if source came from Fourth of the list
        {
            //seal the entrance of third of the list because
            //first of the list will be destroyed. also from second of the
            //list, player can see possible destroyed object, which is why
            //i chose to delete third of the list
            GameObject seal = listRooms[2].transform.GetChild(0).gameObject; //get EnterBox
            seal.GetComponent<BoxCollider>().isTrigger = false; //remove triggreable-lity
            //since trigger is false, this is basically a wall.
            seal.SetActive(true); //reactivate it
        }

        //destroy further than 2 rooms away
        if (listRooms.Count > maxRoom)
        {
            GameObject room = listRooms[0];
            listRooms.RemoveAt(0);
            Destroy(room);
        }

        //add score and display it
        score += 1;
        if (score > 99999)
        {
            scoreAmount.text = "+99999";
            scoreResult.text = "Room +99999";
            scoreCurrently.text = "Room +99999";
        }
        else
        {
            scoreAmount.text = score.ToString();
            scoreResult.text = "Room " + score.ToString();
            scoreCurrently.text = "Room " + score.ToString();
        }
    }
}
