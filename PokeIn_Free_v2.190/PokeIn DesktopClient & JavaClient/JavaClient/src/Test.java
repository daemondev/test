/*
 * PokeIn Java Client 1.0
 * Copyright Zondig sp. 2013 All Rights Reserved.
 * 
 * www.pokein.com
 */
import java.util.Calendar;

public class Test{
		public Test(){}
		
		public void ServerTimeUpdated(Calendar dt){
			System.out.println("Date Received:" + socketTest.client.DateTo(dt) );
		}
		
		public void Subscribed(){
			System.out.println("Subscribed to time group");
		}
	}
