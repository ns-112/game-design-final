using System.Collections;
using UnityEngine;

public class WallDetection : MonoBehaviour
{
    private Player parentPlayer;
    public SideType type = SideType.Left;

    IEnumerator Start()
    {
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
        switch (type)
        {
            case SideType.Left:
            parentPlayer.wallLeft = true;
            break;

            case SideType.Right:
            parentPlayer.wallRight = true;
            break;
        }
        
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        switch (type)
        {
            case SideType.Left:
            parentPlayer.wallLeft = false;
            break;

            case SideType.Right:
            parentPlayer.wallRight = false;
            break;
        }
    }
}
