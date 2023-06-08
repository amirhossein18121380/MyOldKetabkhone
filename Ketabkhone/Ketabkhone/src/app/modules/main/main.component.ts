import { DOCUMENT } from '@angular/common';
import { Component, HostListener, Inject, OnInit, Renderer2, TemplateRef } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AccountService } from 'src/app/shared/services/account.service';
import { AccountClient, UserClient } from 'src/app/shared/services/api.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.css']
})
export class MainComponent implements OnInit {
  whatsApp = environment.whatsApp;

  scrollTop = 0;
  isCollapsed = true;
  currentUrl = '/';

  isLoaded: boolean = false;
  isShowLoading: boolean = true;
  isError: boolean = false;
  errorMessage: string = '';

  logoutModalRef: BsModalRef;

  playerImagePath = environment.profileThumbnailPicturePath

  constructor(@Inject(DOCUMENT) private document: Document,
              private renderer2: Renderer2,
              private toaster: ToastService,
              public accountService: AccountService,
              private router: Router,
              private userClient: UserClient,
              private modalService: BsModalService,
              private accountClient: AccountClient,) {

    this.router.events.subscribe(value => {
      if (value instanceof NavigationEnd) {
        this.currentUrl = value.url;
        this.isCollapsed = true;
      }
    });

    // load page
    this.loadPage();
  }

  ngOnInit(): void {
  }

  @HostListener('window:scroll', ['$event'])
  onScroll() {
    this.scrollTop = document.documentElement.scrollTop;
  }

  scrollToTop() {
    window.scroll({
      top: 0,
      left: 0,
      behavior: 'smooth'
    });
  }

  reload() {
    this.isError = false;
    this.errorMessage = ''
    this.loadPage();
  }

  loadPage() {
    if (!this.accountService.isAuthenticated()) {
      setTimeout(() => this.isLoaded = true, 1000)
      setTimeout(() => this.isShowLoading = false, 1200)
    } else {
      //this.getProfileInfo();
    }
  }

  // logout() {
  //   //this.spinner.show();
  //   this.accountService.logOut();
  //   //this.spinner.hide();
  // }

  logout(confirm: boolean) {
    this.logoutModalRef.hide();
    if (confirm) {
      this.accountClient.logout(this.accountService.getRefreshToken()).subscribe(value => {
        this.accountService.setTokens(null, null);
        this.toaster.showSuccess2('با موفقیت از حساب خود خارج شده اید.');
        this.router.navigate(['/'], {replaceUrl: true}).then()
      }, error => {
        this.accountService.setTokens(null, null);
        this.toaster.showSuccess2('با موفقیت از حساب خود خارج شده اید.');
        this.router.navigate(['/'], {replaceUrl: true}).then()
      })
    }
  }

  onClickLogout(template: TemplateRef<any>) {
    this.logoutModalRef = this.modalService.show(
      template,
      Object.assign({}, { class: 'modal-md' })
    );
  }


  

  getProfileInfo() {
    this.userClient.profileInfo().subscribe(value => {
      this.accountService.setProfileInfo(value);
      this.isLoaded = true;
      console.log(this.isLoaded)
      setTimeout(() => this.isShowLoading = false, 200)
    }, error => {
      if (error.status === 401) {
        this.isLoaded = true
        setTimeout(() => this.isShowLoading = false, 200)
      } else {
        this.isError = true;
        this.errorMessage = error.response;
        if (!this.errorMessage || error.length === 0) {
          this.errorMessage = 'مشکل در برقراری ارتباط با سرور، لطفا دوباره تلاش نمایید.'
        }
        this.toaster.showError2(this.errorMessage);
      }

    })
  }
}
