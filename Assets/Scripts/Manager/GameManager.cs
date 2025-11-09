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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
