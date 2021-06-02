using UnityEngine;

public class ComportamientoLamparaParedPrefab : MonoBehaviour
{
    private bool _estado;
    
    // Start is called before the first frame update
    void Start()
    {
        _estado = false;
    }

    public bool estado
    {
        get { return _estado; }
    }

    public void apagarLuz()
    {
        this.transform.Find("Luz").gameObject.SetActive(false);
        _estado = false;
    }

    public void prenderLuz()
    {
        this.transform.Find("Luz").gameObject.SetActive(true);
        _estado = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
