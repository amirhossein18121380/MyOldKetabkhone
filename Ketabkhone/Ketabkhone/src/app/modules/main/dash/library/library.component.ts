import { Book, BookGetListFilterViewModel, BookListViewModel, UserBook, UserBookClient } from './../../../../shared/services/api.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { MatSort } from '@angular/material/sort';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-library',
  templateUrl: './library.component.html',
  styleUrls: ['./library.component.css']
})
export class LibraryComponent implements OnInit {

  //public filterModel: BookGetListFilterViewModel =new BookGetListFilterViewModel();
  public dataSource: MatTableDataSource<Book>;
  public totalLength: number;
  public pageSize: number;

  displayedColumns: string[] = [
    "img",
    "id",
    "bookName",
    "publisher",
    "yearOfPublication",
    "bookFormat",
    "bookType",
    "numberOfPages",
    "language",
    "isbn",
    "electronicVersionPrice",
  ];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;

  //public listofbook: UserBook;
 // public listofbook: UserBook = new UserBook();
  studentList$! : Observable<Book[]>;
  //listofbook: Book[];
  bookid: number;
  IsPurchase: boolean;
  
  userbookList$! : Observable<UserBook[]>;

  //public userb: UserBook = new UserBook();

  constructor(private userbook: UserBookClient,private _activatedRoute: ActivatedRoute,) {
    

    //this.Getlibrary();
    this.bookid = this._activatedRoute.snapshot.params['id'];
    //this.GetIsPurchases();

  //   if(this.bookid !== null && !undefined){
  //     this.Getlibrary();
  //  }  
  }

  ngOnInit(): void {
    this.studentList$ = this.userbook.getlibrarybyuserid();
    this.userbookList$ = this.userbook.getAllByUserId();
    console.log(this.bookid)
  }
  
  
  GetIsPurchases() {
    try {
      let res = this.userbook
        .getAllByUserId()
        .toPromise()
        .then(
          (result) => {
            //this.userb = result;
            //this.userbookList$ = result
            //console.log(result)
            //console.log(this.IsPurchase)
            //this.dataSource = new MatTableDataSource(result);
            //this.totalLength = result.totalRecords;
            //this.dataSource.sort = this.sort;
            //this.studentList$ = result;

            //result = this.listofbook;
          },
        );
    } catch (error) {
      console.log(error);
    }
  }



  deletebook(bookid: number) {
    if (confirm("Are you sure you want to delete this ?")) {
      console.log(bookid)
      this.userbook.deleteFromLibrary(bookid).subscribe(() => {
        //this.Getlibrary();
      });
    }
    this.studentList$
  }
  
  // async pageChanged(e: PageEvent) {
  //   await this.Getlibrary();
  // }

}