using UnityEngine;
using UnityEngine.InputSystem;

public class BasicPlayer : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputActions;

    private InputAction left_right;
    private InputAction jump;
    private InputAction crouch;
    private InputAction interact;
    private InputAction switch_player;

    private Rigidbody2D rb;
    private float moveSpeed = 8f;

    public PlayerType playerType;
    private Player self;




    //Awake called before children added i think, sorta unreliable
    void Awake()
    {
        var actionMap = inputActions.FindActionMap("Player1");
        left_right = actionMap.FindAction("LeftRight");
        jump = actionMap.FindAction("Jump");
        crouch = actionMap.FindAction("Crouch");
        interact = actionMap.FindAction("Interact");
        switch_player = actionMap.FindAction("SwitchCharacter");

        actionMap.Enable();

        rb = GetComponent<Rigidbody2D>();

        self = new(playerType)
        {
            gameObject = gameObject
        };
        PlayerManager.Instance.Players[playerType] = self;

    }

    //Called on the first frame
    void Start()
    {
        
    }

    void OnJump()
    {
        if (self.canJump)
        {
            self.canJump = false;
            rb.AddForce(Vector2.up * 7f, ForceMode2D.Impulse);
        }   
        else if (self.canDoubleJump)
        {
            self.canDoubleJump = false;
            rb.AddForce(Vector2.up * 7f, ForceMode2D.Impulse);
        }
            
    }

    void FixedUpdate()
    {
        float x = left_right.ReadValue<float>();
        float d = crouch.ReadValue<float>();
        float i = interact.ReadValue<float>();
        float s = switch_player.ReadValue<float>();
        //Check if on ground before allowing a jump


        

        Vector2 move = new Vector2(x, 0);
        rb.linearVelocity = new(move.x * moveSpeed, rb.linearVelocity.y);
    }

    void Update()
    {
       if (jump.WasPressedThisFrame() && self.canDoubleJump)
        {
            OnJump();
        } 
    }
}
