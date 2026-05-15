using UnityEngine;
using UnityEngine.SceneManagement;
using HighScore;

//ToDo make this not an animation so that it can have more text added to it
public class EndscreenTextHandler : MonoBehaviour
{
    private Animator animator;
    public GameObject Input;
    public GameObject FadeScreen;

    public string name;
    public int score;


    void Start()
    {
        animator = GetComponent<Animator>();
        HS.Init(this, "Thick as Thieves");
    }

    public void SetAdvanceFalse()
    {
        animator.SetBool("MoveReady", false);
    }

    public void SetAdvanceTrue()
    {
        animator.SetBool("MoveReady", true);
    }

    public void StartFadeOut()
    {
        FadeScreen.GetComponent<FadeScreen>().StartFadeOut();
    }

    public void ActivateInput()
    {
        Input.GetComponent<NameInput>().inputActive = true;
        Input.GetComponent<NameInput>().FocusInput();
    }

    public void SubmitScore()
    {
        name = Input.GetComponent<NameInput>().playerName;
        if (MoneySystem.Instance)
        {
            score = (int)MoneySystem.Instance.TotalMoney;
        }
        //Submit Score to server here
        HS.SubmitHighScore(this, name, score);



        
    }

    void Update()
    {
        if (Input.GetComponent<NameInput>().inputActive)
        {
            if (Input.GetComponent<NameInput>().nameFinalized)
            {
                SubmitScore();
                SetAdvanceTrue();
                Input.GetComponent<NameInput>().inputActive = false;
            }
        }

        if (FadeScreen.GetComponent<FadeScreen>().SceneSwitchReady)
        {
            Debug.Log("Scene Switch");
            FadeScreen.GetComponent<FadeScreen>().SceneSwitchReady = false;
            if (GameLevelManager.Instance)
                Destroy(GameLevelManager.Instance.gameObject);

            if (PlayerManager.Instance)
                Destroy(PlayerManager.Instance.gameObject);

            if (MoneySystem.Instance)
                Destroy(MoneySystem.Instance.gameObject);

            if (EscapeTimer.Instance)
                Destroy(EscapeTimer.Instance.gameObject);

            SceneManager.LoadScene(0);
        }
    }
}