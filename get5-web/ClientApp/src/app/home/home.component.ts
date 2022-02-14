import { Component } from '@angular/core';
import { AuthService } from '../services/auth.services';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  providers: [AuthService]
})

export class HomeComponent {

  constructor(private auth : AuthService){  }

}
