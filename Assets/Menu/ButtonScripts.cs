using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScripts : MonoBehaviour
{
    public void OnPlay()
    {
        //Switch with a quick intro first
        Debug.Log("PlayPressed");
        SceneManager.LoadScene(1);
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
