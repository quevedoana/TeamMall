# TeamMall

TeamMall es un sistema web MVC orientado a la gestión y comunicación interna de una organización con vendedores distribuidos en distintos locales.

El sistema permite administrar usuarios y locales, publicar comunicaciones dirigidas a determinados locales, generar notificaciones para los vendedores, gestionar inasistencias y justificativos, administrar perfiles y avatares, consultar horarios mediante una planilla de Google Sheets e integrar una Web API protegida con JWT.

## Tecnologías utilizadas

- C#
- ASP.NET Core MVC
- .NET 10
- Entity Framework Core
- MySQL
- Pomelo.EntityFrameworkCore.MySql
- Razor Views
- Vue 3
- JavaScript / AJAX
- Cookie Authentication
- JWT Bearer Authentication
- Google Drive API
- Bootstrap

## Funcionalidades principales

### Usuarios y roles

El sistema maneja los roles `Administrador` y `Vendedor`.

El administrador puede gestionar usuarios y locales, crear publicaciones, consultar los accesos a publicaciones y consultar las inasistencias.

El vendedor puede consultar las publicaciones dirigidas a su local, recibir notificaciones, cargar inasistencias y administrar su perfil.

La autorización se realiza en el backend mediante `[Authorize]` y restricciones por rol. Por ejemplo, `LocalesController` y `UsuariosController` están restringidos al rol `Administrador`.

### Autenticación

El acceso web se realiza mediante DNI y contraseña.

La aplicación utiliza `PasswordHasher<Usuario>` para verificar las contraseñas y autenticación mediante cookies. La cookie de autenticación tiene una duración configurada de 8 horas y expiración deslizante.

El inicio y cierre de sesión se encuentran en:

- `Controllers/LoginController.cs`

La configuración de autenticación web y JWT se encuentra en:

- `Program.cs`

### Perfil y avatar

Los usuarios autenticados pueden modificar su nombre, apellido, email y avatar desde el módulo de perfil.

El avatar:

- admite `.jpg`, `.jpeg`, `.png` y `.webp`;
- tiene un límite de 2 MB;
- se almacena en `wwwroot/Uploads/avatares`;
- utiliza un nombre generado con `Guid`.

La gestión se encuentra en:

- `Controllers/PerfilController.cs`
- `Views/Perfil/Index.cshtml`

### Locales

Los administradores pueden:

- listar locales;
- buscar locales;
- crear locales;
- editar locales;
- activar/desactivar locales mediante baja lógica.

El ABM de locales utiliza Vue 3 y AJAX/JSON. Las operaciones de listado, alta, modificación y baja lógica se realizan mediante peticiones `fetch` y respuestas JSON.

Archivos principales:

- `Controllers/LocalesController.cs`
- `Views/Locales/Index.cshtml`
- `Views/Locales/Create.cshtml`
- `Views/Locales/Edit.cshtml`

### Usuarios

Los administradores pueden consultar y gestionar usuarios.

El listado utiliza Vue 3, búsqueda y paginación del lado servidor. El cambio de estado activo/inactivo se realiza mediante AJAX.

Archivos principales:

- `Controllers/UsuariosController.cs`
- `Views/Usuarios/Index.cshtml`
- `Views/Usuarios/Create.cshtml`
- `Views/Usuarios/Edit.cshtml`

### Publicaciones

Los administradores pueden crear, modificar y eliminar publicaciones.

Una publicación contiene:

- título;
- descripción;
- tipo;
- fecha de publicación;
- archivo opcional;
- usuario creador;
- uno o varios locales destinatarios.

Los vendedores visualizan las publicaciones correspondientes al local que tienen asociado.

Archivos principales:

- `Controllers/PublicacionesController.cs`
- `Views/Publicaciones/Index.cshtml`
- `Views/Publicaciones/Create.cshtml`
- `Views/Publicaciones/Edit.cshtml`
- `Views/Publicaciones/Delete.cshtml`

