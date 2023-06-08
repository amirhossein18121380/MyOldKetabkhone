import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ResetPasswordComponent } from './reset-password.component';

const routes: Routes = [
   {path: '', component: ResetPasswordComponent, children: []},
   {path: ':id', component: ResetPasswordComponent, children: [], data: {breadcrumb: 'تغییر رمز عبور'}}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ResetPasswordRoutingModule { }
