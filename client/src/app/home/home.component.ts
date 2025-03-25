import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  registerMode = true;

  registerToggle() {
    this.registerMode = !this.registerMode;
  }
}
