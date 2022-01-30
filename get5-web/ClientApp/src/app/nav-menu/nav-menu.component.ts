import { Component, NgModule } from '@angular/core';
import {MatButtonModule} from '@angular/material/button';

const MaterialComponents = [
  MatButtonModule
];
 @NgModule({
   imports: [MaterialComponents],
   exports: [MaterialComponents]
 })
 export class MaterialModule { }

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;

  umdLogin() {

  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}