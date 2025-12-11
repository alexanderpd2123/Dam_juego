using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq; 

public class DebugLevel : MonoBehaviour
{
    // --- Referencias Necesarias ---
    [Header("Referencias de Sistema")]
    public LanzadorArma lanzadorArma; 
    
    // --- Input Action Asset ---
    private Controles controls; 
    
    // Lista de las armas que el debug puede otorgar (las que no son el Bumerán)
    private readonly List<WeaponType> unlockableWeapons = new List<WeaponType>
    {
        WeaponType.OrbitalShield,
        WeaponType.FrostZone
    };

    
    void Awake()
    {
        controls = new Controles();

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
        controls.Debug.Enable();
        
        // Tecla '1': Otorga la siguiente arma no poseída
        controls.Debug.GiveWeapon.performed += context => GiveNextAvailableWeapon();

        // Tecla '2': Sube de nivel
        controls.Debug.LevelUp.performed += context => LevelUpCurrentWeapon();
    }

    void OnDisable()
    {
        controls.Debug.Disable();
    }


    /// <summary>
    /// Otorga la siguiente arma no poseída. (Tecla '1')
    /// </summary>
    private void GiveNextAvailableWeapon()
    {
        if (lanzadorArma == null)
        {
            Debug.LogError("[DEBUG ERROR] Falta la referencia a LanzadorArma.");
            return;
        }

        WeaponType? weaponToGrant = null;
        
        // Busca la primera arma de la lista que no esté marcada como isOwned
        foreach (var type in unlockableWeapons)
        {
            WeaponInventoryItem item = lanzadorArma.inventory.Find(w => w.type == type);

            if (item != null && !item.isOwned)
            {
                weaponToGrant = type;
                break; 
            }
        }

        if (weaponToGrant.HasValue)
        {
            lanzadorArma.GrantWeapon(weaponToGrant.Value);
            Debug.Log($"[DEBUG] Arma obtenida: {weaponToGrant.Value}.");
        }
        else
        {
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
            // Aplicar el límite máximo de nivel (10)
            if (lanzadorArma.level < LanzadorArma.MAX_LEVEL)
            {
                lanzadorArma.level++; 
                Debug.Log($"[DEBUG] Nivel subido a: {lanzadorArma.level}. Estadísticas actualizadas.");
            }
            else
            {
                Debug.LogWarning($"[DEBUG] Nivel máximo ({LanzadorArma.MAX_LEVEL}) alcanzado. Nivel: {lanzadorArma.level}.");
            }
        } 
        else
        {
            Debug.LogError("[DEBUG ERROR] Falta la referencia a LanzadorArma.");
        }
    }
}