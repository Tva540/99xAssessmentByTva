import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { Balances } from './features/balances/balances';
import { Upload } from './features/upload/upload';
import { Reports } from './features/reports/reports';
import { authGuard, adminGuard, viewerGuard, rootRedirect } from './core/guards/auth.guard';

const routes: Routes = [
  { path: '',         canActivate: [rootRedirect], children: [], pathMatch: 'full' },
  { path: 'login',    component: Login },
  { path: 'register', component: Register },
  { path: 'balances', component: Balances, canActivate: [authGuard, viewerGuard] },
  { path: 'upload',   component: Upload,   canActivate: [authGuard, adminGuard] },
  { path: 'reports',  component: Reports,  canActivate: [authGuard, adminGuard] },
  { path: '**',       redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
