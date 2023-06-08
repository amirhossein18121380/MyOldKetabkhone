import { CurrencyPipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { GatewayViewModel, TransactionClient } from 'src/app/shared/services/api.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-deposit',
  templateUrl: './deposit.component.html',
  styleUrls: ['./deposit.component.css']
})
export class DepositComponent implements OnInit {

  gatewayImagePath = environment.apiURL + '/api/Common/GetImageById/'

  paymentForm: FormGroup;
  amount: number = 0;
  isClicked: boolean;
  isSubmit: boolean;

  selectedGateWay: GatewayViewModel;
  paymentUrl: string;
  gateways: GatewayViewModel[];

  staticgateway: any[]

  constructor(private toaster: ToastService,
              private transactionClient: TransactionClient,
              private currencyPipe: CurrencyPipe,
              private router: Router) { }

  ngOnInit(): void {
    this.staticgateway = [
      {"id":1, "title": "Service 1", "minAmount": 809, "maxAmount": 1,"currencyTitle": "Toman"},
      {"id":2,"title": "Service 2", "minAmount": 8090, "maxAmount": 4, "currencyTitle": "dollar" },
      {"id":3, "title": "Service 3", "minAmount": 8090, "maxAmount": 10,"currencyTitle": "euro" }
    ]
    this.loadGateway();
    

    this.paymentForm = new FormGroup({
      amount: new FormControl( '', [
        Validators.required
      ])
    });

    this.paymentForm.valueChanges.subscribe(form => {
      if (form.amount?.replace(/\D/g,'').length > 0) {
        this.amount = form.amount.replace(/\D/g,'');
      } else {
        this.amount = 0;
      }
      if (form.amount) {
        this.paymentForm.patchValue({
          amount: this.currencyPipe.transform(form.amount.replace(/\D/g, '').replace(/^0+/, ''), 'US', '', '1.0-0')
        }, {emitEvent: false})
      }
    });
  }

  get f() {
    return this.paymentForm.controls;
  }

  loadGateway() {
    this.staticgateway = [
      {"id":1, "title": "Service 1", "minAmount": 809, "maxAmount": 1,"currencyTitle": "Toman"},
      {"id":2,"title": "Service 2", "minAmount": 8090, "maxAmount": 4, "currencyTitle": "dollar" },
      {"id":3, "title": "Service 3", "minAmount": 8090, "maxAmount": 10,"currencyTitle": "euro" }
    ]
  }

  onSelectGateway(gateway: GatewayViewModel) {
    this.selectedGateWay = gateway;
  }

  onClickPay() {
    this.isClicked = true;

    if (this.selectedGateWay == null
      || this.amount < this.selectedGateWay.minAmount
      || this.amount > this.selectedGateWay.maxAmount
      || this.isSubmit) {
      return;
    } else {
      this.isSubmit = true;
    }

    // const paymentModel = new GetPaymentUrlRequestViewModel();
    // paymentModel.amount = Number(this.amount);
    // paymentModel.gatewayId = this.selectedGateWay.id;
    // paymentModel.domainName = location.origin;

    // this.transactionClient.getPaymentUrl(paymentModel).subscribe(value => {
    //   this.paymentUrl = value;
    //   window.location.href = value;
    // }, error => {
    //   this.toaster.showError2(error.response);
    //   this.isSubmit = false;
    // })
  }

}
