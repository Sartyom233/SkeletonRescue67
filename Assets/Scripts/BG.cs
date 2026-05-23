using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [Header("Привязка")]
    public Transform mainCamera;

    [Header("Настройки")]
    public float scrollSpeed = 0.5f;

    private Material mat;
    private float lastCameraX;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        
        if (mainCamera == null) 
        {
            mainCamera = Camera.main.transform;
        }
            
        lastCameraX = mainCamera.position.x;
    }

    void LateUpdate()
    {
        float deltaX = mainCamera.position.x - lastCameraX;
        
        transform.position = new Vector3(mainCamera.position.x, transform.position.y, transform.position.z);
        
        mat.mainTextureOffset += new Vector2(deltaX * scrollSpeed * 0.05f, 0);
        
        lastCameraX = mainCamera.position.x;
    }
}