using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenButtons : MonoBehaviour
{
  public void PlayAgain()
  {
    SceneManager.LoadScene("MenuScene");
  }

  public void Quit()
  {
    Application.Quit();
  }
}