using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.IO;




public class ManejadorUtilizacion : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private GameObject seleccion;
    private GameObject panelAireAcondicionado, panelLampara, panelLuz, panelVentiladorPiso, panelVentiladorTecho, panelTelevisor;
    private ManejadorCamarasOtros auxiliar;
    private Vector3 posCam; //posicion de la cámara, antes de que se toque un objeto
    private Vector3 posTouch; //posicion que fue tocada
    private Vector3 posZoom; //posicion a la que realiza el acercamiento
    public List<Objeto> listaObjetos;
    private string lado;
    public int indiceElemento;
    private float timerStart = 0.0f;
    private float timerFinish = 0.0f;
    private float timer = 0.0f;
    int cant = 0;



    // Use this for initialization
    void Start()
    {
        panelAireAcondicionado = GameObject.Find("PanelAireAcondicionado");
        panelAireAcondicionado.SetActive(false);
        panelLampara = GameObject.Find("PanelLampara");
        panelLampara.SetActive(false);
        panelLuz = GameObject.Find("PanelLuz");
        panelLuz.SetActive(false);
        panelTelevisor = GameObject.Find("PanelTelevisor");
        panelTelevisor.SetActive(false);
        panelVentiladorPiso = GameObject.Find("PanelVentiladorPiso");
        panelVentiladorPiso.SetActive(false);
        panelVentiladorTecho = GameObject.Find("PanelVentiladorTecho");
        panelVentiladorTecho.SetActive(false);
        auxiliar = GameObject.Find("CodigosModoUtilizacion").GetComponent<ManejadorCamarasOtros>();
        seleccion = null;
        


    }

    public void manejadorBotonAyuda()
    {
        VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ALERTA, "Ayuda para Usar Objetos", "Seleccione el objeto que desee usar.\n Y seleccione accion.");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {                    
                    procesarSeleccion(touch);
                }
            }
        }
        controlarImagenConexion();
       
        if (seleccion != null) {
            listaObjetos = VariablesGlobales.Instance.auxiliarMQTT.cargarTodosObjetosParticular(auxiliar.getHabitacionActiva(), auxiliar.getTipoObjeto(seleccion));
            actualizar();
        }

    }
    private void controlarImagenConexion()
    {        
        if (VariablesGlobales.Instance.casaActual.IPBroker[0] != -1 && VariablesGlobales.Instance.auxiliarMQTT.client != null && VariablesGlobales.Instance.auxiliarMQTT.client.IsConnected)//Pongo imagen, dependiendo si esta conectado o no
        {
            GameObject.Find("ImagenConexion").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icono Conectado");
        }
        else
        {
            GameObject.Find("ImagenConexion").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icono Desconectado");
        }        
    }

    private void procesarSeleccion (Touch tap)
    {
        //Resetear color del elemento seleccionado anterior
        if (seleccion != null)
        {
            Behaviour halo = (Behaviour)seleccion.transform.Find("Cuerpo").GetComponent("Halo");
            halo.enabled = false;
        }
        //Nueva seleccion
        //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ray = Camera.main.ScreenPointToRay(tap.position);
        lado = auxiliar.ordenCamaras[auxiliar.camaraActiva];
        if (Physics.Raycast(ray, out hit, 100.0f)) //Corrobora si el rayo toco un objeto
        {
            //Desactiva todos los paneles
            DesactivarPaneles();
            //Encuentra la referencia al objeto seleccionado
            seleccion = hit.transform.gameObject;
            if (!seleccion.transform.parent.name.Contains("Habitacion") && seleccion.transform.parent.transform.parent.name.Contains(auxiliar.getHabitacionActiva().nombre))
            {   //Si lo seleccionado no es un Piso, Pared o Vertice y corresponde a la habitacion activa
                //Debug.Log(seleccion.ToString());
                //Debug.Log(seleccion.name);
                //Se le activa el Halo
                Behaviour halo = (Behaviour)seleccion.transform.Find("Cuerpo").GetComponent("Halo");
                halo.enabled = true;
                posTouch = new Vector3(seleccion.transform.position.x, 1f, seleccion.transform.position.z);
                posZoom = auxiliar.checkZoom(seleccion.transform.position);
                auxiliar.StartCoroutine(auxiliar.ZoomIn(posZoom, posTouch));

                indiceElemento = calculoIndice(seleccion.name);
                //Debug.Log("indiceElemento= "+ indiceElemento);
                listaObjetos = VariablesGlobales.Instance.auxiliarMQTT.cargarTodosObjetosParticular(auxiliar.getHabitacionActiva(), auxiliar.getTipoObjeto(seleccion));

                mostrarControl(seleccion, true);
               // actualizar();
            }
            else
            {   //Si lo seleccionado no corresponde a un objeto de la habitacion activa, se descarta
                seleccion = null;
                posCam = verificarLado(lado);
                auxiliar.StartCoroutine(auxiliar.MoverCamara(posCam));
            }
        }
        else
        {
            //Si no tocaste nada, descarta el objeto y se oculta el control que este visible
            mostrarControl(seleccion, false);
            seleccion = null;
        }
    }

    private void mostrarControl(GameObject objeto, bool activo)
    {
        if (objeto != null)
        {
            if (objeto.name.Contains("AireAcondicionado"))
            {
                panelAireAcondicionado.SetActive(activo);
            }
            if (objeto.name.Contains("LamparaPref"))
            {
                panelLampara.SetActive(activo);
            }
            if (objeto.name.Contains("LamparaParedPref"))
            {
                panelLuz.SetActive(activo);
            }
            if (objeto.name.Contains("Televisor"))
            {
                panelTelevisor.SetActive(activo);
            }
            if (objeto.name.Contains("VentiladorPiso"))
            {
                panelVentiladorPiso.SetActive(activo);
            }
            if (objeto.name.Contains("VentiladorTecho"))
            {
                panelVentiladorTecho.SetActive(activo);
            }
            //Solo si tengo que mostrar algo, cambio el nombre del texto de ese panel
            if (activo)
            {
               GameObject.Find("TextNombreObjeto").GetComponent<Text>().text = objeto.name.Substring(0, objeto.name.IndexOf("Pref(Clone)")) + '-' + objeto.name.Split('-')[objeto.name.Split('-').Length - 1];
               cambioEstadoTextoPrender(auxiliar.getTipoObjeto(objeto));
            }
        }
    }

    public void DesactivarPaneles()
    {
        List<GameObject> listaPaneles = new List<GameObject>();
        listaPaneles.Add(panelAireAcondicionado);
        listaPaneles.Add(panelLampara);
        listaPaneles.Add(panelLuz);
        listaPaneles.Add(panelTelevisor);
        listaPaneles.Add(panelVentiladorPiso);
        listaPaneles.Add(panelVentiladorTecho);
        auxiliar.desactivarObjetos(listaPaneles);

        //Agregado para que cuando cambie de habitacion, tambien se haga el reseteo del color
        //Resetear color del elemento seleccionado anterior.
        if (seleccion != null)
        {
            Behaviour halo = (Behaviour)seleccion.transform.Find("Cuerpo").GetComponent("Halo");
            halo.enabled = false;
        }
    }

    //Cambia entre "Prender" y "Apagar" el texto del boton, dependiendo el objeto
    //Se descartan los ventiladores
    private void cambioEstadoTextoPrender(string tipoElemento)
    {
        switch (tipoElemento)
        {
            case ("AireAcondicionado"):
                {
                    if (!seleccion.GetComponent<ComportamientoAireAcondicionadoPrefab>().estado)
                    {
                        panelAireAcondicionado.transform.Find("PrenderButton").GetComponentInChildren<Text>().text = "PRENDER";
                        panelAireAcondicionado.transform.Find("PanelPrendidoAire").gameObject.SetActive(false);
                    }
                    else
                    {
                        panelAireAcondicionado.transform.Find("PrenderButton").GetComponentInChildren<Text>().text = "APAGAR";
                        panelAireAcondicionado.transform.Find("PanelPrendidoAire").gameObject.SetActive(true);
                        actualizarTextGrados();
                    }
                    break;
                }
            case ("LamparaPref"):
                {
                    if (!listaObjetos[indiceElemento].estado)
                    {
                        panelLampara.transform.Find("PrenderButton").GetComponentInChildren<Text>().text = "PRENDER";
                        seleccion.GetComponent<ComportamientoLamparaPrefab>().apagarLampara();

                    }
                    else
                    {
                        panelLampara.transform.Find("PrenderButton").GetComponentInChildren<Text>().text = "APAGAR";
                        seleccion.GetComponent<ComportamientoLamparaPrefab>().prenderLampara();
                    }
                    break;
                }
            case ("LamparaParedPref"):
                {
                    if (!listaObjetos[indiceElemento].estado)
                    {
                        panelLuz.transform.Find("PrenderButton").GetComponentInChildren<Text>().text = "Prender";
                        seleccion.GetComponent<ComportamientoLamparaParedPrefab>().apagarLuz();

                    }
                    else
                    {
                        panelLuz.transform.Find("PrenderButton").GetComponentInChildren<Text>().text = "APAGAR";
                        seleccion.GetComponent<ComportamientoLamparaParedPrefab>().apagarLuz();

                    }
                    break;
                }
            case ("Televisor"):
                {
                    if (!seleccion.GetComponent<ComportamientoTelevisionPrefab>().estado)
                    {
                        panelTelevisor.transform.Find("PrenderButton").GetComponentInChildren<Text>().text = "PRENDER";
                        panelTelevisor.transform.Find("PanelPrendidoTV").gameObject.SetActive(false);
                    }
                    else
                    {
                        panelTelevisor.transform.Find("PrenderButton").GetComponentInChildren<Text>().text = "APAGAR";
                        panelTelevisor.transform.Find("PanelPrendidoTV").gameObject.SetActive(true);
                        actualizarTextCanal();
                    }
                    break;
                }
            default:
                break;
        }
    }

    private int calculoIndice(string nombre)//Saca del nombre el ultimo numero presente despues de '-'
    {   //. Asi se puede calcular el indice que corresponde dentro de un abjeto "AireAcondicionadoPrefab(Clone)-4"
        string[] partesText = nombre.Split('-');
        int indice = int.Parse(partesText[partesText.Length - 1]);
        return indice;
    }

    private void enviarTextosTopico(string tipoElemento,string modeloTopico, string textoEnviar)
    {//Crea el arreglo de textos, para mandar al metodo de MQTT (generico), y luego envia el topico (siempre que tenga Relacion asociada
        int indiceElemento = calculoIndice(seleccion.name);
        string[] textos = new string[3];
        textos[0] = VariablesGlobales.Instance.casaActual.nombre;
        textos[1] = auxiliar.getHabitacionActiva().nombre;
        textos[2] = "No Enviar";
        switch (tipoElemento)
        {
            case "AireAcondicionado":
            {
                if (auxiliar.getHabitacionActiva().listaObjetos.Aires[indiceElemento].IDObjetoFisico != null && !auxiliar.getHabitacionActiva().listaObjetos.Aires[indiceElemento].IDObjetoFisico.Equals(""))
                {
                    textos[2] = auxiliar.getHabitacionActiva().listaObjetos.Aires[indiceElemento].nombre;
                }
                break;
            }
            case "LamparaPref":
            {
                if (auxiliar.getHabitacionActiva().listaObjetos.Lamparas[indiceElemento].IDObjetoFisico != null && !auxiliar.getHabitacionActiva().listaObjetos.Lamparas[indiceElemento].IDObjetoFisico.Equals(""))
                {
                    textos[2] = auxiliar.getHabitacionActiva().listaObjetos.Lamparas[indiceElemento].nombre;
                }
                break;
            }
            case "LamparaParedPref":
            {
                if (auxiliar.getHabitacionActiva().listaObjetos.Luces[indiceElemento].IDObjetoFisico != null && !auxiliar.getHabitacionActiva().listaObjetos.Luces[indiceElemento].IDObjetoFisico.Equals(""))
                {
                    textos[2] = auxiliar.getHabitacionActiva().listaObjetos.Luces[indiceElemento].nombre;
                }
                break;
            }
            case "Televisor":
            {
                if (auxiliar.getHabitacionActiva().listaObjetos.Televisores[indiceElemento].IDObjetoFisico != null && !auxiliar.getHabitacionActiva().listaObjetos.Televisores[indiceElemento].IDObjetoFisico.Equals(""))
                {
                    textos[2] = auxiliar.getHabitacionActiva().listaObjetos.Televisores[indiceElemento].nombre;
                }
                break;
            }
            case "VentiladorPiso":
            {
                if (auxiliar.getHabitacionActiva().listaObjetos.VentiladoresPiso[indiceElemento].IDObjetoFisico != null && !auxiliar.getHabitacionActiva().listaObjetos.VentiladoresPiso[indiceElemento].IDObjetoFisico.Equals(""))
                {
                    textos[2] = auxiliar.getHabitacionActiva().listaObjetos.VentiladoresPiso[indiceElemento].nombre;
                }
                break;
            }
            case "VentiladorTecho":
            {
                if (auxiliar.getHabitacionActiva().listaObjetos.VentiladoresTecho[indiceElemento].IDObjetoFisico != null && !auxiliar.getHabitacionActiva().listaObjetos.VentiladoresTecho[indiceElemento].IDObjetoFisico.Equals(""))
                {
                    textos[2] = auxiliar.getHabitacionActiva().listaObjetos.VentiladoresTecho[indiceElemento].nombre;
                }
                break;
            }            
            default:
            {
                break;
            }
        }
        //textos[3] = modeloTopico;
        if (!textos[2].Equals("No Enviar"))//Si el objeto no tenia Relacion con un NODE Fisico, no genera el topico ni lo envia
        {
            string topicoEnviar = VariablesGlobales.Instance.auxiliarMQTT.armarTopico(textos);
            //Debug.Log(topicoEnviar + "  " + textoEnviar);
            VariablesGlobales.Instance.auxiliarMQTT.enviarMensaje(textoEnviar, topicoEnviar);
            VariablesGlobales.Instance.auxiliarMQTT.subscribirse("informacion/" + topicoEnviar);
         }
    }
    //Manejadores AireAcondicionado!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public void manejadorAirePrender()
    {
        if (seleccion.GetComponent<ComportamientoAireAcondicionadoPrefab>().estado) 
        {   //Si el Aire esta prendido, lo apaga
            seleccion.GetComponent<ComportamientoAireAcondicionadoPrefab>().apagarAire();
            //MQTT ---> APAGAR AIRE
            enviarTextosTopico("AireAcondicionado", "Enviar","Apagar");
        }
        else//Si esta apagado, lo prende
        {
            seleccion.GetComponent<ComportamientoAireAcondicionadoPrefab>().prenderAire();
            //MQTT ---> PRENDER AIRE
            enviarTextosTopico("AireAcondicionado", "Enviar", "Prender");
            //Al prender el aire cambio el texto del boton Split
            if (seleccion.GetComponent<ComportamientoAireAcondicionadoPrefab>().splitEstado)
            {
                panelAireAcondicionado.transform.Find("PanelPrendidoAire").Find("SplitButton").GetComponentInChildren<Text>().text = "Split(OFF)";
            }
            else
            {
                panelAireAcondicionado.transform.Find("PanelPrendidoAire").Find("SplitButton").GetComponentInChildren<Text>().text = "Split(ON)";
            }
        }
        cambioEstadoTextoPrender("AireAcondicionado");
    }
    public void manejadorAireSubirTemp()
    {
        seleccion.GetComponent<ComportamientoAireAcondicionadoPrefab>().subirTemperatura();
        //MQTT ---> SUBIR TEMPERATURA AIRE
        enviarTextosTopico("AireAcondicionado", "Enviar", "SubirT");
        actualizarTextGrados();
    }
    public void manejadorAireBajarTemp()
    {
        seleccion.GetComponent<ComportamientoAireAcondicionadoPrefab>().bajarTemperatura();
        //MQTT ---> BAJAR TEMPERATURA AIRE
        enviarTextosTopico("AireAcondicionado", "Enviar", "BajarT");
        actualizarTextGrados();
    }
    public void manejadorAireSplit()
    {
        if (seleccion.GetComponent<ComportamientoAireAcondicionadoPrefab>().splitEstado)
        {   //Si el Split del Aire esta prendido, lo apaga
            seleccion.GetComponent<ComportamientoAireAcondicionadoPrefab>().apagarSplit();
            //MQTT ---> APAGAR SPLIT AIRE
            enviarTextosTopico("AireAcondicionado", "Enviar", "ApagarSplit");
            panelAireAcondicionado.transform.Find("PanelPrendidoAire").Find("SplitButton").GetComponentInChildren<Text>().text = "SPLIT(ON)";
        }
        else//Si esta apagado, lo prende
        {
            seleccion.GetComponent<ComportamientoAireAcondicionadoPrefab>().prenderSplit();
            //MQTT ---> PRENDER SPLIT AIRE
            enviarTextosTopico("AireAcondicionado", "Enviar", "PrenderSplit");
            panelAireAcondicionado.transform.Find("PanelPrendidoAire").Find("SplitButton").GetComponentInChildren<Text>().text = "SPLIT(OFF)";
        }
    }
    private void actualizarTextGrados()
    {
        if (panelAireAcondicionado.activeSelf && seleccion.GetComponent<ComportamientoAireAcondicionadoPrefab>().estado)
        {
            Text textoCanal = panelAireAcondicionado.transform.Find("PanelPrendidoAire").transform.Find("TextGrados").GetComponent<Text>();
            textoCanal.text = seleccion.GetComponent<ComportamientoAireAcondicionadoPrefab>().temperatura.ToString() + " ºC";
        }
    }




    //Manejadores Lampara!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public void manejadorLamparaPrender()
    {
        if (listaObjetos[indiceElemento].estado)
        {   //Si el Aire esta prendido, lo apaga
          // seleccion.GetComponent<ComportamientoLamparaPrefab>().apagarLampara();
            listaObjetos[indiceElemento].cambioEstado();
            //MQTT ---> APAGAR LAMPARA
            enviarTextosTopico("LamparaPref", "Enviar", "Apagar");
        }
        else//Si esta apagado, lo prende
        {
          // seleccion.GetComponent<ComportamientoLamparaPrefab>().prenderLampara();
            listaObjetos[indiceElemento].cambioEstado();
            //MQTT ---> PRENDER LAMPARA
            enviarTextosTopico("LamparaPref", "Enviar", "Prender");
        }
        cambioEstadoTextoPrender("LamparaPref");
    }
    //Manejadores LamparaPared!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public void manejadorLamparaParedPrender()
    {
        if (listaObjetos[indiceElemento].estado)
        {   //Si el Aire esta prendido, lo apaga
            //seleccion.GetComponent<ComportamientoLamparaParedPrefab>().apagarLuz();
            listaObjetos[indiceElemento].cambioEstado();
            //MQTT ---> APAGAR LUZ
            enviarTextosTopico("LamparaParedPref", "Enviar", "Apagar");
        }
        else//Si esta apagado, lo prende
        {
            // seleccion.GetComponent<ComportamientoLamparaParedPrefab>().prenderLuz();
            listaObjetos[indiceElemento].cambioEstado();
            //MQTT ---> PRENDER LUZ
            enviarTextosTopico("LamparaParedPref", "Enviar", "Prender");
        }
        cambioEstadoTextoPrender("LamparaParedPref");
    }



          
    //Manejadores TELEVISION!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public void manejadorTVPrender()
    {
        //Si lo encontro el panel, el boton estaba en Apagar. Y se lo pasa a Prender, se desactiva el panel y se apaga la TV
        if (seleccion.GetComponent<ComportamientoTelevisionPrefab>().estado) 
        {   //Si el TV esta prendido, lo apaga        
            seleccion.GetComponent<ComportamientoTelevisionPrefab>().apagarTV();
            //MQTT ---> APAGAR TV
            enviarTextosTopico("Televisor", "Enviar", "Apagar");
        }
        else//Si esta apagado, lo prende
        {  
            seleccion.GetComponent<ComportamientoTelevisionPrefab>().prenderTV();
            //MQTT ---> PRENDER TV
            enviarTextosTopico("Televisor", "Enviar", "Prender");
        }
        cambioEstadoTextoPrender("Televisor");
    }
    public void manejadorTVSubirCanal()
    {
        seleccion.GetComponent<ComportamientoTelevisionPrefab>().subirCanal();
        //MQTT ---> SUBIR CANAL TV
        enviarTextosTopico("Televisor", "Enviar", "SubirC");
        actualizarTextCanal();
    }
    public void manejadorTVBajarCanal()
    {
        seleccion.GetComponent<ComportamientoTelevisionPrefab>().bajarCanal();
        //MQTT ---> BAJAR CANAL TV
        enviarTextosTopico("Televisor", "Enviar", "BajarC");
        actualizarTextCanal();
    }
    public void manejadorTVCambiarCanal()
    {
        string textoCanal = GameObject.Find("InputCanal").GetComponent<InputField>().text;
        if (!textoCanal.Equals(""))
        {
            int canal = int.Parse(textoCanal);
            seleccion.GetComponent<ComportamientoTelevisionPrefab>().cambiarCanal(canal);
            //MQTT ---> CAMBIAR A X CANAL TV
            enviarTextosTopico("Televisor", "Enviar", "Cambiar-"+canal);//Fijarse donde hacer chekeo del canal, si aca o en el node que recibe
            actualizarTextCanal();
        }
    }
    private void actualizarTextCanal()
    {
        if (panelTelevisor.activeSelf && seleccion.GetComponent<ComportamientoTelevisionPrefab>().estado)
        {
            Text textoCanal = panelTelevisor.transform.Find("PanelPrendidoTV").transform.Find("TextCanal").GetComponent<Text>();
            textoCanal.text = "Canal: " + seleccion.GetComponent<ComportamientoTelevisionPrefab>().canal.ToString();
        }
    }


    //Manejadores VentiladorPiso!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public void manejadorVentiladorPisoVelocidad1()
    {        
        seleccion.GetComponent<ComportamientoVentiladorPisoPrefab>().velocidad1();
        //MQTT ---> PONER EN VELOCIDAD 1 VENTILADOR PISO
        enviarTextosTopico("VentiladorPiso", "Enviar", "Vel-1");
    }
    public void manejadorVentiladorPisoVelocidad2()
    {
        seleccion.GetComponent<ComportamientoVentiladorPisoPrefab>().velocidad2();
        //MQTT ---> PONER EN VELOCIDAD 2 VENTILADOR PISO
        enviarTextosTopico("VentiladorPiso", "Enviar", "Vel-2");
    }
    public void manejadorVentiladorPisoVelocidad3()
    {
        seleccion.GetComponent<ComportamientoVentiladorPisoPrefab>().velocidad3();
        //MQTT ---> PONER EN VELOCIDAD 3 VENTILADOR PISO
        enviarTextosTopico("VentiladorPiso", "Enviar", "Vel-3");
    }
    public void manejadorVentiladorPisoApagar()
    {
        seleccion.GetComponent<ComportamientoVentiladorPisoPrefab>().apagar();
        //MQTT ---> APAGAR VENTILADOR PISO
        enviarTextosTopico("VentiladorPiso", "Enviar", "Apagar");
    }


    //Manejadores VentiladorTecho!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public void manejadorVentiladorTechoVelocidad1()
    {
        seleccion.GetComponent<ComportamientoVentiladorTechoPrefab>().velocidad1();
        //MQTT ---> PONER EN VELOCIDAD 1 VENTILADOR TECHO
        enviarTextosTopico("VentiladorTecho", "Enviar", "Vel-1");
    }
    public void manejadorVentiladorTechoVelocidad2()
    {
        seleccion.GetComponent<ComportamientoVentiladorTechoPrefab>().velocidad2();
        //MQTT ---> PONER EN VELOCIDAD 2 VENTILADOR TECHO
        enviarTextosTopico("VentiladorTecho", "Enviar", "Vel-2");
    }
    public void manejadorVentiladorTechoVelocidad3()
    {
        seleccion.GetComponent<ComportamientoVentiladorTechoPrefab>().velocidad3();
        //MQTT ---> PONER EN VELOCIDAD 3 VENTILADOR TECHO
        enviarTextosTopico("VentiladorTecho", "Enviar", "Vel-2");
    }
    public void manejadorVentiladorTechoApagar()
    {
        seleccion.GetComponent<ComportamientoVentiladorTechoPrefab>().apagar();
        //MQTT ---> APAGAR VENTILADOR TECHO
        enviarTextosTopico("VentiladorTecho", "Enviar", "Apagar");
    }
    
    public void actualizar()
    {
      
        if (seleccion.name.Contains("LamparaPref")) {
            //Debug.Log( auxiliar.getHabitacionActiva().nombre + "/" + auxiliar.getHabitacionActiva().listaObjetos.Lamparas[indiceElemento] +"-"+indiceElemento  + " " + listaObjetos[indiceElemento].MsjNodeEstado);
            if (listaObjetos[indiceElemento].MsjNodeEstado != "")
            {
                if (listaObjetos[indiceElemento].MsjNodeEstado == "ON")
                {
                    seleccion.GetComponent<ComportamientoLamparaPrefab>().prenderLampara();
                    listaObjetos[indiceElemento].estado = true;
                }
                else
                {
                    if (listaObjetos[indiceElemento].MsjNodeEstado == "OFF")
                    {

                        seleccion.GetComponent<ComportamientoLamparaPrefab>().apagarLampara();
                        listaObjetos[indiceElemento].estado = false;
                    }

                }
                listaObjetos[indiceElemento].MsjNodeEstado = "";
                cambioEstadoTextoPrender("LamparaPref");
            }
           
        }

        //.............................................................................................................

        if (seleccion.name.Contains("LamparaParedPref"))
        {
            if (listaObjetos[indiceElemento].MsjNodeEstado != "")
            {
                if (listaObjetos[indiceElemento].MsjNodeEstado == "ON")
                {
                    seleccion.GetComponent<ComportamientoLamparaParedPrefab>().prenderLuz();
                    listaObjetos[indiceElemento].estado = true;
                }
                else
                {
                    if (listaObjetos[indiceElemento].MsjNodeEstado == "OFF")
                    {
                        seleccion.GetComponent<ComportamientoLamparaParedPrefab>().apagarLuz();
                        listaObjetos[indiceElemento].estado = false;
                    }

                }
                listaObjetos[indiceElemento].MsjNodeEstado = "";
                cambioEstadoTextoPrender("LamparaParedPref");
                
            }

        }

        //.............................................................................................................
        //REALIZAR EL MIMSO COMPORTAMIENTO PARA LOS DEMAS ELEMENTOS. VERIFICAR QUE LLEGA Y EN BASE A ESO IMPLEMENTAR ACCION.


    }

    

    private void armarTopcioInformaion() {

        //cuando se referencia esta clase, realizar las suscripciones invocando esta funcion desde start();

        }

    private Vector3 verificarLado(string lado)
    {
        Vector3 posCamara = new Vector3();
        switch (lado)
        {
            case "Abajo":
                {
                    posCamara = new Vector3(auxiliar.habitacionActiva.posInicial + ((float)(auxiliar.habitacionActiva.ancho + 1)) / 2, 6f, 0);
                    break;
                }
            case "Derecha":
                {
                    posCamara = new Vector3(auxiliar.habitacionActiva.posInicial + auxiliar.habitacionActiva.ancho + 2, 6f, ((float)(auxiliar.habitacionActiva.largo + 1)) / 2);
                    break;
                }
            case "Arriba":
                {
                    posCamara = new Vector3(auxiliar.habitacionActiva.posInicial + ((float)(auxiliar.habitacionActiva.ancho + 1)) / 2, 6f, auxiliar.habitacionActiva.largo + 2);
                    break;
                }
            case "Izquierda":
                {
                    posCamara = new Vector3(auxiliar.habitacionActiva.posInicial, 6f, ((float)(auxiliar.habitacionActiva.largo + 1)) / 2);
                    break;
                }
            default:
                break;
        }
        return posCamara;
    }
    void prueba()
    {
        timerFinish = timer;
        if (timerFinish - timerStart >= 1.0 && cant < 101)
        {
            cant++;
            manejadorLamparaPrender();
            timerStart = timer;
        }

    }
}
