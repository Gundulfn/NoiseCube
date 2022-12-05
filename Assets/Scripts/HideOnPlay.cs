using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false);
    }
}
