using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Clase que sirve para mover al objeto que el dron va a perseguir, es mas comodo
/// dejar el movimiento del objeto libre para que el usuario interactue de la forma que quiera.
/// </summary>
public class ControladorObjeto : MonoBehaviour
{

    public float speed;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    /// <summary>
    /// Aqui es donde se actualiza el movimiento con base a las teclas que precione el usuario (las teclas de direccion).
    /// </summary>
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);
    }
}
