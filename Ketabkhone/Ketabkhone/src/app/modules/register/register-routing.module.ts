import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from 'src/app/shared/services/auth.guard';
import { RegisterComponent } from './register.component';

const registerRoutes: Routes = [
  {path: '',canActivate: [AuthGuard], component: RegisterComponent, children: []}
];

@NgModule({
  imports: [RouterModule.forChild(registerRoutes)],
  exports: [RouterModule]
})
export class RegisterRoutingModule { }
