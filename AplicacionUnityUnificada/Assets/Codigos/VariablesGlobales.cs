using System.Collections.Generic;
using UnityEngine;

public class VariablesGlobales : MonoBehaviour
{
    public static VariablesGlobales Instance;
    public Casa casaActual = null;
    public Casa casaAux = null;
    private int numeroHabitacion;
    private int numeroCamara;
    public mqtt auxiliarMQTT;
    public List<Objeto> listaObjetos;
    public VentanaEmergente auxiliarVentana;
    public string topico = "";
    public string msj = "";

    public List<string> listaNodes;//Lista con las MACs de los Nodes

    private void Awake()
    { 
        DontDestroyOnLoad(this);//Al cambiar de escenas se mantiene
        if (Instance == null)//Si es el primero guarda la instancia
        {
            Instance = this;
        }
        else//Si no destruye el objeto, para evitar duplicados
        {
             Object.Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        listaNodes = new List<string>();
        auxiliarMQTT = GameObject.Find("GlobalObject").GetComponent<mqtt>();
        auxiliarVentana = GameObject.Find("GlobalObject").GetComponent<VentanaEmergente>();
    }
    // Update is called once per frame
   public void actualizarmsj(string msjmqtt) {

        msj = msjmqtt;
    
    }
    void Update()
    {
        //msj = auxiliarMQTT.msj;
    }
  
}
