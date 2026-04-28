using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerType
{
    Player1,
    Player2
}

public class Player
{
    public PlayerType playerType;
    public GameObject gameObject;
    public bool canJump = true;
    public bool canDoubleJump = true;
    public bool characterActive = true;

    public Player(PlayerType type)
    {
        playerType = type;
    }
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public Dictionary<PlayerType, Player> Players = new Dictionary<PlayerType, Player>();

    


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
        }
    }

    void Update()
    {
        if (Players[PlayerType.Player1] != null)
        {
            Debug.Log(string.Format("player 1: can jump: {0} can djump: {1}", Players[PlayerType.Player1].canJump, Players[PlayerType.Player1].canDoubleJump));
        }
        /*
        if (Players[PlayerType.Player2] != null)
        {
            Debug.Log(string.Format("player 2: can jump: {0} can djump: {1}", Players[PlayerType.Player2].canJump, Players[PlayerType.Player2].canDoubleJump));
        }
        */
    }
}