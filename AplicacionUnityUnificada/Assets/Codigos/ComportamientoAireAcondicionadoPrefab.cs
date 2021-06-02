using UnityEngine;

public class ComportamientoAireAcondicionadoPrefab : MonoBehaviour
{
    private int _temperatura;
    private bool _splitEstado;
    private bool _estado;
    // Start is called before the first frame update
    void Start()
    {
        _temperatura = 24;
        _estado = false;
        _splitEstado = false;
        comprobarLuz();
    }

    public bool estado
    {
        get { return _estado; }
    }

    public bool splitEstado
    {
        get { return _splitEstado; }
    }

    public int temperatura
    {
        get { return _temperatura; }
    }

    public void apagarAire()
    {
        this.transform.Find("Luz").gameObject.SetActive(false);
        _estado = false;
    }

    public void prenderAire()
    {
        this.transform.Find("Luz").gameObject.SetActive(true);
        _estado = true;
    }

    public void subirTemperatura()
    {
        if (_temperatura < 31)
        {
            _temperatura++;
            comprobarLuz();
        }
    }

    public void bajarTemperatura()
    {
        if (_temperatura > 15)
        {
            _temperatura--;
            comprobarLuz();
        }
    }

    public void apagarSplit()
    {
        _splitEstado = false;
        //Cuando apago el split, me aseguro de que quede la luz prendida
        this.transform.Find("Luz").gameObject.SetActive(true);
    }

    public void prenderSplit()
    {
        _splitEstado = true;
    }

    private void comprobarLuz()//Temperatura de 15 a 19, 20 a 25 y de 26 a 31
    {
        if(temperatura<=19)
        {
            this.transform.Find("Luz").gameObject.GetComponent<Light>().color = Color.cyan;
        }
        else
        {
            if (temperatura >= 26)
            {
                this.transform.Find("Luz").gameObject.GetComponent<Light>().color = Color.red;
            }
            else
            {
                this.transform.Find("Luz").gameObject.GetComponent<Light>().color = Color.green;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (estado && _splitEstado)//Si esta prendido el AireAcondicionado con Split, alterna por frame, la luz de encendido
        {
            this.transform.Find("Luz").gameObject.SetActive(!this.transform.Find("Luz").gameObject.activeSelf);
        }
    }
}
