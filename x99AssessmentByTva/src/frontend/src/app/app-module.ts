import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';

import { Navbar } from './shared/navbar/navbar';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { Balances } from './features/balances/balances';
import { Upload } from './features/upload/upload';
import { Reports } from './features/reports/reports';

import { authInterceptor } from './core/interceptors/auth.interceptor';

@NgModule({
  declarations: [
    App,
    Navbar,
    Login,
    Register,
    Balances,
    Upload,
    Reports
  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    FormsModule,
    AppRoutingModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptors([authInterceptor]))
  ],
  bootstrap: [App]
})
export class AppModule { }
