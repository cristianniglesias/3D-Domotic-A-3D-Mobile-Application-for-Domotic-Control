using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.IO;


public class mqtt : MonoBehaviour
{

    public MqttClient client;
    public string brokerIP;
    public List<Objeto> listaObjetos;
    string path;
    public float timer = 0.0f;
    private float timerStart = 0.0f;
    private float timerFinish = 0.0f;
    private float timerTotal = 0.0f;


    void Start()
    {
        path = Application.dataPath + "/latenciaDeVueltaMQTT.txt";
    }

    void Update()
    {
        timer += Time.deltaTime;
    }
    void CreateText()
    {

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "Latencia de vuelta \n\n");
        }
        File.AppendAllText(path, timerTotal.ToString());
        File.AppendAllText(path, " \n");

    }


    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {


        //VariablesGlobales.Instance.msj= Encoding.UTF8.GetString(e.Message);
        //  Debug.Log("MSJ EN MQTT " + VariablesGlobales.Instance.msj);
        // Debug.Log("Mensaje = " + Encoding.UTF8.GetString(e.Message) + " - Topico " + e.Topic);
        if (e.Topic.Contains("Config"))//Si el topico es de config, entonces recibo MACs de los Nodes
        {
            string topico = e.Topic;
            //string[] macNode = topico.Split('/');// Topico: /Config --mensaje---> MACNode Tipo de nodo (Aire, Lampara, TV, Ventilador)
            VariablesGlobales.Instance.listaNodes.Add(Encoding.UTF8.GetString(e.Message));
        }

        if (e.Topic.Contains("informacion"))//Si el topico es de informacion, pertenece a un node fisico
        {
            timerFinish = timer;
            timerTotal = (timerFinish - timerStart) * 1000;
            CreateText();
            timerStart = timer;
            timerFinish = 0.0f;
            timerFinish = 0.0f;
            VariablesGlobales.Instance.topico = e.Topic;
            VariablesGlobales.Instance.msj = Encoding.UTF8.GetString(e.Message);
            analizarTopico();
        }

    }

    void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
    {
        //  Debug.Log("ID Mensaje = " + e.MessageId + " - Publicado = " + e.IsPublished);
    }

    // Use this for initialization



    public bool conexionBroker(int[] IP)
    {
        if (IP[0] != -1)
        {
            brokerIP = IP[0] + "." + IP[1] + "." + IP[2] + "." + IP[3];
            // create client instance 
            client = new MqttClient(brokerIP);
            // register to message received 
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; //Registro el manejador del evento de mensaje recibido
            client.MqttMsgPublished += client_MqttMsgPublished; //Registro el manejador del evento de publicacion de un mensaje al broker

            string clientId = Guid.NewGuid().ToString();
            try
            {//Intento hacer conexion
                byte codigo = client.Connect(clientId);
                if (codigo == 0)
                {
                    VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.BIEN, "Conexión al Broker", "Conexión exitosa al Broker: " + brokerIP);
                    StartCoroutine(MonitoreoNodos(3, 20));//Inicio Rutina de Monitoreo                    
                }
                else
                {
                    VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ERROR, "Conexión al Broker", "Error en la conexión - Codigo: " + codigo);
                    return false;
                }
                return true;
            }
            catch//Si da cualquier excepcion (pensado para error en la conexion, "Socket"), saltar ventana de error
            {
                VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ERROR, "Conexión al Broker", "Error en la conexión, no accedo a esa IP");
                client = null;//El cliente se vuelve nulo
                return false;
            }
        }
        else
        {
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ALERTA, "Error de IP", "Esta casa no tiene IP asociado.");
            return false;
        }
    }

    public string armarTopico(string[] textos)
    {
        string topico = "";
        for (int i = 0; i < textos.Length - 1; i++)
        {
            topico = topico + textos[i] + "/";
        }
        topico = topico + textos[textos.Length - 1];
        return topico;
    }

    public bool enviarMensaje(string mensaje, string topico)
    {
        if (client != null)
        {
            client.Publish(topico, // topic
                Encoding.UTF8.GetBytes(mensaje), // message body
                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                false); // retained
            return true;
        }
        else
        {
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ERROR, "NULO", "Este EnviarMensaje dio nulo.\nTopico: " + topico);
            return false;
        }
    }

    public bool subscribirse(string topico)
    {
        if (client != null)
        {
            client.Subscribe(new string[] { topico }, // topic
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });// QoS level 

            // client.Subscribe(new string[] { "informacion/cristian/Habitacion0/Lamp-0" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            return true;

        }
        else
        {
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ERROR, "NULO", "Esta Subscripcion dio nulo.\nTopico: " + topico);
            return false;
        }

    }

    public void limpiarMQTT()
    {
        if (client != null)
        {
            client.Disconnect();
            client = null;
            brokerIP = null;
        }
    }

    public List<Objeto> cargarTodosObjetos(Habitacion hab)
    {//Cargo una lista con todos los objetos de la habitacion
        List<Objeto> listaObjetos2 = new List<Objeto>();
        for (int j = 0; j < hab.listaObjetos.Aires.Count; j++)
        {
            listaObjetos2.Add(hab.listaObjetos.Aires[j]);
        }
        for (int j = 0; j < hab.listaObjetos.Lamparas.Count; j++)
        {
            listaObjetos2.Add(hab.listaObjetos.Lamparas[j]);
        }
        for (int j = 0; j < hab.listaObjetos.Luces.Count; j++)
        {
            listaObjetos2.Add(hab.listaObjetos.Luces[j]);
        }
        for (int j = 0; j < hab.listaObjetos.Televisores.Count; j++)
        {
            listaObjetos2.Add(hab.listaObjetos.Televisores[j]);
        }
        for (int j = 0; j < hab.listaObjetos.VentiladoresPiso.Count; j++)
        {
            listaObjetos2.Add(hab.listaObjetos.VentiladoresPiso[j]);
        }
        for (int j = 0; j < hab.listaObjetos.VentiladoresTecho.Count; j++)
        {
            listaObjetos2.Add(hab.listaObjetos.VentiladoresTecho[j]);
        }
        return listaObjetos2;
    }

    public List<Objeto> cargarTodosObjetosParticular(Habitacion hab, string nom)
    {//Cargo una lista con todos los objetos especifico de la habitacion

        //List<Objeto> listaObjetos = new List<Objeto>();
        // Debug.Log("nom= " + nom);
        //Se probo solo con Lamparas, deberia verificar cual es el nombre nom.Contains("Lamp") con que deberia comparar cada if para su correcta verificacion de los demas elementos
        if (nom.Contains("AireAcondicionado"))
        {
            for (int j = 0; j < hab.listaObjetos.Aires.Count; j++)
            {
                listaObjetos.Add(hab.listaObjetos.Aires[j]);
            }
        }
        if (nom.Contains("LamparaPref") || nom.Contains("Lamp-"))
        {
            //   Debug.Log("Cargando lista lamparas");
            for (int j = 0; j < hab.listaObjetos.Lamparas.Count; j++)
            {
                listaObjetos.Add(hab.listaObjetos.Lamparas[j]);
            }
        }
        if (nom.Contains("LamparaParedPref"))
        {
            for (int j = 0; j < hab.listaObjetos.Luces.Count; j++)
            {
                listaObjetos.Add(hab.listaObjetos.Luces[j]);
            }
        }
        if (nom.Contains("Televisor"))
        {
            for (int j = 0; j < hab.listaObjetos.Televisores.Count; j++)
            {
                listaObjetos.Add(hab.listaObjetos.Televisores[j]);
            }
        }
        if (nom.Contains("VentiladorPiso"))
        {
            for (int j = 0; j < hab.listaObjetos.VentiladoresPiso.Count; j++)
            {
                listaObjetos.Add(hab.listaObjetos.VentiladoresPiso[j]);
            }
        }
        if (nom.Contains("VentiladorTecho"))
        {
            for (int j = 0; j < hab.listaObjetos.VentiladoresTecho.Count; j++)
            {
                listaObjetos.Add(hab.listaObjetos.VentiladoresTecho[j]);
            }
        }
        return listaObjetos;
    }


    IEnumerator MonitoreoNodos(int segundosEsperaNodes, int segundosDeMonitoreo)
    {
        MqttClient miClient = client;
        while (client != null && miClient == client)//Mientras el cliente sea distinto de null y a su vez sea el mismo cliente que creo el monitoreo
        {//Si no estoy en el menu principal (pensado para que no interrumpa el Modo Configuracion)            
            if (!SceneManager.GetActiveScene().name.Equals("MenuPrincipal"))
            {
                List<Habitacion> listaHabitaciones = VariablesGlobales.Instance.casaActual.habitaciones;
                VariablesGlobales.Instance.listaNodes.Clear();
                VariablesGlobales.Instance.auxiliarMQTT.enviarMensaje("Registrense", "Detect");//Avisa al broker, de que se registren los Nodes sin relacion
                VariablesGlobales.Instance.auxiliarMQTT.subscribirse("Config");//Escucha en el canal a los Nodes sin relacion
                yield return new WaitForSeconds(segundosEsperaNodes);
                for (int i = 0; i < VariablesGlobales.Instance.listaNodes.Count; i++)
                {
                    int j = 0;
                    bool exito = false;
                    while (j < listaHabitaciones.Count && exito == false)
                    {
                        int k = 0;
                        List<Objeto> listaObjetos = cargarTodosObjetos(listaHabitaciones[j]);//Cargo una lista con todos los objetos de la habitacion j
                        while (k < listaObjetos.Count && exito == false)
                        {
                            if (VariablesGlobales.Instance.listaNodes[i].Equals(listaObjetos[k].IDObjetoFisico))
                            {
                                exito = true;
                            }
                            else
                            {
                                k++;
                            }
                        }
                        if (exito == true)
                        {
                            string[] textos = new string[3];
                            textos[0] = VariablesGlobales.Instance.casaActual.nombre;
                            textos[1] = listaHabitaciones[j].nombre;
                            textos[2] = listaObjetos[k].nombre;
                            string TopicoIDObjeto = VariablesGlobales.Instance.auxiliarMQTT.armarTopico(textos);
                            VariablesGlobales.Instance.auxiliarMQTT.enviarMensaje(TopicoIDObjeto, "Config/" + VariablesGlobales.Instance.listaNodes[i]);
                            //  Debug.Log("SE GENERO EL TOPICO DE INFORMACIONNN " + TopicoIDObjeto);
                            subscribirse("informacion/" + TopicoIDObjeto);

                        }
                        j++;
                    }
                }
            }
            yield return new WaitForSeconds(segundosDeMonitoreo);
        }
    }


    private void analizarTopico()
    {

        if (VariablesGlobales.Instance.topico != "")
        {
            string[] words = VariablesGlobales.Instance.topico.Split('/'); //posicion=> 0=informacion 1=nombrecasa 2=nombrehabitacion 3=nombreobjeto
            VariablesGlobales.Instance.casaAux = ManejadorArchivos.Cargar(words[1]);
            List<Habitacion> listaHabitaciones = VariablesGlobales.Instance.casaAux.habitaciones;
            int numHabitacion = VariablesGlobales.Instance.casaAux.buscarHabitacion(words[2]);
            int indice = calculoIndice(words[3]);
            listaObjetos = VariablesGlobales.Instance.auxiliarMQTT.cargarTodosObjetosParticular(listaHabitaciones[indice], words[3]);
            listaObjetos[indice].MsjNodeEstado = VariablesGlobales.Instance.msj;
            VariablesGlobales.Instance.topico = "";
            VariablesGlobales.Instance.msj = "";
        }
    }

    private int calculoIndice(string nombre)//Saca del nombre el ultimo numero presente despues de '-'
    {   //. Asi se puede calcular el indice que corresponde dentro de un abjeto "AireAcondicionadoPrefab(Clone)-4"
        string[] partesText = nombre.Split('-');
        int indice = int.Parse(partesText[partesText.Length - 1]);
        return indice;
    }



}
