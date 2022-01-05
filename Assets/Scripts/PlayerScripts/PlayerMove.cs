using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMove : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private Transform cameraTransform;

    [Header("Settings")]
    [SerializeField] private float _speed = 0.5f;

    private Transform playerTransform;
    private Rigidbody2D rbComponent2D;

    private float _horizontal, _vertical;

    private void Start() 
    {
        playerTransform = GetComponent<Transform>();
        rbComponent2D = GetComponent<Rigidbody2D>();
    }

    private void Update() 
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical  = Input.GetAxis("Vertical");

        cameraTransform.position = playerTransform.position - new Vector3(0, 0, 10);
    }

    private void FixedUpdate() 
    {
        
        
        rbComponent2D.velocity = new Vector3(_horizontal, _vertical, 0) * _speed;
    }
}
