using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    public GameObject panel;
    void Start()
    {
        Time.timeScale = 1f;
    }

    public void Load(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
    }
    public void HidePanel()
    {
        panel.SetActive(false);
    }
}
