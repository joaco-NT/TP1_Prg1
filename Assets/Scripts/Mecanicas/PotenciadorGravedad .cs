using UnityEngine;

public class PotenciadorItem : MonoBehaviour
{
    private Vector3 posicionInicial;
    private bool enCooldown = false;
    private MeshRenderer meshRenderer;
    private Collider col;

    void Start()
    {
        InicializarComponentes();
    }

    void Update()
    {
        AnimarItem();
    }

    // ==========================================
    // MÉTODOS AUXILIARES
    // ==========================================
    private void InicializarComponentes()
    {
        posicionInicial = transform.position;
        meshRenderer = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
    }

    private void AnimarItem()
    {
        if (!enCooldown && GestorPowerUpsCentral.Instance != null)
        {
            var cfg = GestorPowerUpsCentral.Instance;

            // Rotación y flotación usando los datos globales del gestor
            transform.Rotate(Vector3.up * cfg.velocidadRotacion * Time.deltaTime, Space.World);
            float nuevoY = posicionInicial.y + Mathf.Sin(Time.time * cfg.frecuenciaFlote) * cfg.amplitudFlote;
            transform.position = new Vector3(transform.position.x, nuevoY, transform.position.z);
        }
    }

    // ==========================================
    // TRIGGERS DE UNITY
    // ==========================================
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !enCooldown && GestorPowerUpsCentral.Instance != null)
        {
            MovimientoJugador movimiento = other.GetComponent<MovimientoJugador>();
            if (movimiento != null)
            {
                enCooldown = true;
                // Le avisa al gestor para que busque el valor específico de este objeto
                GestorPowerUpsCentral.Instance.ActivarCicloItem(this, meshRenderer, col);
            }
        }
    }

    public void TerminarCooldown()
    {
        enCooldown = false;
    }
}