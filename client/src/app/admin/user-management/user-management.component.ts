import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { AdminService } from 'src/app/ClientServices/admin.service';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
users: Partial<User[]>;
bsModalRef: BsModalRef

  constructor(private adminService: AdminService
    , private modalService: BsModalService) { }

  ngOnInit(): void 
  {

    this.getUsersWithRoles();

  }

  getUsersWithRoles()
  {

    this.adminService.getUsersWithRoles()
    .subscribe(
    
      users => 
      {
        
        this.users = users;
    
      });

  }

  openRolesModal(user: User)
  {

    const config = {

      class: 'modal-dialog-centered',
      initialState: 
      {

        user,
        roles:this.getRoelsArray(user)

      }
    }
    
  this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    this.bsModalRef.content.updateSelectedRoles.subscribe(value =>
      {

        const rolesToUpdate = 
        {

          roles:[...value.filter(el => el.checked === true).map(element => element.name)]
       
        };

        if(rolesToUpdate)
        {

          this.adminService.updateUserroles(user.username, rolesToUpdate.roles).subscribe(
            () => {

              user.roles = [...rolesToUpdate.roles];
            });
            
        }

      });


  }

  private getRoelsArray(user)
  {

    const roles = [];

    const userRoles = user.roles;
    const availableroles: any[] = [
      {name: 'Admin', value: 'Admin'},
      {name: 'Moderator', value: 'Moderator'},
      {name: 'Member', value: 'Member'}
    ];

    availableroles.forEach(role =>
      {

        let isMatch = false;

        for(const userRole of userRoles)
        {

          if(role.name === userRole)
          {

            isMatch = true;
            role.checked = true;
            roles.push(role);
            break;

          }

        }

        if(!isMatch)
        {

          role.checked = false;
          roles.push(role);

        }

      });

      return roles;

  }

}
