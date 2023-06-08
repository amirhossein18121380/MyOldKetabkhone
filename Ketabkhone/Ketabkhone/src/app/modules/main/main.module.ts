import { ShareModule } from './../../shared/share.module';
import { HomeComponent } from './home/home.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MainRoutingModule } from './main-routing.module';
import { MainComponent } from './main.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { RegisterRoutingModule } from './register/register-routing.module';
import { ModalModule } from 'ngx-bootstrap/modal';


@NgModule({
  declarations: [
    MainComponent,
    HeaderComponent,
    FooterComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MainRoutingModule,
    RegisterRoutingModule,
    ModalModule.forRoot(),
    ShareModule
    //CollapseModule.forRoot(),
    //BsDropdownModule.forRoot(),
  ]
})
export class MainModule { }
