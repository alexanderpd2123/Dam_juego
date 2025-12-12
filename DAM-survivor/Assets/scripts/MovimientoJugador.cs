using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    ///////////////////////////////////// VARIABLES /////////////////////////////////
    private bool puedeMoverse = true; // Bandera para habilitar/deshabilitar el movimiento
    private float velocidadMovimiento = 5f; // Velocidad lineal de desplazamiento
    private float velocidadRotacion = 720f; // Velocidad angular de rotación (rápida)
    private Vector2 direccionPlana; // Vector 2D (X, Y) leído del Input

    public Controles control; // Instancia del Input Action Asset

    ///////////////////////////////////// FUNCIONES UNITY /////////////////////////////////
    
    private void Awake()
    {
        control = new Controles(); // Inicializa la clase que maneja las acciones de Input
    }

    private void OnEnable()
    {
        control.Enable(); // Activa el mapa de acciones de Input
    }
    
    private void OnDisable()
    {
        control.Disable(); // Desactiva el mapa de acciones de Input
    }

    void Start()
    {
        // Se puede añadir inicialización aquí
    }

    // Lógica de movimiento y rotación por frame
    void Update()
    {
       if (puedeMoverse) // Solo se ejecuta si el movimiento está permitido
        {
            // Coge el valor de la entrada 2D (joystick, WASD, flechas)
            direccionPlana = control.Player.move.ReadValue<Vector2>();
            
            // Convierte el Vector2 (X, Y) en un Vector3 (X, Z) para el movimiento en el plano horizontal 3D
            Vector3 direccionMovimiento = new Vector3(direccionPlana.x, 0f, direccionPlana.y);
            
            // --- Lógica de Rotación ---
            
            // Solo rota si hay una entrada de movimiento perceptible
            if (direccionMovimiento.magnitude > 0.1f)
            {
                // Normaliza la dirección para asegurar velocidad constante
                direccionMovimiento.Normalize();

                // Calcula la rotación (Quaternion) necesaria para mirar en la dirección de movimiento
                Quaternion rotacionDeseada = Quaternion.LookRotation(direccionMovimiento, Vector3.up);

                // Rota el personaje suavemente hacia la dirección deseada (Interpolación)
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, 
                    rotacionDeseada, 
                    velocidadRotacion * Time.deltaTime
                );
            }
            
            // --- Lógica de Movimiento ---
            
            // Mueve el personaje usando la dirección calculada (ya normalizada si hubo input)
            transform.position += direccionMovimiento * velocidadMovimiento * Time.deltaTime;

        }
    }
    
}