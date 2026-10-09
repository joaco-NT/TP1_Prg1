using UnityEngine;

public class ZonaMeta : MonoBehaviour
{
    // ==========================================
    // 1. CONFIGURACIÓN EN EL INSPECTOR
    // ==========================================
    [Header("Configuración de Victoria")]
    [Tooltip("Tag exacto que debe tener el objeto transportable para que sea válido.")]
    [SerializeField] private string tagObjetoValido = "ObjetoTransportable";

    [Tooltip("Objeto vacío hijo de la meta donde se asentará y quedará fija la caja al entregarse.")]
    [SerializeField] private Transform puntoDeResguardo;

    [Header("Efecto de Flotación (Objeto Depositado)")]
    [Tooltip("¿Activar el efecto visual de flotación de arriba y abajo para el objeto en la meta?")]
    [SerializeField] private bool activarFlotacion = true;
    [Tooltip("Altura máxima que sube y baja el objeto.")]
    [SerializeField] private float amplitudFlotacion = 0.25f;
    [Tooltip("Velocidad del ciclo de flotación.")]
    [SerializeField] private float velocidadFlotacion = 2f;

    // ==========================================
    // 2. ESTADO INTERNO
    // ==========================================
    private bool juegoTerminado = false;
    private Transform objetoDepositadoRef;
    private Vector3 posicionLocalResguardo;

    // ==========================================
    // 3. CICLO DE VIDA DE UNITY
    // ==========================================
    void Start()
    {
        InicializarComponentes();
    }

    void Update()
    {
        EjecutarEfectoFlotacionObjeto();
    }

    // ==========================================
    // 4. MÉTODOS AUXILIARES
    // ==========================================
    private void InicializarComponentes()
    {
        if (puntoDeResguardo != null)
        {
            posicionLocalResguardo = puntoDeResguardo.localPosition;
        }
    }

    private void EjecutarEfectoFlotacionObjeto()
    {
        if (!activarFlotacion || !juegoTerminado || objetoDepositadoRef == null || puntoDeResguardo == null) return;

        float desplazamientoY = Mathf.Sin(Time.time * velocidadFlotacion) * amplitudFlotacion;
        
        objetoDepositadoRef.localPosition = new Vector3(
            posicionLocalResguardo.x, 
            posicionLocalResguardo.y + desplazamientoY, 
            posicionLocalResguardo.z
        );
    }

    // ==========================================
    // 5. DETECCIÓN DE ENTRADA (TRIGGERS)
    // ==========================================
    void OnTriggerEnter(Collider other)
    {
        if (juegoTerminado) return;

        if (other.CompareTag(tagObjetoValido))
        {
            juegoTerminado = true;
            ProcesarVictoria(other.gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            MovimientoJugador jugador = other.GetComponent<MovimientoJugador>();
            if (jugador != null)
            {
                GameObject objetoEnMano = jugador.ObtenerObjetoTransportado();
                if (objetoEnMano != null && objetoEnMano.CompareTag(tagObjetoValido))
                {
                    juegoTerminado = true;
                    jugador.SoltarObjetoTransportado();
                    ProcesarVictoria(objetoEnMano);
                }
            }
        }
    }

    // ==========================================
    // 6. LÓGICA DE VICTORIA Y MENSAJE
    // ==========================================
    private void ProcesarVictoria(GameObject objetoDepositado)
    {
        Debug.Log("¡VICTORIA! Objeto correcto depositado en la zona de meta.");
        
        FinalizarEntregaObjeto(objetoDepositado);
    }

    private void FinalizarEntregaObjeto(GameObject objeto)
    {
        if (puntoDeResguardo != null)
        {
            objeto.transform.position = puntoDeResguardo.position;
            objeto.transform.rotation = puntoDeResguardo.rotation;
            objeto.transform.SetParent(puntoDeResguardo);

            objetoDepositadoRef = objeto.transform;
            posicionLocalResguardo = objeto.transform.localPosition;
        }

        Rigidbody rbObjeto = objeto.GetComponent<Rigidbody>();
        if (rbObjeto != null)
        {
            rbObjeto.isKinematic = true;
            rbObjeto.linearVelocity = Vector3.zero;
            rbObjeto.angularVelocity = Vector3.zero;
        }

        ObjetoAgarrable scriptAgarrable = objeto.GetComponent<ObjetoAgarrable>();
        if (scriptAgarrable != null)
        {
            Destroy(scriptAgarrable);
        }
    }
}