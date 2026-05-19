using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
  public void Play()
  {
    SceneManager.LoadScene("GameScene");
  }

  public void Quit()
  {
    Application.Quit();
  }
}