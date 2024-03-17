using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class camera_thid_person : MonoBehaviour
{
    // public
    private Transform player;
    public float distance = 5.0f;
    public float height = 2.0f;
    public float followSpeed = 15.0f;
    // private
    private Vector3 offset;

    void Start()
    {
        offset = new Vector3(0, height, -distance);
    }

    void Update()
    {
        if (player == null) {
            FindPlayer();
            return;
        }
        Vector3 desiredOffset = player.rotation * offset;
        Vector3 desiredPosition = player.position + desiredOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.LookAt(player);
    }

    void FindPlayer()
    {
        try {
            GameObject playerGameObject = GameObject.FindWithTag("PlayerMain");
            if (playerGameObject != null) {
                player = playerGameObject.transform;
                Debug.Log("Camera found player.");
            } else {
                Debug.Log("Warning: camera can't find player.");
            }
        } catch (Exception ex) {
            Debug.Log("Searching for PlayerMain in scene...");
        }
    }
}
