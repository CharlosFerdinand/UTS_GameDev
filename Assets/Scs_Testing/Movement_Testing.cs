using UnityEngine;

public class Movement_Testing : MonoBehaviour
{
    public string ground_tag = "groundTag";

    //state of the body
    private string action = "mobile";
    private Vector3 startpos;
    private float flinch_time = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //knee level ray
        Ray ray = new Ray(this.transform.position, Vector3.down);
        RaycastHit hit;

        //feet level ray
        Ray ray2 = new Ray(this.transform.position - new Vector3(0,0.25f,0), Vector3.down);
        RaycastHit hit2;

        Physics.Raycast(ray, out hit,0.25f);
        Physics.Raycast(ray2, out hit2, 0.25f);
        
        //logics
        if(hit.transform.tag == ground_tag || hit2.transform.tag == ground_tag)
        {
            //ground detected
            
        }
    }

    
}