### Relación entre publicaciones y locales

Una publicación puede dirigirse a varios locales y un local puede recibir varias publicaciones.

Esta relación N:M se resuelve mediante la entidad intermedia:

- `Models/PublicacionLocal.cs`

La clave primaria está formada por:

- `IdPublicacion`
- `IdLocal`

La configuración de esta relación se encuentra en:

- `Data/DataContext.cs`

### Notificaciones

Cuando un administrador crea una publicación y selecciona locales destinatarios, el sistema busca los vendedores activos asociados a esos locales y genera una notificación para cada uno.

Las notificaciones registran:

- usuario destinatario;
- publicación;
- fecha de creación;
- estado de lectura;
- fecha de lectura;
- fecha de acceso a la publicación.

El listado de la campanita utiliza Vue y AJAX mediante:

- `wwwroot/js/site.js`
- `Controllers/NotificacionesController.cs`

La pantalla completa de notificaciones también posee paginación.

### Inasistencias

Los vendedores pueden registrar inasistencias y adjuntar justificativos.

Cada registro contiene:

- tipo;
- fecha de ausencia;
- fecha de carga;
- archivo;
- comentario;
- estado;
- usuario asociado.

Los administradores pueden consultar y filtrar las inasistencias por fecha y tipo.

La consulta posee paginación del lado servidor.

Archivos principales:

- `Controllers/InasistenciasController.cs`
- `Views/Inasistencias/Index.cshtml`
- `Views/Inasistencias/Create.cshtml`

## Manejo de archivos

El sistema procesa archivos enviados desde formularios y los almacena físicamente en el servidor.

### Archivos de publicaciones

Los archivos adjuntos de publicaciones se almacenan en:

```text
wwwroot/Uploads/Publicaciones/
```

Se permiten extensiones como:

- PDF
- JPG/JPEG
- PNG
- GIF
- WEBP
- DOC/DOCX
- XLS/XLSX
- ZIP
- RAR

El servidor valida la extensión y genera un nombre único mediante `Guid`.

La implementación se encuentra en:

- `Controllers/PublicacionesController.cs`

### Archivos de inasistencias

Los justificativos se almacenan en:

```text
wwwroot/Uploads/Inasistencias/
```

Se permiten:

- PDF
- JPG/JPEG
- PNG
- GIF
- WEBP

La implementación se encuentra en:

- `Controllers/InasistenciasController.cs`

El manejo de archivos es independiente del almacenamiento de avatares.

## Vue.js y AJAX

El proyecto utiliza Vue 3 para distintas interfaces dinámicas.

El ABM de locales utiliza Vue para:

- listar registros;
- crear registros;
- editar registros;
- realizar la baja lógica;
- actualizar la información sin recargar la página.

Las peticiones se realizan mediante `fetch` y los controladores devuelven JSON cuando corresponde.

Ejemplo de la implementación:

```javascript
const respuesta = await fetch(...);
const resultado = await respuesta.json();
```

La evidencia del ABM se encuentra en:

- `Docs/abm-Ajax.mp4`

## Búsqueda asíncrona de entidades relacionadas

Al crear una publicación, los locales destinatarios se buscan de forma asíncrona desde el servidor.

La vista realiza una petición a:

```text
/Publicaciones/BuscarLocales
```

y recibe los resultados en JSON.

La implementación se encuentra en:

- `Views/Publicaciones/Create.cshtml`
- `Controllers/PublicacionesController.cs`

También existe búsqueda y paginación asíncrona en el listado de locales mediante:

- `Views/Locales/Index.cshtml`
- `Controllers/LocalesController.cs`

La evidencia correspondiente se encuentra en:

- `Docs/manejo-archivos-buscador.mp4`

## Paginación del lado servidor

Los listados principales utilizan paginación desde el backend.

La consulta recibe la página solicitada y utiliza `Skip` y `Take` para obtener solamente los registros correspondientes.

