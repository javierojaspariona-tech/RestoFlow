/*
  BiteFlow - Esquema inicial SQL Server
  Ejecutar en una instancia local con permisos para crear la BD.
  Precios de Producto se consideran IGV incluido (18/118 para extraer IGV).
*/
IF DB_ID(N'BiteFlowDb') IS NULL
    CREATE DATABASE BiteFlowDb;
GO
USE BiteFlowDb;
GO

CREATE TABLE dbo.Rol (
    IdRol INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Rol PRIMARY KEY,
    Nombre NVARCHAR(30) NOT NULL CONSTRAINT UQ_Rol_Nombre UNIQUE
);
GO

CREATE TABLE dbo.Usuario (
    IdUsuario INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuario PRIMARY KEY,
    IdRol INT NOT NULL,
    Nombres NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Correo NVARCHAR(254) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Activo BIT NOT NULL CONSTRAINT DF_Usuario_Activo DEFAULT(1),
    FechaCreacion DATETIME2(0) NOT NULL CONSTRAINT DF_Usuario_Fecha DEFAULT(SYSDATETIME()),
    CONSTRAINT FK_Usuario_Rol FOREIGN KEY (IdRol) REFERENCES dbo.Rol(IdRol),
    CONSTRAINT UQ_Usuario_Correo UNIQUE (Correo)
);
GO

CREATE TABLE dbo.Cliente (
    IdCliente INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Cliente PRIMARY KEY,
    IdUsuario INT NOT NULL,
    Telefono NVARCHAR(20) NULL,
    Direccion NVARCHAR(250) NULL,
    CONSTRAINT FK_Cliente_Usuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuario(IdUsuario),
    CONSTRAINT UQ_Cliente_Usuario UNIQUE (IdUsuario)
);
GO

CREATE TABLE dbo.Categoria (
    IdCategoria INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categoria PRIMARY KEY,
    Nombre NVARCHAR(80) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    Activo BIT NOT NULL CONSTRAINT DF_Categoria_Activo DEFAULT(1),
    CONSTRAINT UQ_Categoria_Nombre UNIQUE (Nombre)
);
GO

CREATE TABLE dbo.Producto (
    IdProducto INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Producto PRIMARY KEY,
    IdCategoria INT NOT NULL,
    Nombre NVARCHAR(120) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    Precio DECIMAL(10,2) NOT NULL,
    ImagenUrl NVARCHAR(500) NULL,
    Disponible BIT NOT NULL CONSTRAINT DF_Producto_Disponible DEFAULT(1),
    Activo BIT NOT NULL CONSTRAINT DF_Producto_Activo DEFAULT(1),
    FechaCreacion DATETIME2(0) NOT NULL CONSTRAINT DF_Producto_Fecha DEFAULT(SYSDATETIME()),
    CONSTRAINT FK_Producto_Categoria FOREIGN KEY (IdCategoria) REFERENCES dbo.Categoria(IdCategoria),
    CONSTRAINT CK_Producto_Precio CHECK (Precio >= 0)
);
GO
CREATE INDEX IX_Producto_Categoria_Activo_Disponible
ON dbo.Producto(IdCategoria, Activo, Disponible);
GO

