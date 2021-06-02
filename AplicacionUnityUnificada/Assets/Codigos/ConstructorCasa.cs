using System.Text.RegularExpressions;
using UnityEngine;

public class ConstructorCasa : MonoBehaviour
{
    private const float ajustePos = 0.4f; //ajusta las posiciones desde el centro hacia los bordes de su porcion de piso 
    private const float alturaCamaras = 6f; // altura para las camaras de las habitaciones   
    private GameObject _prefabPared, _prefabPiso, _prefabVertice;
    private GameObject _prefabAire, _prefabLampara, _prefabLuz, _prefabTV, _prefabVentPiso, _prefabVentTecho;

    // Start is called before the first frame update
    void Start()
    {
        _prefabPared = Resources.Load<GameObject>("BloquePared");
        _prefabPiso = Resources.Load<GameObject>("BloquePiso");
        _prefabVertice = Resources.Load<GameObject>("BloqueVertice");
        _prefabAire = Resources.Load<GameObject>("AireAcondicionadoPref");
        _prefabLampara = Resources.Load<GameObject>("LamparaPref");
        _prefabLuz = Resources.Load<GameObject>("LamparaParedPref");
        _prefabTV = Resources.Load<GameObject>("TelevisorPisoPref");
        _prefabVentPiso = Resources.Load<GameObject>("VentiladorPisoPref");
        _prefabVentTecho = Resources.Load<GameObject>("VentiladorTechoPref");
    }

    public GameObject prefabPared
    {
        get { return _prefabPared; }
    }

    public GameObject prefabPiso
    {
        get { return _prefabPiso; }
    }

    public GameObject prefabVertice
    {
        get { return _prefabVertice; }
    }

    public GameObject prefabAire
    {
        get { return _prefabAire; }
    }

    public GameObject prefabLampara
    {
        get { return _prefabLampara; }
    }

    public GameObject prefabLuz
    {
        get { return _prefabLuz; }
    }

    public GameObject prefabTV
    {
        get { return _prefabTV; }
    }

    public GameObject prefabVentPiso
    {
        get { return _prefabVentPiso; }
    }

    public GameObject prefabVentTecho
    {
        get { return _prefabVentTecho; }
    }

    public void construirCasa(Casa c)
    {
        foreach (Habitacion h in c.habitaciones)
        {            
            crearHabitacion(h.ancho, h.largo, h.posInicial, h.nombre, h.nombreFicticio);
            foreach (Objeto o in h.listaObjetos.getObjetos())
            {                
                instanciarObjeto(o, h.nombre);
            }
        }
    }

    public Habitacion crearHabitacion(int metrosAncho, int metrosLargo, float posInicio, string nombre, string nombreFicticio)
    {
        if (metrosAncho <= 0 || metrosLargo <= 0)
        {
            VariablesGlobales.Instance.auxiliarVentana.mostrarVentana(tipoVentana.ALERTA, "Error de datos - Medidas", "Ingrese valores mayores a cero en Ancho y Largo.");
            return null;
        }
        else
        {
            //Creo una Habitacion en la escena
            GameObject Habitacion = new GameObject(nombre);
            GameObject CentroHab = new GameObject(Habitacion.name + "_Centro");
            CentroHab.transform.parent = Habitacion.transform;
            CentroHab.transform.position = new Vector3(posInicio + ((float)(metrosAncho + 1)) / 2, 2, ((float)(metrosLargo + 1)) / 2);
            crearPiso(metrosAncho, metrosLargo, Habitacion, posInicio);
            crearVertices(metrosAncho, metrosLargo, Habitacion, posInicio);
            crearParedes(metrosAncho, metrosLargo, Habitacion, posInicio);
            crearCamaras(metrosAncho, metrosLargo, Habitacion, CentroHab, posInicio);
            //Creo el objeto habitacion
            Habitacion nuevaHab = new Habitacion(nombre, nombreFicticio);
            nuevaHab.ancho = metrosAncho;
            nuevaHab.largo = metrosLargo;
            nuevaHab.posInicial = posInicio;
            return nuevaHab;
        }
    }

