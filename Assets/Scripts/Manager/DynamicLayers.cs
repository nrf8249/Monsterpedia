using UnityEngine;

using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicLayers : MonoBehaviour
{
    private TilemapRenderer tilemapRenderer;
    private SpriteRenderer playerRenderer;
    private Transform playerTransform;
    public GameObject player;

    void Start()
    {
        // Get this object's renderer
        tilemapRenderer = GetComponent<TilemapRenderer>();

        // Get player references
        player = GameObject.FindGameObjectWithTag("Player");
        playerRenderer = player.GetComponent<SpriteRenderer>();
        playerTransform = player.transform;
    }

    void Update()
    {
        playerTransform = player.transform;
        // If player is above this object, draw this behind the player
        if (playerTransform.position.y > transform.position.y)
        {
            tilemapRenderer.sortingOrder = playerRenderer.sortingOrder - 1;
        }
        else
        {
            tilemapRenderer.sortingOrder = playerRenderer.sortingOrder + 1;
        }
    }
}