CREATE TABLE dbo.Pedido (
    IdPedido BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Pedido PRIMARY KEY,
    IdCliente INT NOT NULL,
    Codigo NVARCHAR(24) NOT NULL,
    Modalidad NVARCHAR(20) NOT NULL,
    NumeroMesa TINYINT NULL,
    Estado NVARCHAR(15) NOT NULL CONSTRAINT DF_Pedido_Estado DEFAULT(N'Pendiente'),
    Subtotal DECIMAL(12,2) NOT NULL,
    Igv DECIMAL(12,2) NOT NULL,
    Total DECIMAL(12,2) NOT NULL,
    FechaPedido DATETIME2(0) NOT NULL CONSTRAINT DF_Pedido_Fecha DEFAULT(SYSDATETIME()),
    Observacion NVARCHAR(300) NULL,
    CONSTRAINT FK_Pedido_Cliente FOREIGN KEY (IdCliente) REFERENCES dbo.Cliente(IdCliente),
    CONSTRAINT UQ_Pedido_Codigo UNIQUE (Codigo),
    CONSTRAINT CK_Pedido_Modalidad CHECK (Modalidad IN (N'Mesa', N'ParaLlevar')),
    CONSTRAINT CK_Pedido_Mesa CHECK (
        (Modalidad = N'Mesa' AND NumeroMesa IS NOT NULL AND NumeroMesa BETWEEN 1 AND 99)
        OR (Modalidad = N'ParaLlevar' AND NumeroMesa IS NULL)
    ),
    CONSTRAINT CK_Pedido_Estado CHECK (Estado IN (N'Pendiente', N'Pagado', N'Anulado')),
    CONSTRAINT CK_Pedido_Montos CHECK (Subtotal >= 0 AND Igv >= 0 AND Total >= 0)
);
GO
CREATE INDEX IX_Pedido_Estado_Fecha ON dbo.Pedido(Estado, FechaPedido DESC);
CREATE INDEX IX_Pedido_Cliente_Fecha ON dbo.Pedido(IdCliente, FechaPedido DESC);
GO

CREATE TABLE dbo.DetallePedido (
    IdDetallePedido BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DetallePedido PRIMARY KEY,
    IdPedido BIGINT NOT NULL,
    IdProducto INT NOT NULL,
    NombreProducto NVARCHAR(120) NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Importe AS (CONVERT(DECIMAL(12,2), Cantidad * PrecioUnitario)) PERSISTED,
    CONSTRAINT FK_DetallePedido_Pedido FOREIGN KEY (IdPedido) REFERENCES dbo.Pedido(IdPedido),
    CONSTRAINT FK_DetallePedido_Producto FOREIGN KEY (IdProducto) REFERENCES dbo.Producto(IdProducto),
    CONSTRAINT CK_DetallePedido_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_DetallePedido_Precio CHECK (PrecioUnitario >= 0)
);
GO
CREATE INDEX IX_DetallePedido_Pedido ON dbo.DetallePedido(IdPedido);
GO

IF OBJECT_ID(N'dbo.SeqComprobante', N'SO') IS NULL
    EXEC(N'CREATE SEQUENCE dbo.SeqComprobante AS BIGINT START WITH 1 INCREMENT BY 1;');
GO

CREATE TABLE dbo.Pago (
    IdPago BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Pago PRIMARY KEY,
    IdPedido BIGINT NOT NULL,
    IdUsuarioCajero INT NOT NULL,
    MetodoPago NVARCHAR(20) NOT NULL,
    Monto DECIMAL(12,2) NOT NULL,
    Estado NVARCHAR(15) NOT NULL CONSTRAINT DF_Pago_Estado DEFAULT(N'Registrado'),
    NumeroComprobante NVARCHAR(24) NOT NULL,
    FechaPago DATETIME2(0) NOT NULL CONSTRAINT DF_Pago_Fecha DEFAULT(SYSDATETIME()),
    CONSTRAINT FK_Pago_Pedido FOREIGN KEY (IdPedido) REFERENCES dbo.Pedido(IdPedido),
    CONSTRAINT FK_Pago_Usuario FOREIGN KEY (IdUsuarioCajero) REFERENCES dbo.Usuario(IdUsuario),
    CONSTRAINT UQ_Pago_IdPedido UNIQUE (IdPedido),
    CONSTRAINT UQ_Pago_Comprobante UNIQUE (NumeroComprobante),
    CONSTRAINT CK_Pago_Metodo CHECK (MetodoPago IN (N'Efectivo', N'YapePlin', N'Tarjeta')),
    CONSTRAINT CK_Pago_Estado CHECK (Estado IN (N'Registrado', N'Anulado')),
    CONSTRAINT CK_Pago_Monto CHECK (Monto >= 0)
);
GO
