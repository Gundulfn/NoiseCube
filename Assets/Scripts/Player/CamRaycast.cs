using UnityEngine;

public class CamRaycast : MonoBehaviour
{
    private RaycastHit hit;
    public bool isHit;

    private float size = 1f;

    private float rayDistance = 5;
    public Vector3 hitFacePos;
    public Vector3 hitFacePos2;
    public Vector3 hitNormal;
    public Vector3 hitNormal2;

    public GameObject currentHit;

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
        {
            isHit = true;
            currentHit = hit.collider.gameObject;
            hitFacePos = hit.point;
            hitFacePos2 = hit.point;

            int x = Mathf.FloorToInt((hit.point.x + .4f)/ size);
            int y = Mathf.FloorToInt((hit.point.y + .4f)/ size);
            int z = Mathf.FloorToInt((hit.point.z + .4f)/ size);

            hitFacePos.x = x;
            hitFacePos.y = y;
            hitFacePos.z = z;

            hitNormal = hit.normal;
            hitNormal2 = hit.normal;

            if (Mathf.Abs(hit.normal.x) > Mathf.Abs(hit.normal.y))
            {
                if (Mathf.Abs(hit.normal.x) > Mathf.Abs(hit.normal.z))
                {
                    // X
                    hitNormal = new Vector3(hitNormal.x < 0 ? -1 : 1, 0, 0);
                }
                else
                {
                    // Z
                    hitNormal = new Vector3(0, 0, hitNormal.z < 0 ? -1 : 1);
                }
            }
            else if (Mathf.Abs(hit.normal.y) > Mathf.Abs(hit.normal.z))
            {
                // Y
                hitNormal = new Vector3(0, hitNormal.y == 1 ? 1 : hitNormal.y == - 1 ? -1 : 0, 0);
            }
            else
            {
                // Z 
                hitNormal = new Vector3(0, 0, hitNormal.z < 0 ? -1 : 1);
            }
        }
        else
        {
            isHit = false;
        }
    }

    public void DebugHitInfo()
    {
        //Debug.Log("HitPos: " + hit.transform.InverseTransformPoint(hit.point));
        Debug.Log("HitPos: " + hitFacePos);
        Debug.Log("HitNormal: " + hitNormal);
    }
}
