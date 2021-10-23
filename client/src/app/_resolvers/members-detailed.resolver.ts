import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { Observable } from "rxjs";
import { MemberService } from "../ClientServices/member.service";
import { Member } from "../_models/member";

//Because this is not a component, then we do need to add the injectable operator onto this.
@Injectable({
    providedIn: 'root'
})
export class MemberDetailedResolver implements Resolve<Member>
{
    
    constructor(private memberService: MemberService) {
        
        
    }
    resolve(route: ActivatedRouteSnapshot):  Observable<Member> {
        
        // we don't need to subscribe inside resolvers. 
        //The router is going to take care of this for us
        return this.memberService.getMember(route.paramMap.get('username'));
    }

    
}