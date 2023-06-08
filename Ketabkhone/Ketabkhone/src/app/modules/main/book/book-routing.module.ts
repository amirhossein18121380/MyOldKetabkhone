import { BookComponent } from './book.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {path: '', component: BookComponent, children: []},
  {path: ':id', component: BookComponent, children: [], data: {breadcrumb: 'تغییر رمز عبور'}}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BookRoutingModule { }
