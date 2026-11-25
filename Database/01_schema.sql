/* =====================================================================
   BiteFlow - Esquema inicial (SQL Server / LocalDB)
   Ejecutar PRIMERO. Es re-ejecutable: no borra datos ni duplica objetos.
   Orden de ejecución:  01_schema.sql -> Sp/*.sql -> 02_seed.sql
   Fuente de verdad del esquema: si cambias una tabla, cambia tambien la
   configuracion EF en BiteFlow.Web/Data/Configurations/ (y viceversa).
   ===================================================================== */
IF DB_ID(N'BiteFlowDb') IS NULL
    CREATE DATABASE BiteFlowDb;
GO

USE BiteFlowDb;
GO

/* ---------- Rol ---------- */
IF OBJECT_ID(N'dbo.Rol', N'U') IS NULL
CREATE TABLE dbo.Rol
(
    IdRol        INT IDENTITY(1,1) NOT NULL,
    Nombre       NVARCHAR(50)      NOT NULL,
    Descripcion  NVARCHAR(150)     NULL,
    CONSTRAINT PK_Rol        PRIMARY KEY (IdRol),
    CONSTRAINT UQ_Rol_Nombre UNIQUE (Nombre)
);
GO

/* ---------- Usuario ---------- */
IF OBJECT_ID(N'dbo.Usuario', N'U') IS NULL
CREATE TABLE dbo.Usuario
(
    IdUsuario      INT IDENTITY(1,1) NOT NULL,
    Nombres        NVARCHAR(100)     NOT NULL,
    Apellidos      NVARCHAR(100)     NOT NULL,
    Correo         NVARCHAR(100)     NOT NULL,
    PasswordHash   NVARCHAR(200)     NOT NULL,
    IdRol          INT               NOT NULL,
    Activo         BIT               NOT NULL CONSTRAINT DF_Usuario_Activo DEFAULT (1),
    FechaRegistro  DATETIME2         NOT NULL CONSTRAINT DF_Usuario_FechaRegistro DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Usuario        PRIMARY KEY (IdUsuario),
    CONSTRAINT UQ_Usuario_Correo UNIQUE (Correo),
    CONSTRAINT FK_Usuario_Rol    FOREIGN KEY (IdRol) REFERENCES dbo.Rol (IdRol)
);
GO

/* ---------- Cliente ---------- */
IF OBJECT_ID(N'dbo.Cliente', N'U') IS NULL
CREATE TABLE dbo.Cliente
(
    IdCliente   INT IDENTITY(1,1) NOT NULL,
    Nombres     NVARCHAR(120)     NOT NULL,
    Apellidos   NVARCHAR(120)     NOT NULL,
    Dni         VARCHAR(8)        NOT NULL,
    Telefono    NVARCHAR(20)      NULL,
    Correo      NVARCHAR(120)     NULL,
    IdUsuario   INT               NULL,
    Activo      BIT               NOT NULL CONSTRAINT DF_Cliente_Activo DEFAULT (1),
    CONSTRAINT PK_Cliente         PRIMARY KEY (IdCliente),
    CONSTRAINT UQ_Cliente_Dni     UNIQUE (Dni),
    CONSTRAINT CK_Cliente_Dni     CHECK (LEN(Dni) = 8 AND Dni NOT LIKE '%[^0-9]%'),
    CONSTRAINT FK_Cliente_Usuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuario (IdUsuario)
);
GO

/* Un usuario solo puede estar vinculado a UN cliente; varios clientes pueden no tener usuario (NULL). */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Cliente_IdUsuario' AND object_id = OBJECT_ID(N'dbo.Cliente'))
    CREATE UNIQUE INDEX UX_Cliente_IdUsuario ON dbo.Cliente (IdUsuario) WHERE IdUsuario IS NOT NULL;
GO

/* ---------- Categoria ---------- */
IF OBJECT_ID(N'dbo.Categoria', N'U') IS NULL
CREATE TABLE dbo.Categoria
(
    IdCategoria  INT IDENTITY(1,1) NOT NULL,
    Nombre       NVARCHAR(100)     NOT NULL,
    Descripcion  NVARCHAR(250)     NULL,
    Activo       BIT               NOT NULL CONSTRAINT DF_Categoria_Activo DEFAULT (1),
    CONSTRAINT PK_Categoria        PRIMARY KEY (IdCategoria),
    CONSTRAINT UQ_Categoria_Nombre UNIQUE (Nombre)
);
GO

/* ---------- Producto ---------- */
IF OBJECT_ID(N'dbo.Producto', N'U') IS NULL
CREATE TABLE dbo.Producto
(
    IdProducto   INT IDENTITY(1,1) NOT NULL,
    IdCategoria  INT               NOT NULL,
    Nombre       NVARCHAR(150)     NOT NULL,
    Descripcion  NVARCHAR(300)     NULL,
    Precio       DECIMAL(10,2)     NOT NULL,
    Disponible   BIT               NOT NULL CONSTRAINT DF_Producto_Disponible DEFAULT (1),
    Activo       BIT               NOT NULL CONSTRAINT DF_Producto_Activo DEFAULT (1),
    CONSTRAINT PK_Producto           PRIMARY KEY (IdProducto),
    CONSTRAINT CK_Producto_Precio    CHECK (Precio >= 0),
    CONSTRAINT UQ_Producto_Cat_Nombre UNIQUE (IdCategoria, Nombre),
    CONSTRAINT FK_Producto_Categoria FOREIGN KEY (IdCategoria) REFERENCES dbo.Categoria (IdCategoria)
);
GO

