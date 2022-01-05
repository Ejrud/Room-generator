using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private float _offset = 90f;
    private Transform playerTransform;

    private void Start() 
    {
        playerTransform = GetComponent<Transform>(); 
    }

    private void Update() 
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - playerTransform.position;

        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        playerTransform.localEulerAngles = new Vector3(0, 0, rotationZ + _offset);     
    }
}
