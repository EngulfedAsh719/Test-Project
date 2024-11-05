using UnityEngine;

public class FrustumOcclusion : MonoBehaviour
{
    private Camera mainCamera;
    private Renderer[] renderers;

    private Plane[] frustumPlanes;

    private void Start()
    {
        mainCamera = Camera.main;
        renderers = FindObjectsOfType<Renderer>();
    }

    private void Update()
    {
        Occlusion();
    }

    private void Occlusion()
    {
        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        foreach (Renderer renderer in renderers)
        {
            if (GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds))
            {
                renderer.enabled = true;
            }
            else
            {
                renderer.enabled = false;
            }
        }
    }
}
