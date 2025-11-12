using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerInput playerInput;

    enum GameState
    {
        MainMenu,
        FreeMove,
        InDialogue,
        Paused,
        GameOver
    }

    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