Ejemplo:

```csharp
.Skip((pagina - 1) * tamaño)
.Take(tamaño)
```

La paginación está implementada en los módulos de:

- usuarios;
- locales;
- publicaciones;
- notificaciones;
- inasistencias.

En publicaciones y notificaciones se utilizan hasta 10 registros por página. En usuarios y locales el tamaño de página se maneja mediante el parámetro recibido por el controlador.

La evidencia de paginación se encuentra en:

- `Docs/paginado.PNG`

## Web API REST y JWT

El proyecto dispone de una Web API bajo `/api`.

### Login de la API

```http
POST /api/auth/login
```

El endpoint recibe:

```json
{
  "DNI": "99999999",
  "Contrasenia": "..."
}
```

Cuando las credenciales son válidas, se genera un JWT firmado mediante HMAC-SHA256.

El token incluye claims relacionados con:

- identificador del usuario;
- nombre;
- rol;
- DNI;
- nombre completo.

La implementación se encuentra en:

- `Controllers/Api/AuthController.cs`

### API de locales

```http
GET    /api/localesapi
GET    /api/localesapi/{id}
POST   /api/localesapi
PUT    /api/localesapi/{id}
DELETE /api/localesapi/{id}
```

Los endpoints protegidos requieren autenticación JWT y las operaciones de modificación requieren el rol `Administrador`.

La implementación se encuentra en:

- `Controllers/Api/LocalesApiController.cs`

### API de perfil

```http
GET /api/perfil
```

Devuelve información del usuario autenticado utilizando los claims contenidos en el JWT.

La implementación se encuentra en:

- `Controllers/Api/PerfilController.cs`

La evidencia de autenticación mediante Bearer Token se encuentra en:

- `Docs/postman.PNG`

El repositorio contiene archivos de configuración de Postman en:

- `.postman/resources.yaml`
- `postman/globals/workspace.globals.yaml`

> Para una entrega que requiera la colección de pruebas exportada como condición de promoción, debe incorporarse al repositorio el archivo de colección correspondiente. En el contenido analizado no se encontró un archivo de colección exportada independiente.

## Google Sheets y horarios

Los horarios de los locales se encuentran en una planilla de Google Sheets.

La página de inicio integra la planilla mediante un `iframe` y también ofrece un enlace para abrirla directamente en Google Sheets.

La integración con Google Drive se realiza mediante:

- `Services/GoogleDriveService.cs`

El servicio utiliza la API de Google Drive con alcance de solo lectura y consulta la fecha de última modificación de la planilla.

La configuración utiliza:

```json
"GoogleDrive": {
  "CredentialPath": "Credenciales/teammall-555bf325996c.json",
  "SpreadsheetId": "1fyf_E3BKftPzF9A9V9rcc4gAc_18rsAkIm"
}
```

El controlador expone:

```text
GET /Home/ObtenerFechaActualizacion
```

para obtener la fecha y devolverla en JSON.

La vista de inicio correspondiente es:

- `Views/Home/Index.cshtml`

El archivo de credenciales de Google se encuentra excluido del repositorio mediante `.gitignore`.

## Modelo de datos

El modelo principal está compuesto por seis entidades:

- `Usuario`
- `Local`
- `Publicacion`
- `PublicacionLocal`
- `Inasistencia`
- `Notificacion`

Las tablas principales de MySQL son:

```text
usuario
local
publicacion
publicacionlocal
inasistencia
notificacion
```

Entre las relaciones implementadas se encuentran:

- `Local -> Usuario`: un local puede estar asociado a varios usuarios.
- `Usuario -> Inasistencia`: un usuario puede tener varias inasistencias.
- `Usuario -> Publicacion`: un usuario puede crear varias publicaciones.
- `Usuario -> Notificacion`: un usuario puede recibir varias notificaciones.
- `Publicacion -> Notificacion`: una publicación puede generar varias notificaciones.
- `Publicacion <-> Local`: relación N:M mediante `PublicacionLocal`.

