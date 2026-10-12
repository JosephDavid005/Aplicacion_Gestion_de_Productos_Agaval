import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'productos'
  },
  {
    path: 'productos',
    loadChildren: () =>
      import('./features/productos/productos.routes').then((m) => m.PRODUCTOS_ROUTES)
  },
  {
    path: 'categorias',
    loadChildren: () =>
      import('./features/categorias/categorias.routes').then((m) => m.CATEGORIAS_ROUTES)
  }
];
