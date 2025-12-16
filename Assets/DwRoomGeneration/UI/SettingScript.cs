using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingScript : MonoBehaviour
{
    public TMP_Text fovTitle;
    public void fovSlider(Slider slider)
    {
        float fov = slider.value;
        if (fovTitle != null)
        {
            fovTitle.text = "FOV(" + ((int)fov).ToString() + "):";
        }
        Camera.main.fieldOfView = fov;
        DwGameManager.gameManager.GetComponent<DwGameManager>().fov = fov;
    }
}
