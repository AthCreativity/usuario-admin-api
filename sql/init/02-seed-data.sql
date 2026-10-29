USE UsuariosDb;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Correo = 'ana@empresa.com')
BEGIN
    INSERT INTO dbo.Usuarios (Nombre, Correo, Estatus)
    VALUES ('Ana López', 'ana@empresa.com', 'activo');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Correo = 'carlos@empresa.com')
BEGIN
    INSERT INTO dbo.Usuarios (Nombre, Correo, Estatus)
    VALUES ('Carlos Ruiz', 'carlos@empresa.com', 'activo');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Correo = 'luisa@empresa.com')
BEGIN
    INSERT INTO dbo.Usuarios (Nombre, Correo, Estatus)
    VALUES ('Luisa García', 'luisa@empresa.com', 'desactivado');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Correo = 'mateo@empresa.com')
BEGIN
    INSERT INTO dbo.Usuarios (Nombre, Correo, Estatus)
    VALUES ('Mateo Silva', 'mateo@empresa.com', 'activo');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Correo = 'sofia@empresa.com')
BEGIN
    INSERT INTO dbo.Usuarios (Nombre, Correo, Estatus)
    VALUES ('Sofía Ortega', 'sofia@empresa.com', 'desactivado');
END;
GO
