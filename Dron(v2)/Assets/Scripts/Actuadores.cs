using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * Clase que almacena los actuadores del dron, es decir, todo lo relacionado
 * al movimiento del dron.
 */
public class Actuadores : MonoBehaviour
{
    private Rigidbody rb; // Componente para simular acciones físicas realistas
    private Bateria bateria; // Componente adicional (script) que representa la batería
    private Sensores sensor; // Componente adicional (script) para obtener información de los sensores

    private float upForce; // Indica la fuerza de elevación del dron
    private float movementForwardSpeed = 20.0f; // Escalar para indicar fuerza de movimiento frontal
    private float wantedYRotation; // Auxiliar para el cálculo de rotación
    private float currentYRotation; // Auxiliar para el cálculo de rotación
    private float rotateAmountByKeys = 2.5f; // Auxiliar para el cálculo de rotación
    private float rotationYVelocity; // Escalar (calculado) para indicar velocidad de rotación
    private float sideMovementAmount = 75.0f; // Escalar para indicar velocidad de movimiento lateral

    // Asignaciones de componentes
    void Start(){
        rb = GetComponent<Rigidbody>();
        sensor = GetComponent<Sensores>();
        bateria = GameObject.Find("Bateria").gameObject.GetComponent<Bateria>();
    }

    // ========================================
    // A partir de aqui, todos los métodos definidos son públicos, la intención
    // es que serán usados por otro componente (Controlador)

    /**
     * Metodo para que el dron ascienda.
     * La fuera debe ser mayor que la utilizada para flotar.
     */
    public void Ascender(){
        upForce = 99.5f;
        rb.AddRelativeForce(Vector3.up * upForce);
    }

    /**
     * Metodo para que el dron descienda.
     */
    public void Descender(){
        upForce = 10;
        rb.AddRelativeForce(Vector3.up * upForce);
    }

    /**
     * Metodo que le permite al dron flotar.
     */
    public void Flotar(){
        upForce = 98.09f;
        rb.AddRelativeForce(Vector3.up * upForce);
    }

    /**
     * Metodo que sirve para que el dron avance.
     */
    public void Adelante(){
        rb.AddRelativeForce(Vector3.forward * movementForwardSpeed);
    }
    
    /**
     * Metodo para que el dron se mueva hacia atras.
     */
    public void Atras(){
        rb.AddRelativeForce(Vector3.back * movementForwardSpeed);
    }

    /**
     * Metodo que sirve para que el dron gire a al derecha.
     */
    public void GirarDerecha(){
        wantedYRotation += rotateAmountByKeys;
        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, 0.25f);
        rb.rotation = Quaternion.Euler(new Vector3(rb.rotation.x, currentYRotation, rb.rotation.z));
    }

    /**
     * Metodo que sirve para que el dron gire a la izquierda
     */
    public void GirarIzquierda(){
        wantedYRotation -= rotateAmountByKeys;
        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, 0.25f);
        rb.rotation = Quaternion.Euler(new Vector3(rb.rotation.x, currentYRotation, rb.rotation.z));
    }

    /**
     * Metodo que sirve para que el dron se mueva a la derecha sin afectar su rotacion.
     */
    public void Derecha(){
        rb.AddRelativeForce(Vector3.right * sideMovementAmount);
    }
    
    /**
     * Metodo que sirve para que el dron se mueva a la izquierda sin afectar su rotacion.
     */
    public void Izquierda(){
        rb.AddRelativeForce(Vector3.left * sideMovementAmount);
    }

    /**
     * Metodo que sirve para detener al dron independientemente del movimiento que este realizando.
     */
    public void Detener(){
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    /*
     * Metodo que sirve para cargar la bateria del dron una vez que este en contacto
     * con la base de carga. El metodo llama a otro metodo de la clase Bateria.
     */
    public void CargarBateria(){
        bateria.Cargar();
    }
}
