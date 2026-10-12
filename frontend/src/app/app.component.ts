import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatIconModule],
  styleUrl: './app.component.scss',
  templateUrl: './app.component.html',
})
export class App {
  protected readonly title = signal('Gestor de Inventario Agaval');
}
