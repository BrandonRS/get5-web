import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { UserInfo } from "../models/user_info";

@Injectable({
    providedIn: 'root',
})

export class AuthService{

    private auth_url : string = "/api/auth/";

    constructor(private httpClient : HttpClient){}

    getUser(): Observable<string>{
        
        return this.httpClient.get(this.auth_url + "username", {responseType: 'text'});
    }

    getUserInfo() : Observable<UserInfo>{

        return this.httpClient.get<UserInfo>(this.auth_url + "userInfo");
    }
}
