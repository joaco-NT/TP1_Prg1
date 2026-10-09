using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class MovimientoJugador : MonoBehaviour
{
    // ==========================================
    // 1. COMPONENTES Y ESTADO INTERNO
    // ==========================================
    private Rigidbody rb;
    private bool estaEnSuelo;
    private bool saltoSolicitado;
    private bool estaCorriendo = false;

    // Control de Doble W (Correr)
    private float tiempoUltimaPulsacionW = 0f;
    private float umbralDobleClick = 0.3f;

    // Referencia para la Plataforma Móvil
    private Rigidbody rbPlataformaActual;

    // Sistema de Respawn y Checkpoints
    private Vector3 posicionRespawn;
    private bool tieneCheckpointActivo = false;

    // Control de Gravedad / Power-Up
    private float multiplicadorGravedadActual = 1f;
    private Coroutine corrutinaRestaurarGravedad;
    private float fuerzaSaltoOriginal = 0f;

    // ==========================================
    // 2. CONFIGURACIÓN EN EL INSPECTOR
    // ==========================================
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadCaminata = 5f;
    [SerializeField] private float velocidadCorrer = 9f;
    [SerializeField] private float fuerzaSalto = 6f;
    [Range(0f, 1f)] [SerializeField] private float controlAereo = 0.5f;

    [Header("Detección de Suelo")]
    [SerializeField] private float distanciaRaycast = 1.1f;

    [Header("Cámara")]
    [SerializeField] private Transform camaraPrincipal;

    // ==========================================
    // 3. CICLO DE VIDA DE UNITY
    // ==========================================
    void Start()
    {
        InicializarComponentes();
    }

    void Update()
    {
        LeerTeclado();
    }

    void FixedUpdate()
    {
        ComprobarSuelo();
        ProcesarMovimiento();
        AplicarGravedad();
        RotarHaciaCursor();
    }

    // ==========================================
    // 4. LÓGICA DE INICIALIZACIÓN
    // ==========================================
    private void InicializarComponentes()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        tieneCheckpointActivo = false;
        fuerzaSaltoOriginal = fuerzaSalto; 

        if (camaraPrincipal == null && Camera.main != null)
        {
            camaraPrincipal = Camera.main.transform;
        }
    }

    // ==========================================
    // 5. ENTRADAS Y CONTROL DEL JUGADOR
    // ==========================================
    private void LeerTeclado()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            saltoSolicitado = true;
        }

        // Lógica de Doble W para Correr
        if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            float tiempoActual = Time.time;
            if (tiempoActual - tiempoUltimaPulsacionW < umbralDobleClick)
            {
                estaCorriendo = true;
            }
            tiempoUltimaPulsacionW = tiempoActual;
        }

        if (!Keyboard.current.wKey.isPressed && !Keyboard.current.upArrowKey.isPressed)
        {
            estaCorriendo = false;
        }
    }

    // ==========================================
    // 6. FÍSICAS Y MOVIMIENTO
    // ==========================================
    private void ProcesarMovimiento()
    {
        Vector3 movimientoTeclado = CalcularMovimientoCamara();
        Vector3 velocidadPlataforma = ObtenerVelocidadPlataforma();
        
        MoverPersonaje(movimientoTeclado, velocidadPlataforma);
        Saltar();
    }

    private Vector3 ObtenerDireccion()
    {
        float movX = 0f;
        float movZ = 0f;

        if (Keyboard.current != null)
        {
            var teclado = Keyboard.current;
            if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed) movX += 1f;
            if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed) movX -= 1f;
            if (teclado.wKey.isPressed || teclado.upArrowKey.isPressed) movZ += 1f;
            if (teclado.sKey.isPressed || teclado.downArrowKey.isPressed) movZ -= 1f;
        }

        return new Vector3(movX, 0, movZ).normalized;
    }

    private Vector3 CalcularMovimientoCamara()
    {
        Vector3 inputDireccion = ObtenerDireccion();
        float velocidadActualMov = estaCorriendo ? velocidadCorrer : velocidadCaminata;

        if (camaraPrincipal != null)
        {
            Vector3 frenteCamara = camaraPrincipal.forward;
            Vector3 derechaCamara = camaraPrincipal.right;
            frenteCamara.y = 0f; 
            derechaCamara.y = 0f;
            frenteCamara.Normalize(); 
            derechaCamara.Normalize();

            return (frenteCamara * inputDireccion.z + derechaCamara * inputDireccion.x) * velocidadActualMov;
        }

        return inputDireccion * velocidadActualMov;
    }

    private void MoverPersonaje(Vector3 movimientoTeclado, Vector3 velocidadAdicionalPlataforma)
    {
        Vector3 velocidadRigidBodyActual = rb.linearVelocity;
        float factorMovimiento = estaEnSuelo ? 1f : controlAereo;
        
        Vector3 velocidadDeseada = new Vector3(
            movimientoTeclado.x * factorMovimiento, 
            velocidadRigidBodyActual.y, 
            movimientoTeclado.z * factorMovimiento
        ) + velocidadAdicionalPlataforma;
        
        if (estaEnSuelo)
        {
            rb.linearVelocity = velocidadDeseada;
        }
        else
        {
            rb.linearVelocity = new Vector3(
                Mathf.Lerp(velocidadRigidBodyActual.x, velocidadDeseada.x, 5f * Time.fixedDeltaTime),
                velocidadRigidBodyActual.y,
                Mathf.Lerp(velocidadRigidBodyActual.z, velocidadDeseada.z, 5f * Time.fixedDeltaTime)
            );
        }
    }

    private void Saltar()
    {
        if (estaEnSuelo && saltoSolicitado)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            estaEnSuelo = false;
            rbPlataformaActual = null; 
        }

        saltoSolicitado = false;
    }

    private void RotarHaciaCursor()
    {
        if (Mouse.current == null || Camera.main == null) return;

        Plane planoSuelo = new Plane(Vector3.up, transform.position);
        Ray rayoRaton = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (planoSuelo.Raycast(rayoRaton, out float distancia))
        {
            Vector3 puntoIntersec = rayoRaton.GetPoint(distancia);
            Vector3 direccionMirada = puntoIntersec - transform.position;
            direccionMirada.y = 0f; 

            if (direccionMirada != Vector3.zero)
            {
                Quaternion rotacionDeseada = Quaternion.LookRotation(direccionMirada);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, 20f * Time.fixedDeltaTime);
            }
        }
    }

    private void ComprobarSuelo()
    {
        estaEnSuelo = Physics.Raycast(transform.position, Vector3.down, distanciaRaycast);
        if (!estaEnSuelo)
        {
            rbPlataformaActual = null; 
        }
    }

    private Vector3 ObtenerVelocidadPlataforma()
    {
        if (rbPlataformaActual == null) return Vector3.zero;

        ObstaculoMovil obstaculo = rbPlataformaActual.GetComponent<ObstaculoMovil>();
        if (obstaculo != null)
        {
            Vector3 velObs = obstaculo.ObtenerVelocidad();
            return new Vector3(velObs.x, 0f, velObs.z);
        }
        else
        {
            return new Vector3(rbPlataformaActual.linearVelocity.x, 0f, rbPlataformaActual.linearVelocity.z);
        }
    }

    // ==========================================
    // 7. GESTIÓN DE RESPAWN Y CHECKPOINTS
    // ==========================================
    public void GuardarCheckpoint(Vector3 nuevaPosicion)
    {
        posicionRespawn = nuevaPosicion;
        tieneCheckpointActivo = true; 
    }

    public void ReaparecerEnCheckpoint()
    {
        if (tieneCheckpointActivo)
        {
            rb.linearVelocity = Vector3.zero;    
            transform.position = posicionRespawn; 
            rbPlataformaActual = null;
            rb.useGravity = true; 
        }
        else
        {
            Debug.Log("[MovimientoJugador] El jugador cayó al vacío, pero NO hay ningún checkpoint activo guardado.");
        }
    }

    // ==========================================
    // 8. POWER-UP DE GRAVEDAD
    // ==========================================
    private void AplicarGravedad()
    {
        if (estaEnSuelo) 
        {
            rb.useGravity = true;
            return;
        }

        if (multiplicadorGravedadActual != 1f)
        {
            rb.useGravity = false;
            
            float gravedadEfectiva = Mathf.Max(multiplicadorGravedadActual, 0.05f);
            Vector3 gravedadPersonalizada = Physics.gravity * gravedadEfectiva;
            rb.AddForce(gravedadPersonalizada, ForceMode.Acceleration);
        }
        else
        {
            rb.useGravity = true;
        }
    }

    public void ActivarPowerUpGravedad(float nuevoMultiplicador, float duracion)
    {
        multiplicadorGravedadActual = nuevoMultiplicador;
        
        fuerzaSalto = fuerzaSaltoOriginal * 1.5f; 

        if (corrutinaRestaurarGravedad != null)
        {
            StopCoroutine(corrutinaRestaurarGravedad);
        }
        corrutinaRestaurarGravedad = StartCoroutine(RestaurarGravedad(duracion));
    }

    private IEnumerator RestaurarGravedad(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        multiplicadorGravedadActual = 1f;
        fuerzaSalto = fuerzaSaltoOriginal; 
        rb.useGravity = true;
    }

    // ==========================================
    // 9. INTERACCIONES FÍSICAS (PLATAFORMAS)
    // ==========================================
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlataformaMovil"))
        {
            rbPlataformaActual = collision.gameObject.GetComponent<Rigidbody>();
            estaEnSuelo = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlataformaMovil"))
        {
            if (rbPlataformaActual != null && collision.gameObject == rbPlataformaActual.gameObject)
            {
                rbPlataformaActual = null;
            }
        }
    }

    // ==========================================
    // 10. GESTIÓN DE TRANSPORTE Y ZONA DE META
    // ==========================================
    public GameObject ObtenerObjetoTransportado()
    {
        Transform punto = transform.Find("PuntoTransporte");
        if (punto != null && punto.childCount > 0)
        {
            return punto.GetChild(0).gameObject;
        }
        return null;
    }

    public void SoltarObjetoTransportado()
    {
        GameObject objetoMano = ObtenerObjetoTransportado();
        if (objetoMano != null)
        {
            objetoMano.transform.SetParent(null);
        }
    }
}