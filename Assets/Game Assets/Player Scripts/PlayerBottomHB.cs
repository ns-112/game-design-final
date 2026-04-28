using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBottomHB : MonoBehaviour
{
    private Player parentPlayer;

    void Start()
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), transform.parent.GetComponent<Collider2D>());
        parentPlayer = PlayerManager.Instance.Players[transform.parent.GetComponent<BasicPlayer>().playerType];
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