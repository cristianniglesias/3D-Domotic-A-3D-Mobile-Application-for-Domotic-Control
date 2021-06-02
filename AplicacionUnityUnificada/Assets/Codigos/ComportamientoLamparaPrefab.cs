using UnityEngine;
using System.Collections.Generic;
public class ComportamientoLamparaPrefab : MonoBehaviour
{



    private bool _estado=false;
    // Start is called before the first frame update
    private string _msj = "";
    private GameObject seleccion;
    private ManejadorCamarasOtros auxiliar;
    private int indiceElemento;
    List<Objeto> listaObjetos;
    void Start()
    {
        Debug.Log("Start");
     
    }

    public bool estado
    {
        get { return _estado; }
    }

    public string msj
    {
        get { return _msj; }
        set { _msj = value; }
    }



    public void apagarLampara()
    {
       // Debug.Log("ENTRO A LA FUNCION ANTES DEL TRANSFORM");
        this.transform.Find("Luz").gameObject.SetActive(false);
        _estado = false;
        
       // Debug.Log("ENTRO A APAGARLAMPARA");
    }

    public void prenderLampara()
    {
       // Debug.Log("ENTRO A LA FUNCION ANTES DEL TRANSFORM");
        this.transform.Find("Luz").gameObject.SetActive(true);
        _estado = true;
        
        // Debug.Log("ENTRO A PRENDERLAMPARA");
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void analizarEstado()
    {

       


    }

  
 
}
