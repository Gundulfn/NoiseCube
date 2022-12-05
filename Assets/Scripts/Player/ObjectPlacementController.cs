using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacementController : MonoBehaviour
{
    private CamRaycast camRaycast;
    private KeyCode hotKey = KeyCode.E;
    private GameObject currentPlaceableObject;

    private float mouseWheelPosition;

    void Start()
    {
        
    }

    void Update()
    {
        HandleHotKeyPressed();

        if(currentPlaceableObject != null)
        {

        }
    }

    private void HandleHotKeyPressed()
    {
        if(Input.GetKeyDown(hotKey))
        {
            if(currentPlaceableObject != null)
            {
                currentPlaceableObject = null;
            }
            else
            {
                currentPlaceableObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
        }
    }
}
