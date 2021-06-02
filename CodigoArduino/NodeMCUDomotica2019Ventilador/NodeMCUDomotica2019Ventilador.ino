#include <ESP8266WiFi.h>
#include <PubSubClient.h>

#define BUILTIN_LED 2

// Update these with values suitable for your network.

//Wi-Fi
/*
const char* ssid = "externalLIDI";
const char* password = "lab3SalasCMR";
const char* mqtt_server = "192.168.0.105";
*/
const char* ssid = "DomoticaUNLPTest";
const char* password = "MovilesLIDI2019";
const char* mqtt_server = "192.168.5.1";


//Topics
String detectTopic = "Detect";
String configTopic = "Config";
String linkTopic;
String deviceTopic;

WiFiClient espClient;
PubSubClient client(espClient);

long lastMsg = 0;
char msg[50];
int value = 0;

String macStr;

void titilar(unsigned int miliSeg)
{
  digitalWrite(BUILTIN_LED, HIGH);
  delay(miliSeg);
  digitalWrite(BUILTIN_LED, LOW);
  delay(miliSeg);
  digitalWrite(BUILTIN_LED, HIGH);
}
  
void setup_wifi() {

  delay(10);
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    titilar(100);
    Serial.print(".");
  }

  randomSeed(micros());

  Serial.println("");
  Serial.println("WiFi connected");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
  Serial.print("MAC: ");
  Serial.println(WiFi.macAddress());  
}

void callback(char* topic, byte* payload, unsigned int length) 
{   
  Serial.print("Mensaje recibido - Contenido: ");
  String msjRecibido = "";
  String topicoRecibido = "";
  for (int i = 0; i < length; i++) 
  {
    msjRecibido += (char)payload[i];    
  }
  Serial.print(msjRecibido);
  Serial.print(" - Topico: ");
  for (int i = 0; topic[i]!='\0'; i++) 
  {
    topicoRecibido += topic[i];    
  }
  Serial.println(topicoRecibido); 
  int topicoVar = topicoToOpt(topic);
  Serial.print("Topic Code: ");
  Serial.println(topicoVar);
  switch (topicoVar) 
  {
        case 0: // el msj proviene del topico detect
        {          
          Serial.print("Enviando mensaje - Topico: ");
          Serial.print(configTopic);
          Serial.print(" - Mensaje: ");
          Serial.println(macStr);           
          client.publish(&configTopic[0], &macStr[0]);                
          break;                 
        }
        case 1: // el msj proviene del topico link
        {          
          client.unsubscribe(&detectTopic[0]); // se desubscribe del topico detect ya que pasa a estar enlazado          
          if(msjRecibido != "Reset") 
          { 
             Serial.print("Suscripto a ");
             Serial.println(msjRecibido);
             Serial.println("Enlace realizado con exito!");
             deviceTopic = msjRecibido;
             client.subscribe(&deviceTopic[0]);             
          }
          else
          {            
            Serial.println("Reseteando dispositivo...");
            client.unsubscribe(&deviceTopic[0]);
            client.subscribe(&detectTopic[0]);
            deviceTopic = "";
            digitalWrite(D5,HIGH);                        
          }
          break;                  
        }
        case 2: //el mensaje es de funcionamiento
        {
          Serial.println("Recibi mensaje de mi objeto real");
          if (msjRecibido == "Apagar")
          {            
            digitalWrite(D5,HIGH);            
          }
          if (msjRecibido == "Vel-1")
          {            
            digitalWrite(D5, LOW);
            digitalWrite(D0, HIGH);
            digitalWrite(D1, HIGH);            
          } 
          if (msjRecibido == "Vel-2")
          {            
            digitalWrite(D5, LOW);
            digitalWrite(D0, HIGH);
            digitalWrite(D1, LOW);            
          }
          if (msjRecibido == "Vel-3")
          {            
            digitalWrite(D5, LOW);
            digitalWrite(D0, LOW);
            digitalWrite(D1, LOW);            
          }            
          break;
        }
        default:
          Serial.println("Algo anduvo mal");
          break;               
  }
}

int topicoToOpt (String str)
{
  if (str == detectTopic)
    return 0;
  if (str == linkTopic)
    return 1;   
  if (str == deviceTopic) 
    return 2;
  return -1; // ocurrio algo inesperado     
}

void reconnect() 
{
  // Loop hasta que se reconecte
  while (!client.connected()) 
  {    
    Serial.print("Attempting MQTT connection...");
    titilar(250);    
    // Crea un ID de cliente al azar
    //Eventualmente considerar reemplazar esto por la MAC
    String clientId = "ESP8266Client-";
    clientId += String(random(0xffff), HEX);
    // Intentando conectar
    if (client.connect(clientId.c_str())) 
    {
      Serial.println("connected");      
      Serial.print("Suscripto al topico: ");
      Serial.println(detectTopic);
      client.subscribe(&detectTopic[0]);
      Serial.print("Suscripto al topico: ");
      Serial.println(linkTopic);
      client.subscribe(&linkTopic[0]);      
    } else 
    {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
  digitalWrite(BUILTIN_LED, LOW);
}

void setup() {
  pinMode(BUILTIN_LED, OUTPUT);     // Initialize the BUILTIN_LED pin as an output  pinMode(PIN_F1, INPUT);  
  pinMode(D5, OUTPUT);
  pinMode(D1, OUTPUT);
  pinMode(D0, OUTPUT);

  digitalWrite(D5, HIGH);
  digitalWrite(D0, HIGH);
  digitalWrite(D1, HIGH);
  digitalWrite(BUILTIN_LED, HIGH);  
 
  macStr = (String)WiFi.macAddress();     
  linkTopic = configTopic + '/' + macStr; // armo el topico de configuracion que me corresponde "Config/MiDirMac"  
  deviceTopic = ""; // cuando el topico de enlace esta vacio significa que no estoy enlazado
  Serial.begin(115200);
  setup_wifi();
  client.setServer(mqtt_server, 1883);
  client.setCallback(callback);   
}

void loop() 
{
  if (!client.connected()) 
  {
    reconnect();
  }
  if (deviceTopic == "") // Si estoy esperando enlazarme
  {
    titilar(500);
  } else 
  {
    if (digitalRead(BUILTIN_LED) != LOW)
    {
      digitalWrite(BUILTIN_LED, LOW); // Si tengo un enlace dejo prendido el led interno
    }    
  }
  client.loop();
}
