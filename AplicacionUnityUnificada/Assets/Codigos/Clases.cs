using System.Collections.Generic;
using UnityEngine;
public enum tipoObjeto { AIREACON, LAMPARA, LUZ, PERSIANA, SENSORHUM, SENSORTEMP, TV, VENT, VENTTECHO}
public class Clases: MonoBehaviour
{

}

[System.Serializable]
public class Objeto
{
    //Si el atributo es privado debo marcar que es serializable
    [SerializeField]
    private string _nombre;
    [SerializeField]
    private bool _estado;// Prendido/Apagado --- Abierto/Cerrado
    [SerializeField]
    private float _posX, _posY, _posZ;
    [SerializeField]
    private float _rotacionY;
    [SerializeField]
    private string _nombrePadre;
    [SerializeField]
    private tipoObjeto _t;
    [SerializeField]
    private string _IDObjetoFisico;
    [SerializeField]
    private string  _msjNodeEstado;
   
    public Objeto() { }
    public Objeto(string nom)
    {
        nombre = nom;
        _IDObjetoFisico = "";
    }
    public Objeto(tipoObjeto t, string nom, Vector3 pos, float rotacion, string nombrePadre)
    {
        _t = t;
        _nombre = nom;
        _posX = pos.x;
        _posY = pos.y;
        _posZ = pos.z;
        _rotacionY = rotacion;
        _nombrePadre = nombrePadre;
        _IDObjetoFisico = "";
        _msjNodeEstado = "";
       _estado = false;
    }
    public string MsjNodeEstado
    {
        get { return _msjNodeEstado; }

        set { _msjNodeEstado = value; }
    }

    public tipoObjeto tipo
    {
        get { return _t; }
    }
    public string nombre
    {
        get { return _nombre; }
        set { _nombre = value; }
    }
    public string IDObjetoFisico
    {
        get { return _IDObjetoFisico; }
        set { _IDObjetoFisico = value; }
    }
    public bool estado
    {
        get { return _estado; }
        set { _estado = value; }
    }
    public string nombrePadre
    {
        get { return _nombrePadre; }
        set { _nombrePadre = value; }
    }
    public void cambioEstado() { estado = !_estado; }//Cambia al estado opuesto   
    public float posX
    {
        get { return _posX; }
        set { _posX = value; }
    }
    public float posY
    {
        get { return _posY; }
        set { _posY = value; }
    }
    public float posZ
    {
        get { return _posZ; }
        set { _posZ = value; }
    }
    public float rotacionY 
    {
        get { return _rotacionY; }
        set { _rotacionY = value; }
    }

 
}

[System.Serializable]
public class VentiladorTecho:Objeto
{    
    private int velocidad;
    public VentiladorTecho(string nombre, Vector3 pos, float rotY, string padre) : base(tipoObjeto.VENTTECHO, nombre, pos, rotY, padre)
    {

    }
}

[System.Serializable]
public class VentiladorPiso : Objeto
{
    private int velocidad;
    public VentiladorPiso(string nombre, Vector3 pos, float rotY, string padre) : base(tipoObjeto.VENT, nombre, pos, rotY, padre)
    {

    }
}

[System.Serializable]
public class Luz:Objeto
{
    private Color colorLuz;
    private int intensidad;
    public Luz(string nombre, Vector3 pos, float rotY, string padre) : base(tipoObjeto.LUZ, nombre, pos, rotY, padre)
    {

    }
}

[System.Serializable]
public class Lampara : Objeto
{
    private Color colorLuz;
    private int intensidad;
    public Lampara(string nombre, Vector3 pos, float rotY, string padre) : base(tipoObjeto.LAMPARA, nombre, pos, rotY, padre)
    {

    }
}

[System.Serializable]
public class AireAcondicionado:Objeto
{
    private int temperatura;
    private bool split;
    public AireAcondicionado(string nombre, Vector3 pos, float rotY, string padre) : base(tipoObjeto.AIREACON, nombre, pos, rotY, padre)
    {

    }
}

