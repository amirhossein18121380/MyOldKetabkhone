import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from 'src/app/shared/services/auth.guard';
import { LoginComponent } from './login.component';

const loginRoutes: Routes = [
  { path: '', canActivate: [AuthGuard], component: LoginComponent, children: [] }
];

@NgModule({
  imports: [RouterModule.forChild(loginRoutes)],
  exports: [RouterModule]
})
export class LoginRoutingModule { }
