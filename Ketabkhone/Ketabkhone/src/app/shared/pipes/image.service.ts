import { Injectable } from "@angular/core";
import { ActivatedRoute, NavigationEnd, Router, RouterStateSnapshot } from "@angular/router";
import { environment } from "src/environments/environment";


@Injectable({
  providedIn: "root",
})
export class ImageService {

  //private basePath2 = environment.apiEndpoint;

  private basePath = environment.apiEndpoint + "/api/";

  constructor(private router: Router, private route: ActivatedRoute) {
  }

  // getImagePath2(userId: number, imgName: string) {
  //   if (imgName !== "null" && imgName) {
  //     return this.basePath + userId;
  //   } else {
  //     return "assets/img/User-Defualt.png";
  //   }
  // }

  getImagePath(userId: number, imgName: string, url: string) {
    if (imgName !== "null" && imgName) {
      return this.basePath + url + '/GetThumbnailProfilePicture/' + userId;
    } else {
      return "assets/img/User-Defualt.png";
    }
  }
}
