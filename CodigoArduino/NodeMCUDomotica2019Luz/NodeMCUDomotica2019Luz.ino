#include <ESP8266WiFi.h>
#include <PubSubClient.h>

#define BUILTIN_LED 2
#define LED_D1 14 // tiene el valor real de unity
#define LED_D2 12 // valor invertido de unity 
#define LED_D3 13 //lectura del estado de la luz
int cant=0;
int prendida=0;
unsigned long myTime; 
long lastPublishMillis;
// Update these with values suitable for your network.

//Wi-Fi
/*
const char* ssid = "externalLIDI";
const char* password = "lab3SalasCMR";
const char* mqtt_server = "192.168.0.105";
*/
const char* ssid = "TP-Link_BerissoConectado";
const char* password = "16791115";
const char* mqtt_server = "192.168.1.16";


//Topics
String detectTopic = "Detect";
String configTopic = "Config";
String linkTopic;
String deviceTopic;
String deviceTopicInfo=""; //publica el estado del objeto sin importoar si se apago del interruptor o la app, se realiza comunicacion bidireccional.

WiFiClient espClient;
PubSubClient client(espClient);

long lastMsg = 0;
char msg[50];
int value = 0;
char topico[50]="";
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

void callback(char* topic, byte* payload, unsigned int length) 
{   
 // Serial.print("Mensaje recibido - Contenido: ");
  String msjRecibido = "";
  String topicoRecibido = "";
  for (int i = 0; i < length; i++) 
  {
    msjRecibido += (char)payload[i];      
  }
 // Serial.print(msjRecibido);
 // Serial.print(" - Topico: ");
  for (int i = 0; topic[i]!='\0'; i++) 
  {
    topicoRecibido += topic[i];    
  }
 // Serial.println(topicoRecibido); 
  int topicoVar = topicoToOpt(topic);
 // Serial.print("Topic Code: ");
 // Serial.println(topicoVar);
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
        case 1: // el msj proviene del topico link=config/mac
        {          
          client.unsubscribe(&detectTopic[0]); // se desubscribe del topico detect ya que pasa a estar enlazado          
          if(msjRecibido != "Reset") 
          { 
             Serial.print("Suscripto a ");
             Serial.println(msjRecibido);
             Serial.println("Enlace realizado con exito!");
             deviceTopic = msjRecibido;
             client.subscribe(&deviceTopic[0]);  
             deviceTopicInfo= "informacion/" + msjRecibido;
           //  Serial.print("Suscripto a ");
             //Serial.println(deviceTopicInfo);
             //client.subscribe(&deviceTopicInfo[0]); // se suscrbe a un topico para publicar los mensajes de estado del del objeto
          }
          else
          {            
            Serial.println("Reseteando dispositivo...");
            client.unsubscribe(&deviceTopic[0]);
            client.unsubscribe(&deviceTopicInfo[0]);
            client.subscribe(&detectTopic[0]);
            deviceTopic = "";
            digitalWrite(LED_D1, HIGH);                        
          }
          break;                  
        }
        case 2: //el mensaje es de funcionamiento
        {
        //  Serial.println("Recibi mensaje de mi objeto real");
          
          lastPublishMillis=millis()-lastPublishMillis;
          Serial.println(lastPublishMillis); 
          lastPublishMillis=millis();
          
          if (msjRecibido == "Prender") {
       
            if (digitalRead(LED_D3)==HIGH && digitalRead(LED_D1)==LOW ){ //SI LA LUZ ESTA PRENDIDA
                 digitalWrite(LED_D2, LOW);
                 digitalWrite(LED_D1, HIGH);
            }else{
            if (digitalRead(LED_D3)==HIGH && digitalRead(LED_D1)==HIGH){
                 digitalWrite(LED_D1, LOW);
                 digitalWrite(LED_D2, HIGH);
                   }
                }
        }else{
            
         if (digitalRead(LED_D3)==LOW && digitalRead(LED_D1)==LOW ){ //SI LA LUZ ESTA PRENDIDA
               digitalWrite(LED_D1, HIGH);
               digitalWrite(LED_D2, LOW);
         }else{
             if ( digitalRead(LED_D3)==LOW && digitalRead(LED_D1)==HIGH ){
                   digitalWrite(LED_D2, HIGH);
                   digitalWrite(LED_D1, LOW);}
               }
      }
            
            break;
        }
        default:
          Serial.println("Algo anduvo mal");
          break;               
  }
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
   pinMode(LED_D1,OUTPUT);
  pinMode(LED_D2,OUTPUT);
  pinMode(LED_D3,INPUT);
  
  digitalWrite(BUILTIN_LED, HIGH);  
  digitalWrite(LED_D1, HIGH);  

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
   if(digitalRead(LED_D3)== HIGH && cant == 0){
   
    prendida=0;
    cant++;
   // Serial.println("Tiempo de publciacion: ");
   // myTime= millis(); 
    if(deviceTopic!=""){client.publish(&deviceTopicInfo[0], "OFF");}
   // Serial.println(myTime);
    // Serial.println("LUZ APAGADA");
      }else{
        if(digitalRead(LED_D3)==LOW && prendida==0){
          prendida++;
          myTime= millis(); 
             if(deviceTopic!=""){client.publish(&deviceTopicInfo[0], "ON");}
          //client.publish(&deviceTopic[0], "ON");
         // myTime = millis() - myTime;
         // Serial.println("Tiempo de publciacion: ");
         // Serial.println(myTime);
        //  Serial.println("LUZ PRENDIDA");
          
        
          cant=0;
        }
    }
  client.loop();
}