    private void crearPiso(int metrosAncho, int metrosLargo, GameObject Habitacion, float posInicio)
    {
        GameObject Piso = new GameObject(Habitacion.name + "_Piso");
        Piso.transform.parent = Habitacion.transform;//Le asigno el PadreHabitacion
        // creo el piso de la habitacion        
        for (int i = 1; i < metrosAncho + 1; i++)
        {
            for (int j = 1; j < metrosLargo + 1; j++)
            {
                GameObject refBloque = Instantiate(_prefabPiso, new Vector3(posInicio + i, -0.5f, j), Quaternion.identity);
                refBloque.transform.parent = Piso.transform;
                refBloque.name = "Piso" + i + j;
            }
        }
    }

    private void crearVertices(int metrosAncho, int metrosLargo, GameObject Habitacion, float posInicio)
    {
        GameObject refVertice;
        //crea vertice que une pared abajo con pared izquierda
        refVertice = Instantiate(_prefabVertice, new Vector3(posInicio + ajustePos, 1, 0 + ajustePos), Quaternion.identity);
        refVertice.name = Habitacion.name + "_Vertice1";
        refVertice.transform.parent = Habitacion.transform;
        //crea vertice que une pared abajo con pared derecha
        refVertice = Instantiate(_prefabVertice, new Vector3(posInicio + metrosAncho + 1 - ajustePos, 1, 0 + ajustePos), Quaternion.identity);
        refVertice.name = Habitacion.name + "_Vertice2";
        refVertice.transform.parent = Habitacion.transform;
        //crea vertice que une pared derecha con pared arriba
        refVertice = Instantiate(_prefabVertice, new Vector3(posInicio + metrosAncho + 1 - ajustePos, 1, metrosLargo + 1 - ajustePos), Quaternion.identity);
        refVertice.name = Habitacion.name + "_Vertice3";
        refVertice.transform.parent = Habitacion.transform;
        //crea vertice que une pared arriba con pared izquierda
        refVertice = Instantiate(_prefabVertice, new Vector3(posInicio + ajustePos, 1, metrosLargo + 1 - ajustePos), Quaternion.identity);
        refVertice.name = Habitacion.name + "_Vertice4";
        refVertice.transform.parent = Habitacion.transform;
    }

    private void crearParedes(int metrosAncho, int metrosLargo, GameObject Habitacion, float posInicio)
    {
        crearPared('1', metrosAncho, metrosLargo, Habitacion, posInicio);
        crearPared('2', metrosAncho, metrosLargo, Habitacion, posInicio);
        crearPared('3', metrosAncho, metrosLargo, Habitacion, posInicio);
        crearPared('4', metrosAncho, metrosLargo, Habitacion, posInicio);
    }

