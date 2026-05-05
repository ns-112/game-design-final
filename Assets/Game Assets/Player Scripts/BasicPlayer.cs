using UnityEngine;
using UnityEngine.InputSystem;

public class BasicPlayer : MonoBehaviour
{

    private Rigidbody2D rb;
    private float moveSpeed = 8f;

    public PlayerType playerType;
    private Player self;

    




    //Awake called before children added i think, sorta unreliable
    void Awake()
    {
        

        rb = GetComponent<Rigidbody2D>();

        self = new(playerType)
        {
            gameObject = gameObject
        };
        
        
    }

    //Called on the first frame
    void Start()
    {
        PlayerManager.Instance.Players[playerType] = self;
        rb.mass = self.properties.weight / 2;
    }

    void OnJump()
    {
      if (self.canJump)
      {
        self.canJump = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * self.properties.jumpHeight, ForceMode2D.Impulse);
      }
      // wall jump only for Player2, must be touching a wall and not on the ground
      else if (self.properties.canWallClimb && !self.canJump)
      {
        if (self.wallLeft)
        {
          rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
          rb.AddForce(new Vector2(self.properties.moveSpeed, self.properties.jumpHeight), ForceMode2D.Impulse);
        }
        else if (self.wallRight)
        {
          rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
          rb.AddForce(new Vector2(-self.properties.moveSpeed, self.properties.jumpHeight), ForceMode2D.Impulse);
        }
      }
    }

  void FixedUpdate()
    {
        if (PlayerManager.Instance.ActivePlayer == self.playerType)
        {
            float x = PlayerManager.Instance.left_right.ReadValue<float>();
            float d = PlayerManager.Instance.crouch.ReadValue<float>();
            float i = PlayerManager.Instance.interact.ReadValue<float>();
            float s = PlayerManager.Instance.switch_player.ReadValue<float>();
            //Check if on ground before allowing a jump


            if (self.wallLeft && x < 0)
            {
                x = 0;
            }
            if (self.wallRight && x > 0)
            {
                x = 0;
            }

            Vector2 move = new Vector2(x, 0);
            rb.linearVelocity = new(move.x * self.properties.moveSpeed, rb.linearVelocity.y);
        }
        
    }

    void Update()
    {
        if (PlayerManager.Instance.ActivePlayer == self.playerType)
        {
            if (PlayerManager.Instance.jump.WasPressedThisFrame())
            {
                OnJump();
            } 
        }
       
    }
}