[System.Serializable]
public class Persiana:Objeto
{
    private int altura;//Si es que puede estar semi abierta    
    public Persiana(string nombre, Vector3 pos, float rotY, string padre) : base(tipoObjeto.PERSIANA, nombre, pos, rotY, padre)
    {
        
    }
}

[System.Serializable]
public class Televisor:Objeto
{
    private int canal;
    private Color luz;//Cambiar la luz, cuando se cambia de canal, para que parezca que anda el cambio de canal    
    public Televisor(string nombre, Vector3 pos, float rotY, string padre) : base(tipoObjeto.TV, nombre, pos, rotY, padre)
    {

    }
}

[System.Serializable]
public class SensorTemperatura:Objeto
{
    private float temperatura;
    public SensorTemperatura(string nombre, Vector3 pos, float rotY, string padre) : base(tipoObjeto.SENSORTEMP, nombre, pos, rotY, padre)
    {

    }
}

[System.Serializable]
public class SensorHumedad:Objeto
{
    private float humedad;
    public SensorHumedad(string nombre, Vector3 pos, float rotY, string padre) : base(tipoObjeto.SENSORHUM, nombre, pos, rotY, padre)
    {

    }
}

[System.Serializable]
public class Equipamiento
{
    public List<AireAcondicionado> Aires;
    public List<Lampara> Lamparas;
    public List<Luz> Luces;
    public List<Persiana> Persianas;
    public List<SensorHumedad> SensoresHumedad;
    public List<SensorTemperatura> SensoresTemperatura;
    public List<Televisor> Televisores;
    public List<VentiladorPiso> VentiladoresPiso;
    public List<VentiladorTecho> VentiladoresTecho; 

    public Equipamiento()
    {
        Aires = new List<AireAcondicionado>();
        Lamparas = new List<Lampara>();
        Luces = new List<Luz>();
        Persianas = new List<Persiana>();
        SensoresHumedad = new List<SensorHumedad>();
        SensoresTemperatura = new List<SensorTemperatura>();
        Televisores = new List<Televisor>();
        VentiladoresPiso = new List<VentiladorPiso>();
        VentiladoresTecho = new List<VentiladorTecho>();
    }

    public List<Objeto> getObjetos()
    {        
        List<Objeto> listaObjetos = new List<Objeto>();
        foreach (Objeto o in Aires){
            listaObjetos.Add(o);
        }
        foreach (Objeto o in Lamparas)
        {
            listaObjetos.Add(o);
        }
        foreach (Objeto o in Luces)
        {
            listaObjetos.Add(o);
        }
        foreach (Objeto o in Persianas)
        {
            listaObjetos.Add(o);
        }
        foreach (Objeto o in SensoresHumedad)
        {
            listaObjetos.Add(o);
        }
        foreach (Objeto o in SensoresTemperatura)
        {
            listaObjetos.Add(o);
        }
        foreach (Objeto o in Televisores)
        {
            listaObjetos.Add(o);
        }
        foreach (Objeto o in VentiladoresPiso)
        {
            listaObjetos.Add(o);
        }
        foreach (Objeto o in VentiladoresTecho)
        {
            listaObjetos.Add(o);
        }
        return listaObjetos;
    }   
}
[System.Serializable]
public class Habitacion
{
    [SerializeField]
    private string _nombre;//Nombre de la Habitacion
    [SerializeField]
    private int _ancho;//Ancho de la habitacion
    [SerializeField]
    private int _largo;//Largo de la habitacion
    [SerializeField]
    private float _posInicial;//Vector de la posicion inicial de la construccion(esquina inferior izquierda)
    [SerializeField]
    private Color _colorPiso;//Color del piso de la habitacion
    [SerializeField]
    private Color _colorPared;//Color de la pared de la habitacion
    [SerializeField]
    private Vector3  _centro;//Vector de la posicion del centro de la habitacion    
    [SerializeField]
    private Equipamiento _objetos;//Lista con los Objetos de la Habitacion
    [SerializeField]
    private string _nombreFicticio;//Nombre Ficticio de la Habitacion, para el usuario

