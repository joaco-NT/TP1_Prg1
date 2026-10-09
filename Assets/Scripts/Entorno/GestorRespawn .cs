using UnityEngine;
using System.Collections.Generic;

public class GestorRespawn : MonoBehaviour
{
    [System.Serializable]
    public struct ZonaRespawn
    {
        [Header("Estructura de la Zona")]
        [Tooltip("El colisionador que el jugador cruza para activar el checkpoint y su spawn point.")]
        public Collider checkpoint; 

        [Tooltip("El lugar físico (posición) donde reaparecerá el jugador al activar este checkpoint.")]
        public Transform spawnPoint;         
    }

    [Header("--- ZONAS DE CHECKPOINTS ---")]
    [SerializeField] private ZonaRespawn[] zonasRespawn;

    [Header("--- HITBOX DE CAÍDA GLOBAL ---")]
    [Tooltip("Una única hitbox general que cubre todo el abismo/suelo de caída del nivel.")]
    [SerializeField] private Collider hitboxCaidaGlobal;

    // Diccionario exclusivo para los checkpoints 
    private Dictionary<Collider, Transform> diccionarioCheckpoints = new Dictionary<Collider, Transform>();

    private void Awake()
    {
        InicializarCheckpointsYHitbox();
    }

    // ==========================================
    // MÉTODOS AUXILIARES
    // ==========================================
    private void InicializarCheckpointsYHitbox()
    {
        // 1. Configurar todos los Checkpoints individuales
        foreach (var zona in zonasRespawn)
        {
            if (zona.checkpoint != null && zona.spawnPoint != null)
            {
                EscuchaTrigger helperCheckpoint = ObtenerOAgregarHelper(zona.checkpoint);
                helperCheckpoint.InicializarComoCheckpoint(this, zona.checkpoint);

                if (!diccionarioCheckpoints.ContainsKey(zona.checkpoint))
                {
                    diccionarioCheckpoints.Add(zona.checkpoint, zona.spawnPoint);
                }
            }
        }

        // 2. Configurar la Hitbox de caída única y global
        if (hitboxCaidaGlobal != null)
        {
            EscuchaTrigger helperHitboxGlobal = ObtenerOAgregarHelper(hitboxCaidaGlobal);
            helperHitboxGlobal.InicializarComoHitboxGlobal(this);
        }
        else
        {
            Debug.LogWarning("[GestorRespawnAvanzado] No se ha asignado una Hitbox de Caída Global en el Inspector.", this);
        }
    }

    private EscuchaTrigger ObtenerOAgregarHelper(Collider col)
    {
        EscuchaTrigger helper = col.gameObject.GetComponent<EscuchaTrigger>();
        if (helper == null)
        {
            helper = col.gameObject.AddComponent<EscuchaTrigger>();
        }
        return helper;
    }

    public void ProcesarEntradaCheckpoint(Collider colCheckpoint, Collider jugadorCollider)
    {
        if (diccionarioCheckpoints.TryGetValue(colCheckpoint, out Transform spawnAsociado))
        {
            MovimientoJugador jugador = jugadorCollider.GetComponent<MovimientoJugador>();
            if (jugador != null && spawnAsociado != null)
            {
                jugador.GuardarCheckpoint(spawnAsociado.position);
                Debug.Log($"[Checkpoint] Activado. Nuevo Spawn guardado en: {spawnAsociado.position}");
            }
        }
    }

    public void ProcesarCaidaGlobal(Collider jugadorCollider)
    {
        MovimientoJugador jugador = jugadorCollider.GetComponent<MovimientoJugador>();
        if (jugador != null)
        {
            jugador.ReaparecerEnCheckpoint();
            Debug.Log("[Hitbox Caída Global] ¡El jugador cayó al vacío! Reapareciendo en su checkpoint activo.");
        }
    }
}

// ==========================================
// CLASE AUXILIAR MODULAR PARA LOS TRIGGERS
// ==========================================
public class EscuchaTrigger : MonoBehaviour
{
    private GestorRespawn gestor;
    private Collider miCollider;
    private bool esHitboxGlobal = false;

    public void InicializarComoCheckpoint(GestorRespawn gestorPrincipal, Collider col)
    {
        gestor = gestorPrincipal;
        miCollider = col;
        esHitboxGlobal = false;
    }

    public void InicializarComoHitboxGlobal(GestorRespawn gestorPrincipal)
    {
        gestor = gestorPrincipal;
        esHitboxGlobal = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gestor != null)
        {
            if (esHitboxGlobal)
            {
                gestor.ProcesarCaidaGlobal(other);
            }
            else
            {
                gestor.ProcesarEntradaCheckpoint(miCollider, other);
            }
        }
    }
}