import { TestBed } from '@angular/core/testing';
import { provideRouter, RouterLink } from '@angular/router';
import { By } from '@angular/platform-browser';
import { App } from './app.component';
import { routes } from './app.routes';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideRouter(routes)]
    }).compileComponents();
  });

  it('debe crear la aplicación', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('debe renderizar el título de la marca Agaval', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Gestor de Inventario');
  });

  it('debe renderizar enlaces de navegación para Productos y Categorías', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    await fixture.whenStable();

    const links = fixture.debugElement.queryAll(By.directive(RouterLink));
    expect(links.length).toBeGreaterThanOrEqual(2);

    const hrefs = links.map(l => l.injector.get(RouterLink).href);
    expect(hrefs).toContain('/productos');
    expect(hrefs).toContain('/categorias');
  });
});
