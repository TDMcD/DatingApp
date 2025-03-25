import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { User } from '../models/User';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private httpClient = inject(HttpClient);
  baseUrl = 'https://localhost:5001/api/';

  currentUser = signal<User | null>(null);

  login(model: any) {
    return this.httpClient
      .post<User>(this.baseUrl + 'account/login', model)
      .pipe(
        map((user) => {
          if (user) {
            localStorage.setItem('user', JSON.stringify(user));
            this.currentUser.set(user);
          }
        })
      );
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }
}
