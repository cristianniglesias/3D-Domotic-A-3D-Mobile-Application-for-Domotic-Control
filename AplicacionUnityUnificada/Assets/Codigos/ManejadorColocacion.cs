using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ManejadorColocacion : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private GameObject seleccion;
    private Renderer rend;
    private Color colorOriginal;
    private GameObject panelPiso, panelPared, panelObjeto, botonRotar;   
    private GameObject prefabAire, prefabLampara, prefabLuz, prefabTV, prefabVentPiso, prefabVentTecho;
    private ManejadorCamarasOtros auxiliar;
   //private Text textoDebug;
    private Touch tap;

    // Use this for initialization
    void Start () {        
        prefabAire = GameObject.Find("GlobalObject").GetComponent<ConstructorCasa>().prefabAire;
        prefabLampara = GameObject.Find("GlobalObject").GetComponent<ConstructorCasa>().prefabLampara;
        prefabLuz = GameObject.Find("GlobalObject").GetComponent<ConstructorCasa>().prefabLuz;
        prefabTV = GameObject.Find("GlobalObject").GetComponent<ConstructorCasa>().prefabTV;
        prefabVentPiso = GameObject.Find("GlobalObject").GetComponent<ConstructorCasa>().prefabVentPiso;
        prefabVentTecho = GameObject.Find("GlobalObject").GetComponent<ConstructorCasa>().prefabVentTecho;


        //textoDebug = GameObject.Find("TextoDebug").GetComponent<Text>();
        //textoDebug.text = "";
        panelPiso = GameObject.Find("PanelPiso");
        panelPiso.SetActive(false);
        panelPared = GameObject.Find("PanelPared");
        panelPared.SetActive(false);
        botonRotar = GameObject.Find("RotarButton");
        botonRotar.SetActive(false);
        panelObjeto = GameObject.Find("PanelObjeto");
        panelObjeto.SetActive(false);
        auxiliar = GameObject.Find("CodigosModoColocacion").GetComponent<ManejadorCamarasOtros>();
        colorOriginal = Color.clear;
        seleccion = null;        
    }

    // Update is called once per frame
    void Update () {

        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == 0)
            {                
                if (Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {                    
                    //textoDebug.text = "No toco UI";
                    procesarSeleccion(touch);                    
                }
            }
        }        
    }

    private void procesarSeleccion (Touch tap)
    {
        //Resetear color del elemento seleccionado anterior
        if (seleccion != null)
        {
            if (seleccion.transform.parent.name.Contains("Habitacion"))//Si sos un piso o una pared
            {
                rend = seleccion.transform.GetComponent<Renderer>();
                if (colorOriginal != Color.clear)
                {
                    rend.material.color = colorOriginal;
                }
            }
            else//Si sos un objeto colocable
            {
                Behaviour halo = (Behaviour)seleccion.transform.Find("Cuerpo").GetComponent("Halo");
                halo.enabled = false;
            }
        }        
        //Nueva seleccion
        //ray = Camera.main.ScreenPointToRay(Input.mousePosition);                
        ray = Camera.main.ScreenPointToRay(tap.position);
        if (Physics.Raycast(ray, out hit, 100.0f)) //Corrobora si el rayo toco un objeto
        {
            //Desactivo los paneles
            DesactivarPaneles();            
            //Encuentra la referencia al objeto seleccionado
            seleccion = hit.transform.gameObject;
            if (!seleccion.name.Contains("Vertice") && (seleccion.transform.parent.name.Contains(auxiliar.getHabitacionActiva().nombre) || seleccion.transform.parent.transform.parent.name.Contains(auxiliar.getHabitacionActiva().nombre)))
            {   //Si lo seleccionado no es un Vertice y corresponde a la habitacion activa:
                //Es decir: Si es Pared o Piso (el padre contiene el nombre de la habitacion activa). O sos objeto (el padre de su padre contiene el nombre de la habitacion activa)                        
                if (seleccion.transform.parent.name.Contains("Habitacion"))//Si sos un piso o una pared
                {
                    //Se le pone el color Verde
                    rend = seleccion.transform.GetComponent<Renderer>();
                    colorOriginal = rend.material.color;
                    rend.material.color = Color.green;
                }
                else//Si sos un objeto colocable
                {
                    Behaviour halo = (Behaviour)seleccion.transform.Find("Cuerpo").GetComponent("Halo");
                    halo.enabled = true;
                }
                mostrarControl(seleccion, true);
            }
            else
            {   //Si lo seleccionado no corresponde a la habitacion activa, se descarta
                seleccion = null;
            }
        }
        else
        {   //Si no tocaste nada, descarta el objeto y se oculta el control que este visible
            mostrarControl(seleccion, false);
            seleccion = null;
        }
    }

    public void DesactivarPaneles()
    {
        List<GameObject> listaPaneles = new List<GameObject>();
        listaPaneles.Add(panelObjeto);
        listaPaneles.Add(panelPared);
        listaPaneles.Add(panelPiso);
        auxiliar.desactivarObjetos(listaPaneles);

        //Agregado para que cuando cambie de habitacion, tambien se haga el reseteo del color
        //Resetear color del elemento seleccionado anterior
        if (seleccion != null)
        {
            if (seleccion.transform.parent.name.Contains("Habitacion"))//Si sos un piso o una pared
            {
                rend = seleccion.transform.GetComponent<Renderer>();
                if (colorOriginal != Color.clear)
                {
                    rend.material.color = colorOriginal;
                }
            }
            else//Si sos un objeto colocable
            {
                Behaviour halo = (Behaviour)seleccion.transform.Find("Cuerpo").GetComponent("Halo");
                halo.enabled = false;
            }
        }
    }

    private void mostrarControl (GameObject objeto, bool activo)
    {
        if (objeto != null)
        {
            if (objeto.transform.parent.name.Contains("Habitacion") )//Si es Pared o Piso
            {
                if (objeto.transform.childCount == 0)
                {
                    if (string.Equals( auxiliar.getTipoElemento(seleccion), "Piso"))
                    {
                        panelPiso.SetActive(activo);
                    }
                    else
                    {
                        panelPared.SetActive(activo);
                    }
                }            
            }
            else//Si es Otro Objeto
            {                
                panelObjeto.SetActive(activo);
                if (activo)
                {
                    //Si el boton rotar esta activo y el objeto esta sobre la pared, lo deshabilito 
                    if (botonRotar.activeSelf && objeto.transform.parent.name.Contains("Pared"))
                        botonRotar.SetActive(false);
                    //Si el boton rotar no esta activo y el objeto esta sobre el piso, lo habilito
                    if (!botonRotar.activeSelf && objeto.transform.parent.name.Contains("Piso"))
                        botonRotar.SetActive(true);
                    //Le doy el nombre del objeto al texto del panel
                    GameObject.Find("TextNombreObjeto").GetComponent<Text>().text = objeto.name.Substring(0, objeto.name.IndexOf("Pref(Clone)"))+'-'+ objeto.name.Split('-')[objeto.name.Split('-').Length - 1];
                }
            }
        }        
    }  

    public void manejadorBotonAireAcon ()
    {        
        GameObject objeto = Instantiate(prefabAire, new Vector3(0, 0, 0), Quaternion.identity);
        objeto.name = objeto.name + "-" + auxiliar.getHabitacionActiva().listaObjetos.Aires.Count;
        mostrarControl(seleccion, false);
        GameObject.Find("GlobalObject").GetComponent<ConstructorCasa>().colocarObjeto(seleccion, objeto, auxiliar.getTipoElemento(seleccion));              
        string nombre = "Aire-" + auxiliar.getHabitacionActiva().listaObjetos.Aires.Count;
        AireAcondicionado nuevoAire = new AireAcondicionado(nombre, objeto.transform.position, objeto.transform.rotation.eulerAngles.y, seleccion.name);
        auxiliar.getHabitacionActiva().listaObjetos.Aires.Add(nuevoAire);
        auxiliar.cambioRealizado();//Indico que se produjo un cambio para guardar el Archivo
    }

    public void manejadorBotonLampara()
    {
        GameObject objeto = Instantiate(prefabLampara, new Vector3(0, 0, 0), Quaternion.identity);
        objeto.name = objeto.name + "-" + auxiliar.getHabitacionActiva().listaObjetos.Lamparas.Count;
        mostrarControl(seleccion, false);
        GameObject.Find("GlobalObject").GetComponent<ConstructorCasa>().colocarObjeto(seleccion, objeto, auxiliar.getTipoElemento(seleccion));        
        string nombre = "Lamp-" + auxiliar.getHabitacionActiva().listaObjetos.Lamparas.Count;
        Lampara nuevaLamp = new Lampara(nombre, objeto.transform.position, objeto.transform.rotation.eulerAngles.y, seleccion.name);
        auxiliar.getHabitacionActiva().listaObjetos.Lamparas.Add(nuevaLamp);
        auxiliar.cambioRealizado();//Indico que se produjo un cambio para guardar el Archivo
    }

    public void manejadorBotonLuz()
    {
        GameObject objeto = Instantiate(prefabLuz, new Vector3(0, 0, 0), Quaternion.identity);
        objeto.name = objeto.name + "-" + auxiliar.getHabitacionActiva().listaObjetos.Luces.Count;
        mostrarControl(seleccion, false);
        GameObject.Find("GlobalObject").GetComponent<ConstructorCasa>().colocarObjeto(seleccion, objeto, auxiliar.getTipoElemento(seleccion));    
        string nombre = "Luz-" + auxiliar.getHabitacionActiva().listaObjetos.Luces.Count;
        Luz nuevaLuz = new Luz(nombre, objeto.transform.position, objeto.transform.rotation.eulerAngles.y, seleccion.name);
        auxiliar.getHabitacionActiva().listaObjetos.Luces.Add(nuevaLuz);
        auxiliar.cambioRealizado();//Indico que se produjo un cambio para guardar el Archivo
    }

    public void manejadorBotonTV()
    {
        GameObject objeto = Instantiate(prefabTV, new Vector3(0, 0, 0), Quaternion.identity);
        objeto.name=objeto.name+"-"+ auxiliar.getHabitacionActiva().listaObjetos.Televisores.Count;
        mostrarControl(seleccion, false);
        GameObject.Find("GlobalObject").GetComponent<ConstructorCasa>().colocarObjeto(seleccion, objeto, auxiliar.getTipoElemento(seleccion));        
        string nombre = "TV-" + auxiliar.getHabitacionActiva().listaObjetos.Televisores.Count;
        Televisor nuevoTV = new Televisor(nombre, objeto.transform.position, objeto.transform.rotation.eulerAngles.y, seleccion.name);
        auxiliar.getHabitacionActiva().listaObjetos.Televisores.Add(nuevoTV);
        auxiliar.cambioRealizado();//Indico que se produjo un cambio para guardar el Archivo
    }


    public void manejadorBotonVentPiso()
    {
        GameObject objeto = Instantiate(prefabVentPiso, new Vector3(0, 0, 0), Quaternion.identity);
        objeto.name = objeto.name + "-" + auxiliar.getHabitacionActiva().listaObjetos.VentiladoresPiso.Count;
        mostrarControl(seleccion, false);
        GameObject.Find("GlobalObject").GetComponent<ConstructorCasa>().colocarObjeto(seleccion, objeto, auxiliar.getTipoElemento(seleccion));        
        string nombre = "VentPiso-" + auxiliar.getHabitacionActiva().listaObjetos.VentiladoresPiso.Count;
        VentiladorPiso nuevoVentP = new VentiladorPiso(nombre, objeto.transform.position, objeto.transform.rotation.eulerAngles.y, seleccion.name);
        auxiliar.getHabitacionActiva().listaObjetos.VentiladoresPiso.Add(nuevoVentP);
        auxiliar.cambioRealizado();//Indico que se produjo un cambio para guardar el Archivo
    }

    public void manejadorBotonVentTecho()
    {
        GameObject objeto = Instantiate(prefabVentTecho, new Vector3(0, 0, 0), Quaternion.identity);
        objeto.name = objeto.name + "-" + auxiliar.getHabitacionActiva().listaObjetos.VentiladoresTecho.Count;
        mostrarControl(seleccion, false);
        GameObject.Find("GlobalObject").GetComponent<ConstructorCasa>().colocarObjeto(seleccion, objeto, auxiliar.getTipoElemento(seleccion));        
        string nombre = "VentTecho-" + auxiliar.getHabitacionActiva().listaObjetos.VentiladoresTecho.Count;
        VentiladorTecho nuevoVentT = new VentiladorTecho(nombre, objeto.transform.position, objeto.transform.rotation.eulerAngles.y, seleccion.name);
        auxiliar.getHabitacionActiva().listaObjetos.VentiladoresTecho.Add(nuevoVentT);
        auxiliar.cambioRealizado();//Indico que se produjo un cambio para guardar el Archivo
    }
    
    public void manejadorBotonAyuda()
    {
        /*EditorUtility.DisplayDialog("Ayuda", "Seleccione una porción de piso o pared y elija el objeto que desee colocar.\nPuede seleccionar " +  
            "también un objeto ya colocado, para rotarlo y/o eliminarlo.", "Entendido");*/
        VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.AYUDA, "Ayuda para Colocar Objetos", "Seleccione una porción de piso o pared y elija el objeto que desee colocar.\nPuede seleccionar también un objeto ya colocado, para rotarlo y/o eliminarlo.");
    }

    public void manejadorBotonGuardar()//En desuso, por el momento
    {
        ManejadorArchivos.Guardar(auxiliar.getCasa());
    }

    public void manejadorBotonRotar()
    {
        seleccion.transform.Rotate(0, 90, 0);//Roto 90 el objeto 3D
        string[] partesText = seleccion.name.Split('-');
        int indice = int.Parse(partesText[partesText.Length - 1]);           
        switch( auxiliar.getTipoObjeto(seleccion))//Luego dependediendo que tipo de objeto es, modifico su rotacionY. Presente en la lista de la Casa
        {
            case "LamparaPref":
            {
                auxiliar.getHabitacionActiva().listaObjetos.Lamparas[indice].rotacionY = (auxiliar.getHabitacionActiva().listaObjetos.Lamparas[indice].rotacionY + 90) % 360;
                break;
            }
            case "Televisor":
            {
                auxiliar.getHabitacionActiva().listaObjetos.Televisores[indice].rotacionY = (auxiliar.getHabitacionActiva().listaObjetos.Televisores[indice].rotacionY + 90) % 360;
                break;
            }
            case "VentiladorPiso":
            {
                auxiliar.getHabitacionActiva().listaObjetos.VentiladoresPiso[indice].rotacionY = (auxiliar.getHabitacionActiva().listaObjetos.VentiladoresPiso[indice].rotacionY + 90) % 360;
                break;
            }
            case "VentiladorTecho":
            {
                auxiliar.getHabitacionActiva().listaObjetos.VentiladoresTecho[indice].rotacionY = (auxiliar.getHabitacionActiva().listaObjetos.VentiladoresTecho[indice].rotacionY + 90) % 360;
                break;
            }
            default: { break; }
        }
        auxiliar.cambioRealizado();//Indico que se produjo un cambio para guardar el Archivo
    }
    public void manejadorBotonEliminar()
    {
        //Saco el nombre del objeto seleccionado (el 3D)
        string tipoObj = seleccion.name;
        //Dependiendo que contiene el nombre, es a que tipo de lista debo eliminar y cambiar nombres de los demas
        if (seleccion.name.Contains("AireAcondicionadoPref"))
        { eliminarAire(); }
        if (seleccion.name.Contains("LamparaPref")) 
        { eliminarLampara(); }
        if (seleccion.name.Contains("LamparaParedPref")) 
        { eliminarLuz(); }
        if (seleccion.name.Contains("PersianaPref")) 
        { eliminarPersiana(); }
        if (seleccion.name.Contains("SensorHumedadPref")) 
        { eliminarSensorHumedad(); }
        if (seleccion.name.Contains("SensorTemperaturaPref")) 
        { eliminarSensorTemperatura(); }
        if (seleccion.name.Contains("TelevisorPisoPref")) 
        { eliminarTelevisor();}
        if (seleccion.name.Contains("VentiladorPisoPref")) 
        { eliminarVentiladorPiso(); }
        if (seleccion.name.Contains("VentiladorTechoPref")) 
        { eliminarVentiladorTecho(); }
        Destroy(seleccion);
        panelObjeto.SetActive(false);
        auxiliar.cambioRealizado();//Indico que se produjo un cambio para guardar el Archivo
    }    
    
    //Arma un nombre a partir del original, reemplazando la ultima cadena, por el valor del numero final. Y la retorna
    //Ej, Tele-ded-23-dda --->> armadoNombre(cadena,'-',52) --->> Tele-ded-23-52
    private string armadoNombre(string nombre,char separador,int numeroFinal)
    {
        string[] nombreSeparado;
        string nombreNuevo="";
        //Divido el nombre por '-'
        nombreSeparado = nombre.Split('-');
        for (int j = 0; j < nombreSeparado.Length - 1; j++)
        {
            //Le voy rearmando el nombre, hasta llegar al ultimo -
            nombreNuevo = nombreNuevo + nombreSeparado[j] + '-';
        }
        //Le doy el valor numerico nuevo al final
        nombreNuevo = nombreNuevo + numeroFinal.ToString();
        return nombreNuevo;
    }
    private void eliminarAire()
    {
        //Numero de Aire a borrar
        int numeroDeAire;
        //Del Aire 3D, le separo el nombre por '-'
        string[] nombreSeparado = seleccion.name.Split('-');
        //Convierto el ultimo string, a entero (debido a que ese ultimo string indica su numero de lista)
        int.TryParse(nombreSeparado[nombreSeparado.Length - 1], out numeroDeAire);
        //Arranco desde el Aire siguiente al que se quiere borrar
        for (int i = numeroDeAire + 1; i < auxiliar.getHabitacionActiva().listaObjetos.Aires.Count; i++)
        {
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            auxiliar.getHabitacionActiva().listaObjetos.Aires[i].nombre = armadoNombre(auxiliar.getHabitacionActiva().listaObjetos.Aires[i].nombre, '-', i - 1);
        }
        //Arranco desde el Aire siguiente al seleccionado
        for (int i = numeroDeAire + 1; i < auxiliar.getHabitacionActiva().listaObjetos.Aires.Count; i++)
        {
            //Pido la lista de Aire 3D
            GameObject[] l = GameObject.FindGameObjectsWithTag("AireTag");
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            l[i].name = armadoNombre(l[i].name, '-', i - 1);
        }
        auxiliar.getHabitacionActiva().listaObjetos.Aires.RemoveAt(seleccion.name[seleccion.name.Length - 1] - '0');
    }
    private void eliminarLampara()
    {
        //Numero de Lampara a borrar
        int numeroDeLampara;
        //Del Lampara 3D, le separo el nombre por '-'
        string[] nombreSeparado = seleccion.name.Split('-');
        //Convierto el ultimo string, a entero (debido a que ese ultimo string indica su numero de lista)
        int.TryParse(nombreSeparado[nombreSeparado.Length - 1], out numeroDeLampara);

        //Arranco desde el Lampara siguiente al que se quiere borrar
        for (int i = numeroDeLampara + 1; i < auxiliar.getHabitacionActiva().listaObjetos.Lamparas.Count; i++)
        {
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            auxiliar.getHabitacionActiva().listaObjetos.Lamparas[i].nombre = armadoNombre(auxiliar.getHabitacionActiva().listaObjetos.Lamparas[i].nombre, '-', i - 1);
        }
        //Arranco desde la Lampara siguiente al seleccionado
        for (int i = numeroDeLampara + 1; i < auxiliar.getHabitacionActiva().listaObjetos.Lamparas.Count; i++)
        {
            //Pido la lista de Lampara 3D
            GameObject[] l = GameObject.FindGameObjectsWithTag("LamparaTag");
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            l[i].name = armadoNombre(l[i].name, '-', i - 1);
        }
        auxiliar.getHabitacionActiva().listaObjetos.Lamparas.RemoveAt(seleccion.name[seleccion.name.Length - 1] - '0');
    }
    private void eliminarLuz()
    {
        //Numero de Luz a borrar
        int numeroDeLuz;
        //Del Luz 3D(LamparaPared), le separo el nombre por '-'
        string[] nombreSeparado = seleccion.name.Split('-');
        //Convierto el ultimo string, a entero (debido a que ese ultimo string indica su numero de lista)
        int.TryParse(nombreSeparado[nombreSeparado.Length - 1], out numeroDeLuz);
        
        //Arranco desde el Luz siguiente al que se quiere borrar
        for (int i = numeroDeLuz + 1; i < auxiliar.getHabitacionActiva().listaObjetos.Luces.Count; i++)
        {
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            auxiliar.getHabitacionActiva().listaObjetos.Luces[i].nombre = armadoNombre(auxiliar.getHabitacionActiva().listaObjetos.Luces[i].nombre, '-', i - 1);
        }
        //Arranco desde la Luz siguiente al seleccionado
        for (int i = numeroDeLuz + 1; i < auxiliar.getHabitacionActiva().listaObjetos.Luces.Count; i++)
        {
            //Pido la lista de Luz 3D
            GameObject[] l = GameObject.FindGameObjectsWithTag("LuzTag");
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            l[i].name = armadoNombre(l[i].name, '-', i - 1);
        }
        auxiliar.getHabitacionActiva().listaObjetos.Luces.RemoveAt(seleccion.name[seleccion.name.Length - 1] - '0');
    }
    private void eliminarPersiana()
    {
        //Numero de Persiana a borrar
        int numeroDePersiana;
        //Del Persiana 3D, le separo el nombre por '-'
        string[] nombreSeparado = seleccion.name.Split('-');
        //Convierto el ultimo string, a entero (debido a que ese ultimo string indica su numero de lista)
        int.TryParse(nombreSeparado[nombreSeparado.Length - 1], out numeroDePersiana);
        
        //Arranco desde el Persiana siguiente al que se quiere borrar
        for (int i = numeroDePersiana + 1; i < auxiliar.getHabitacionActiva().listaObjetos.Persianas.Count; i++)
        {
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            auxiliar.getHabitacionActiva().listaObjetos.Persianas[i].nombre = armadoNombre(auxiliar.getHabitacionActiva().listaObjetos.Persianas[i].nombre, '-', i - 1);
        }
        //Arranco desde la Persiana siguiente al seleccionado
        for (int i = numeroDePersiana + 1; i < auxiliar.getHabitacionActiva().listaObjetos.Persianas.Count; i++)
        {
            //Pido la lista de Persiana 3D
            GameObject[] l = GameObject.FindGameObjectsWithTag("PersianaTag");
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            l[i].name = armadoNombre(l[i].name, '-', i - 1);
        }
        auxiliar.getHabitacionActiva().listaObjetos.Persianas.RemoveAt(seleccion.name[seleccion.name.Length - 1] - '0');
    }
    private void eliminarSensorHumedad()
    {
        //Numero de SensorHumedad a borrar
        int numeroDeSensorHumedad;
        //Del SensorHumedad 3D, le separo el nombre por '-'
        string[] nombreSeparado = seleccion.name.Split('-');
        //Convierto el ultimo string, a entero (debido a que ese ultimo string indica su numero de lista)
        int.TryParse(nombreSeparado[nombreSeparado.Length - 1], out numeroDeSensorHumedad);
        
        //Arranco desde el SensorHumedad siguiente al que se quiere borrar
        for (int i = numeroDeSensorHumedad + 1; i < auxiliar.getHabitacionActiva().listaObjetos.SensoresHumedad.Count; i++)
        {
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            auxiliar.getHabitacionActiva().listaObjetos.SensoresHumedad[i].nombre = armadoNombre(auxiliar.getHabitacionActiva().listaObjetos.SensoresHumedad[i].nombre, '-', i - 1);
        }
        //Arranco desde el SensorHumedad siguiente al seleccionado
        for (int i = numeroDeSensorHumedad + 1; i < auxiliar.getHabitacionActiva().listaObjetos.SensoresHumedad.Count; i++)
        {
            //Pido la lista de SensoresHumedad 3D
            GameObject[] l = GameObject.FindGameObjectsWithTag("SensorHumedadTag");
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            l[i].name = armadoNombre(l[i].name, '-', i - 1);
        }
        auxiliar.getHabitacionActiva().listaObjetos.SensoresHumedad.RemoveAt(seleccion.name[seleccion.name.Length - 1] - '0');
    }
    private void eliminarSensorTemperatura()
    {
        //Numero de SensorTemperatura a borrar
        int numeroDeSensorTemperatura;
        //Del SensorTemperatura 3D, le separo el nombre por '-'
        string[] nombreSeparado = seleccion.name.Split('-');
        //Convierto el ultimo string, a entero (debido a que ese ultimo string indica su numero de lista)
        int.TryParse(nombreSeparado[nombreSeparado.Length - 1], out numeroDeSensorTemperatura);

        //Arranco desde el SensorTemperatura siguiente al que se quiere borrar
        for (int i = numeroDeSensorTemperatura + 1; i < auxiliar.getHabitacionActiva().listaObjetos.SensoresTemperatura.Count; i++)
        {
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            auxiliar.getHabitacionActiva().listaObjetos.SensoresTemperatura[i].nombre = armadoNombre(auxiliar.getHabitacionActiva().listaObjetos.SensoresTemperatura[i].nombre, '-', i - 1);
        }
        //Arranco desde el SensorTemperatura siguiente al seleccionado
        for (int i = numeroDeSensorTemperatura + 1; i < auxiliar.getHabitacionActiva().listaObjetos.SensoresTemperatura.Count; i++)
        {
            //Pido la lista de SensoresTemperatura 3D
            GameObject[] l = GameObject.FindGameObjectsWithTag("SensorTemperaturaTag");
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            l[i].name = armadoNombre(l[i].name, '-', i - 1);
        }
        auxiliar.getHabitacionActiva().listaObjetos.SensoresTemperatura.RemoveAt(seleccion.name[seleccion.name.Length - 1] - '0');
    }
    private void eliminarTelevisor()
    {
        //Numero de Televisor a borrar
        int numeroDeTelevisor;
        //Del Televisor 3D, le separo el nombre por '-'
        string [] nombreSeparado = seleccion.name.Split('-');
        //Convierto el ultimo string, a entero (debido a que ese ultimo string indica su numero de lista)
        int.TryParse(nombreSeparado[nombreSeparado.Length - 1], out numeroDeTelevisor);

        //Arranco desde el televisor siguiente al que se quiere borrar
        for (int i = numeroDeTelevisor + 1; i < auxiliar.getHabitacionActiva().listaObjetos.Televisores.Count; i++)
        {
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            auxiliar.getHabitacionActiva().listaObjetos.Televisores[i].nombre = armadoNombre(auxiliar.getHabitacionActiva().listaObjetos.Televisores[i].nombre, '-', i - 1);
        }
        //Arranco desde el Televisor siguiente al seleccionado
        for (int i = numeroDeTelevisor + 1; i < auxiliar.getHabitacionActiva().listaObjetos.Televisores.Count; i++)
        {
            //Pido la lista de Televisor 3D
            GameObject[] l = GameObject.FindGameObjectsWithTag("TelevisorTag");
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            l[i].name = armadoNombre(l[i].name, '-', i - 1);
        }
        auxiliar.getHabitacionActiva().listaObjetos.Televisores.RemoveAt(seleccion.name[seleccion.name.Length - 1] - '0');
    }
    private void eliminarVentiladorPiso()
    {
        //Numero de VentiladorPiso a borrar
        int numeroDeVentiladorPiso;
        //Del VentiladorPiso 3D, le separo el nombre por '-'
        string[] nombreSeparado = seleccion.name.Split('-');
        //Convierto el ultimo string, a entero (debido a que ese ultimo string indica su numero de lista)
        int.TryParse(nombreSeparado[nombreSeparado.Length - 1], out numeroDeVentiladorPiso);
        
        //Arranco desde el VentiladorPiso siguiente al que se quiere borrar
        for (int i = numeroDeVentiladorPiso + 1; i < auxiliar.getHabitacionActiva().listaObjetos.VentiladoresPiso.Count; i++)
        {
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            auxiliar.getHabitacionActiva().listaObjetos.VentiladoresPiso[i].nombre = armadoNombre(auxiliar.getHabitacionActiva().listaObjetos.VentiladoresPiso[i].nombre, '-', i - 1);
        }
        //Arranco desde el VentiladorPiso siguiente al seleccionado
        for (int i = numeroDeVentiladorPiso + 1; i < auxiliar.getHabitacionActiva().listaObjetos.VentiladoresPiso.Count; i++)
        {
            //Pido la lista de VentiladorPiso 3D
            GameObject[] l = GameObject.FindGameObjectsWithTag("VentiladorPisoTag");
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            l[i].name = armadoNombre(l[i].name, '-', i - 1);
        }
        auxiliar.getHabitacionActiva().listaObjetos.VentiladoresPiso.RemoveAt(seleccion.name[seleccion.name.Length - 1] - '0');
    }
    private void eliminarVentiladorTecho()
    {
        //Numero de VentiladorTecho a borrar
        int numeroDeVentiladorTecho;
        //Del VentiladorTecho 3D, le separo el nombre por '-'
        string[] nombreSeparado = seleccion.name.Split('-');
        //Convierto el ultimo string, a entero (debido a que ese ultimo string indica su numero de lista)
        int.TryParse(nombreSeparado[nombreSeparado.Length - 1], out numeroDeVentiladorTecho);

        //Arranco desde el VentiladorTecho siguiente al que se quiere borrar
        for (int i = numeroDeVentiladorTecho + 1; i < auxiliar.getHabitacionActiva().listaObjetos.VentiladoresTecho.Count; i++)
        {
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            auxiliar.getHabitacionActiva().listaObjetos.VentiladoresTecho[i].nombre = armadoNombre(auxiliar.getHabitacionActiva().listaObjetos.VentiladoresTecho[i].nombre, '-', i - 1);
        }
        //Arranco desde el VentiladorTecho siguiente al seleccionado
        for (int i = numeroDeVentiladorTecho + 1; i < auxiliar.getHabitacionActiva().listaObjetos.VentiladoresTecho.Count; i++)
        {
            //Pido la lista de VentiladorTecho 3D
            GameObject[] l = GameObject.FindGameObjectsWithTag("VentiladorTechoTag");
            //Le cambio el valor del nombre, por uno nuevo con un numero final una unidad menor
            l[i].name = armadoNombre(l[i].name, '-', i - 1);
        }
        auxiliar.getHabitacionActiva().listaObjetos.VentiladoresTecho.RemoveAt(seleccion.name[seleccion.name.Length - 1] - '0');
    }



}
