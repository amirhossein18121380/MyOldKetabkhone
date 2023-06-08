import { AccountService } from './../../../shared/services/account.service';
import { Component, OnInit } from '@angular/core';


@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  constructor(private account: AccountService) { }

  ngOnInit(): void {
  }

   logout() {
    //this.spinner.show();
    this.account.logOut();
    //this.spinner.hide();
}

}
