import { ResetPasswordModule } from './modules/main/reset-password/reset-password.module';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './shared/services/auth.guard';

const routes: Routes = [
  {path: '', loadChildren: () => import('./modules/main/main.module').then(m => m.MainModule)},
  {path: 'login', loadChildren: () => import('./modules/login/login.module').then(m => m.LoginModule)},
  {path: 'register', loadChildren: () => import('./modules/register/register.module').then(m => m.RegisterModule)},
  {path: '**', loadChildren: () => import('./modules/not-found/not-found.module').then(m => m.NotFoundModule)}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
