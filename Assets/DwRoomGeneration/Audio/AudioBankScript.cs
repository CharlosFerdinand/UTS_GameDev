using System.Collections.Generic;
using UnityEngine;


//add this for key
public enum Audio
{
    Dash,
    Spiketrap,
    Walk,
    Run,
    Lantern
}


public class AudioBankScript : MonoBehaviour
{
    [Header("Get Audio")]
    [SerializeField] AudioClip dashSound;


    //static
    public static AudioClip dash;

    private void Start()
    {
        dash = dashSound;
    }
}
