using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private CamRaycast camRaycast;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        camRaycast = playerMovement._camera.GetComponent<CamRaycast>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * x + transform.forward * z;

        playerMovement.Move(movement * playerMovement.currentSpeed * Time.deltaTime);

        bool jump = Input.GetButtonDown("Jump");

        if (jump)
        {
            playerMovement.Jump();
        }

        if(Input.GetMouseButtonDown(0) && camRaycast.isHit)
        {

            GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), camRaycast.hitFacePos + camRaycast.hitNormal, Quaternion.identity);
            
            Debug.Log(camRaycast.hitFacePos2);
            Debug.Log(camRaycast.hitNormal2);
                        
            camRaycast.DebugHitInfo();
        }
    }
}