    public Habitacion() { }//Constructor vacio
    public Habitacion(string nom)//Constructor que crea la habitacion con el nombre, y crea una lista
    {
        _nombre = nom;
        _objetos = new Equipamiento();
    }
    public Habitacion(string nom,string nomFicticio)//Constructor que crea la habitacion con el nombre y su nombre ficticio, y crea una lista
    {
        _nombre = nom;
        _objetos = new Equipamiento();
        _nombreFicticio = nomFicticio;
    }
    public Habitacion(string nom, Equipamiento lista)//Constructor que crea la habitacion con el nombre, y con la lista de objetos
    {
        _nombre = nom;
        _objetos = lista;
    }
    public Habitacion(string nom,string nomFicticio, Equipamiento lista)//Constructor que crea la habitacion con el nombre y su nombre ficticio, y con la lista de objetos
    {
        _nombre = nom;
        _objetos = lista;
        _nombreFicticio = nombreFicticio;
    }
    public string nombre
    {
        get { return _nombre; }
        set { _nombre = value; }
    }
    public int ancho
    {
        get { return _ancho; }
        set { _ancho = value; }
    }
    public int largo
    {
        get { return _largo; }
        set { _largo = value; }
    }
    public float posInicial
    {
        get { return _posInicial; }
        set { _posInicial = value; }
    }
    public Color colorPiso
    {
        get { return _colorPiso; }
        set { _colorPiso = value; }
    }
    public Color colorPared
    {
        get { return _colorPared; }
        set { _colorPared = value; }
    }
    public Vector3 centro
    {
        get { return _centro; }
        set { _centro = value; }
    }    
    public Equipamiento listaObjetos
    {
        get { return _objetos; }
    }
    public string nombreFicticio
    {
        get { return _nombreFicticio; }
        set { _nombreFicticio = value; }
    }
}

[System.Serializable]
public class Casa
{
    [SerializeField]
    private string _nombre;//Nombre de la Casa
    [SerializeField]
    private List<Habitacion> _habitaciones;//Lista con las Habitaciones de la casa
    [SerializeField]
    private int[] _IPBroker;//IP del Borker, para Relaciones a lo Fisico

    public Casa()//Constructor vacio
    {
        _IPBroker = new int[4];
        _IPBroker[0] = -1;//Arranca con IP invalido
    }
    public Casa(string nom)//Constructor que crea la casa con el nombre, y crea una lista
    {
        _nombre = nom;
        _habitaciones = new List<Habitacion>();
        _IPBroker = new int[4];
        _IPBroker[0] = -1;//Arranca con IP invalido
    }
    public Casa(string nom, List<Habitacion> lista)//Constructor que crea la casa con el nombre, y con la lista de habitaciones
    {
        _nombre = nom;
        _habitaciones = lista;
        _IPBroker = new int[4];
        _IPBroker[0] = -1;//Arranca con IP invalido
    }


    public string nombre
    {
        get { return _nombre; }
        set { _nombre = value; }
    }

    public List<Habitacion> habitaciones
    {
        get { return _habitaciones; }
    }
   
    public void agregarHabitacion(Habitacion hab)
    {
        _habitaciones.Add(hab);
    }

    public int buscarHabitacion(string nombreHabitacion)
    {
        int indice = -1;
        for(int i=0;i<habitaciones.Count;i++)
        {
            if(habitaciones[i].nombre.Equals(nombreHabitacion))
            {
                indice = i;
                break;
            }
        }
        return indice;
    }

    public int[] IPBroker
    {
        get { return _IPBroker; }
        set { _IPBroker = value; }
    }
    public string getIPString()
    {
        if (_IPBroker[0] != -1)
        {
            return _IPBroker[0] + "." + _IPBroker[1] + "." + _IPBroker[2] + "." + _IPBroker[3];
        }
        else
        {
            return null;
        }
    }
}
