using UnityEngine;
using UnityEngine.InputSystem;

public class BasicPlayer : MonoBehaviour
{

    private Rigidbody2D rb;
    //private float moveSpeed = 8f;

    public PlayerType playerType;
    private Player self => PlayerManager.Instance.Players[playerType];

    private Vector3 startPos;

    public bool debug;
    




    //Awake called before children added i think, sorta unreliable
    void Awake()
    {
        
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();

        
        
        
    }

    //Called on the first frame
    void Start()
    {
        PlayerManager.Instance.Players[playerType] = self;
        rb.mass = self.properties.weight / 2;

        PlayerManager.Instance.Players[playerType].gameObject = gameObject;
    }

    void OnJump()
    {
        if (self.canJump)
        {
            self.canJump = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * self.properties.jumpHeight, ForceMode2D.Impulse);
        }
        // wall jump only triggers if in the air AND touching a wall
        else if (self.properties.canWallClimb && !self.canJump && !self.canDoubleJump)
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
            Vector2 vel = rb.linearVelocity;
            vel.x = move.x * self.properties.moveSpeed;
            rb.linearVelocity = vel;
        }
        
    }

    void Update()
    {
        debug = self.characterActive;
        if (PlayerManager.Instance.ActivePlayer == self.playerType)
        {
            if (PlayerManager.Instance.jump.IsPressed())
            {
                OnJump();
            } 
        }

        if (transform.position.y < -30)
        {
            transform.position = startPos;
        }

       
    }
}
