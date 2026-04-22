using UnityEngine;
using UnityEngine.InputSystem;

public class BasicMovement : MonoBehaviour
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

        //animator = transform.GetChild(1).GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        float x = left_right.ReadValue<float>();
        float j = jump.ReadValue<float>();
        float d = crouch.ReadValue<float>();
        float i = interact.ReadValue<float>();
        float s = switch_player.ReadValue<float>();


        Vector2 move = new Vector2(x, 0);
        rb.linearVelocity = new(move.x * moveSpeed, rb.linearVelocityY);
    }
}
