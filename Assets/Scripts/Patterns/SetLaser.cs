using UnityEngine;

public class SetLaser : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform firePoint; 
    public float maxLaserDistance = 50f;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, maxLaserDistance))
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + firePoint.forward * maxLaserDistance);
        }
    }
}
