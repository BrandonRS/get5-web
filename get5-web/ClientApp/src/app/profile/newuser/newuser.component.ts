import { Component } from '@angular/core';

@Component({
  selector: 'app-profile-newuser',
  templateUrl: './newuser.component.html',
  styleUrls: ['./newuser.component.css']
})
export class ProfileNewuserComponent {

  umdRedirect() {
    window.location.href = "/api/auth/umd";
  }

  guestRedirect(){
    window.location.href = "/";
  }
}