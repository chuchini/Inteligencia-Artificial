using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportamientoAutomatico : MonoBehaviour
{

	private Sensores sensor;
	private Actuadores actuador;

	private float speed = 0.001F;

	private float startTime;

	private float journeyLength;
	
	private enum Percepcion
	{
		CercaDePared	= 0,
		NoCercaDePared	= 1,
		FrenteAPared	= 2,
		NoFrenteAPared	= 3,
		CercaDeObjeto	= 4,
		FrenteAObjeto	= 5,
		NoFrenteAObjeto	= 6,
		NoCercaDeObjeto	= 7,
		BateriaBaja		= 8,
		//NoBateriaBaja	= 9,
		TocandoBase		= 9,
		//NoTocandoBase	= 11
	}

	private enum Estado
	{
		Avanzar		= 0,
		Retroceder	= 1,
		Detenerse	= 2,
		Girar		= 3,
		Retornar	= 4,
		Cargarse	= 5,
		Ascender	= 6
	}

	private Estado estadoActual;
	private Percepcion percepcionActual;

	private void Start()
	{
		sensor = GetComponent<Sensores>();
		actuador = GetComponent<Actuadores>();
		estadoActual = Estado.Avanzar;

		startTime = Time.time;
		
		journeyLength = Vector3.Distance(sensor.Ubicacion(), sensor.UbicacionBase());
	}

	void FixedUpdate()
	{
		if (sensor.Bateria() <= 0)
		{
			return;
		}
		
		if (sensor.Bateria() <= 50)
		{
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(sensor.Ubicacion(), sensor.UbicacionBase(), fracJourney);
			Debug.Log("Estoy atorado aqui");
		}

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
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA PARED
					case Percepcion.CercaDePared:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrenteAPared:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoFrenteAPared:
						estado = Estado.Avanzar;
						break;
						
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DEL OBJETO
					case Percepcion.CercaDeObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoCercaDeObjeto:
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.BateriaBaja:
						estado = Estado.Retornar;
						break;
					case Percepcion.TocandoBase:
						estado = Estado.Cargarse;
						break;
				}
				break;
			
			case Estado.Detenerse:
				switch (percepcion)
				{
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA PARED
					case Percepcion.CercaDePared:
						estado = Estado.Retroceder;
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrenteAPared:
						estado = Estado.Girar;
						break;
					case Percepcion.NoFrenteAPared:
						estado = Estado.Avanzar;
						break;
					
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DEL OBJETO
					case Percepcion.CercaDeObjeto:
						estado = Estado.Girar;
						break;
					case Percepcion.NoCercaDeObjeto:
						estado = sensor.CercaDePared() == false ? Estado.Avanzar : Estado.Retroceder;
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.BateriaBaja:
						estado = Estado.Retornar;
						break;
					case Percepcion.TocandoBase:
						estado = sensor.Bateria() > 50 ? Estado.Ascender : Estado.Cargarse;
						break;
				}
				break;
			
			case Estado.Girar:
				switch (percepcion)
				{
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA PARED
					case Percepcion.CercaDePared:
						estado = Estado.Retroceder; //Antes avanzaba
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrenteAPared:
						estado = Estado.Girar;
						break;
					case Percepcion.NoFrenteAPared:
						estado = Estado.Avanzar;
						break;
					
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DEL OBJETO
					case Percepcion.CercaDeObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoCercaDeObjeto:
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.BateriaBaja:
						estado = Estado.Retornar;
						break;
					case Percepcion.TocandoBase:
						estado = sensor.Bateria() > 50 ? Estado.Avanzar : Estado.Cargarse;
						break;
				}
				break;
			
			case Estado.Retroceder:
				switch (percepcion)
				{
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA PARED
					case Percepcion.CercaDePared:
						estado = Estado.Retroceder;
						break;
					case Percepcion.NoCercaDePared:
						//estado = sensor.FrenteAPared() == false ? Estado.Avanzar : Estado.Girar;
						//Debug.Log("Estoy aqui");
						estado = Estado.Avanzar;
						break;
					case Percepcion.FrenteAPared:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoFrenteAPared:
						estado = Estado.Avanzar;
						break;
			
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DEL OBJETO
					case Percepcion.CercaDeObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.NoCercaDeObjeto:
						estado = sensor.CercaDePared() == false ? Estado.Avanzar : Estado.Retroceder;
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Detenerse;
						break;
					case Percepcion.BateriaBaja:
						estado = Estado.Retornar;
						break;
					case Percepcion.TocandoBase:
						estado = sensor.Bateria() > 50 ? Estado.Avanzar : Estado.Cargarse;
						break;

				}
				break;
			
			case Estado.Retornar:
				switch (percepcion)
				{
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA PARED
					case Percepcion.CercaDePared:
						estado = Estado.Retornar;
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Retornar;
						break;
					case Percepcion.FrenteAPared:
						estado = Estado.Retornar;
						break;
					case Percepcion.NoFrenteAPared:
						estado = Estado.Retornar;
						break;
					
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DEL OBJETO
					case Percepcion.CercaDeObjeto:
						estado = Estado.Retornar;
						break;
					case Percepcion.NoCercaDeObjeto:
						estado = Estado.Retornar;
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Retornar;
						break;
					case Percepcion.BateriaBaja:
						estado = Estado.Retornar;
						break;
					case Percepcion.TocandoBase:
						estado = sensor.Bateria() > 50 ? Estado.Avanzar : Estado.Cargarse;
						break;
				}
				break;
			
			case Estado.Cargarse:
				switch (percepcion)
				{
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA PARED
					case Percepcion.CercaDePared:
						estado = Estado.Retornar;
						break;
					case Percepcion.NoCercaDePared:
						estado = Estado.Retornar;
						break;
					case Percepcion.FrenteAPared:
						estado = Estado.Retornar;
						break;
					case Percepcion.NoFrenteAPared:
						estado = Estado.Retornar;
						break;
					
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DEL OBJETO
					case Percepcion.CercaDeObjeto:
						estado = Estado.Retornar;
						break;
					case Percepcion.NoCercaDeObjeto:
						estado = Estado.Retornar;
						break;
					case Percepcion.FrenteAObjeto:
						estado = Estado.Retornar;
						break;
					case Percepcion.BateriaBaja:
						estado = Estado.Retornar;
						break;
					case Percepcion.TocandoBase:
						estado = Estado.Cargarse;
						estado = sensor.Bateria() < sensor.BateriaMaxima() ? Estado.Cargarse : Estado.Ascender;
						break;
				}
				break;
			
			case Estado.Ascender:
				estado = transform.position.y >= 1.83 ? Estado.Detenerse : Estado.Ascender;
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

	void Retornar()
	{
		actuador.Flotar();
		actuador.Detener();
	}

	void Cargarse()
	{
		actuador.Detener();
		actuador.CargarBateria();
	}

	void Ascender()
	{
		if (transform.position.y >= 1.84)
		{
			actuador.Detener();
		}
		else
			actuador.Ascender();
	}
	
	
	Percepcion PercibirMundo() 
	{ 
		Percepcion percepcionActual = Percepcion.NoCercaDePared;
		if (sensor.CercaDePared())
		{
			percepcionActual = Percepcion.CercaDePared;
			Debug.Log("Cerca de Pared");
		}
                         
		if (sensor.FrenteAPared()) 
		{
			percepcionActual = Percepcion.FrenteAPared; 
			Debug.Log("Frente a Pared");
		}
                                
		if (!sensor.FrenteAPared())
		{ 
			percepcionActual = Percepcion.NoFrenteAPared; 
			Debug.Log("No Frente a Pared");
		}

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

		if (sensor.FrenteAObjeto())
		{
			percepcionActual = Percepcion.FrenteAObjeto;
			Debug.Log("Frente a Objeto");
		}

		if (sensor.Bateria() < 50)
		{
			percepcionActual = Percepcion.BateriaBaja;
			Debug.Log("Bateria Baja");
		}

		if (sensor.TocandoBase())
		{
			percepcionActual = Percepcion.TocandoBase;
			Debug.Log("Tocando Base");
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
			case Estado.Retornar:
				Retornar();
				break;
			case Estado.Cargarse:
				Cargarse();
				break;
			case Estado.Ascender:
				Ascender();
				break;
			default: 
				Detenerse(); 
				break;
		}
	}
}
