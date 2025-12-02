using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    ///////////////////////////////////// VARIABLES /////////////////////////////////
    private bool puedeMoverse = true;
    private float velocidadMovimiento = 5f;
    private float velocidadRotacion = 720f; // Velocidad de rotación en grados por segundo
    private Vector2 direccionPlana;

    public Controles control;

    ///////////////////////////////////// FUNCIONES UNITY /////////////////////////////////
    
    private void Awake()
    {
        control = new Controles();
    }

    private void OnEnable()
    {
        control.Enable();
    }
    
    private void OnDisable()
    {
        control.Disable();
    }

    void Start()
    {
        // Puedes añadir aquí cualquier inicialización si es necesario
    }

    // Update se llama una vez por frame
    void Update()
    {
       if (puedeMoverse)
        {
            // Coger el valor del vector 2 gracias a control
            direccionPlana = control.Player.move.ReadValue<Vector2>();
            
            // Cargamos el vector 3 a través del vector2 (ignoramos Y, usamos Z para la profundidad)
            Vector3 direccionMovimiento = new Vector3(direccionPlana.x, 0f, direccionPlana.y);
            
            // --- Lógica de Rotación ---
            
            // Solo intentamos rotar si hay una entrada de movimiento significativa
            if (direccionMovimiento.magnitude > 0.1f)
            {
                // Normalizamos la dirección para obtener una magnitud de 1
                direccionMovimiento.Normalize();

                // Calcula la rotación (Quaternion) que mira en la dirección de movimiento
                // Vector3.up indica la dirección "hacia arriba" del personaje
                Quaternion rotacionDeseada = Quaternion.LookRotation(direccionMovimiento, Vector3.up);

                // Aplica la rotación de forma suave
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, 
                    rotacionDeseada, 
                    velocidadRotacion * Time.deltaTime
                );
            }
            
            // --- Lógica de Movimiento ---
            
            // Movemos el personaje
            // Nota: Usamos direccionMovimiento (ya normalizada si hubo entrada, o el vector sin normalizar si no)
            transform.position += direccionMovimiento * velocidadMovimiento * Time.deltaTime;

        }
    }
    
    ///////////////////////////////////// FUNCIONES PROPIAS /////////////////////////////////
}