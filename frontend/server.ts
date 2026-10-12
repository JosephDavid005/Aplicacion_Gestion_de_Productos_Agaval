import { serve } from 'bun';
import { join } from 'path';
import { existsSync, statSync } from 'fs';

const PORT = 4200;
const DIST_DIR = join(import.meta.dir, 'dist', 'frontend', 'browser');

serve({
  port: PORT,
  hostname: '0.0.0.0',
  fetch(req) {
    const url = new URL(req.url);
    const pathname = url.pathname;
    const filePath = join(DIST_DIR, pathname);

    // Servir archivo estático si existe y es archivo
    if (pathname !== '/' && existsSync(filePath) && statSync(filePath).isFile()) {
      return new Response(Bun.file(filePath));
    }

    // SPA Fallback a index.html para rutas Angular
    const indexPath = join(DIST_DIR, 'index.html');
    if (existsSync(indexPath)) {
      return new Response(Bun.file(indexPath), {
        headers: { 'Content-Type': 'text/html' }
      });
    }

    return new Response('Frontend build no encontrado. Ejecuta `bun run build`.', { status: 404 });
  }
});

console.log(`🚀 Agaval Frontend servido en http://0.0.0.0:${PORT}`);
