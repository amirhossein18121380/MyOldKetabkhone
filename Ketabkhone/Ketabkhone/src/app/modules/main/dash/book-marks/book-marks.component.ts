import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { Book, UserBook, UserBookClient } from 'src/app/shared/services/api.service';

@Component({
  selector: 'app-book-marks',
  templateUrl: './book-marks.component.html',
  styleUrls: ['./book-marks.component.css']
})
export class BookMarksComponent implements OnInit {
  studentList$! : Observable<Book[]>;
  userbookList$! : Observable<UserBook[]>;

  bookid: number;

  constructor(private userbook: UserBookClient,private _activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.studentList$ = this.userbook.getUserBookMarksByUserid();
    this.userbookList$ = this.userbook.getAllByUserId();
    //this.bookid = this._activatedRoute.snapshot.params['id'];
  }

  // Getlibrary() {
  //   try {
  //     let res = this.userbook
  //       .getlibrarybyuserid()
  //       .toPromise()
  //       .then(
  //         (result) => {
  //           this.dataSource = new MatTableDataSource(result);
  //           //this.totalLength = result.totalRecords;
  //           this.dataSource.sort = this.sort;
  //           //this.studentList$ = result;

  //           //result = this.listofbook;
  //         },
  //       );
  //   } catch (error) {
  //     console.log(error);
  //   }
  //}

  deletebook(bookid: number) {
    if (confirm("Are you sure you want to delete this ?")) {
      console.log(bookid)
      this.userbook.deleteFromBookMarks(bookid).subscribe(() => {
       
      });
    }
    this.studentList$
  }



  // isbookmarkChange(e) {
  //   if (e.checked) {
  //     this.userbook.addToBookMarks(this.bookid)
  //   } else {
  //     this.userbook.deleteFromBookMarks(this.bookid)
  //   }
  // }
}