La configuración de las relaciones se encuentra en:

- `Data/DataContext.cs`

El diagrama de entidad-relación se encuentra en:

- `Docs/ER-TEAMMALL.png`

## Estructura del proyecto

```text
TeamMall/
├── Controllers/
│   ├── Api/
│   │   ├── AuthController.cs
│   │   ├── LocalesApiController.cs
│   │   └── PerfilController.cs
│   ├── HomeController.cs
│   ├── InasistenciasController.cs
│   ├── LocalesController.cs
│   ├── LoginController.cs
│   ├── NotificacionesController.cs
│   ├── PerfilController.cs
│   ├── PublicacionesController.cs
│   └── UsuariosController.cs
├── Data/
│   └── DataContext.cs
├── DTOs/
│   └── LocalDto.cs
├── Docs/
│   ├── Avatar.PNG
│   ├── ER-TEAMMALL.png
│   ├── abm-Ajax.mp4
│   ├── manejo-archivos-buscador.mp4
│   ├── paginado.PNG
│   └── postman.PNG
├── Migrations/
├── Models/
├── Services/
│   └── GoogleDriveService.cs
├── Views/
├── wwwroot/
├── Program.cs
├── TeaMall.csproj
├── appsettings.json
├── teamMall.sql
└── README.md
```

## Base de datos

El proyecto utiliza MySQL mediante Entity Framework Core.

El repositorio incluye:

- `teamMall.sql`
- migraciones de Entity Framework Core en `Migrations/`

El archivo SQL contiene estructura y datos de ejemplo de las tablas principales.

## Matriz de Evidencias de Requerimientos Mínimos

| Requerimiento mínimo | Estado | Enlace al código / controladores | Evidencia |
|---|---|---|---|
| **1. Modelo de datos y persistencia: 6 entidades relacionadas, 1:N y N:M** | ✅ Cumplido | [`DataContext.cs`](Data/DataContext.cs), [`Models`](Models/) | [`ER-TEAMMALL.png`](Docs/ER-TEAMMALL.png) |
| **2. Login, Logout, roles, seguridad backend y avatar** | ✅ Cumplido | [`LoginController.cs`](Controllers/LoginController.cs), [`Program.cs`](Program.cs), [`PerfilController.cs`](Controllers/PerfilController.cs), [`LocalesController.cs`](Controllers/LocalesController.cs), [`UsuariosController.cs`](Controllers/UsuariosController.cs) | [`Avatar.PNG`](Docs/Avatar.PNG) |
| **3. Manejo de archivos adicional al avatar** | ✅ Cumplido | [`PublicacionesController.cs`](Controllers/PublicacionesController.cs), [`InasistenciasController.cs`](Controllers/InasistenciasController.cs) | [`manejo-archivos-buscador.mp4`](Docs/manejo-archivos-buscador.mp4) |
| **4. ABM/CRUD con Vue.js + AJAX/JSON** | ✅ Cumplido | [`LocalesController.cs`](Controllers/LocalesController.cs), [`Views/Locales/Index.cshtml`](Views/Locales/Index.cshtml), [`Views/Locales/Create.cshtml`](Views/Locales/Create.cshtml), [`Views/Locales/Edit.cshtml`](Views/Locales/Edit.cshtml) | [`abm-Ajax.mp4`](Docs/abm-Ajax.mp4) |
| **5. Paginado por servidor** | ✅ Cumplido | [`UsuariosController.cs`](Controllers/UsuariosController.cs), [`LocalesController.cs`](Controllers/LocalesController.cs), [`PublicacionesController.cs`](Controllers/PublicacionesController.cs), [`NotificacionesController.cs`](Controllers/NotificacionesController.cs), [`InasistenciasController.cs`](Controllers/InasistenciasController.cs) | [`paginado.PNG`](Docs/paginado.PNG) |
| **6. Búsqueda asíncrona de entidades relacionadas** | ✅ Cumplido | [`PublicacionesController.cs`](Controllers/PublicacionesController.cs), [`Views/Publicaciones/Create.cshtml`](Views/Publicaciones/Create.cshtml) | [`manejo-archivos-buscador.mp4`](Docs/manejo-archivos-buscador.mp4) |
| **7. Web API REST con JWT** | ⚠️ Implementado; falta agregar una colección exportada independiente si la entrega exige ese archivo | [`AuthController.cs`](Controllers/Api/AuthController.cs), [`LocalesApiController.cs`](Controllers/Api/LocalesApiController.cs), [`PerfilController.cs`](Controllers/Api/PerfilController.cs) | [`postman.PNG`](Docs/postman.PNG) |

