using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerType
{
    Player1,
    Player2
}

public enum SideType
{
    Left,
    Right
}

public class Player
{
    public PlayerType playerType;
    public GameObject gameObject;
    public bool canJump = true;
    public bool canDoubleJump = true;
    public bool characterActive = true;

    public bool wallLeft = false;
    public bool wallRight = false;

    public Player(PlayerType type)
    {
        playerType = type;
    }
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public Dictionary<PlayerType, Player> Players = new Dictionary<PlayerType, Player>();
    public PlayerType ActivePlayer = PlayerType.Player1;



    public InputActionAsset inputActions;

    public InputAction left_right;
    public InputAction jump;
    public InputAction crouch;
    public InputAction interact;
    public InputAction switch_player;


    public Camera GameCamera;

    


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            var actionMap = inputActions.FindActionMap("Player1");
            left_right = actionMap.FindAction("LeftRight");
            jump = actionMap.FindAction("Jump");
            crouch = actionMap.FindAction("Crouch");
            interact = actionMap.FindAction("Interact");
            switch_player = actionMap.FindAction("SwitchCharacter");

            actionMap.Enable();
        }
    }

    void SwapPlayers()
    {
        if (ActivePlayer == PlayerType.Player1)
        {
            ActivePlayer = PlayerType.Player2;
        } else
        {
            ActivePlayer = PlayerType.Player1;
        }
    }
    

    void Update()
    {
        if (switch_player.WasPressedThisFrame())
        {
            SwapPlayers();
        }
    }
}