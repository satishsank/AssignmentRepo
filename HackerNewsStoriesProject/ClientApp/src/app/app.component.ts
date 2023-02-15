import { Component } from '@angular/core';
import { NgxPaginationModule } from 'ngx-pagination';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  })
export class AppComponent {
  title = 'Hacker Newest Stories';
  searchText: string = '';
  p: number = 1;
 }
