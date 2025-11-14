using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicLayers : MonoBehaviour
{
    private SpriteRenderer propRenderer;
    private SpriteRenderer playerRenderer;
    private Transform playerTransform;
    public GameObject player;

    void Start()
    {
        // Get this object's renderer
        propRenderer = GetComponent<SpriteRenderer>();

        // Get player references
        player = GameObject.FindGameObjectWithTag("Player");
        playerRenderer = player.GetComponent<SpriteRenderer>();
        playerTransform = player.transform;
    }

    void Update()
    {
        playerTransform = player.transform;
        // If player is above this object, draw this behind the player
        if (playerTransform.position.y - 0.25 > transform.position.y)
        {
            propRenderer.sortingOrder = playerRenderer.sortingOrder + 1;
        }
        else
        {
            propRenderer.sortingOrder = playerRenderer.sortingOrder - 1;
        }
    }
}