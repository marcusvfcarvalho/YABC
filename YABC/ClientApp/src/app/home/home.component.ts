import { Component, ElementRef, OnChanges, OnInit, ViewChild } from '@angular/core';
import { Block } from '../models/block';
import { HttpClient } from '@angular/common/http'
import { catchError, map, Observable, retry, throwError } from 'rxjs';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { CreateBlockComponent } from '../create-block/create-block.component';
import { Person } from '../models/person';
import { CreateBlockViewModel } from '../models/create-block';
import { SignalRService } from '../services/signal-r.service';
import { NotificationMessage } from '../models/notification-message';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  public dataSource!: MatTableDataSource<Block>;
  public columnsToDisplay = ['id', 'name', "hash256", "nonce", "view"];

  public people!: Person[];

  constructor(private _httpClient: HttpClient, public signalRService: SignalRService) { }

  @ViewChild(MatTable) table!: MatTable<any>;
  @ViewChild('paginator') paginator!: MatPaginator;
  @ViewChild('createBlock') createBlock!: CreateBlockComponent;

  ngOnInit(): void {
    this.signalRService.startConnection();
    this.signalRService.hubConnection.on("ReceiveNotification", (data) => {
      this.updateBlock(data);
    });

    this.getBlocks().subscribe(data => {
      this.dataSource = new MatTableDataSource(data);
      this.dataSource.paginator = this.paginator;
      if (this.table) {
        this.table.renderRows();
      }
    });

    this.getPeople().subscribe(data => {
      this.people = data;
    })

  }

  showTooltip(id: number) {
    alert(id);
  }

  getPeople(): Observable<Person[]> {
    return this._httpClient.get<Person[]>("https://localhost:7007/api/people")
      .pipe(
        retry(1),
        catchError(this.httpError)
      )
  }

  getBlocks(): Observable<Block[]> {
    return this._httpClient.get<Block[]>("https://localhost:7007/api/blocks")
      .pipe(
        retry(1),
        catchError(this.httpError)
      )
  }

  postBlock(block: CreateBlockViewModel): Observable<Block> {
    return this._httpClient.post("https://localhost:7007/api/blocks", block)
      .pipe(
        map(this.extractData),
        catchError(this.httpError)
      )
  }

  httpError(error: { error: { message: string; }; status: any; message: any; }) {
    let msg = '';
    if (error.error instanceof ErrorEvent) {
      msg = error.error.message;
    } else {
      msg = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    console.log(msg);
    return throwError(msg);
  }

  postNewImage(e: any) {
    this.postBlock(e).subscribe(data => {
    });
  }

  private extractData(res: any) {
    let body = res;
    return body;
  }

  private updateBlock(data: NotificationMessage) {
    switch (data.messageType) {
      case 0:
        this.dataSource.data.unshift(data.block);
        break;
      case 1:
        var index = this.dataSource.data.findIndex((block) => {
          return block.id == data.block.id
        })
        if (index != -1) {
          this.dataSource.data[index] = data.block;
        } else {
          this.dataSource.data.unshift(data.block);
        }
        break;
    }

    this.dataSource.data =  this.dataSource.data;
    this.table.renderRows()
  }

}

