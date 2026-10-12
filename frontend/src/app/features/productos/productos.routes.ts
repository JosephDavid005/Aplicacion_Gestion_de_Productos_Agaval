import { Routes } from '@angular/router';

export const PRODUCTOS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./components/producto-list/producto-list.component').then(
        (m) => m.ProductoListComponent
      )
  },
  {
    path: 'nuevo',
    loadComponent: () =>
      import('./components/producto-form/producto-form.component').then(
        (m) => m.ProductoFormComponent
      )
  },
  {
    path: ':id/editar',
    loadComponent: () =>
      import('./components/producto-form/producto-form.component').then(
        (m) => m.ProductoFormComponent
      )
  }
];
