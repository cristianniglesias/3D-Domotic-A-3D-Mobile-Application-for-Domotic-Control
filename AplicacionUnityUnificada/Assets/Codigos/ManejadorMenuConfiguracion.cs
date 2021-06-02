using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManejadorMenuConfiguracion : MonoBehaviour
{

    private Dropdown barritaHabitacion;
    private Dropdown barritaObjeto;
    private Dropdown barritaNode;
    private Toggle radioDesconfigurado;
    private GameObject panelAsociado;
    private GameObject panelNoAsociado;
    private Text LabelNodo;
    private Objeto objAModificar;
    private InputField[] inputIP;
    private GameObject panelConfiguracion;

    private GameObject ventanaCargando;// Venata para mostrar que estan cargando los nodes (esperando Respuestas)
    
    // Start is called before the first frame update
    void Start()
    {

    }
    public void inicializarPanelConfiguracion()//Se lo llama cuando se toca el boton Configuracion, en el panel Con Casa
    {
        panelConfiguracion = GameObject.Find("PanelConfiguracion");
        panelAsociado = panelConfiguracion.transform.Find("PanelAsociado").gameObject;//Me quedo con la referencia al panel Asociado
        panelNoAsociado = panelConfiguracion.transform.Find("PanelNoAsociado").gameObject;//Me quedo con la referencia al panel no Asociado
        barritaHabitacion = panelConfiguracion.transform.Find("DropdownHabitaciones").GetComponent<Dropdown>();//Me quedo con la referencia a la barrita
        barritaHabitacion.ClearOptions();//Vacio la barrita
        barritaObjeto = panelConfiguracion.transform.Find("DropdownObjetos").GetComponent<Dropdown>();//Me quedo con la referencia a la barrita
        barritaObjeto.ClearOptions();//Vacio la barrita
        barritaNode = panelNoAsociado.transform.Find("DropdownNodes").GetComponent<Dropdown>();//Me quedo con la referencia a la barrita
        barritaNode.ClearOptions();//Vacio la barrita
        radioDesconfigurado = panelConfiguracion.transform.Find("RadioButton").transform.Find("ToggleDesconfigurados").GetComponent<Toggle>();
        inputIP = new InputField[4];
         //Vieja version de la Ventana Cargando 
        /*
        ventanaCargando = GameObject.Find("PanelConfiguracion").transform.Find("Ventana Cargando").gameObject;
        ventanaCargando.SetActive(false);
        */
        cargarInputIP();
        manejadorBotonRecargar();//Si tiene un IP valido, busca Nodes sin relacion
        llenarBarritaHabitacion();
        llenarBarritaObjeto(radioDesconfigurado.isOn);
    }
    private void cargarInputIP()//Busco los Inputs, y si la casa ya tiene una ip, la muestro
    {
        inputIP[0] = GameObject.Find("InputIP0").GetComponent<InputField>();
        inputIP[1] = GameObject.Find("InputIP1").GetComponent<InputField>();
        inputIP[2] = GameObject.Find("InputIP2").GetComponent<InputField>();
        inputIP[3] = GameObject.Find("InputIP3").GetComponent<InputField>();
        if(VariablesGlobales.Instance.casaActual.IPBroker[0]!=-1)
        {
            inputIP[0].text = VariablesGlobales.Instance.casaActual.IPBroker[0].ToString();
            inputIP[1].text = VariablesGlobales.Instance.casaActual.IPBroker[1].ToString();
            inputIP[2].text = VariablesGlobales.Instance.casaActual.IPBroker[2].ToString();
            inputIP[3].text = VariablesGlobales.Instance.casaActual.IPBroker[3].ToString();
        }
        else
        {
            inputIP[0].text = "";
            inputIP[1].text = "";
            inputIP[2].text = "";
            inputIP[3].text = "";
        }
    }


    //Codigos Panel Configuracion

    public void manejadorBotonAceptarIP()//Cuando se quiere Asociar el IP a la casa
    {
        controlIPBroker();//Controla si el IP debe aosciarlo a la casa o no
        manejadorBotonRecargar();//Recarga la lista de Nodes
    }

    private void controlIPBroker()//Hace el chekeo de que Los inputs del IP. Esten todos escritos y sean validos
    {//Ademas Hace la conexion con el broker si puede, le asiga a la casa el IP y la guarda
        if (!inputIP[0].text.Equals("") && !inputIP[1].text.Equals("") && !inputIP[2].text.Equals("") && !inputIP[3].text.Equals(""))
        {
            int[] numeros = new int[4];
            for (int i = 0; i < 4; i++)
            {
                numeros[i] = int.Parse(inputIP[i].text);
            }
            if ((numeros[0] >= 0 && numeros[0] <= 255) && (numeros[1] >= 0 && numeros[1] <= 255) && (numeros[2] >= 0 && numeros[2] <= 255) && (numeros[3] >= 0 && numeros[3] <= 255))
            {
                Debug.Log("IP VALIDO");
                Debug.Log(" IP0 " + numeros[0] + " IP1 " + numeros[1] + " IP2 " + numeros[2] + " IP3 " + numeros[3]);

                if (VariablesGlobales.Instance.auxiliarMQTT.conexionBroker(numeros))
                {
                    VariablesGlobales.Instance.casaActual.IPBroker = numeros;
                    ManejadorArchivos.Guardar(VariablesGlobales.Instance.casaActual);
                }
            }
            else
            {
                VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ALERTA, "IP Invalido", "IP Invalido.\nUno o mas campos son invalidos (los valores deben ser de 0 a 255).");
            }
        }
        else
        {
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ALERTA, "IP Invalido", "Debe llenar todos los campos de IPBroker.");
        }
    }

    public void manejadorBotonBorrarIP()//Cuando se quiere Borrar el IP a la casa, se debe bajar de todos los topicos
    {
        VariablesGlobales.Instance.casaActual.IPBroker[0] = -1;
        VariablesGlobales.Instance.casaActual.IPBroker[1] = 0;
        VariablesGlobales.Instance.casaActual.IPBroker[2] = 0;
        VariablesGlobales.Instance.casaActual.IPBroker[3] = 0;
        VariablesGlobales.Instance.auxiliarMQTT.limpiarMQTT();
        cargarInputIP();
        ManejadorArchivos.Guardar(VariablesGlobales.Instance.casaActual);
    }


    public void manejadorBotonAceptar()//Cuando se quiere hacer un linkeo
    {
        if (!barritaNode.options[barritaNode.value].text.Equals("Ninguno"))//Si le selecciono una opcion que no sea Ninguno
        {
            if (barritaObjeto.options.Count!=0)
            {
                objAModificar.IDObjetoFisico = barritaNode.options[barritaNode.value].text;
                string[] textos = new string[3];
                textos[0] = VariablesGlobales.Instance.casaActual.nombre;
                textos[1] = VariablesGlobales.Instance.casaActual.habitaciones[barritaHabitacion.value].nombre;
                textos[2] = objAModificar.nombre;
                string TopicoIDObjeto = VariablesGlobales.Instance.auxiliarMQTT.armarTopico(textos);
                VariablesGlobales.Instance.auxiliarMQTT.enviarMensaje(TopicoIDObjeto, "Config/" + objAModificar.IDObjetoFisico);
                manejadorCambioBarritaObjeto();
                ManejadorArchivos.Guardar(VariablesGlobales.Instance.casaActual);
                VariablesGlobales.Instance.listaNodes.RemoveAt(barritaNode.value - 1);
                llenarBarritaNode();
            }
            else
            {
                VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ALERTA, "Error al asociar objeto", "No hay objeto para asociar.");
            }
        }
        else
        {
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ALERTA, "IP Invalido", "Debe colocar un ID para asociar la relacion.");
        }
    }

    public void manejadorBotonBorrarLink()//Cuando se quiere borrar un linkeo
    {
        VariablesGlobales.Instance.auxiliarMQTT.enviarMensaje("Reset", "Config/" + objAModificar.IDObjetoFisico);
        objAModificar.IDObjetoFisico = "";
        manejadorCambioBarritaObjeto();
        ManejadorArchivos.Guardar(VariablesGlobales.Instance.casaActual);
    }

    public void manejadorRadioDesconfigurados()//Cuando se cambia el valor del RadioButton Desconfigurados
    {
        llenarBarritaObjeto(radioDesconfigurado.isOn);
    }

    public void manejadorCambioBarritaHabitacion()//Cuando se cambia el valor en la barra de Habitacion
    {
        llenarBarritaObjeto(radioDesconfigurado.isOn);
    }

    public void manejadorCambioBarritaObjeto()//Cuando se cambia el valor en la barra de Objetos
    {
        objAModificar = busquedaObjeto();
        if (!objAModificar.IDObjetoFisico.Equals(""))//Si tiene una Relacion
        {
            panelAsociado.SetActive(true);//Activo Label con el Node Asociado
            panelNoAsociado.SetActive(false);
            GameObject.Find("TextLabelNode").GetComponent<Text>().text = objAModificar.IDObjetoFisico;
        }
        else
        {
            panelNoAsociado.SetActive(true);//Activo la barrita con las opciones del node
            panelAsociado.SetActive(false);
        }
    }

    private void llenarBarritaHabitacion()
    {
        List<string> listaHabitaciones = new List<string>();
        Casa casaActual = VariablesGlobales.Instance.casaActual;
        for (int i = 0; i < casaActual.habitaciones.Count; i++)
        {
            listaHabitaciones.Add(casaActual.habitaciones[i].nombre);
        }
        barritaHabitacion.ClearOptions();
        barritaHabitacion.AddOptions(listaHabitaciones);
        barritaHabitacion.value = 0;
    }

    private void llenarBarritaObjeto(bool opcion) //Opcion falso es todos, true con Filtro
    {
        List<string> listaObjetos = new List<string>();
        Casa casaActual = VariablesGlobales.Instance.casaActual;
        //Agrego Aires
        for (int i = 0; i < casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Aires.Count; i++)
        {
            if (opcion == false || casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Aires[i].IDObjetoFisico.Equals(""))
            {//Si la opcion es todos o no tiene Relacion Fisica asociada
                listaObjetos.Add(casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Aires[i].nombre);
            }
        }
        //Agrego Lamparas
        for (int i = 0; i < casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Lamparas.Count; i++)
        {
            if (opcion == false || casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Lamparas[i].IDObjetoFisico.Equals(""))
            {//Si la opcion es todos o no tiene Relacion Fisica asociada
                listaObjetos.Add(casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Lamparas[i].nombre);
            }
        }
        //Agrego Luces
        for (int i = 0; i < casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Luces.Count; i++)
        {
            if (opcion == false || casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Luces[i].IDObjetoFisico.Equals(""))
            {//Si la opcion es todos o no tiene Relacion Fisica asociada
                listaObjetos.Add(casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Luces[i].nombre);
            }
        }
        //Agrego Televisores
        for (int i = 0; i < casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Televisores.Count; i++)
        {
            if (opcion == false || casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Televisores[i].IDObjetoFisico.Equals(""))
            {//Si la opcion es todos o no tiene Relacion Fisica asociada
                listaObjetos.Add(casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Televisores[i].nombre);
            }
        }
        //Agrego VentiladoresPiso
        for (int i = 0; i < casaActual.habitaciones[barritaHabitacion.value].listaObjetos.VentiladoresPiso.Count; i++)
        {
            if (opcion == false || casaActual.habitaciones[barritaHabitacion.value].listaObjetos.VentiladoresPiso[i].IDObjetoFisico.Equals(""))
            {//Si la opcion es todos o no tiene Relacion Fisica asociada
                listaObjetos.Add(casaActual.habitaciones[barritaHabitacion.value].listaObjetos.VentiladoresPiso[i].nombre);
            }
        }
        //Agrego VentiladoresTecho
        for (int i = 0; i < casaActual.habitaciones[barritaHabitacion.value].listaObjetos.VentiladoresTecho.Count; i++)
        {
            if (opcion == false || casaActual.habitaciones[barritaHabitacion.value].listaObjetos.VentiladoresTecho[i].IDObjetoFisico.Equals(""))
            {//Si la opcion es todos o no tiene Relacion Fisica asociada
                listaObjetos.Add(casaActual.habitaciones[barritaHabitacion.value].listaObjetos.VentiladoresTecho[i].nombre);
            }
        }
        barritaObjeto.ClearOptions();
        barritaObjeto.AddOptions(listaObjetos);
        barritaObjeto.value = 0;

        llenarBarritaNode();

        if (barritaObjeto.options.Count != 0)//Si no es vacia la lista de objetos
        {//La vista de paneles Asociado o No Asociado depende del objeto evaluado
            manejadorCambioBarritaObjeto();
        }
        else
        {//Debe verse solo el panel No Asociado, el de la lista de  Nodes
            panelNoAsociado.SetActive(true);
            panelAsociado.SetActive(false);
        }

    }

    private void llenarBarritaNode()
    {
        List<string> listaNode = new List<string>();
        listaNode.Add("Ninguno");//Para que la barrita tenga la opcion de ninguno
        for (int i = 0; i < VariablesGlobales.Instance.listaNodes.Count; i++)
        {
           listaNode.Add(VariablesGlobales.Instance.listaNodes[i]);
        }
        barritaNode.ClearOptions();
        barritaNode.AddOptions(listaNode);
        barritaNode.value = 0;       
    }

    private int numeroObjeto(string nombre)//Devuelve el numero del objeto, el cual corresponde con su posicion en la lista(de ese tipo de objeto)
    {
        int numero;
        string[] cadena = nombre.Split('-');
        numero = int.Parse(cadena[cadena.Length-1]);
        return numero;
    }

    private Objeto busquedaObjeto()//Devuelve el objeto de la casa, que se esta seleccionando
    {
        Casa casaActual = VariablesGlobales.Instance.casaActual;
        string nombreObjetoSeleccionado = barritaObjeto.options[barritaObjeto.value].text;
        if (nombreObjetoSeleccionado.Contains("Aire"))
        {
            return casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Aires[numeroObjeto(nombreObjetoSeleccionado)];
        }
        if (nombreObjetoSeleccionado.Contains("Lamp"))
        {
            return casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Lamparas[numeroObjeto(nombreObjetoSeleccionado)];
        }
        if (nombreObjetoSeleccionado.Contains("Luz"))
        {
            return casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Luces[numeroObjeto(nombreObjetoSeleccionado)];
        }
        if (nombreObjetoSeleccionado.Contains("TV"))
        {
            return casaActual.habitaciones[barritaHabitacion.value].listaObjetos.Televisores[numeroObjeto(nombreObjetoSeleccionado)];
        }
        if (nombreObjetoSeleccionado.Contains("VentPiso"))
        {
            return casaActual.habitaciones[barritaHabitacion.value].listaObjetos.VentiladoresPiso[numeroObjeto(nombreObjetoSeleccionado)];
        }
        if (nombreObjetoSeleccionado.Contains("VentTecho"))
        {
            return casaActual.habitaciones[barritaHabitacion.value].listaObjetos.VentiladoresTecho[numeroObjeto(nombreObjetoSeleccionado)];
        }

        return null;            
    }

    public void manejadorBotonRecargar()
    {        
        if (VariablesGlobales.Instance.casaActual.IPBroker[0] != -1 && VariablesGlobales.Instance.auxiliarMQTT.client!=null && VariablesGlobales.Instance.auxiliarMQTT.client.IsConnected)
        {
            VariablesGlobales.Instance.listaNodes.Clear();            
            VariablesGlobales.Instance.auxiliarMQTT.enviarMensaje("Registrense", "Detect");//Avisa al broker, de que se registren los Nodes sin relacion
            VariablesGlobales.Instance.auxiliarMQTT.subscribirse("Config");//Escucha en el canal a los Nodes sin relacion            
            int tiempoEspera=3;
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.CARGANDO, "Cargando-"+ tiempoEspera, "Cargando");
            StartCoroutine(esperarNodes(tiempoEspera));//Espero a que los Nodes Respondan             
        }
    }
    
    IEnumerator esperarNodes(int segundos)//Sin los comentarios, es la vieja vision de la ventana
    {        
       /* ventanaCargando.SetActive(true);//Muestro Ventana Cargando
        Slider cargando = GameObject.Find("SliderCargando").GetComponent<Slider>();
        cargando.value = 0;*/
        for (int i = 1; i <= segundos; i++)
        {
            
            yield return new WaitForSeconds(1);//Espero un segundo y le agrego un "." al texto
            /*ventanaCargando.GetComponentInChildren<Text>().text = ventanaCargando.GetComponentInChildren<Text>().text + ".";
            cargando.value = (float)i / segundos;*/
        }/*
        ventanaCargando.GetComponentInChildren<Text>().text = "Cargando";//Reseteo el texto
        ventanaCargando.SetActive(false);//Oculto la Ventana Cargando*/
        llenarBarritaNode();//Lleno la barrita       
    }


    public void manejadorBotonGuardar()//En desuso, por el momento
    {
        ManejadorArchivos.Guardar(VariablesGlobales.Instance.casaActual);
        llenarBarritaObjeto(radioDesconfigurado.isOn);
    }

    public void manejadorBotonVolver()
    {
        //Por ahora todo cambio hecho en el menu configuracion, se guarda al instante
        //VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.GUARDAR, "Esta Saliendo de la Configuracion", "Quiere guardar los cambios realizados.");
    }

    public void manejadorBotonAyuda()
    {
        /*EditorUtility.DisplayDialog("Ayuda", "Coloque el IP del Broker\nSeleccione un elemento dentro de la APP para asociarle el ID del Fisico.", "Entendido");*/
        VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.AYUDA, "Ayuda para Configurar", "Coloque el IP del Broker\nSeleccione un elemento dentro de la APP para asociarle el ID del Fisico.");
    }




    // Update is called once per frame
    void Update()
    {

    }
}
