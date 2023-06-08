import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BookRoutingModule } from './book-routing.module';
import { BookComponent } from './book.component';
import { ShareModule } from 'src/app/shared/share.module';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { StarRatingModule } from 'angular-rating-star';


@NgModule({
  declarations: [
    BookComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    BookRoutingModule,
    ShareModule,
    ReactiveFormsModule,
    MatCheckboxModule,
    FormsModule,
    StarRatingModule,
    //NgbRatingModule 

  ]
})
export class BookModule { }
