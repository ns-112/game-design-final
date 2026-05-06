using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMidSection : MonoBehaviour
{
    private Player parentPlayer;

    IEnumerator Start()
    {
        foreach (Transform child in transform.parent)
        {
            Physics2D.IgnoreCollision(
                GetComponent<Collider2D>(),
                child.GetComponent<Collider2D>()
            );
        }
        Physics2D.IgnoreCollision(
            GetComponent<Collider2D>(),
            transform.parent.GetComponent<Collider2D>()
        );
        
        

        var type = transform.parent.GetComponent<BasicPlayer>().playerType;

        while (!PlayerManager.Instance.Players.TryGetValue(type, out parentPlayer))
            yield return null;
        
        parentPlayer.canWC = false;

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        parentPlayer.canWC = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        parentPlayer.canWC = false;
    }
}
