using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class followcamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject player;
    private Vector3 offset;
    private Controles controles;
    private float zoomMin = 0.5f;
    private float zoom = 1f;
    private float zoomMax = 2f;

    private float zoomSpeed = 10f;

    private void Awake()
    {
        controles = new Controles();
        
    }
    private void OnEnable()
    {
        controles.Enable();
    }
    private void OnDisable()
    {
        controles.Disable();
    }
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float scrollValue = controles.camera.zoom.ReadValue<float>();
        zoom -= scrollValue / zoomSpeed;
        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
        Vector3 zoomFinal = offset * zoom;

        transform.position = player.transform.position + zoomFinal;
    }
 
}
