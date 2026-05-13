using TMPro;
using UnityEngine;

public class LabelScript : MonoBehaviour
{
    private Animator animator;
    private TextMeshProUGUI textMesh;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void StartLabel(string label, Vector2 position)
    {
        textMesh.text = label;
        animator.SetBool("showLabel", true);
    }

    public void KillLabel()
    {
        animator.SetBool("showLabel", false);
    }

    public void OnLabelExitComplete()
    {
        GameLevelManager.Instance.ActiveLabels.Remove(transform.parent.gameObject);
        Destroy(gameObject);
    }

    public void OnLabelEnterComplete()
    {
        GameLevelManager.Instance.newLabelReady = true;
        
    }
}
