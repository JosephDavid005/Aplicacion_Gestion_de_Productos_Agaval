CREATE DATABASE GestorInventarioDB;
GO
USE GestorInventarioDB;
GO

CREATE TABLE Categorias (
    Id            INT IDENTITY(1,1)   NOT NULL,
    Nombre        NVARCHAR(100)        NOT NULL,
    Activo        BIT                  NOT NULL DEFAULT (1),
    CONSTRAINT PK_Categorias PRIMARY KEY (Id)
);
GO

CREATE TABLE Productos (
    Id            INT IDENTITY(1,1)   NOT NULL,
    Nombre        NVARCHAR(150)        NOT NULL,
    Descripcion   NVARCHAR(500)        NULL,
    Precio        DECIMAL(10,2)        NOT NULL,
    Stock         INT                  NOT NULL DEFAULT (0),
    StockMinimo   INT                  NOT NULL DEFAULT (5),
    CategoriaId   INT                  NOT NULL,
    FechaCreacion DATETIME2            NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT PK_Productos PRIMARY KEY (Id),
    CONSTRAINT FK_Productos_Categorias FOREIGN KEY (CategoriaId)
        REFERENCES Categorias (Id),
    CONSTRAINT CK_Productos_Stock CHECK (Stock >= 0)
);
GO

CREATE TABLE MovimientosInventario (
    Id             INT IDENTITY(1,1)   NOT NULL,
    ProductoId     INT                  NOT NULL,
    TipoMovimiento VARCHAR(10)         NOT NULL,
    Cantidad       INT                  NOT NULL,
    Fecha          DATETIME2            NOT NULL DEFAULT (GETDATE()),
    Observacion    NVARCHAR(255)        NULL,
    CONSTRAINT PK_MovimientosInventario PRIMARY KEY (Id),
    CONSTRAINT FK_Movimientos_Productos FOREIGN KEY (ProductoId)
        REFERENCES Productos (Id) ON DELETE CASCADE,
    CONSTRAINT CK_Movimientos_Tipo CHECK (TipoMovimiento IN ('ENTRADA', 'SALIDA'))
);
GO

-- Datos iniciales (seed obligatorio)
INSERT INTO Categorias (Nombre) VALUES
    (N'Electrónica'), (N'Oficina'), (N'Aseo');
GO

-- Datos iniciales de productos para prueba
INSERT INTO Productos (Nombre, Descripcion, Precio, Stock, StockMinimo, CategoriaId) VALUES
    (N'Portátil Lenovo ThinkPad', N'Intel Core i5, 16GB RAM, 512GB SSD', 3200000.00, 12, 5, 1),
    (N'Mouse Inalámbrico Logitech', N'Sensor óptico 1000 DPI USB', 65000.00, 3, 5, 1),
    (N'Resma Papel Carta Reprograf', N'500 hojas 75g blancura 95%', 24500.00, 20, 10, 2),
    (N'Bolígrafo Kilométrico Negro', N'Caja x 12 unidades', 18000.00, 4, 8, 2),
    (N'Detergente Multiusos 1L', N'Biodegradable con aroma cítrico', 14500.00, 15, 6, 3),
    (N'Desinfectante en Spray', N'Elimina 99.9% bacterias 400ml', 19900.00, 2, 5, 3);
GO

-- Movimientos iniciales para demostración (Kardex)
INSERT INTO MovimientosInventario (ProductoId, TipoMovimiento, Cantidad, Observacion) VALUES
    (1, 'ENTRADA', 12, N'Carga de inventario inicial'),
    (2, 'ENTRADA', 10, N'Carga inicial de accesorios'),
    (2, 'SALIDA', 7, N'Venta mostrador lote 1');
GO
