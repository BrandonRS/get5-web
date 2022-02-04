import { HttpClient } from "@angular/common/http";

export class AuthService{

    constructor(private httpClient : HttpClient){}

    getUser(){
        
        return this.httpClient.get("username");
    }

    getUserInfo(){

        return this.httpClient.get("userInfo");
    }

    logout(){

        this.httpClient.get("logout");
    }

    isSignedIn(){

        return this.httpClient.get("username") != "";
    }
}