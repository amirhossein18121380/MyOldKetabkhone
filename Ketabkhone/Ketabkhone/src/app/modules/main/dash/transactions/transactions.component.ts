import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AccountService } from 'src/app/shared/services/account.service';
import { GetTransactionReportFilterViewModel, GetTransactionReportViewModel, TransactionClient } from 'src/app/shared/services/api.service';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.css']
})
export class TransactionsComponent implements OnInit {
  
  error: string;
  isLoading: boolean;

  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  dataSource: MatTableDataSource<GetTransactionReportViewModel>;
  totalLength: number;
  displayedColumns: string[] = ['operationTitle', 'amount', 'trDate', 'time', 'statusTitle'];

  lastPageIndex: number;
  lastPageSize: number;
  currentPageIndex: number;
  currentPageSize: number;

  form: FormGroup;

  constructor(
    private toaster: ToastService,
    public accountService: AccountService,
    private transactionClient: TransactionClient) { }

  ngOnInit(): void {
    this.form = new FormGroup({
      fromDate: new FormControl(),
      toDate: new FormControl()
    });
  }

  ngOnDestroy(): void {
  }

  ngAfterViewInit() {
    this.paginator._intl.itemsPerPageLabel = "each in row";
    this.getTransactions(0, 0, this.paginator.pageSize);
  }

  getTransactions(currentSize: number, pageIndex: number, pageSize: number) {
    if (this.isLoading) {
      this.paginator.pageIndex = this.currentPageIndex;
      this.paginator.pageSize = this.currentPageSize;
      return;
    } else {
      this.isLoading = true;
      // save last page
      this.currentPageIndex = pageIndex;
      this.currentPageSize = pageSize;
    }

    const requestModel = new GetTransactionReportFilterViewModel()
    requestModel.pageNumber = pageIndex;
    requestModel.pageSize = pageSize;
    requestModel.fromDate = this.form.controls['fromDate'].value;
    requestModel.toDate = this.form.controls['toDate'].value;

    this.transactionClient.getTransactionReport(requestModel).subscribe(response => {
      // @ts-ignore
      response.data.forEach(item => item.time = item.trDate)
      this.isLoading = false;
      // update dataSource
      this.dataSource = new MatTableDataSource(response.data);
      setTimeout(() => this.dataSource.sort = this.sort)
      this.totalLength = response.totalRecords;
      // handle pagination
      this.paginator.pageIndex = pageIndex;
      this.paginator.pageSize = pageSize;
      this.lastPageIndex = pageIndex;
      this.lastPageSize = pageSize;
    }, error => {
      this.isLoading = false;
      // handle pagination
      this.paginator.pageIndex = this.lastPageIndex;
      this.paginator.pageSize = this.lastPageSize;
      // handle error
      if (this.dataSource == null) {
        this.error = error.response
      } else {
        this.toaster.showError2(error.response)
      }
    })
  }

  pageChanged(e: PageEvent) {
    this.getTransactions(e.previousPageIndex, e.pageIndex, e.pageSize);
  }

  onClickRetry() {
    this.getTransactions(0, 0, this.paginator.pageSize)
  }

}
