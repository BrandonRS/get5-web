import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root',
   })
export class AuthService{

    private auth_url : string = "/api/auth/";

    constructor(private httpClient : HttpClient){}

    getUser(){
        
        this.httpClient.get(this.auth_url + "username", {responseType: 'text'}).subscribe(response => {
            
            console.log(response);
        });
    }

    getUserInfo(){

        this.httpClient.get(this.auth_url + "userInfo", {responseType: 'json'}).subscribe(response=> {
            
            console.log(response);
        });;
    }

    logout(){

        this.httpClient.get(this.auth_url +  "logout").subscribe(response => {});
    }

    isSignedIn(){

        //return this.httpClient.get(this.auth_url + "username").subscribe() != "";
    }
}