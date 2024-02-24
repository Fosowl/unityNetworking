using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_thid_person : MonoBehaviour
{
    public Transform player; // Assign your player's transform in the inspector
    public float distance = 5.0f; // Distance behind the player
    public float height = 2.0f; // Height above the player
    public float followSpeed = 10.0f; // How fast the camera catches up to the target position

    private Vector3 offset;

    void Start()
    {
        offset = new Vector3(0, height, -distance);
    }

    void LateUpdate()
    {
        Vector3 desiredOffset = player.rotation * offset;
        Vector3 desiredPosition = player.position + desiredOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.LookAt(player);
    }
}
