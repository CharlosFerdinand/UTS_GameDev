using UnityEngine;
using UnityEngine.SceneManagement;

public class DwMainMenuButtonHandlerScript : MonoBehaviour
{
    //attributes
    [SerializeField] private GameObject manual;
    [SerializeField] private GameObject option;
    [SerializeField] private GameObject credits;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {

    }


    //Core ============================================================================
    public void NavigatePlayBtn()
    {
        SceneManager.LoadScene("DwScene");
    }

    public void NavigateShopBtn()
    {
        SceneManager.LoadScene("DwShopScene");
    }

    public void NavigateExitBtn()
    {
        Application.Quit();
    }

    //Manual ==========================================================================
    public void OpenManualBtn()
    {
        //activate included element
        manual.SetActive(true);
        manual.transform.Find("BackGround").gameObject.SetActive(true);

        iTween.MoveFrom(
            manual.transform.Find("ManualUi").gameObject,
            iTween.Hash(
                "y", 1000,
                "easeType", iTween.EaseType.easeOutExpo,
                "time", 1f
                )
            );
    }

    public void CloseManualBtn()
    {
        manual.transform.Find("BackGround").gameObject.SetActive(false);
        Invoke("UnactivateManual", 1f);
        //unactivate manual after 2 second
        iTween.MoveTo(
            manual.transform.Find("ManualUi").gameObject,
            iTween.Hash(
                "y", 1000,
                "easeType", iTween.EaseType.easeOutExpo,
                "time", 1f
                )
            );
    }

    public void UnactivateManual()
    {
        manual.SetActive(false);
        manual.transform.Find("ManualUi").localPosition = Vector3.zero;
    }




    //Option ==========================================================================
    public void OpenOptionBtn()
    {
        //activate included element
        option.SetActive(true);
        option.transform.Find("BackGround").gameObject.SetActive(true);

        iTween.MoveFrom(
            option.transform.Find("OptionUi").gameObject,
            iTween.Hash(
                "y", -1000,
                "easeType", iTween.EaseType.easeOutExpo,
                "time", 1f
                )
            );
    }

    public void CloseOptionBtn()
    {
        option.transform.Find("BackGround").gameObject.SetActive(false);
        Invoke("UnactivateOption", 1f);
        //unactivate option after 2 second
        iTween.MoveTo(
            option.transform.Find("OptionUi").gameObject,
            iTween.Hash(
                "y", -1000,
                "easeType", iTween.EaseType.easeOutExpo,
                "time", 1f
                )
            );
    }

    public void UnactivateOption()
    {
        option.SetActive(false);
        option.transform.Find("OptionUi").localPosition = Vector3.zero;
    }




    //Credit ==========================================================================
    public void OpenCreditBtn()
    {
        //activate included element
        credits.SetActive(true);
        credits.transform.Find("BackGround").gameObject.SetActive(true);

        iTween.MoveFrom(
            credits.transform.Find("CreditsUi").gameObject,
            iTween.Hash(
                "x", -1000,
                "easeType", iTween.EaseType.easeOutExpo,
                "time", 1f
                )
            );
    }

    public void CloseCreditBtn()
    {
        credits.transform.Find("BackGround").gameObject.SetActive(false);
        Invoke("UnactivateCredit", 1f);
        //unactivate option after 2 second
        iTween.MoveTo(
            credits.transform.Find("CreditsUi").gameObject,
            iTween.Hash(
                "x", -1000,
                "easeType", iTween.EaseType.easeOutExpo,
                "time", 1f
                )
            );
    }

    public void UnactivateCredit()
    {
        credits.SetActive(false);
        credits.transform.Find("CreditsUi").localPosition = Vector3.zero;
    }




    //asdf
}
