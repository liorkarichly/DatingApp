import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';

// const routes: Routes = [
//   //Objects in array each one of the list

//   //The path is empty because the HomeComponent and we need to inside the component object
//   //Empty string, when somebody browses to localhost for 200 directly, then this is the component that's going to be loaded.
//   {path: '', component: HomeComponent},
//   {path: 'members', component: MemberListComponent},
//   {path: 'members/:id', component: MemberDetailComponent},
//   {path: 'lists', component: ListsComponent},
//   {path: 'messages', component: MessagesComponent},
//   {path: '**', component: HomeComponent, pathMatch: 'full'}//user's typed in something that doesn't match
//                                         //pathMatch: the router checks URL elements from the left to see if the URL matches a given path and stops when there is a config match. 
// ];


//The path is empty because the HomeComponent and we need to inside the component object
//Empty string, when somebody browses to localhost for 200 directly, then this is the component that's going to be loaded.
//[AuthGuard] - its prevet from user that not connect to route in our web
const routes: Routes = [
  {path: '', component: HomeComponent},
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children:[
      {path: 'members', component: MemberListComponent},
      {path: 'members/:username', component: MemberDetailComponent},
      {path: 'member/edit', component: MemberEditComponent, canDeactivate:[PreventUnsavedChangesGuard]},
      {path: 'lists', component: ListsComponent},
      {path: 'messages', component: MessagesComponent},
    ]
  },
  {path:'errors', component: TestErrorsComponent},
  {path:'not-found', component: NotFoundComponent},
  {path:'server-error', component:ServerErrorComponent},
  {path: '**', component: NotFoundComponent, pathMatch: 'full'},
  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
