import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { ImagesPipe } from "./pipes/images.pipe";



const components = [
    ImagesPipe,

    
];

@NgModule({
    declarations: [components],
    imports: [CommonModule],
    exports: [components],
})
export class ShareModule { }
