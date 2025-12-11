using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Define los tipos de arma para el inventario
public enum WeaponType { 
    Boomerang, 
    FrostZone, 
    OrbitalShield 
}

// Clase para rastrear el estado de un arma en el inventario
[System.Serializable]
public class WeaponInventoryItem
{
    public WeaponType type;
    public bool isOwned;
    public int level; 
}


public class LanzadorArma : MonoBehaviour
{
    // AÑADIDO: Variable de Nivel Máximo como constante
    public const int MAX_LEVEL = 10;
    
    // AÑADIDO: Variable de Nivel para el escalado manual
    [Header("Escalado de Poder")]
    [Tooltip("Nivel actual del jugador/arma. Limitado a 10.")]
    public int level = 1; // Empieza en 1
    
    [Tooltip("Porcentaje de reducción del Cooldown por nivel (ej: 0.05 para 5% de reducción).")]
    public float cooldownReductionPerLevel = 0.05f; 
    [Tooltip("Daño base que se añade por nivel (ej: 5 por nivel).")]
    public float damageBonusPerLevel = 5f;
    
    [Header("Daño Base de Armas")]
    public float boomerangBaseDamage = 20f;
    public float frostZoneBaseDamage = 5f;
    public float orbitalShieldBaseDamage = 10f;
    
    // --- Configuración General ---
    public GameObject projectilePrefab; // Usado para el DISPARO AUTOMÁTICO (ej. Bumerán)
    public Transform launchPoint; 
    public float launchForce = 20f; 
    public float fireRate = 0.5f; 
    
    // --- Variables de Inventario ---
    [Header("Inventario de Armas")]
    public List<WeaponInventoryItem> inventory = new List<WeaponInventoryItem>();
    
    // --- Configuración Zona de Escarcha ---
    [Header("Configuración Zona de Escarcha")]
    public GameObject frostZonePrefab; 
    private GameObject activeFrostZone = null; 
    private bool frostZoneActive = false; 

    // --- Configuración Escudo Orbital ---
    [Header("Configuración Escudo Orbital")]
    public GameObject orbitalPrefab;         
    public float orbitalRadius = 3f;         
    public float orbitalSpeed = 150f;        
    private bool shieldActive = false;       

    private Coroutine autoFireCoroutine;
    
    private int previousLevel = 1;

    void Awake()
    {
        // Inicializar el inventario: Solo el Bumerán es poseído al inicio
        inventory.Add(new WeaponInventoryItem { type = WeaponType.Boomerang, isOwned = true, level = 1 });
        inventory.Add(new WeaponInventoryItem { type = WeaponType.FrostZone, isOwned = false, level = 0 });
        inventory.Add(new WeaponInventoryItem { type = WeaponType.OrbitalShield, isOwned = false, level = 0 });
    }

    void Start()
    {
        // Solo el Bumerán se activa al inicio (si está en el inventario)
        if (IsWeaponOwned(WeaponType.Boomerang) && projectilePrefab != null)
        {
            autoFireCoroutine = StartCoroutine(AutoFireRoutine());
        }
    }

    void LateUpdate()
    {
        // Limita el nivel en caso de que se haya modificado manualmente en el Inspector
        if (level > MAX_LEVEL)
        {
            level = MAX_LEVEL;
        }

        if (level != previousLevel)
        {
            previousLevel = level;
            UpdatePersistentWeaponStats();

            // Reinicia corrutina de disparo si el Bumerán está activo
            if (IsWeaponOwned(WeaponType.Boomerang))
            {
                if (autoFireCoroutine != null)
                {
                    StopCoroutine(autoFireCoroutine);
                }
                autoFireCoroutine = StartCoroutine(AutoFireRoutine());
            }
        }
    }
    
    // ----------------------------------------------------------------------------------
    // MÉTODOS PÚBLICOS PARA DEBUG Y OBTENCIÓN DE ARMAS
    // ----------------------------------------------------------------------------------
    