## Evidencias

Todas las evidencias utilizadas para la entrega se encuentran en:

```text
Docs/
```

### Diagrama del modelo

[`ER-TEAMMALL.png`](Docs/ER-TEAMMALL.png)

### Avatar

[`Avatar.PNG`](Docs/Avatar.PNG)

### ABM con AJAX

[`abm-Ajax.mp4`](Docs/abm-Ajax.mp4)

### Manejo de archivos y búsqueda

[`manejo-archivos-buscador.mp4`](Docs/manejo-archivos-buscador.mp4)

### Paginación

[`paginado.PNG`](Docs/paginado.PNG)

### API / Postman

[`postman.PNG`](Docs/postman.PNG)

## Configuración

La cadena de conexión se configura mediante:

```text
ConnectionStrings:DefaultConnection
```

El archivo `appsettings.json` del repositorio no contiene una cadena de conexión de base de datos completa.

Para JWT se utilizan las siguientes claves de configuración:

```text
TokenAuthentication:SecretKey
TokenAuthentication:Issuer
TokenAuthentication:Audience
```

Estas claves deben configurarse de forma segura.

Para Google Drive se utilizan:

```text
GoogleDrive:CredentialPath
GoogleDrive:SpreadsheetId
```

El archivo de credenciales de Google está excluido del repositorio mediante:

```text
Credenciales/*
```

## Ejecución local

Restaurar las dependencias:

```bash
dotnet restore
```

Compilar:

```bash
dotnet build
```

Ejecutar:

```bash
dotnet run
```

También puede ejecutarse desde Visual Studio o Visual Studio Code utilizando el perfil configurado en `Properties/launchSettings.json`.

## Base de datos de prueba

El repositorio incluye:

```text
teamMall.sql
```

que contiene la estructura y datos de ejemplo de la base de datos `teammall`.

### Credenciales de prueba

El archivo SQL incluido en el proyecto contiene usuarios de ejemplo, pero las contraseñas almacenadas en ese dump aparecen como valores de reemplazo (`CAMBIAR_HASH_ADMIN` y `CAMBIAR_HASH_VENDEDORA`). Por ese motivo, no se indican contraseñas de prueba inventadas en este README.

Para una entrega destinada a promoción, deben incorporarse credenciales de prueba válidas para cada rol, tal como solicita el enunciado.

## .gitignore

El proyecto incluye un `.gitignore` para excluir archivos temporales, resultados de compilación, configuraciones locales y credenciales privadas de Google Drive.

Entre las exclusiones específicas del proyecto se encuentra:

```text
Credenciales/*
```

Esto evita subir las credenciales privadas de Google Drive al repositorio.

## Estado de los requerimientos

El proyecto contiene las implementaciones correspondientes a los requerimientos técnicos principales y las evidencias asociadas en la carpeta `Docs`.

Para una entrega de promoción, además de mantener el proyecto funcional, se deben verificar los elementos administrativos exigidos por el enunciado: credenciales de usuarios de prueba para cada rol y, cuando corresponda, la colección exportada de Postman/Bruno/Insomnia para probar la Web API con JWT.
