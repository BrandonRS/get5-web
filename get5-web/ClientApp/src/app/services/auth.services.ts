import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { UserInfo } from "./user_info";

@Injectable({
    providedIn: 'root',
   })
export class AuthService{

    private auth_url : string = "/api/auth/";

    constructor(private httpClient : HttpClient){}

    getUser(){
        
        this.httpClient.get(this.auth_url + "username", {responseType: 'text'}).subscribe(response => {

          //response is already string
          return response;
        });
    }

    getUserInfo(){

        this.httpClient.get(this.auth_url + "userInfo", {responseType: 'json'}).subscribe(response=> {


          let info: UserInfo = { username: Object.values(Object.values(response)[5]).toString().split(",").join("") } as UserInfo;

          console.log(info);

        });;
    }
}
