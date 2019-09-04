using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportamientoAutomatico : MonoBehaviour
{
	// Componente adicional para obtener informacon de los sensores.
	private Sensores sensor; 
	
	// Componente adicional para obtener informacion de los actuadores.
	private Actuadores actuador; 
	
	// Velocidad que sera utilizada durante el retorno del dron a la base de carga.
	private float speed = 0.001F; 
	
	// Variable que servira para conocer el tiempo inicial una vez que el dron deba regresar a la base
	private float startTime; 

	// Variable que servira para guardar la distancia que habra entre el dron y la base de carga, segun el punto
	// donde se encuentre el dron en dicho instante.
	private float journeyLength;
	
	/*
	 * Percepciones que el dron "tendrá", sirven para conocer el mundo y poder tomar una decicion para cambiar de
	 * estado dentro de la maquina de estados.
	 */
	private enum Percepcion
	{
		CercaDePared	= 0,
		NoCercaDePared	= 1,
		FrenteAPared	= 2,
		NoFrenteAPared	= 3,
		CercaDeObjeto	= 4,
		FrenteAObjeto	= 5,
		NoCercaDeObjeto	= 6,
		BateriaBaja		= 7,
		TocandoBase		= 8
	}

	/**
	 * Conjunto de estados que forman parte de la maquina de estados y
	 * a los cuales puede moverse el dron con base a las percepciones.
	 */
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
		
		// Guardamos el tiempo en la variable de startTime.
		startTime = Time.time;
		// Calculamos la distancia que hay entre la ubicacion del dron (sensor.Ubicacion()) y la
		// ubicacion de la base de carga (sensor.UbicacionBase()).
		journeyLength = Vector3.Distance(sensor.Ubicacion(), sensor.UbicacionBase());
	}

	void FixedUpdate()
	{
		// Si por alguna razon la bateria no se carga y esta llega a 0, el dron se detiene y no realiza
		// ya ninguna accion.
		if (sensor.Bateria() <= 0)
		{
			return;
		}
		
		// Una vez que el dron detecta un nivel de bateria bajo, 30 para nuestro caso, inicia el retorno hacia la base
		// de carga, usando la funcion auxiliar de Lerp que se encarga de hacer una interpolacion entre un vector
		// que representa una posicion inicial, la ubicacion del dron para nuestro caso, y otro vector que represente
		// la posicion final, la ubicacion de la base de carga para nosotros. Gracias a esto, Lerp realiza movimientos
		// suavizados para que no parezca que el dron se teletransporta.
		// Hacer uso de este metodo es valido ya que desde un principio el dron tiene conocimiento de donde se encuentra
		// su base de carga.
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

	// A partir de este punto se representa un agente basado en modelos.
	// La idea es similar a crear una máquina de estados finita donde se hacen las siguientes consideraciones:
	// - El alfabeto es un conjunto predefinido de percepciones hechas con sensores del agente
	// - El conjunto de estados representa un conjunto de métodos con acciones del agente
	// - La función de transición es un método
	// - El estado inicial se inicializa en Start()
	// - El estado final es opcional
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
					
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA BATERIA
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
					
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA BATERIA
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
					
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA BATERIA
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
					
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA BATERIA
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
					
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA BATERIA
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
					
					// COMPORTAMIENTO CON BASE A LAS PERCEPCIONES DE LA BATERIA
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

	/// <summary>
	/// Los siguientes metodos sirven para aplicar las acciones relacionadas al mismo nombre del metodo,
	/// todos aplican el estado de flotar para que el dron no se caiga o ascienda al "infinito".
	///
	/// El metodo de Retornar() detiene todas las acciones del dron mientras la funcion de Vector3.Lerp() regresa al
	/// dron a la base de carga y evitar que otros movimientos del dron interfieran durante el proceso.
	/// </summary>
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
		// Nos apoyamos en que el dron no puede superar una altura
		// y evitar que se eleve mas de lo que deseamos.
		if (transform.position.y >= 1.84)
		{
			actuador.Detener();
		}
		else
			actuador.Ascender();
	}
	
	
	/// <summary>
	/// Dentro de este metodo se contemplan todas las percepciones que tiene el dron y con base a ellas
	/// se mueve dentro de la maquina de estados. 
	/// </summary>
	/// <returns>percepcionActual, la percepcion del mundo que tiene el dron</returns>
	Percepcion PercibirMundo() 
	{ 
		// La percepcion inicial que el dron tiene.
		Percepcion percepcionActual = Percepcion.NoCercaDePared;
		
		if (sensor.CercaDePared())
			percepcionActual = Percepcion.CercaDePared;

		if (sensor.FrenteAPared())
			percepcionActual = Percepcion.FrenteAPared;
		
		if (!sensor.FrenteAPared())
			percepcionActual = Percepcion.NoFrenteAPared;
		
		if (!sensor.CercaDePared())
			percepcionActual = Percepcion.NoCercaDePared;
		
		if (sensor.CercaDeObjeto())
			percepcionActual = Percepcion.CercaDeObjeto;

		if (sensor.FrenteAObjeto())
			percepcionActual = Percepcion.FrenteAObjeto;
		
		if (sensor.Bateria() < 50)
			percepcionActual = Percepcion.BateriaBaja;

		if (sensor.TocandoBase())
			percepcionActual = Percepcion.TocandoBase;

		return percepcionActual;
	}
	
	/// <summary>
	/// Segun el estado que se indique, el dron aplicara una accion la cual se encuentra definida en los metodos del
	/// mismo nombre que el estado.
	/// </summary>
	/// <param name="estado">El estado actual</param>
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
