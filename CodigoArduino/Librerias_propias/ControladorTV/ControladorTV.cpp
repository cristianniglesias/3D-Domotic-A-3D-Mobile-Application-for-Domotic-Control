#include "ControladorTV.h"
 
ControladorTV::ControladorTV(int CLOCK, int DATAIO)
{
	CLK = CLOCK;
	DIO = DATAIO;
	pinMode(CLK, OUTPUT);
	pinMode(DIO, OUTPUT);
	display = new TM1637Display(CLK, DIO, DEFAULT_BIT_DELAY);
	display->setBrightness(0x0a); //set the diplay to maximum brightness
	resetTV();
    mostrarInfo();   
}

void ControladorTV::mostrarInfo()
{
	display->setSegments(data);
}

void ControladorTV::apagar()
{
    if (estado)
    {
        estado = false;
		data[0] = valorArreglo(ESPACIO);
		data[1] = valorArreglo(ESPACIO);
		data[2] = valorArreglo(ESPACIO);
		data[3] = valorArreglo(ESPACIO);
		mostrarInfo();
    }      
}
void ControladorTV::resetTV()
{	
	estado = false;
	data[0] = valorArreglo(ESPACIO);
	data[1] = valorArreglo(ESPACIO);
	data[2] = valorArreglo(ESPACIO);
	data[3] = valorArreglo(ESPACIO);
	canal = 2;
	mostrarInfo();
}
void ControladorTV::prender()
{
    if (!estado)
    {
		data[0] = valorArreglo(LETRAC);
		data[1] = valorArreglo(LETRAH);
		cambiarValor(data, 4, canal);
        estado = true;
		mostrarInfo();
    }    
}

void ControladorTV::subirCanal()
{
    if (estado)
    {
		if (canal < MAXCANAL)
		{
			canal++;
		}
		else
		{
			canal = MINCANAL;
		}
		cambiarValor(data, 4, canal);
        mostrarInfo();
    }  
}

void ControladorTV::bajarCanal()
{
    if (estado)
    {
		if (canal > MINCANAL)
		{
			canal--;
		}
		else
		{
			canal = MAXCANAL;
		}
		cambiarValor(data, 4, canal);
        mostrarInfo();
    }    
}  

void ControladorTV::cambiarCanal(int chanel)
{
	if (estado)
	{
		if (chanel >= MINCANAL && chanel <= MAXCANAL && canal!=chanel)
		{
			canal = chanel;
			cambiarValor(data, 4, canal);
		}
		mostrarInfo();
	}
}

//Recibe el arreglo que se va a mostrar en el display, y le cambia el valor los numeros, al numero de canal elegido
void ControladorTV::cambiarValor(uint8_t arreglo[], int tam, int canal)
{
	if (canal < MINCANAL || canal > MAXCANAL)
	{
		canal = 0;
	}
	arreglo[2] = valorArreglo(canal / 10);
	arreglo[3] = valorArreglo(canal % 10);
}

//Devuelve la palabra de byte, de la posicion valor del arreglo
uint8_t ControladorTV::valorArreglo(int valor)
{
	if (valor >= 27 || valor < 0)
	{
		valor = 0;
	}
	return letras[valor][7] * 128 + letras[valor][6] * 64 + letras[valor][5] * 32 + letras[valor][4] * 16 + letras[valor][3] * 8 + letras[valor][2] * 4 + letras[valor][1] * 2 + letras[valor][0] * 1;
}



