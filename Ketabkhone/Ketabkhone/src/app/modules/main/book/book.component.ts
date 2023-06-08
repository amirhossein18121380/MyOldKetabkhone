import { BookClient, BookViewModel, Dropdownget, UserBook, RateClient, GetRateSendViewModel, RateViewModel, Like, LikeClient, GetLikeSendViewModel, ChangeLikeViewModel } from './../../../shared/services/api.service';
import { Component, OnInit } from '@angular/core';
import { Observable, reduce } from 'rxjs';
import { Book, UserBookClient } from 'src/app/shared/services/api.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-book',
  templateUrl: './book.component.html',
  styleUrls: ['./book.component.css']
})
export class BookComponent implements OnInit {


  //stars: number[] = [1, 2, 3, 4, 5];
  //public rate: number;

  bookid: number;
  student$!: Observable<Book>;
  public book!: Observable<BookViewModel>;
  public info: BookViewModel = new BookViewModel();

  public averagerate: number;

  selectedAuthors: Dropdownget[];
  selectedTranslators: Dropdownget[];
  //selectedTranslators: number[] = [];
  selectedSubjects: Dropdownget[];
  selectedCategories: Dropdownget[];

 
  IsLiked: boolean;
  IsDesLiked: boolean;

  dataSet = {
    starSize: 20,
    colors: ["#ff4545", "#ffa534", "#c3c366", "#6ac34e", "#45B523"], // Set the five colours 
    //showNumber: false, // hide the visible in the label
    labels: ["Bad", "Not Bad", "Average", "Good", "Best",], // Set the five Labels 
    labelsStyle: {
      background: '#3F51B5',
      color: '#f8f8f8',
    }
  }

  public userb: UserBook = new UserBook();
  public getrate: GetRateSendViewModel = new GetRateSendViewModel();
  public addrate: RateViewModel = new RateViewModel();
  public sendlike: GetLikeSendViewModel = new GetLikeSendViewModel();
  public like: Like = new Like();
  public adlike: ChangeLikeViewModel = new ChangeLikeViewModel();

  constructor(private userbook: UserBookClient,
    private BookClient: BookClient,
    private likeClient: LikeClient,
    //private loading: LoadingService,
    private _activatedRoute: ActivatedRoute,
    private rateClient: RateClient) {
    this.bookid = this._activatedRoute.snapshot.params['id'];

    this.getrate.entityId = this.bookid;
    this.getrate.entityType = 4;

    this.sendlike.entityType = 4;
    this.sendlike.entityId = this.bookid;
  }

  ngOnInit(): void {
    this.book = this.BookClient.getById(this.bookid);
    this.getBookById(this.bookid);
    this.getBookById2(this.bookid);
    this.getRateforBook();
    this.getRateforBookbyuser();
    this.getLikeforBookbyuser();
    console.log(this.userb.isMarked)
  }

  countStar(star) {
    console.log('Value of star', star);
    //console.log(this.rate)
  }

  async getLikeforBookbyuser() {
    try {
      let res = await this.likeClient
        .getLike(this.sendlike)
        .toPromise()
        .then(
          (res) => {
            this.like = res;
            if(res.type == 1){
              this.IsLiked = true;
            }
            else{
              this.IsLiked = false;
            }

            if(res.type == 2){
              this.IsDesLiked = true;
            }
            else{
              this.IsDesLiked = false;
            }

            console.log(res); 
            //this.rate = res.rateValue;    
            //console.log(this.rate)    
          },
          (err) => {
            //this.loading.showError("", err.response);
          }
        );
    } catch (error) {
      console.log(error);
    }
  }

  async getRateforBookbyuser() {
    try {
      let res = await this.rateClient
        .getRate(this.getrate)
        .toPromise()
        .then(
          (res) => {
            console.log(res); 
            //this.rate = res.rateValue;    
            //console.log(this.rate)    
          },
          (err) => {
            //this.loading.showError("", err.response);
          }
        );
    } catch (error) {
      console.log(error);
    }
  }

