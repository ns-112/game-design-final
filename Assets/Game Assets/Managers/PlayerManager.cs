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

public struct PlayerProperties
{
    public float moveSpeed;
    public float jumpHeight;
    public float weight;

    public bool canDoubleJump;
    public bool canWallClimb;

    public float strength;
}

public struct LevelOptions
{
    public bool P1Locked;
    public bool P2Locked;
    
}

public class Player
{
    public PlayerType playerType;
    public PlayerProperties properties;
    public GameObject gameObject;
    
    public bool canJump = true;
    public bool canWC = true;
    public bool canDoubleJump = true;
    public bool characterActive = true;

    public bool wallLeft = false;
    public bool wallRight = false;

    public ItemPickup heldItem = null;

    public Player(PlayerType type)
    {
        playerType = type;
        switch (type)
        {
            //Heavy
            case PlayerType.Player1:
            properties.canDoubleJump = false;
            properties.canWallClimb = true;

            properties.moveSpeed = 5.5f;
            properties.jumpHeight = 4.25f;
            properties.weight = 0.7f;
            properties.strength = 5.0f;
            break;

            //Light
            case PlayerType.Player2:
            properties.canDoubleJump = false;
            properties.canWallClimb = false;
            properties.moveSpeed = 9.0f;
            properties.jumpHeight = 2.2f;
            properties.weight = 0.3f;
            properties.strength = 1.0f;
            break;
        }
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
    public LevelOptions levelOptions = new LevelOptions
    {
        P1Locked = false,
        P2Locked = false
    };
    


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