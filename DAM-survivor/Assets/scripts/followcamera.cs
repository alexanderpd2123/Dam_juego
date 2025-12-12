using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class followcamera : MonoBehaviour
{
    // --- Variables de Referencia y Configuración ---
    public GameObject player; // Referencia al objeto del jugador que debe seguir
    private Vector3 offset; // Distancia y ángulo fijos iniciales entre la cámara y el jugador
    private Controles controles; // Instancia del asset de Input System de Unity
    
    // Límites y estado del zoom
    private float zoomMin = 0.5f; // Factor mínimo de zoom (más cerca del jugador)
    private float zoom = 1f; // Factor de zoom actual (1.0 = offset original)
    private float zoomMax = 2f; // Factor máximo de zoom (más lejos del jugador)

    private float zoomSpeed = 10f; // Factor de sensibilidad del zoom

    // --- Funciones de Gestión de Input ---
    private void Awake()
    {
        controles = new Controles(); // Inicializa el sistema de Input
        
    }
    private void OnEnable()
    {
        controles.Enable(); // Activa el mapa de acciones de Input
    }
    private void OnDisable()
    {
        controles.Disable(); // Desactiva el mapa de acciones de Input
    }
    
    void Start()
    {
        // Calcula el desplazamiento inicial (la posición relativa de la cámara)
        offset = transform.position - player.transform.position;
    }

    // LateUpdate se llama después de que todos los Update() se hayan ejecutado, asegurando que sigue al jugador después de su movimiento
    void LateUpdate()
    {
       
        float scrollValue = controles.camera.zoom.ReadValue<float>();
        
    
        zoom -= scrollValue / zoomSpeed;
        
       
        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
        

        Vector3 zoomFinal = offset * zoom;


        transform.position = player.transform.position + zoomFinal;
    }
 
}