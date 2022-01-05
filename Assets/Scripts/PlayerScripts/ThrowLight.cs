using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowLight : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private GameObject lightSource;

    private Transform playerTrasnform;

    private void Start()
    {
        playerTrasnform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(lightSource, playerTrasnform.position, playerTrasnform.rotation).GetComponent<LightScript>().AddForce(80);
        }
    }
}
