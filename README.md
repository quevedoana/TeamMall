# TeamMall

Sistema web para la gestión interna de comunicaciones, publicaciones, usuarios, locales e inasistencias de una organización con vendedores distribuidos por distintos locales.

## Descripción

TeamMall centraliza información que necesitan administradores y vendedores en un único sistema web. La aplicación permite administrar usuarios y locales, publicar comunicaciones dirigidas a determinados locales, consultar notificaciones, cargar inasistencias y consultar información del perfil.

El proyecto está desarrollado con **ASP.NET Core MVC**, **C#**, **Entity Framework Core** y **MySQL**. También cuenta con una API protegida mediante **JWT** y una integración de solo lectura con **Google Drive** para consultar la fecha de modificación de una planilla.

## Roles

### Administrador

El administrador tiene acceso a las funciones de gestión del sistema, entre ellas:

- Gestión de usuarios.
- Gestión de locales.
- Alta, edición y eliminación de publicaciones.
- Selección de uno o varios locales destinatarios de una publicación.
- Consulta de accesos a publicaciones.
- Consulta y filtrado de inasistencias.
- Acceso a su perfil.

### Vendedor

El vendedor puede:

- Iniciar sesión con DNI y contraseña.
- Consultar publicaciones dirigidas a su local.
- Recibir y consultar notificaciones.
- Acceder al contenido y archivos adjuntos de las publicaciones.
- Cargar inasistencias y justificativos.
- Consultar sus propias inasistencias.
- Administrar la información de su perfil y avatar.

## Funcionalidades principales

### Autenticación y autorización

- Inicio de sesión mediante DNI y contraseña.
- Contraseñas almacenadas mediante `PasswordHasher<Usuario>` de ASP.NET Core.
- Autenticación web mediante cookies.
- Autorización por roles (`Administrador` y `Vendedor`).
- Sesiones con una duración configurada de 8 horas y expiración deslizante.
- API con autenticación mediante tokens JWT.

### Usuarios

Los administradores pueden gestionar usuarios desde el módulo de usuarios.

Los datos principales de un usuario son:

- DNI.
- Nombre.
- Apellido.
- Email.
- Contraseña.
- Rol.
- Local asociado.
- Estado activo/inactivo.
- Avatar.

### Locales

Los administradores pueden:

- Consultar locales.
- Crear locales.
- Editar locales.
- Realizar bajas lógicas mediante el campo `Activo`.
- Buscar locales.

La aplicación también dispone de endpoints API para consultar y administrar locales mediante JWT.

### Publicaciones

Los administradores pueden crear publicaciones seleccionando uno o varios locales destinatarios.

Cada publicación puede contener:

- Título.
- Descripción.
- Tipo.
- Fecha de publicación.
- Archivo adjunto.
- Locales destinatarios.

Los vendedores solamente visualizan las publicaciones destinadas al local al que están asociados.

Los archivos permitidos en publicaciones son:

- PDF.
- JPG/JPEG.
- PNG.
- GIF.
- WEBP.
- DOC/DOCX.
- XLS/XLSX.
- ZIP.
- RAR.

Las publicaciones se encuentran paginadas y se muestran hasta 10 registros por página.

Cuando una publicación contiene una imagen, la interfaz permite abrirla en un visor ampliado.

Los administradores también pueden consultar los accesos registrados a una publicación.

### Selector de locales

Al crear una publicación se pueden seleccionar múltiples locales mediante un selector desplegable.

El selector incluye:

- Selección individual.
- Opción para seleccionar todos los locales.
- Contador de locales seleccionados.
- Búsqueda escribiendo el nombre del local.
- Filtro instantáneo de los locales mientras se escribe.

### Notificaciones

Las publicaciones dirigidas a un local generan notificaciones para los vendedores activos asociados a ese local.

Las notificaciones permiten:

- Consultar el título y descripción de la publicación.
- Ver la fecha de creación.
- Marcar las notificaciones como leídas.
- Acceder directamente a la publicación.
- Registrar cuándo el vendedor accedió a la publicación.

La campanita de notificaciones está disponible para vendedores. El administrador no visualiza el panel/campanita de notificaciones en la interfaz actual.

La pantalla de notificaciones posee paginación de hasta 10 registros por página.

Una notificación deja de mostrarse en la campanita cuando el vendedor ya ingresó a la publicación correspondiente, conservando el registro en la base de datos para mantener la información de lectura y acceso.

### Inasistencias

Los vendedores pueden cargar sus inasistencias y justificativos.

Cada registro puede incluir:

- Tipo de inasistencia.
- Fecha de ausencia.
- Fecha de carga.
- Archivo adjunto.
- Comentario.
- Estado.

Los archivos permitidos son PDF e imágenes JPG/JPEG, PNG, GIF o WEBP.

Los administradores pueden consultar las inasistencias y filtrarlas por:

- Fecha desde.
- Fecha hasta.
- Tipo.

La consulta también posee paginación.

### Perfil

Los usuarios autenticados pueden consultar y modificar:

- Nombre.
- Apellido.
- Email.
- Avatar.

El avatar admite JPG, JPEG, PNG y WEBP y tiene un límite de 2 MB.

### Google Drive

El sistema incorpora un servicio `GoogleDriveService` que utiliza la API de Google Drive en modo de solo lectura.

Actualmente se utiliza para consultar la fecha de última modificación de una planilla configurada en Google Drive.

La integración requiere configurar:

- Ruta del archivo de credenciales de Google.
- ID de la planilla (`SpreadsheetId`).

Las credenciales no deben incluirse en el repositorio.