    /// <summary>
    /// Otorga una nueva arma al jugador (llamado desde DebugLevel).
    /// </summary>
    public void GrantWeapon(WeaponType typeToGrant)
    {
        WeaponInventoryItem item = inventory.Find(w => w.type == typeToGrant);
        
        if (item != null && !item.isOwned)
        {
            item.isOwned = true;
            item.level = 1; // Nivel inicial 1
            Debug.Log($"Arma obtenida: {typeToGrant}. Nivel inicial: 1.");

            // Activar el arma si es persistente
            if (typeToGrant == WeaponType.OrbitalShield)
            {
                ActivateOrbitalShield();
            }
            else if (typeToGrant == WeaponType.FrostZone)
            {
                ActivateFrostZone(); 
            }
            else if (typeToGrant == WeaponType.Boomerang && autoFireCoroutine == null)
            {
                 autoFireCoroutine = StartCoroutine(AutoFireRoutine());
            }
        }
        else if (item != null && item.isOwned)
        {
            Debug.LogWarning($"El arma {typeToGrant} ya está en posesión.");
        }
    }

    // ----------------------------------------------------------------------------------
    // LÓGICA DE ESCALADO Y ACTIVACIÓN
    // ----------------------------------------------------------------------------------

    private IEnumerator AutoFireRoutine()
    {
        while (true)
        {
            float scaledFireRate = CalculateScaledFireRate();
            yield return new WaitForSeconds(scaledFireRate);
            LaunchWeapon();
        }
    }
    
    private float CalculateScaledFireRate()
    {
        float maxReduction = 1f - Mathf.Epsilon; 
        float totalReduction = Mathf.Min( (level - 1) * cooldownReductionPerLevel, maxReduction);
        return fireRate * (1f - totalReduction);
    }
    
    private float CalculateScaledDamage(float baseDamage)
    {
        float damageIncrease = (level - 1) * damageBonusPerLevel;
        return baseDamage + damageIncrease;
    }

    private void LaunchWeapon()
    {
        if (projectilePrefab == null || launchPoint == null) return;

        if (IsWeaponOwned(WeaponType.Boomerang))
        {
            float scaledDamage = CalculateScaledDamage(boomerangBaseDamage);
            Vector3 direction = transform.forward; 
            
            GameObject newObject = Instantiate(projectilePrefab, launchPoint.position, launchPoint.rotation);

            Boomerang boomerangComponent = newObject.GetComponent<Boomerang>();
            if (boomerangComponent != null)
            {
                boomerangComponent.damage = scaledDamage;
                boomerangComponent.Initialize(this.transform, direction); 
            }
        }
    }

    private void UpdatePersistentWeaponStats()
    {
        // Lógica de escalado de armas persistentes...
    }
    
    public void ActivateFrostZone() 
    {
        if (frostZonePrefab == null || frostZoneActive) return;

        GameObject zone = Instantiate(frostZonePrefab, transform.position, Quaternion.identity);

        FrostZone frostZoneComponent = zone.GetComponent<FrostZone>();
        if (frostZoneComponent != null)
        {
            frostZoneComponent.damagePerTick = CalculateScaledDamage(frostZoneBaseDamage);
            frostZoneComponent.Initialize(this.transform);
            activeFrostZone = zone;
            frostZoneActive = true;
        }
    }

    public void ActivateOrbitalShield() 
    {
        if (orbitalPrefab == null || shieldActive) return;
        
        float angleSpacing = 360f / 3f; 
        
        for (int i = 0; i < 3; i++)
        {
            float initialAngle = i * angleSpacing;
            
            GameObject orb = Instantiate(orbitalPrefab, transform.position, Quaternion.identity);
            
            OrbitalShield orbitalComponent = orb.GetComponent<OrbitalShield>();
            
            if (orbitalComponent != null)
            {
                orbitalComponent.damage = CalculateScaledDamage(orbitalShieldBaseDamage);
                
                orbitalComponent.Initialize(
                    center: this.transform, 
                    radius: orbitalRadius, 
                    speed: orbitalSpeed, 
                    initialAngle: initialAngle
                );
            }
        }
        
        shieldActive = true;
    }

    private bool IsWeaponOwned(WeaponType type)
    {
        return inventory.Find(w => w.type == type)?.isOwned ?? false;
    }
}