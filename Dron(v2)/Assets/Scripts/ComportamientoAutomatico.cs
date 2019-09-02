using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportamientoAutomatico : MonoBehaviour
{

	private Sensores sensor;
	private Actuadores actuador;
	
	private enum Percepcion
	{
		CercaDePared	= 0,
		NoCercaDePared	= 1,
		CercaDeObjeto	= 2,
		FrenteAObjeto	= 3,
		NoFrenteAObjeto	= 4,
		NoCercaDeObjeto	= 5
	}

	private enum Estado
	{
		Avanzar		= 0,
		Retroceder	= 1,
		Detenerse	= 2,
		Girar		= 3
	}

	private Estado estadoActual;
	private Percepcion percepcionActual;

	private void Start()
	{
		sensor = GetComponent<Sensores>();
		actuador = GetComponent<Actuadores>();
		estadoActual = Estado.Avanzar;
	}

	void FixedUpdate()
	{
		if (sensor.Bateria() <= 0)
			return;

		percepcionActual = PercibirMundo();
		estadoActual = TablaDeTransicion(estadoActual, percepcionActual);
		AplicarEstado(estadoActual);
	}

	Estado TablaDeTransicion(Estado estado, Percepcion percepcion)
	{
		switch (estado)
		{
			case Estado.Avanzar:
				switch (percepcion)
				{
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.NoCercaDeObjeto:
						estado = Estado.Avanzar;
						break;
					case Percepcion.CercaDeObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoFrenteAObjeto:
						estado = Estado.Avanzar;
						break;
				}
				break;
			
			case Estado.Detenerse:
				switch (percepcion)
				{
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.NoCercaDeObjeto:
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.CercaDeObjeto:
						estado = Estado.Girar;
						break;
					case Percepcion.NoFrenteAObjeto:
						estado = Estado.Girar;
						break;
				}
				break;
			
			case Estado.Girar:
				switch (percepcion)
				{
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.NoCercaDeObjeto:
						estado = Estado.Avanzar;
						break;
					case Percepcion.CercaDeObjeto:
						estado = Estado.Girar;
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Detenerse; 
						break;
					case Percepcion.NoFrenteAObjeto:
						estado = Estado.Girar;
						break;
				}
				break;
		}

		return estado;
	}

	void Avanzar()
	{ 
		actuador.Flotar();
		actuador.Adelante();
	}
	
	void Atras()
	{
		actuador.Flotar();
		actuador.Atras();
	}
	
	void Detenerse() 
	{ 
		actuador.Flotar(); 
		actuador.Detener();
	}
	
	void Girar() 
	{ 
		actuador.Flotar(); 
		actuador.GirarDerecha();
	}
	
	
	Percepcion PercibirMundo() 
	{ 
		Percepcion percepcionActual = Percepcion.NoFrenteAObjeto;
		//if (sensor.CercaDePared())
		//	percepcionActual = Percepcion.CercaDePared;
		if (!sensor.CercaDePared())
		{
			percepcionActual = Percepcion.NoCercaDePared;
			Debug.Log("No Cerca de Pared!");
		}

		if (sensor.CercaDeObjeto())
		{
			percepcionActual = Percepcion.CercaDeObjeto;
			Debug.Log("Cerca de Objeto");
		}

		if (!sensor.CercaDeObjeto())
		{
			percepcionActual = Percepcion.NoCercaDeObjeto;
			Debug.Log("No Cerca de Objeto");
		}

		if (sensor.FrenteAObjeto())
		{
			percepcionActual = Percepcion.FrenteAObjeto;
			Debug.Log("Frente a Objeto");
		}

		if (!sensor.FrenteAObjeto())
		{
			percepcionActual = Percepcion.NoFrenteAObjeto;
			Debug.Log("No Fremte a Objeto");
		}

		return percepcionActual;
	}
	
	void AplicarEstado(Estado estado) 
	{ 
		switch (estado) 
		{ 
			case Estado.Avanzar: 
				Avanzar(); 
				break;
			case Estado.Retroceder:
				Atras();
				break;
			case Estado.Detenerse: 
				Detenerse(); 
				break;
			case Estado.Girar: 
				Girar(); 
				break;
			default: 
				Detenerse(); 
				break;
		}
	}
}