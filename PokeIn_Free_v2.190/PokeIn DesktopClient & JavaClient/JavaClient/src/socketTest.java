/*
 * PokeIn Java Client 1.0
 * Copyright Zondig sp. 2013 All Rights Reserved.
 * 
 * www.pokein.com
 */
import PokeIn.Java.Client;
import PokeIn.Java.ClientEvents;
import PokeIn.Java.LogLevel;

public class socketTest {
	
	public static Client client;
	public static void main(String[] args) throws Exception {
		
		//update below URL to the one your web server application is located
		String connStr = "http://localhost:7777/host.PokeIn";
		client = new Client(new Test(), connStr);
		
		
		ClientEvents dev = new ClientEvents(){
			@Override
			public void OnErrorReceived(Client c, String Message) {
				System.out.println("Error received:" + Message);
			}

			@Override
			public void OnClientConnected(Client c) {
				System.out.println("Client connected, client Id:" + c.getClientId());
				
				//call subscribe method from the server (TestClass under Webserver app)
				c.Send("MyServer.Subscribe");
			}

			@Override
			public void OnClientDisconnected(Client c) {
				System.out.println("Disconnected :" + c.getClientId());
			}

			@Override
			public void OnEventLog(Client c, String log, LogLevel level) {
				if(level == LogLevel.Critical)
					System.out.println("Critical Event:" + log);
			}
		};
		
		client.Events = dev;
		client.Connect();
		
	}
}

