using System.Collections.Generic;
using UnityEngine;



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
