using System.Collections;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
  // range of the melee attack
  public float attackRange = 1.5f;

  // cooldown between attacks in seconds
  public float attackCooldown = 1f;

  private bool canAttack = true;
  private Player self;

  IEnumerator Start()
  {
    var bp = GetComponent<BasicPlayer>();
    while (!PlayerManager.Instance.Players.TryGetValue(bp.playerType, out self))
      yield return null;
  }

  void Update()
  {
    // only Player1/heavy can melee attack
    if (PlayerManager.Instance.ActivePlayer != PlayerType.Player1) return;

    if (PlayerManager.Instance.interact.WasPressedThisFrame() && canAttack)
    {
      // only attack if not holding an item
      if (self.heldItem == null)
        StartCoroutine(Attack());
    }
  }

  // checks for an enemy within attack range and knocks it out
  IEnumerator Attack()
  {
    canAttack = false;

    // find all colliders within attack range
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

    foreach (Collider2D hit in hits)
    {
      GuardDog dog = hit.GetComponent<GuardDog>();
      if (dog != null)
        dog.KnockOut();

      SecurityGuard guard = hit.GetComponent<SecurityGuard>();
      if (guard != null)
        guard.KnockOut();
    }

    yield return new WaitForSeconds(attackCooldown);
    canAttack = true;
  }

}