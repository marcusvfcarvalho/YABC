import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  public constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl('https://localhost:7007/blockHub')
    .build();
  }


  public hubConnection: signalR.HubConnection;

  public startConnection = () => {
    console.log("Connecting");
   
    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err))
  }


}