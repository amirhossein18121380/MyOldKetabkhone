import { Component, OnInit } from '@angular/core';
import { AccountService } from 'src/app/shared/services/account.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(public accountService: AccountService) { }

  ngOnInit(): void {
  }

  data = [
    {
      name: 'Pictures',
      children: [
        {
          name: 'Vacation',
          children: [
            {
              name: 'Italy',
              children: [
                { name: 'Image 01' },
                { name: 'Image 02' },
                { name: 'Image 03' },
                { name: 'Image 04' },
              ],
            },
          ],
        },
        {
          name: 'Video 01',
        },
        {
          name: 'Video 02',
        },
        {
          name: 'Video 03',
        },
      ],
    },
    {
      name: 'Music',
      children: [
        { name: 'song01.mp3' },
        { name: 'song02.mp3' },
        { name: 'song03.mp3' },
        { name: 'song04.mp3' },
      ],
    },
  ];
  
  onCheck(e: any) {
    console.log('%c Returned checked object ', 'background: #222; color:  #ff8080');
    console.log(e);
    console.log('%c ************************************ ', 'background: #222; color: #bada05');
  }
  onCheckedKeys(e: any) {
    console.log('%c Returned array with checked checkboxes ', 'background: #222; color: #bada55');
    console.log(e);
    console.log('%c ************************************ ', 'background: #222; color: #bada05');
  }
  onNodesChanged(e: any) {
    console.log('%c Returned json with marked checkboxes ', 'background: #222; color: #99ccff');
    console.table(e);
    console.log('%c ************************************ ', 'background: #222; color: #bada05');
  }

}
