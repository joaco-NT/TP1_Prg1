using UnityEngine;
using UnityEngine.InputSystem;

public class ObjetoAgarrable : MonoBehaviour
{
    private Transform puntoTransporte;
    private Rigidbody rb;
    private Collider[] collidersObjeto;
    private bool estaEnRango = false;
    private bool estaTransportado = false;

    void Start()
    {
        InicializarComponentes();
    }

    void Update()
    {
        LeerTecladoYGestionarAccion();
    }

    // ==========================================
    // MÉTODOS AUXILIARES
    // ==========================================
    private void InicializarComponentes()
    {
        rb = GetComponent<Rigidbody>();
        collidersObjeto = GetComponentsInChildren<Collider>();
    }

    private void LeerTecladoYGestionarAccion()
    {
        if (Keyboard.current == null) return;

        // Si está en rango y presionas E
        if (estaEnRango && !estaTransportado && Keyboard.current.eKey.wasPressedThisFrame)
        {
            RecogerObjeto();
        }
        // Si está transportado y presionas Q
        else if (estaTransportado && Keyboard.current.qKey.wasPressedThisFrame)
        {
            SoltarObjeto();
        }
    }

    private void RecogerObjeto()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            Transform punto = jugador.transform.Find("PuntoTransporte");
            if (punto != null)
            {
                puntoTransporte = punto;
                
                // 1. Emparentamos la caja al punto de transporte
                transform.SetParent(puntoTransporte);
                
                // 2. Colocamos la posición exactamente en el punto de transporte
                transform.localPosition = Vector3.zero;
                
                // 3. Forzamos a que la caja adopte EXACTAMENTE la misma rotación con la que mira el jugador
                transform.rotation = jugador.transform.rotation;

                // Desactivamos físicas y colisiones para que no interfieran
                rb.isKinematic = true;
                rb.useGravity = false;

                foreach (Collider col in collidersObjeto)
                {
                    col.enabled = false;
                }

                estaTransportado = true;
                estaEnRango = false;
                Debug.Log("¡Objeto recogido y alineado con la dirección del jugador!");
            }
            else
            {
                Debug.LogError("ERROR: No se encontró el objeto hijo llamado 'PuntoTransporte' en el Jugador.");
            }
        }
        else
        {
            Debug.LogError("ERROR: No se encontró ningún objeto con el Tag 'Player' en la escena.");
        }
    }

    private void SoltarObjeto()
    {
        transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = true;

        foreach (Collider col in collidersObjeto)
        {
            col.enabled = true;
        }
        
        // Impulso hacia adelante en la dirección a la que apunta el jugador al soltar
        rb.AddForce(transform.forward * 3f, ForceMode.Impulse);
        
        estaTransportado = false;
        Debug.Log("Objeto soltado correctamente.");
    }

    // ==========================================
    // TRIGGERS DE UNITY
    // ==========================================
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !estaTransportado)
        {
            estaEnRango = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            estaEnRango = false;
        }
    }
}