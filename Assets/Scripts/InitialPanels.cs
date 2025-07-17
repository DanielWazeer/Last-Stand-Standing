using System.Collections;
using UnityEngine;

public class InitialPanels : MonoBehaviour
{
    public GameObject[] toTurnOff;
    public GameObject[] toTurnOn;

    IEnumerator FixPanels()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < toTurnOff.Length; i++)
        {
            toTurnOff[i].SetActive(false);
        }

        for (int i = 0; i < toTurnOn.Length; i++)
        {
            toTurnOn[i].SetActive(true);
        }
    }

    void Start()
    {
        StartCoroutine(FixPanels());
    }

}
