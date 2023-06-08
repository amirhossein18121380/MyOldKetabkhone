import { BookMarksComponent } from './book-marks/book-marks.component';
import { LibraryComponent } from './library/library.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashComponent } from './dash.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { DepositComponent } from './deposit/deposit.component';
import { ProfileComponent } from './profile/profile.component';
import { TransactionsComponent } from './transactions/transactions.component';

const dashRoutes: Routes = [
  {
    path: '', component: DashComponent,
    children: [
      {path: "", redirectTo: "/panel/dashboard", pathMatch: 'full'},
      {path: "dashboard", component: DashboardComponent, pathMatch: "full"},
      {path: "profile", component: ProfileComponent, pathMatch: "full"},
      {path: "bookmarks", component: BookMarksComponent, pathMatch: "full"},
      {path: "library", component: LibraryComponent, pathMatch: "full"},
      {path: "deposit", component: DepositComponent, pathMatch: "full"},
      {path: "transactions", component: TransactionsComponent, pathMatch: "full"},
    ],
  }
];

@NgModule({
  imports: [RouterModule.forChild(dashRoutes)],
  exports: [RouterModule]
})
export class DashRoutingModule { }