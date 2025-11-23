using UnityEngine;

public class BloodEffectScript : MonoBehaviour
{
    //call to destoy this game object within certain second
    private void Start()
    {
        Invoke("life", GetComponent<ParticleSystem>().main.duration);
    }

    private void life()
    {
        Destroy(gameObject);
    }
}
