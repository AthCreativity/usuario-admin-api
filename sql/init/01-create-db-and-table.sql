IF DB_ID(N'UsuariosDb') IS NULL
BEGIN
    CREATE DATABASE UsuariosDb;
END;
GO

USE UsuariosDb;
GO

IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios
    (
        Id INT IDENTITY(1,1) NOT NULL,
        Nombre NVARCHAR(100) NOT NULL,
        Correo NVARCHAR(255) NOT NULL,
        Estatus NVARCHAR(20) NOT NULL,
        FechaCreacion DATETIME2 NOT NULL CONSTRAINT DF_Usuarios_FechaCreacion DEFAULT SYSUTCDATETIME(),
        FechaActualizacion DATETIME2 NULL,
        CONSTRAINT PK_Usuarios PRIMARY KEY (Id),
        CONSTRAINT CK_Usuarios_Estatus CHECK (Estatus IN ('activo', 'desactivado')),
        CONSTRAINT UQ_Usuarios_Correo UNIQUE (Correo)
    );
END;
GO
