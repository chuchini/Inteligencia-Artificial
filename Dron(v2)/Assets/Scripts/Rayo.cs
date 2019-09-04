using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Componente auxiliar para genera rayos que detecten colisiones de manera lineal
// En el script actual se dibuja y comprueban colisiones con un rayo al frente del objeto
// sin embargo, es posible definir más rayos de la misma manera.
public class Rayo : MonoBehaviour
{
    public float longitudDeRayo;
    private bool frenteAPared;
    private bool frenteAObjeto;
    

    void Update(){
        // Se muestra el rayo únicamente en la pantalla de diseño (Scene)
        //var line = transform.position + (transform.forward * longitudDeRayo);
        //var rotatedLine = Quaternion.AngleAxis(45, transform.up) * line;
        Debug.DrawLine(transform.position, transform.position + (transform.forward * longitudDeRayo), Color.blue);
    }

    void FixedUpdate()
    {
        frenteAPared = false;
	    frenteAObjeto = false;
        // Similar a los métodos OnTrigger y OnCollision, se detectan colisiones con el rayo:
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, longitudDeRayo))
        {
            if (raycastHit.collider.gameObject.CompareTag("Objeto"))
            {
                frenteAObjeto = true;
            }
            else
            {
                frenteAObjeto = false;
            }

            if (raycastHit.collider.gameObject.CompareTag("Pared"))
            {
                frenteAPared = true;
            }
            else
            {
                frenteAPared = false;
            }
        }
    }

    /// <summary>
    /// Metodo que indica si el dron esta enfrente de la pared por medio del rayo.
    /// </summary>
    /// <returns>true, en caso de que si este enfrente, false en caso contrario</returns>
    public bool FrenteAPared(){
        return frenteAPared; 
    }

    /// <summary>
    /// Metodo que indica si el dron esta enfrente del objeto por medio del rayo.
    /// </summary>
    /// <returns>true, en caso de que si esta enfrente, false en caso contrario</returns>
    public bool FrenteAObjeto()
    {
        return frenteAObjeto;
    }
}
