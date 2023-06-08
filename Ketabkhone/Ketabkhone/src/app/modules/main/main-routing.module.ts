import { AuthGuard } from './../../shared/services/auth.guard';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { environment } from 'src/environments/environment';
import { MainComponent } from './main.component';


const MainRoutes: Routes = [
  {
    path: '', component: MainComponent, children: [
      {
        path: '',
        loadChildren: () => import('./home/home.module').then(m => m.HomeModule),
        data: {
          breadcrumb: 'main page',
          title: environment.siteTitle,
          description: environment.siteDescription
        }
      },
      // {
      //   path: 'register',
      //   canActivate: [AuthGuard],
      //   loadChildren: () => import('src/app/modules/main/register/register.module').then(m => m.RegisterModule),
      //   data: {
      //     breadcrumb: 'register',
      //     title: 'ketabkhone',
      //     description: 'register'
      //   }
      // },
      // {
      //   path: 'login',
      //   canActivate: [AuthGuard],
      //   loadChildren: () => import('src/app/modules/main/login/login.module').then(m => m.LoginModule),
      //   data: {
      //     breadcrumb: 'login',
      //     title: 'enter',
      //     description: 'to log your account'
      //   }
      // },
      {
        path: 'book',
        canActivate: [AuthGuard],
        loadChildren: () => import('./book/book.module').then(m => m.BookModule),
        data: {
          breadcrumb: 'بازیابی رمز عبور',
          title: 'پوکرنت | بازیابی رمز عبور',
          description: 'بازیابی رمز عبور'
        }
      },
      {
        path: 'reset',
        canActivate: [AuthGuard],
        loadChildren: () => import('./reset-password/reset-password.module').then(m => m.ResetPasswordModule),
        data: {
          breadcrumb: 'بازیابی رمز عبور',
          title: 'پوکرنت | بازیابی رمز عبور',
          description: 'بازیابی رمز عبور'
        }
      },
      {
        path: 'panel',
        canActivate: [AuthGuard],
        loadChildren: () => import('./dash/dash.module').then(m => m.DashModule),
        data: {
          breadcrumb: 'کنترل پنل',
          title: 'پوکرنت | پنل کاربری',
          description: 'پنل کاربری'
        }
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(MainRoutes)],
  exports: [RouterModule]
})
export class MainRoutingModule { }
