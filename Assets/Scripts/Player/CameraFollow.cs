using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The target for the camera to follow
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.transform.position = new Vector3(target.position.x, target.position.y, -7.5f);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(target.position.x, target.position.y, -7.5f);
    }
}
