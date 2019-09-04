using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensores : MonoBehaviour
{
    private GameObject objeto; // Auxiliar para guardar referencia al objeto
    private Radar radar; // Componente auxiliar (script) para utilizar radar esférico
    private Rayo rayo; // Componente auxiliar (script) para utilizar rayo lineal
    private Bateria bateria; // Componente adicional (script) que representa la batería
    private Actuadores actuador; // Componente adicional (script) para obtener información de los ac

    private bool tocandoPared; // Bandera auxiliar para mantener el estado en caso de tocar pared
    private bool cercaPared; // Bandera auxiliar para mantener el estado en caso de estar cerca de una pared
    private bool tocandoObjeto; // Bandera auxiliar para mantener el estado en caso de tocar basura
    private bool cercaObjeto; // Bandera auxiliar para mantener el estado en caso de estar cerca de una basura
    private bool tocandoBase;
    private Vector3 posicionBase;
    public GameObject baseDeCarga;

    // Asignaciones de componentes
    void Start(){
        radar = GameObject.Find("Radar").gameObject.GetComponent<Radar>();
        rayo = GameObject.Find("Rayo").gameObject.GetComponent<Rayo>();
        bateria = GameObject.Find("Bateria").gameObject.GetComponent<Bateria>();
        actuador = GetComponent<Actuadores>();
        posicionBase = baseDeCarga.transform.position;
    }

    // ========================================
    // Los siguientes métodos permiten la detección de eventos de colisión
    // que junto con etiquetas de los objetos permiten identificar los elementos
    // La mayoría de los métodos es para asignar banderas/variables de estado.

    void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag("Pared")){
            tocandoPared = true;
        }

        if (other.gameObject.CompareTag("BaseDeCarga"))
        {
            tocandoBase = true;
        }
    }

    void OnCollisionStay(Collision other){
	tocandoPared = false;
	tocandoBase = false;
        if(other.gameObject.CompareTag("Pared")){
            tocandoPared = true;
        }
        
        if (other.gameObject.CompareTag("BaseDeCarga"))
        {
            tocandoBase = true;
        }
    }

    void OnCollisionExit(Collision other){
        if(other.gameObject.CompareTag("Pared")){
            tocandoPared = false;
        }
        
        if (other.gameObject.CompareTag("BaseDeCarga"))
        {
            tocandoBase = false;
        }
    }

    // ========================================
    // Los siguientes métodos definidos son públicos, la intención
    // es que serán usados por otro componente (Controlador)

    public bool TocandoPared(){
        return tocandoPared;
    }

    public bool TocandoBase()
    {
        return tocandoBase;
    }

    public bool CercaDePared(){
        return radar.CercaDePared();
    }

    public bool FrenteAPared(){
        return rayo.FrenteAPared();
    }

    public bool CercaDeObjeto()
    {
        return radar.CercaDeObjeto();
    }

    public bool FrenteAObjeto()
    {
        return rayo.FrenteAObjeto();
    }

    public float Bateria(){
        return bateria.NivelDeBateria();
    }

    public float BateriaMaxima()
    {
        return bateria.capacidadMaximaBateria;
    }

    public Vector3 UbicacionBase()
    {
        return posicionBase;
    }
    
    public Vector3 Ubicacion(){
        return transform.position;
    }
    
}
