using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField]
    public int ActivateAmount = 1;
    [SerializeField]
    public int DeactivateAmount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ActivateAmount == 0) return;
        if (!CanTrigger(collision)) return;

        ActivateTrigger();
        if (ActivateAmount > 0) ActivateAmount--;
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (DeactivateAmount == 0) return;
        if (!CanTrigger(collision)) return;

        DeactivateTrigger();
        if (DeactivateAmount > 0) DeactivateAmount--;
    }

    protected virtual bool CanTrigger(Collider2D collision)
    {
        return true;
    }

    public virtual void ActivateTrigger()
    {
        Debug.LogWarning("ActivateTrigger() method not implemented!");
    }

    public virtual void DeactivateTrigger()
    {
        Debug.LogWarning("DeactivateTrigger() method not implemented!");
    }
}

public class PlayerTrigger : Trigger
{
    protected override bool CanTrigger(Collider2D collision)
    {
        return collision.CompareTag("Player");
    }

}