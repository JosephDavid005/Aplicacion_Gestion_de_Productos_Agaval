export interface Categoria {
  id: number;
  nombre: string;
  activo: boolean;
}

export interface CreateCategoriaDto {
  nombre: string;
  activo?: boolean;
}

export interface UpdateCategoriaDto {
  nombre: string;
  activo: boolean;
}
