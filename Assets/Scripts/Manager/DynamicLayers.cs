using UnityEngine;

using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicLayers : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer playerRenderer;
    private Transform playerTransform;
    public GameObject player;

    void Start()
    {
        // Get this object's renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Get player references
        player = GameObject.FindGameObjectWithTag("Player");
        playerRenderer = player.GetComponent<SpriteRenderer>();
        playerTransform = player.transform;
    }

    void Update()
    {
        playerTransform = player.transform;
        // If player is above this object, draw this behind the player
        if (playerTransform.position.y - 0.5 < transform.position.y)
        {
            spriteRenderer.sortingOrder = playerRenderer.sortingOrder - 5;
        }
        else
        {
            spriteRenderer.sortingOrder = playerRenderer.sortingOrder + 5;
        }
    }
}