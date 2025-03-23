import { Routes } from '@angular/router';
import { BookListComponent } from './components/books/book-list/book-list.component';
import { BookDetailsComponent } from './components/books/book-details/book-details.component';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/books', pathMatch: 'full' },
  { path: 'books', component: BookListComponent, canActivate: [authGuard] },
  { path: 'books/:id', component: BookDetailsComponent, canActivate: [authGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: '**', redirectTo: '/books' }
];