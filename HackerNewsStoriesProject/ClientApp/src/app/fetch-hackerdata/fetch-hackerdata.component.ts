import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NgxPaginationModule } from 'ngx-pagination';

@Component({
  selector: 'app-fetch-hackerdata',
  templateUrl: './fetch-hackerdata.component.html'
})
export class FetchHackerDataComponent {
  public hackernewslists: HackerNewsList[] = [];
  title = 'Hacker Stories';
  searchText: string = '';
  pagenumber: number = 1;
  pageitemsize: number = 10;
  pageitemtotalcount: number = 500;
  
  handlePageChange(event: any)
  {
    this.pagenumber = event;
  }

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<HackerNewsList[]>(baseUrl + 'hackernewslist').subscribe(result => {
      this.hackernewslists = result;
    }, error => console.error(error));
  }
}

interface HackerNewsList {
  id: number;
  text: string;
  by: string;
  time: string ;
  title: string;
  url: string;
  deleted: boolean;
 }