## Tecnologías

- **C#**
- **ASP.NET Core MVC**
- **.NET 10**
- **Entity Framework Core**
- **Pomelo.EntityFrameworkCore.MySql**
- **MySQL**
- **Bootstrap 5.3.3**
- **Vue 3**
- **JWT Bearer Authentication**
- **Cookie Authentication**
- **Google Drive API**
- **Razor / Razor Views**
- **JavaScript** para interacciones del lado del cliente

## Estructura del proyecto

```text
TeamMall/
├── Controllers/
│   ├── Api/
│   ├── HomeController.cs
│   ├── InasistenciasController.cs
│   ├── LocalesController.cs
│   ├── LoginController.cs
│   ├── NotificacionesController.cs
│   ├── PerfilController.cs
│   ├── PublicacionesController.cs
│   └── UsuariosController.cs
│
├── Data/
│   └── DataContext.cs
│
├── DTOs/
│   └── LocalDto.cs
│
├── Migrations/
│
├── Models/
│   ├── Inasistencia.cs
│   ├── Local.cs
│   ├── Notificacion.cs
│   ├── Publicacion.cs
│   ├── PublicacionLocal.cs
│   └── Usuario.cs
│
├── Services/
│   └── GoogleDriveService.cs
│
├── Views/
│   ├── Home/
│   ├── Inasistencias/
│   ├── Locales/
│   ├── Login/
│   ├── Notificaciones/
│   ├── Perfil/
│   ├── Publicaciones/
│   ├── Shared/
│   └── Usuarios/
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── Uploads/
│
├── Program.cs
├── TeaMall.csproj
├── appsettings.json
└── teamMall.sql
```

## Base de datos

El proyecto utiliza **MySQL** mediante Entity Framework Core.

El archivo `teamMall.sql` contiene un volcado de la base de datos utilizado por el proyecto.

Las principales entidades son:

- `usuario`
- `local`
- `publicacion`
- `publicacionlocal`
- `inasistencia`
- `notificacion`

Las relaciones principales permiten asociar usuarios con locales, publicaciones con sus locales destinatarios, inasistencias con usuarios y notificaciones con usuarios y publicaciones.

## Requisitos

Para ejecutar el proyecto se necesita:

- .NET SDK compatible con .NET 10.
- MySQL Server.
- Un entorno de desarrollo como Visual Studio o VS Code.
- Una base de datos MySQL para TeamMall.
- Credenciales de Google Drive si se desea utilizar la consulta de actualización de la planilla.

## Configuración

### 1. Clonar el repositorio

```bash
git clone <URL_DEL_REPOSITORIO>
cd TeamMall
```

### 2. Configurar MySQL

Crear una base de datos llamada `teammall` y configurar la cadena de conexión en `appsettings.json` o mediante configuración de entorno/User Secrets.

Ejemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=teammall;User=root;Password=TU_PASSWORD;"
  }
}
```

También se puede utilizar el archivo `teamMall.sql` para importar la estructura y los datos de ejemplo.

### 3. Configurar JWT

La aplicación utiliza JWT para los endpoints de la API. Se deben configurar los siguientes valores mediante configuración segura, por ejemplo User Secrets o variables de entorno:

```text
TokenAuthentication:SecretKey
TokenAuthentication:Issuer
TokenAuthentication:Audience
```

No se recomienda guardar claves secretas directamente en el repositorio.

### 4. Configurar Google Drive

El servicio espera una configuración similar a:

```json
"GoogleDrive": {
  "CredentialPath": "Credenciales/teammall-555bf325996c.json",
  "SpreadsheetId": "ID_DE_LA_PLANILLA"
}
```

El archivo de credenciales de Google debe existir en la ruta configurada y **no debe subirse al repositorio**.

## Ejecución

Desde la carpeta del proyecto:

```bash
dotnet restore
dotnet build
dotnet run
```

También se puede ejecutar desde Visual Studio o VS Code utilizando el perfil de lanzamiento configurado en `Properties/launchSettings.json`.

## Migraciones de Entity Framework Core

El proyecto incluye migraciones dentro de la carpeta `Migrations/`.

Para consultar las migraciones:

```bash
dotnet ef migrations list
```

Para aplicar las migraciones:

```bash
dotnet ef database update
```

## API

La aplicación dispone de endpoints API bajo `/api`.

### Autenticación

```http
POST /api/auth/login
```

Recibe DNI y contraseña y devuelve un token JWT cuando las credenciales son válidas.

### Locales

```http
GET    /api/localesapi
GET    /api/localesapi/{id}
POST   /api/localesapi
PUT    /api/localesapi/{id}
DELETE /api/localesapi/{id}
```

Las operaciones de modificación requieren autenticación JWT y rol de administrador.

### Perfil

```http
GET /api/perfil
```

Devuelve información del usuario autenticado a partir de los claims del token JWT.

## Seguridad

- Las contraseñas no se almacenan en texto plano.
- La aplicación utiliza autorización por roles.
- Las operaciones de formularios utilizan protección antiforgery cuando corresponde.
- La API utiliza JWT.
- Las credenciales de Google Drive deben mantenerse fuera del repositorio.
- Los archivos cargados se validan mediante extensiones permitidas según cada funcionalidad.

## Estado del proyecto

TeamMall se encuentra en desarrollo y contiene los módulos principales de gestión de usuarios, locales, publicaciones, notificaciones, inasistencias, perfil, autenticación y la integración con Google Drive definida actualmente en el proyecto.

## Autores

Proyecto académico desarrollado por el equipo de TeamMall.
