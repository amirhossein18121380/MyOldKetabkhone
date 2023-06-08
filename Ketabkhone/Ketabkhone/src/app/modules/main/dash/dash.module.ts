import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';

import { DashRoutingModule } from './dash-routing.module';
import { DashComponent } from './dash.component';
import { ProfileComponent } from './profile/profile.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { DepositComponent } from './deposit/deposit.component';
import { TransactionsComponent } from './transactions/transactions.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import {MatTableModule} from "@angular/material/table";
import { MatPaginatorModule } from '@angular/material/paginator';
import { LibraryComponent } from './library/library.component';
import { BookMarksComponent } from './book-marks/book-marks.component';
import { ShareModule } from 'src/app/shared/share.module';
import { MatSortModule } from '@angular/material/sort';

@NgModule({
  declarations: [
    DashComponent,
    ProfileComponent,
    DepositComponent,
    TransactionsComponent,
    DashboardComponent,
    LibraryComponent,
    BookMarksComponent
  ],
  imports: [
    CommonModule,
    DashRoutingModule,
    ReactiveFormsModule,
    ModalModule.forRoot(),
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    //TooltipModule.forRoot(),
    ShareModule,
  ],
  providers: [CurrencyPipe]
})
export class DashModule { }
