using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    public bool SceneSwitchReady = false;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    public void OnFadeComplete()
    {
        SceneSwitchReady = true;
    }

    public void StartFadeOut()
    {
        animator.SetBool("Fade", true);
    }
}
