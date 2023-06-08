
import { Pipe, PipeTransform } from "@angular/core";
import { ImageService } from "./image.service";

@Pipe({
  name: "images",
})
export class ImagesPipe implements PipeTransform {
  
  constructor(private img: ImageService) {}

  transform(userId: number, imgName: string , url:string) {
    const path = this.img.getImagePath(userId, imgName , url);
    return path;
  }
}