  async addRate(rate: number) {
    try {
      this.addrate.entityType = 4;
      this.addrate.entityId = this.bookid;
      this.addrate.rateValue = rate;
      console.log(rate)
      console.log(this.addrate)
      let res = await this.rateClient
        .addRate(this.addrate)
        .toPromise()
        .then(
          (res) => {
            console.log(res);

 
          },
          (err) => {
            //this.loading.showError("", err.response);
          }
        );
    } catch (error) {
      console.log(error);
    }
  }

  async getRateforBook() {
    try {
      let res = await this.rateClient
        .getTheAvrRateByEntityIdForBook(this.bookid)
        .toPromise()
        .then(
          (res) => {
            console.log(res);
            this.averagerate = res
          },
          (err) => {
            //this.loading.showError("", err.response);
          }
        );
    } catch (error) {
      console.log(error);
    }
  }

  async getBookById(bookid: number) {
    try {
      let res = await this.BookClient
        .getById(bookid)
        .toPromise()
        .then(
          (res) => {
            this.info = res;
            console.log(res);

            this.selectedTranslators = res.translator
            this.selectedAuthors = res.author
            this.selectedCategories = res.category
            this.selectedSubjects = res.subjects

            // console.log(this.selectedTranslators)

          },
          (err) => {
            //this.loading.showError("", err.response);
          }
        );
    } catch (error) {
      console.log(error);
    }
  }

  async getBookById2(bookid: number) {
    try {
      let res = await this.userbook
        .getByBookIdAndUserId(bookid)
        .toPromise()
        .then(
          (res) => {
            this.userb = res;
            console.log(res);
            // console.log(this.selectedTranslators)
          },
          (err) => {
            //this.loading.showError("", err.response);
          }
        );
    } catch (error) {
      console.log(error);
    }
  }


  addtolibrary() {
    try {
      let res = this.userbook
        .addToLibrary(this.bookid)
        .toPromise()
        .then(
          (res) => {
            //this.info = res;
            //console.log(res)

          },
          (err) => {
            //this.loading.showError("", err.response);
          }
        );
    } catch (error) {
      console.log(error);
    }
  }

  addtoBookMarks() {
    try {
      let res = this.userbook
        .addToBookMarks(this.bookid)
        .toPromise()
        .then(
          (res) => {
            //this.info = res;
            //console.log(res)

          },
          (err) => {
            //this.loading.showError("", err.response);
          }
        );
    } catch (error) {
      console.log(error);
    }
  }

  deletefrombookmarks(bookid: number) {
    if (confirm("Are you sure you want to delete this ?")) {
      console.log(bookid)
      this.userbook.deleteFromBookMarks(bookid).subscribe(() => {

      });
    }
  }


  async addlike(liketype: number) {
    try {
      this.adlike.entityType = 4;
      this.adlike.entityId = this.bookid;
      this.adlike.type = liketype;
      console.log(this.adlike)
      console.log(this.addrate)
      let res = await this.likeClient
        .addLike(this.adlike)
        .toPromise()
        .then(
          (res) => {
            console.log(res);
 
          },
          (err) => {
            //this.loading.showError("", err.response);
          }
        );
    } catch (error) {
      console.log(error);
    }
  }

  async deletelike() {
    try {
      this.sendlike.entityType = 4;
      this.sendlike.entityId = this.bookid;
      let res = await this.likeClient
        .deleteBy(this.sendlike)
        .toPromise()
        .then(
          (res) => {
            console.log(res);
 
          },
          (err) => {
            //this.loading.showError("", err.response);
          }
        );
    } catch (error) {
      console.log(error);
    }
  }


  isbookmarkChange(e) {
    if (e.checked) {
      console.log(e)
      this.addtoBookMarks();
    } else {
      this.deletefrombookmarks(this.bookid);
    }
  }

  isLikeChange(e) {
    if (e.checked) {
      this.addlike(1);
    } else {
     this.deletelike();
    }
  }

  isDesLikeChange(e) {
    if (e.checked) {
      console.log(e)
      this.addlike(2);
    } else {
     this.deletelike();
    }
  }
}