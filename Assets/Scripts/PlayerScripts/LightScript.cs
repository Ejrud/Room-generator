using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    public void AddForce(int force)
    {
        GetComponent<Rigidbody2D>().AddForce(transform.up * force);
    }
}
