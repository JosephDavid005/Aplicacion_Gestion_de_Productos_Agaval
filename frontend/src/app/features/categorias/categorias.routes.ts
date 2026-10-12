import { Routes } from '@angular/router';

export const CATEGORIAS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/categoria-list/categoria-list.component').then(
        m => m.CategoriaListComponent
      )
  }
];
