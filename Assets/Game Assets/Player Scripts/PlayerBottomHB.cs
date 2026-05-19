using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBottomHB : MonoBehaviour
{
    private Player parentPlayer;

    IEnumerator Start()
    {
        foreach (Transform child in transform.parent)
        {
            Collider2D childCol = child.GetComponent<Collider2D>();
            if (childCol != null)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), childCol);
            }
        }
    Physics2D.IgnoreCollision(
            GetComponent<Collider2D>(),
            transform.parent.GetComponent<Collider2D>()
        );

        var type = transform.parent.GetComponent<BasicPlayer>().playerType;

        while (!PlayerManager.Instance.Players.TryGetValue(type, out parentPlayer))
            yield return null;

    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        parentPlayer.canJump = true;
        parentPlayer.canDoubleJump = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        parentPlayer.canJump = false;
    }
}