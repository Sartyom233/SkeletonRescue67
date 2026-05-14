using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxEffect = 0.5f;
    private Material mat;
    private float lastCameraX;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        
        if (cameraTransform != null)
        {
            lastCameraX = cameraTransform.position.x;
        }
    }

    void Update()
    {
        if (cameraTransform == null) return;

        float deltaX = cameraTransform.position.x - lastCameraX;

        Vector2 offset = mat.mainTextureOffset;
        offset.x += deltaX * parallaxEffect * 0.1f;
        mat.mainTextureOffset = offset;

        lastCameraX = cameraTransform.position.x;
    }
}