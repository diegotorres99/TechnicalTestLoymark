PRAGMA foreign_keys = ON;

ATTACH DATABASE 'users.db' AS users;

CREATE TABLE IF NOT EXISTS usuarios (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL CHECK(nombre GLOB '[a-zA-Z ]*'),
    apellido TEXT NOT NULL CHECK(apellido GLOB '[a-zA-Z ]*'),
    correo_electronico TEXT NOT NULL CHECK(correo_electronico LIKE '%@%.%'),
    fecha_nacimiento TEXT NOT NULL CHECK(fecha_nacimiento LIKE '____-__-__'),
    telefono TEXT CHECK(telefono GLOB '[0-9]*'),
    pais_residencia TEXT NOT NULL,
    pregunta_contacto BOOLEAN NOT NULL CHECK(pregunta_contacto IN (0, 1))
);

CREATE TABLE IF NOT EXISTS actividades (
    id_actividad INTEGER PRIMARY KEY AUTOINCREMENT,
    create_date DATETIME DEFAULT (datetime('now')) NOT NULL,
    id_usuario INTEGER NOT NULL,
    actividad TEXT NOT NULL,
);

-- Trigger to log activity on user insert
CREATE TRIGGER IF NOT EXISTS after_user_insert
AFTER INSERT ON usuarios
FOR EACH ROW
BEGIN
    INSERT INTO actividades (id_usuario, actividad) 
    VALUES (NEW.id, 'Creación de Usuario');
END;

-- Trigger to log activity on user update
CREATE TRIGGER IF NOT EXISTS after_user_update
AFTER UPDATE ON usuarios
FOR EACH ROW
BEGIN
    INSERT INTO actividades (id_usuario, actividad) 
    VALUES (NEW.id, 'Actualización de Usuario');
END;

-- Trigger to log activity on user delete
CREATE TRIGGER IF NOT EXISTS after_user_delete
AFTER DELETE ON usuarios
FOR EACH ROW
BEGIN
    INSERT INTO actividades (id_usuario, actividad) 
    VALUES (OLD.id, 'Eliminación de Usuario');
END;