/* ---------- Pedido ---------- */
IF OBJECT_ID(N'dbo.Pedido', N'U') IS NULL
CREATE TABLE dbo.Pedido
(
    IdPedido     INT IDENTITY(1,1) NOT NULL,
    IdCliente    INT               NOT NULL,
    FechaPedido  DATETIME2         NOT NULL CONSTRAINT DF_Pedido_Fecha DEFAULT (SYSDATETIME()),
    Estado       VARCHAR(20)       NOT NULL CONSTRAINT DF_Pedido_Estado DEFAULT ('Pendiente'),
    Total        DECIMAL(10,2)     NOT NULL,
    RowVersion   ROWVERSION        NOT NULL,
    CONSTRAINT PK_Pedido         PRIMARY KEY (IdPedido),
    CONSTRAINT CK_Pedido_Estado  CHECK (Estado IN ('Pendiente', 'Pagado', 'Anulado')),
    CONSTRAINT CK_Pedido_Total   CHECK (Total >= 0),
    CONSTRAINT FK_Pedido_Cliente FOREIGN KEY (IdCliente) REFERENCES dbo.Cliente (IdCliente)
);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Pedido_Estado_Fecha' AND object_id = OBJECT_ID(N'dbo.Pedido'))
    CREATE INDEX IX_Pedido_Estado_Fecha ON dbo.Pedido (Estado, FechaPedido);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Pedido_IdCliente' AND object_id = OBJECT_ID(N'dbo.Pedido'))
    CREATE INDEX IX_Pedido_IdCliente ON dbo.Pedido (IdCliente);
GO

/* ---------- DetallePedido ---------- */
IF OBJECT_ID(N'dbo.DetallePedido', N'U') IS NULL
CREATE TABLE dbo.DetallePedido
(
    IdDetalle       INT IDENTITY(1,1) NOT NULL,
    IdPedido        INT               NOT NULL,
    IdProducto      INT               NOT NULL,
    Cantidad        INT               NOT NULL,
    PrecioUnitario  DECIMAL(10,2)     NOT NULL,   -- precio HISTORICO (el de la fecha de compra)
    Subtotal        DECIMAL(10,2)     NOT NULL,
    CONSTRAINT PK_DetallePedido          PRIMARY KEY (IdDetalle),
    CONSTRAINT UQ_Detalle_Pedido_Prod    UNIQUE (IdPedido, IdProducto),
    CONSTRAINT CK_Detalle_Cantidad       CHECK (Cantidad > 0),
    CONSTRAINT CK_Detalle_Precio         CHECK (PrecioUnitario >= 0),
    CONSTRAINT CK_Detalle_Subtotal       CHECK (Subtotal = Cantidad * PrecioUnitario),
    CONSTRAINT FK_Detalle_Pedido         FOREIGN KEY (IdPedido)   REFERENCES dbo.Pedido (IdPedido),
    CONSTRAINT FK_Detalle_Producto       FOREIGN KEY (IdProducto) REFERENCES dbo.Producto (IdProducto)
);
GO

/* ---------- Pago ---------- */
IF OBJECT_ID(N'dbo.Pago', N'U') IS NULL
CREATE TABLE dbo.Pago
(
    IdPago             INT IDENTITY(1,1) NOT NULL,
    IdPedido           INT               NOT NULL,
    IdUsuarioCajero    INT               NOT NULL,
    FechaPago          DATETIME2         NOT NULL CONSTRAINT DF_Pago_Fecha DEFAULT (SYSDATETIME()),
    Monto              DECIMAL(10,2)     NOT NULL,
    MetodoPago         VARCHAR(20)       NOT NULL,
    Estado             VARCHAR(20)       NOT NULL CONSTRAINT DF_Pago_Estado DEFAULT ('Registrado'),
    NumeroComprobante  VARCHAR(20)       NOT NULL,
    CONSTRAINT PK_Pago               PRIMARY KEY (IdPago),
    CONSTRAINT UQ_Pago_IdPedido      UNIQUE (IdPedido),            -- evita pagos duplicados del mismo pedido
    CONSTRAINT UQ_Pago_Comprobante   UNIQUE (NumeroComprobante),
    CONSTRAINT CK_Pago_Monto         CHECK (Monto > 0),
    CONSTRAINT CK_Pago_Metodo        CHECK (MetodoPago IN ('Efectivo', 'Yape', 'Plin', 'Tarjeta')),
    CONSTRAINT CK_Pago_Estado        CHECK (Estado IN ('Registrado', 'Anulado')),
    CONSTRAINT FK_Pago_Pedido        FOREIGN KEY (IdPedido)        REFERENCES dbo.Pedido (IdPedido),
    CONSTRAINT FK_Pago_Cajero        FOREIGN KEY (IdUsuarioCajero) REFERENCES dbo.Usuario (IdUsuario)
);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Pago_FechaPago' AND object_id = OBJECT_ID(N'dbo.Pago'))
    CREATE INDEX IX_Pago_FechaPago ON dbo.Pago (FechaPago);
GO

/* ---------- Secuencia del comprobante interno ----------
   Nota: una secuencia puede dejar "huecos" si una transaccion hace rollback. Es aceptable
   para un comprobante interno sin validez tributaria. */
IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name = N'seq_numero_comprobante')
    CREATE SEQUENCE dbo.seq_numero_comprobante AS INT START WITH 1 INCREMENT BY 1;
GO

PRINT 'Esquema de BiteFlowDb listo.';
GO