    private void crearPared(char opc, int metrosAncho, int metrosLargo, GameObject Habitacion, float posInicio)
    {
        switch (opc)
        {
            case '1': //crear pared abajo
                {
                    GameObject pared1 = new GameObject(Habitacion.name + "_ParedAbajo");
                    pared1.transform.parent = Habitacion.transform;//Le asigno el PadreHabitacion
                    for (int i = 1; i <= metrosAncho; i++)
                    {
                        //(Instantiate(prefabPared, new Vector3(posx + i, 1, 0 + ajustePos), Quaternion.Euler(0, 90, 0) as GameObject).transform.parent = pared2.transform;
                        GameObject refBloque = Instantiate(_prefabPared, new Vector3(posInicio + i, 1, 0 + ajustePos), Quaternion.Euler(0, 90, 0));
                        refBloque.transform.parent = pared1.transform;
                        refBloque.name = "ParedAbajo" + i;
                    }
                    break;
                }
            case '2':
                {
                    GameObject pared2 = new GameObject(Habitacion.name + "_ParedDerecha");
                    pared2.transform.parent = Habitacion.transform;//Le asigno el PadreHabitacion
                    for (int i = 1; i <= metrosLargo; i++)//Esta pared tiene un bloque mas que el largo
                    {
                        //(Instantiate(prefabPared, new Vector3(posx + metrosAncho + 1 - ajustePos, 1, i), Quaternion.identity) as GameObject).transform.parent = pared2.transform;
                        GameObject refBloque = Instantiate(_prefabPared, new Vector3(posInicio + metrosAncho + 1 - ajustePos, 1, i), Quaternion.identity);
                        refBloque.transform.parent = pared2.transform;
                        refBloque.name = "ParedDerecha" + i;
                    }
                    break;
                }
            case '3':
                {
                    GameObject pared3 = new GameObject(Habitacion.name + "_ParedArriba");
                    pared3.transform.parent = Habitacion.transform;//Le asigno el PadreHabitacion
                    for (int i = 1; i <= metrosAncho; i++)//Esta pared tiene la cantidad de bloques del ancho
                    {
                        GameObject refBloque = Instantiate(_prefabPared, new Vector3(posInicio + i, 1, metrosLargo + 1 - ajustePos), Quaternion.Euler(0, 90, 0));
                        refBloque.transform.parent = pared3.transform;
                        refBloque.name = "ParedArriba" + i;
                    }

                    break;
                }
            case '4':
                {
                    GameObject pared4 = new GameObject(Habitacion.name + "_ParedIzquierda");
                    pared4.transform.parent = Habitacion.transform;//Le asigno el PadreHabitacion
                    for (int i = 1; i <= metrosLargo; i++)//Esta pared tiene un bloque mas que el largo
                    {
                        GameObject refBloque = Instantiate(_prefabPared, new Vector3(posInicio + ajustePos, 1, i), Quaternion.identity);
                        refBloque.transform.parent = pared4.transform;
                        refBloque.name = "ParedIzquierda" + i;
                    }
                    break;
                }
            default:
                break;
        }
    }
    
    private void crearCamaras(int metrosAncho, int metrosLargo, GameObject Habitacion, GameObject Centro, float posInicio)
    { 
        Vector3 posCamara;
        // crea camara de la pared de abajo
        posCamara = new Vector3(posInicio + ((float)(metrosAncho + 1)) / 2, alturaCamaras, 0);
        Camera.main.transform.position = posCamara;
        Camera.main.transform.LookAt(Centro.transform.position);
        colocarCamara(Centro, posCamara, "CamaraAbajo", Habitacion);
    }

        
    private void colocarCamara(GameObject referencia, Vector3 posicion, string nombre, GameObject objPadre)
    {
        GameObject posCam = new GameObject(objPadre.name + "_" + nombre);
        posCam.transform.position = posicion;
        posCam.transform.parent = objPadre.transform;//Le asigno como padre la pared1
    }

