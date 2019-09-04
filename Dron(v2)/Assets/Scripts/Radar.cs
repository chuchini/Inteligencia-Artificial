using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Componente auxiliar que utiliza un Collider esférico a manera de radar
// para comprobar colisiones con otros elementos.
// Las comprobaciones y métodos son análogos al componente (script) de Sensores.
public class Radar : MonoBehaviour
{
    private bool cercaDeObjeto;
    private bool cercaDePared;
    private bool cercaDeBaseDeCarga;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Objeto"))
        {
            cercaDeObjeto = true;
        }

        if (other.gameObject.CompareTag("Pared"))
        {
            cercaDePared = true;
        }

        //if (other.gameObject.CompareTag("BaseDeCarga"))
        //{
        //    cercaDeBaseDeCarga = true;
        //}
    }

    void OnTriggerStay(Collider other){
	cercaDeObjeto = false;
	cercaDePared = false;
        if(other.gameObject.CompareTag("Objeto"))
        {
            cercaDeObjeto = true;
        }
        
        if(other.gameObject.CompareTag("Pared"))
        {
            cercaDePared = true;
        }
	    
	
        
        //if (other.gameObject.CompareTag("BaseDeCarga"))
        //{
        //    cercaDeBaseDeCarga = true;
        //}
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.CompareTag("Objeto")){
            cercaDeObjeto = false;
        }
        if(other.gameObject.CompareTag("Pared")){
            cercaDePared = false;
        }

        //if (other.gameObject.CompareTag("BaseDeCarga"))
        //{
        //    cercaDeBaseDeCarga = false;
        //}
    }

    public bool CercaDePared(){
        return cercaDePared;
    }

    public bool CercaDeObjeto()
    {
        return cercaDeObjeto;
    }
    
    //void Update(){
    //    cercaDeObjeto=false; 
    //}
}
