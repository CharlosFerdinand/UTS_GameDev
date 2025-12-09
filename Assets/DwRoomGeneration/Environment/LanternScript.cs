using UnityEngine;

public class LanternScript : MonoBehaviour
{
    [SerializeField] private AudioClip soundLantern;
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = soundLantern;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "hero")
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "hero")
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