    private void instanciarObjeto(Objeto obj, string nombreHab)
    {
        GameObject referencia = null;
        switch (obj.tipo)
        {
            case tipoObjeto.AIREACON:
                {
                    referencia = Instantiate(_prefabAire, new Vector3(obj.posX, obj.posY, obj.posZ), Quaternion.Euler(0, obj.rotacionY, 0));
                    break;
                }
            case tipoObjeto.LAMPARA:
                {
                    referencia = Instantiate(_prefabLampara, new Vector3(obj.posX, obj.posY, obj.posZ), Quaternion.Euler(0, obj.rotacionY, 0));
                    break;
                }
            case tipoObjeto.LUZ:
                {
                    referencia = Instantiate(_prefabLuz, new Vector3(obj.posX, obj.posY, obj.posZ), Quaternion.Euler(0, obj.rotacionY, 0));
                    break;
                }
            case tipoObjeto.PERSIANA:
                {
                    Debug.Log("Falta prefab");
                    //referencia = Instantiate(prefabPersiana, new Vector3(obj.posX, obj.posY, obj.posZ), Quaternion.Euler(0, obj.rotacionY, 0));
                    break;
                }
            case tipoObjeto.SENSORHUM:
                {
                    Debug.Log("Falta prefab");
                    //referencia = Instantiate(prefabSensorHumedad, new Vector3(obj.posX, obj.posY, obj.posZ), Quaternion.Euler(0, obj.rotacionY, 0));
                    break;
                }
            case tipoObjeto.SENSORTEMP:
                {
                    Debug.Log("Falta prefab");
                    //referencia = Instantiate(prefabSensorTemperatura, new Vector3(obj.posX, obj.posY, obj.posZ), Quaternion.Euler(0, obj.rotacionY, 0));
                    break;
                }
            case tipoObjeto.TV:
                {
                    referencia = Instantiate(_prefabTV, new Vector3(obj.posX, obj.posY, obj.posZ), Quaternion.Euler(0, obj.rotacionY, 0));
                    break;
                }
            case tipoObjeto.VENT:
                {
                    referencia = Instantiate(_prefabVentPiso, new Vector3(obj.posX, obj.posY, obj.posZ), Quaternion.Euler(0, obj.rotacionY, 0));
                    break;
                }
            case tipoObjeto.VENTTECHO:
                {
                    referencia = Instantiate(_prefabVentTecho, new Vector3(obj.posX, obj.posY, obj.posZ), Quaternion.Euler(0, obj.rotacionY, 0));
                    break;
                }
            default:
                break;
        }
        if (referencia != null)
        {
            string[] cadenaNumero = obj.nombre.Split('-');
            referencia.name = referencia.name + '-' + cadenaNumero[cadenaNumero.Length - 1];
            GameObject refHab = GameObject.Find(nombreHab); 
            string cadAbuelo = Regex.Replace(obj.nombrePadre, @"\d", ""); // elimina todos los numeros de la cadena y busco el grupo padre           
            Transform padre = refHab.transform.Find(nombreHab+"_"+cadAbuelo);
            referencia.transform.parent = padre.transform.Find(obj.nombrePadre); // asocio al objeto con su padre en la escena           
        }
    }


    public void colocarObjeto(GameObject seleccion, GameObject objetoAColocar, string tipoElemento)
    {
        if (seleccion.transform.parent.name.Contains("Habitacion")) //Solo coloca sobre paredes o piso
        {
            if (seleccion.transform.childCount == 0) //Se coloca un unico elemento por superficie
            {
                float offset = 0;
                Vector3 posicion = seleccion.transform.position; //Posicion de la pared/piso seleccionado
                rotarObjeto(seleccion, objetoAColocar, tipoElemento);
                switch (tipoElemento)
                {
                    case ("ParedArriba"):
                        {
                            posicion.z -= offset;
                            break;
                        }
                    case ("ParedAbajo"):
                        {
                            posicion.z += offset;
                            break;
                        }
                    case ("ParedDerecha"):
                        {
                            posicion.x -= offset;
                            break;
                        }
                    case ("ParedIzquierda"):
                        {
                            posicion.x += offset;
                            break;
                        }
                    default:
                        break;
                }
                posicion.y = 0;                
                objetoAColocar.transform.position = posicion;
                objetoAColocar.transform.parent = seleccion.transform;
            }
            else
            {
                Destroy(objetoAColocar);
            }
        }
    }

    private void rotarObjeto(GameObject seleccion, GameObject objetoAColocar, string tipoElemento)
    {
        switch (tipoElemento) //En base a la pared, roto el elemento
        {
            case ("ParedAbajo"):
                {
                    objetoAColocar.transform.Rotate(0, 180, 0);
                    break;
                }
            case ("ParedIzquierda"):
                {
                    objetoAColocar.transform.Rotate(0, -90, 0);
                    break;
                }
            case ("ParedDerecha"):
                {
                    objetoAColocar.transform.Rotate(0, 90, 0);
                    break;
                }
            default:
                break;
        }
    }    

    // Update is called once per frame
    void Update()
    {
        
    }
}
