using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    public GameObject panel;
    public GameObject how;
    public TMP_Text text;
    public TMP_Text howtext;
    void Start()
    {
        Time.timeScale = 1f;
        panel.SetActive(false);
        text.text = "Show Credits";
        how.SetActive(false);
        howtext.text = "Show how to play";
    }

    public void LoadLemonade()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadBurger()
    {
        SceneManager.LoadScene(2);
    }
    public void LoadCandy()
    {
        SceneManager.LoadScene(3);
    }
    public void LoadNight()
    {
        SceneManager.LoadScene(5);
    }
    public void LoadBalance()
    {
        SceneManager.LoadScene(4);
    }

    public void ShowCredits()
    {
        if(text.text == "Hide Credits")
        {
            HideCredits();
        }
        else
        {
            text.text = "Hide Credits";
            panel.SetActive(true);
        }
    }
    public void HideCredits()
    {
        text.text = "Show Credits";
        panel.SetActive(false);
    }

    public void ShowHow()
    {
        if (howtext.text == "Hide how to play")
        {
            HideHow();
        }
        else
        {
            howtext.text = "Hide how to play";
            how.SetActive(true);
        }
    }
    public void HideHow()
    {
        howtext.text = "Show how to play";
        how.SetActive(false);
    }
}
