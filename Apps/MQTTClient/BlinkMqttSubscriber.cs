using ActionFramework.Logger;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MQTTClient
{
    public class BlinkMqttSubscriber : ActionFramework.Action
    {
        //https://github.com/chkr1011/MQTTnet/wiki/Client

        private static readonly string blinkUsername = "ola.uden@gmail.com";
        private static readonly string blinkPassword = "Visby1234#";

        public string Username { get; set; }
        public string Password { get; set; }
        public string TcpServerUri { get; set; }
        public string SubscribeTopic { get; set; }
        
        public override object Run(dynamic obj)
        {
            TcpServerUri = "mqtt+ssl://portal.blink.services";
            SubscribeTopic = "sensor/lummelunda/unknown/class_a_otaa/a81758fffe0307e6/payload";
            Username = "johan@genza.nu";
            Password = "temp_test_pw1";



            // Create a new MQTT client.
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            // Setup and start a managed MQTT client.
                var options = new MqttClientOptionsBuilder()
                .WithClientId("Client1")
                .WithTcpServer(TcpServerUri)
                .WithCredentials(Username, Password)
                .WithTls()
                .WithCleanSession()
                .Build();

            mqttClient.ConnectAsync(options);
            //mqttClient.ConnectAsync(options);

            mqttClient.UseConnectedHandler(async e =>
            {
                ActionLogger.FileLog.Information("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("my/topic").Build());

                ActionLogger.FileLog.Information("### SUBSCRIBED ###");
            });

            //mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(SubscribeTopic).Build());



            // StartAsync returns immediately, as it starts a new thread using Task.Run, 
            // and so the calling thread needs to wait.
            //Console.ReadLine();

            //Consuming messages
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {

                ActionLogger.FileLog.Information("### RECEIVED APPLICATION MESSAGE ###");
                ActionLogger.FileLog.Information($"+ Topic = {e.ApplicationMessage.Topic}");
                ActionLogger.FileLog.Information($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                ActionLogger.FileLog.Information($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                ActionLogger.FileLog.Information($"+ Retain = {e.ApplicationMessage.Retain}");
            });


            /*


            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            // Create TCP based options using the builder.
            var options = new MqttClientOptionsBuilder()
                //.WithClientId("Client1")
                .WithTcpServer("mqtt+ssl://portal.blink.services:8883")//"broker.hivemq.com")
                .WithCredentials(blinkUsername, blinkPassword)
                .WithTls()
                .WithCleanSession()
                .Build();

            /*
            // Use TCP connection.
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.hivemq.com", 1883) // Port is optional
                .Build();

            // Use secure TCP connection.
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.hivemq.com")
                .WithTls()
                .Build();
            */

            /*
            mqttClient.ConnectAsync(options);


            //Consuming messages
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
            });



            //Subscribing to a topic
            //Once a connection with the server is established subscribing to a topic is possible.The following code shows how to subscribe to a topic after the MQTT client has connected.
            string devEui = "";
            string topic = "sensor/lummelunda/unknown/class_a_otaa/a81758fffe0307e6/payload";//$"sensor/blinkservices/unknown/other/{devEui}/payload";

            mqttClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("my/topic").Build());

                Console.WriteLine("### SUBSCRIBED ###");
            });
            */


            return "";

        }
    }
}
