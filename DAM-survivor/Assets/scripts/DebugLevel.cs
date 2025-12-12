using UnityEngine;
using UnityEngine.InputSystem; // Necesario para el nuevo sistema de Input de Unity
using System.Collections.Generic;
using System.Linq; 

public class DebugLevel : MonoBehaviour
{
    // --- Referencias Necesarias ---
    [Header("Referencias de Sistema")]
    public LanzadorArma lanzadorArma; // Referencia al script principal que gestiona las armas
    
    // --- Input Action Asset ---
    private Controles controls; // Instancia del asset de Input de Unity (Controles)
    
    // Lista de las armas que se pueden otorgar mediante comandos de debug (excluye las iniciales)
    private readonly List<WeaponType> unlockableWeapons = new List<WeaponType>
    {
        WeaponType.OrbitalShield,
        WeaponType.FrostZone
    };

    
    void Awake()
    {
        controls = new Controles(); // Inicializa el sistema de Input
        
        // Bloque de seguridad: intenta encontrar LanzadorArma si no fue asignado en el Inspector
        if (lanzadorArma == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                lanzadorArma = player.GetComponent<LanzadorArma>();
            }
        }
    }

    void OnEnable()
    {
        controls.Debug.Enable(); // Activa el mapa de acciones 'Debug'
        
        // Asigna la función 'GiveNextAvailableWeapon' a la acción 'GiveWeapon' (Tecla '1')
        controls.Debug.GiveWeapon.performed += context => GiveNextAvailableWeapon();

        // Asigna la función 'LevelUpCurrentWeapon' a la acción 'LevelUp' (Tecla '2')
        controls.Debug.LevelUp.performed += context => LevelUpCurrentWeapon();
    }

    void OnDisable()
    {
        controls.Debug.Disable(); // Desactiva el mapa de acciones 'Debug'
    }


    /// <summary>
    /// Otorga la siguiente arma no poseída. (Tecla '1')
    /// </summary>
    private void GiveNextAvailableWeapon()
    {
        // Verifica la referencia esencial
        if (lanzadorArma == null)
        {
            Debug.LogError("[DEBUG ERROR] Falta la referencia a LanzadorArma.");
            return;
        }

        WeaponType? weaponToGrant = null;
        
        // Busca la primera arma en la lista 'unlockableWeapons' que no haya sido obtenida (isOwned == false)
        foreach (var type in unlockableWeapons)
        {
            WeaponInventoryItem item = lanzadorArma.inventory.Find(w => w.type == type);

            if (item != null && !item.isOwned)
            {
                weaponToGrant = type;
                break; // Detiene la búsqueda al encontrar la primera disponible
            }
        }

        // Si se encontró un arma, la otorga
        if (weaponToGrant.HasValue)
        {
            lanzadorArma.GrantWeapon(weaponToGrant.Value);
            Debug.Log($"[DEBUG] Arma obtenida: {weaponToGrant.Value}.");
        }
        else
        {
            // Mensaje si ya se tienen todas las armas
            Debug.LogWarning("[DEBUG] No quedan más armas para desbloquear.");
        }
    }

    /// <summary>
    /// Sube de nivel el arma actual, respetando el nivel máximo de 10. (Tecla '2')
    /// </summary>
    private void LevelUpCurrentWeapon()
    {
        if (lanzadorArma != null)
        {
            // Comprueba que no se exceda el nivel máximo (LanzadorArma.MAX_LEVEL)
            if (lanzadorArma.level < LanzadorArma.MAX_LEVEL)
            {
                lanzadorArma.level++; // Incrementa el nivel
                Debug.Log($"[DEBUG] Nivel subido a: {lanzadorArma.level}. Estadísticas actualizadas.");
            }
            else
            {
                // Mensaje si ya está en el nivel máximo
                Debug.LogWarning($"[DEBUG] Nivel máximo ({LanzadorArma.MAX_LEVEL}) alcanzado. Nivel: {lanzadorArma.level}.");
            }
        } 
        else
        {
            Debug.LogError("[DEBUG ERROR] Falta la referencia a LanzadorArma.");
        }
    }
}