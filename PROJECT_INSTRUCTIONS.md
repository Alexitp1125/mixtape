# Mixtape — instrucciones del proyecto

Sigue `dev-conventions`. Aquí solo va lo propio de esta app.

## Resumen

- **Dónde estoy:** en producción en https://mixtape.alexitp1125.com está la v1.1 más los commits
  1 y 2 de la v2 (el catálogo ya se lee de PostgreSQL; se ve igual que antes): 5 canciones de
  Jósean Log con portada propia, instalable como app, ⏮ ⏭ con paso automático, aviso de carga,
  paleta v3, tema claro, EN/ES. Sin letras. Repo en GitHub (`Alexitp1125/mixtape`).
- **Lo último (2026-11-13):** commit 2 desplegado (primer despliegue con base de datos, bien).
  Commit 3 escrito y en el PC, sin probar: segundo artista (Moneda Dura, 4 canciones), inicio con
  una carátula por artista y una página por artista con sus canciones.
- **Qué sigue:**
  1. Commit 3. Alexi crea la migración
     (`dotnet ef migrations add AddArtistCover --project Mixtape.Data --startup-project Mixtape`)
     y prueba en local: inicio con 2 carátulas, cada artista con sus canciones, el primer
     arranque dice `Catalog synced: 2 artists, 9 tracks, 12 changes`, y los 4 videos de Moneda
     Dura se dejan reproducir (no se pudo comprobar desde el chat). Luego commit
     (`feat: artists with their own cover and page`), push y despliegue con copia antes.
  2. Paso 2 de la v2: usuarios (nombre + PIN) y sus grupos de canciones (ver la hoja de ruta).
     Ahí el `Dockerfile` tiene que crear `/home/app/.aspnet/DataProtection-Keys` con dueño
     `$APP_UID` antes de `USER`, y hay que avisar al chat de estructura para que añada el volumen.
  3. Paso 3: página `/about` (`CONVENTIONS.md` §13).
  4. Chat del hub: tarjeta de Mixtape en *Live* con su enlace y `<install installurl>`, si no
     está hecho.

## Qué es

Un reproductor de música con una experiencia visual mejor que ir directo a YouTube: las
canciones favoritas de Alexi, cada una con su página y su portada, para escucharlas en su PC
y para recomendarlas con un enlace ("mírala aquí").

- El audio va **siempre** por el reproductor oficial de YouTube, y el reproductor se ve
  mientras suena (regla de YouTube).
- **Sin letras en la v1** (decisión del 2026-10-20) para no tener problemas de derechos.

| | |
|---|---|
| Nombre | Mixtape |
| Subdominio | `mixtape.alexitp1125.com` |
| Repo y carpeta | `mixtape` (`C:\dev\mixtape`) |
| Prefijo CSS | `mx-` |
| Puerto del contenedor | 8080 |
| En local | http://localhost:5080 |

## Hoja de ruta

- **v1 (en producción):** el catálogo de Alexi, sin letras, con buena experiencia en móvil, escritorio
  y pantalla grande.
- **v1.1 (en producción):** instalable como app, ⏮ ⏭ con paso automático a la siguiente, aviso de
  carga y una quinta canción.
- **v2 (acordada el 2026-11-12), en tres pasos:**
  1. **PostgreSQL, segundo artista y agrupación.** Entran PostgreSQL y `.Shared`,
     `.Application`, `.Data` y `.Tests` (§1.2). Tablas: artistas, canciones y portadas (los
     usuarios llegan en el paso 2). El catálogo de Alexi se escribe en `catalog.json` y se vuelca
     en la base de datos al arrancar; las páginas leen de la base de datos.
     El inicio muestra una carátula por artista y cada artista tiene su página con sus
     canciones; ⏮ ⏭ se quedan dentro del artista.
  2. **Usuarios y sus canciones.** Sin contraseña y sin guardar datos de nadie: el usuario crea
     un nombre y un PIN para volver a su lista. Ni correo ni nada más. Se perfecciona luego.
     - Los grupos son de cada usuario (acordado el 2026-11-13). Al añadir una canción, el
       usuario crea un "artista", que sirve de grupo, o elige uno suyo. Un artista funciona casi
       como una lista de reproducción, así que no hace falta una tabla de listas: `Artists` gana
       un dueño. Los de Alexi van sin dueño: son ejemplos y su forma de compartir las suyas con
       todos. El grupo de un usuario lleva una carátula genérica.
     - El PIN se guarda cifrado (hash), nunca tal cual, y con límite de intentos: un PIN corto
       se adivina probando.
     - Pega un enlace de YouTube: del enlace sale el ID del video. Título y canal, del servicio
       oEmbed de YouTube (sin clave), que además valida si el video existe y **permite
       incrustarse** (muchos videos oficiales no).
     - Se guarda el ID y el título/artista que confirma el usuario, no los datos de YouTube (sus
       condiciones no permiten guardarlos más de 30 días sin refrescar).
     - Elige una de 3 a 5 portadas genéricas; el título se dibuja encima.
     - Listas visibles solo por enlace, sin buscador de listas ajenas, límite de canciones por
       lista y de peticiones, una forma de que Alexi borre listas, copias de seguridad y una
       página corta de privacidad.
  3. **El "extra":** una explicación breve del proyecto (qué es y cómo está hecho). Mismo
     formato que Chartwave; lo define el chat de configuración.
- **Más adelante: portadas generadas.** Las portadas actuales como plantillas; el título elige
  plantilla, color y composición. Las genéricas de la v2 son el primer paso.
- **Después, si hace falta:** cuentas de usuario; vista previa al compartir (Open Graph, con la
  portada en PNG).

## Cómo añadir una canción o un artista

Esto es para el catálogo de Alexi. Las canciones de los usuarios (paso 2 de la v2) entran por
la app y viven solo en la base de datos.

1. Video oficial que se pueda incrustar: el oEmbed de YouTube
   (`https://www.youtube.com/oembed?url=<enlace>&format=json`) da título y canal, y falla si el
   video no permite incrustarse. Desde el chat de Claude no se puede abrir YouTube: se comprueba
   al probar en local (si no se deja, la app muestra su aviso).
2. Una entrada en `Mixtape/Content/catalog.json`, dentro de su artista: `slug`, `title` y
   `videoId` (lo que va después de `v=`). Un artista nuevo es un bloque más, con `slug`, `name`
   y `tracks`. El orden del archivo es el del inicio y el de ⏮ ⏭.
3. La portada de la canción en `Mixtape/Content/covers/{slug}.svg` y la carátula del artista en
   `Mixtape/Content/covers/artist-{slug}.svg`. Las dos: 800×800, colores de la paleta, e IDs,
   clases y animaciones con un prefijo propio, porque el SVG va incrustado en la página.
   - La de la canción lleva su número romano arriba a la izquierda, en el orden dentro de su
     artista (Jósean Log va por la V; Moneda Dura, por la IV).
   - La del artista sale de su nombre, sin número. Ni fotos, ni logos, ni las carátulas de sus
     discos.
   - Sin animación tiene que verse bien: la pose de reposo va en el `transform` de la clase, no
     solo en la animación (la copia desenfocada y `prefers-reduced-motion` las quitan).
4. Reiniciar la app (en el servidor, desplegar): al arrancar, el archivo se vuelca en la base
   de datos. Añade lo nuevo y actualiza lo que cambió, buscando por slug; nunca borra. Para
   quitar una canción hay que borrarla también de la tabla `Tracks`.

## Decisiones propias de esta app

| Decisión | Por qué |
|---|---|
| Nombre **Mixtape** (antes `Lyrics`). | Una mixtape es una lista de canciones que uno hace y comparte: vale para la v1 (las de Alexi) y la v2 (la lista de cada uno). Retro, como la marca. |
| **Sin letras en la v1.** | Las letras de otros necesitan licencia. Mostrar una sola línea no cambia eso. Ver "Letras" más abajo. |
| Portadas propias con la paleta de la marca; sin el nombre del artista en la portada. El crédito va debajo. | Que se vean de la familia de la plataforma y no parezcan las oficiales. |
| **Sin Bootstrap** (excepción a §4). | Nada lo usa: no hay formularios ni tablas, y el diseño es CSS propio. Entra si llega un formulario (la v2). |
| Proyectos (v2): `Mixtape.Data` (entidades, `MixtapeDbContext`, migraciones), `Mixtape.Application` (`ArtistManager`, `TrackManager`, `CatalogManager`) y `Mixtape.Shared` (`IArtistsService`, `ArtistInfo`, `ArtistDetails`, `ITracksService`, `TrackInfo`, `TrackDetails`, `AppException`). Sin API ni servicio HTTP todavía. | El Manager devuelve modelos que también recibe `SongPlayer` en el navegador, así que tienen que estar en `.Shared`. La API entra cuando un componente del navegador pida datos (paso 2). |
| Base de datos PostgreSQL `mixtape` (§3). Tablas `Artists`, `Tracks` y `Covers`, con nombres en mayúscula inicial como en Apollo. Usuarios y listas se crean en el paso 2, con su migración. | En SQL a mano hay que poner comillas (`select * from "Tracks"`); la app no lo nota. No se crean tablas vacías. |
| El catálogo de Alexi se escribe en `Content/catalog.json` y al arrancar se vuelca en la base de datos (`CatalogManager.SyncAsync`). Las páginas leen siempre de la base de datos. | Queda en git junto a sus portadas, no hay que repetir nada a mano en dos bases de datos (PC y servidor) y, si se pierde la base de datos, vuelve solo. |
| Las portadas siguen siendo archivos SVG del repo (`Content/covers`); `CoverStore` las lee una vez al arrancar. La tabla `Covers` dice qué archivo usa cada canción. La portada de una canción del catálogo es `{slug}.svg`; la carátula de un artista, `artist-{slug}.svg` (`Artist.CoverKey`). | Son diseño, van con el código. Con la tabla, en el paso 2 muchas canciones pueden compartir una portada genérica. |
| ⏮ ⏭ se quedan dentro del artista (`TrackDetails.PreviousSlug` / `NextSlug`). | Acordado con Alexi el 2026-11-12. |
| El inicio (`Home`) muestra una carátula por artista y cada artista tiene su página (`ArtistPage`, `/artists/{slug}`) con sus canciones. En la canción, el enlace de volver lleva a su artista. Las direcciones de las canciones (`/tracks/{slug}`) no cambian. | Elegido por Alexi en la demo (2026-11-13), en vez de secciones por artista: el inicio queda corto y, en el paso 2, el grupo de cada usuario es una carátula más. |
| La carátula del artista es una fila de `Covers` (`Artist.CoverKey`), igual que la portada de una canción. | Un solo mecanismo. En el paso 2, el grupo de un usuario elige ahí una genérica. |
| Las carátulas de artista salen del nombre (Jósean Log: ondas de sonido; Moneda Dura: un puñado de monedas), no de fotos, logos ni de las carátulas de sus discos. Una portada puede tomar la idea de la canción (Los Ojos de Aitana: amanece en La Habana), nunca una frase de la letra. | Son obra de otros, como las letras. |
| `.mx-gallery` (rejilla de portadas) y `.mx-back` (enlace de volver) están en `app.css`. | Cada una la usan dos páginas, y el CSS de un componente solo vale dentro de él. |
| En el log no salen las consultas SQL de EF (`Microsoft.EntityFrameworkCore.Database.Command` en `Warning`, en `appsettings.json`). Sí salen las migraciones y `Catalog synced`. | Cada página escribía su consulta entera y tapaba lo que importa. Para verlas un rato, cambiar ese nivel a `Information` y no subir el cambio. |
| Idioma: lo decide el servidor (cookie si el usuario eligió, si no el idioma del navegador) y el WebAssembly arranca con el mismo (`applicationCulture`). | Una sola fuente de verdad. El cambio de idioma recarga la página entera para que el WebAssembly arranque en el nuevo idioma. |
| Los textos de la interfaz (`SharedResource` y sus `.resx`) viven en `.Client`. | El host referencia al cliente, así los dos los usan sin necesitar `.Shared`. |
| Sin `UseHttpsRedirection` ni HSTS en la app. | El HTTPS lo pone Caddy en el servidor; el contenedor solo habla HTTP en el 8080. |
| Catálogo y portadas en `Mixtape/Content/`, fuera de `wwwroot`. | Las portadas van incrustadas en la página (así cargan las fuentes y corren sus animaciones). Añadir una canción es tocar solo esa carpeta. |
| Páginas SSR (inicio, artista y canción) en el host; un solo componente interactivo, `SongPlayer` (`InteractiveAuto`). La portada, el vinilo y el fondo desenfocado los pinta la página (`TrackPage`); `SongPlayer` recibe solo la canción (título, artista, ID del video). | Solo el reproductor necesita interactividad. Además, los parámetros de un componente interactivo viajan del navegador al servidor al arrancar el circuito, y Blazor Server corta los mensajes de más de 32 KB: con la portada (~10 KB de SVG) como parámetro, el circuito se caía y la música no arrancaba. |
| En `SongPlayer`, Blazor lleva lo que pasa pocas veces (play/pausa, montar y desmontar) y el módulo JS `SongPlayer.razor.js` lo que pasa 4 veces por segundo (tiempo y barra). | En modo Server cada evento cruza SignalR; lo frecuente se queda en el navegador. |
| El JS colocado de `.Client` se importa desde la raíz: `./Tracks/SongPlayer.razor.js`. | Los archivos estáticos de `.Client` se sirven en `/`, no en `_content/`. |
| Página de la canción: la portada manda. Escritorio en dos columnas (portada con vinilo \| título, artista y video); tablet y móvil en una, portada arriba; ≥1600 px más ancho y catálogo a 4 columnas. | Es lo que hace que la app se vea distinta de YouTube; sin letras, la portada ocupa ese sitio. |
| Detrás de la página va una copia de la portada muy desenfocada (`.mx-ambient`), sin animaciones y con sus propios IDs (`amb-…`). | La página toma los colores de cada canción. Sin animaciones, porque el desenfoque redibujado en cada fotograma pesa en móviles; con IDs propios, porque las referencias `url(#…)` se resolverían contra la copia animada. |
| Un vinilo asoma detrás de la portada; al sonar sale un poco más y gira (clase `mx-playing` en `.mx-stage`, que pone `SongPlayer.razor.js` con el estado que manda YouTube). | Refuerza el "Mixtape" y conecta con el vinilo de la tarjeta del hub. Sigue al estado real de YouTube, aunque se use su propio botón. |
| Al salir de la canción se destruye el reproductor. | La música no sigue sonando con el reproductor oculto (regla de YouTube). |
| Al abrir una canción, el video se prepara y espera a play. Solo arranca solo después de ⏮ ⏭ o al terminar la anterior, y solo cuando se ve al menos la mitad del video (sin contar lo que tapa la barra de controles). Lo marca un aviso en `sessionStorage` (`mixtape-autoplay`) que lee el reproductor siguiente. | Quien llega por un enlace compartido no ha tocado la página y el navegador bloquearía el sonido. Lo de la mitad es la regla de YouTube para empezar solo; en el móvil el video queda bajo la portada, así que espera a que se baje hasta él o a que se pulse play. |
| ⏮ ⏭ en la barra de controles; al terminar una canción pasa a la siguiente y, tras la última, vuelve a la primera dentro de su artista (lo calcula `TrackManager`). `TrackPage` pone `@key` al `SongPlayer` para que cada canción tenga un reproductor nuevo. | Escuchar seguido sin volver a la lista. Los enlaces son normales (navegación mejorada) y el `@key` asegura que el reproductor viejo se destruye y el nuevo carga su video. |
| Mientras YouTube prepara el video: anillo girando en el play y "Cargando…" con un ecualizador en el recuadro del video. | Antes el play salía apagado sin explicar por qué. |
| El vinilo no gira entero: gira una capa de dentro (`::before`), recortada por el propio disco. | Un cuadrado girado ocupa más que su caja; en el móvil la página cambiaba de ancho al ritmo del disco (scroll lateral y barra de abajo saltando). |
| Instalable como app según §11 de `CONVENTIONS.md`: manifiesto `standalone` con `id`, archivos de `dev-conventions/pwa/` copiados tal cual (`sw.js`, `offline.html`, `js/app-shell.js`), botón Instalar, diálogo de iPhone, y Atrás/Compartir/Recargar solo instalada. | Regla de la plataforma (2026-10-21). |
| Icono de app: Rako con un vinilo detrás, sobre el degradado "sol de la ciudad" (`#FF7CC8 → #FFE45C`), etiqueta rosa como el disco de la canción. Fuente en `wwwroot/app-icon.svg`; de ahí salen `icon-maskable-512.png` y `apple-touch-icon.png` (opacos). En el PC, el Rako de siempre (`favicon-192/512`). | Elegido por Alexi en la demo: el detalle del vinilo con los colores del casete. Fondo cálido para distinguirlo del hub (cielo de noche). |
| Barra: idioma en la píldora EN\|ES del hub. En el móvil, el enlace a alexitp1125 baja al final de la página (`.mx-footer`); en pantallas de 400 px o menos queda solo Rako cuando la barra se llena (Instalar visible, o instalada). Esa regla usa `:has()` y está en `app.css`. | Con Instalar, y con Atrás/Compartir/Recargar instalada, la barra no cabe en un teléfono. |
| Tema claro como en el hub: botón ◐ en la barra; se guarda en `localStorage` (`theme`) y un script en el `<head>` lo aplica antes de pintar; por defecto, noche. `js/theme.js` lo cambia y lo vuelve a poner tras la navegación mejorada (`enhancedload`). Con `data-theme`, no `data-bs-theme`. | Mismo comportamiento en toda la plataforma, sin parpadeo. `data-theme` porque aquí no hay Bootstrap (`tokens.css` acepta los dos). Cada subdominio guarda su propia elección; compartirla (cookie del dominio) sería un cambio del hub y de las apps. |

## Cómo se arranca

```powershell
cd C:\dev\mixtape
dotnet build
dotnet run --project Mixtape
```

Se abre en http://localhost:5080.

**Base de datos en local:** PostgreSQL 18 de Windows, base de datos `mixtape` (la crea la app la
primera vez). La cadena de conexión se guarda una vez, fuera del repo:

```powershell
# Guarda la cadena de conexión del PC (con la contraseña del usuario postgres del PC)
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Database=mixtape;Username=postgres;Password=<la del PC>" --project Mixtape
```

Al arrancar, la app aplica las migraciones pendientes y vuelca `catalog.json`.

**Cambiar las tablas:** se cambia la entidad en `Mixtape.Data/Entities` y se crea una migración:

```powershell
# Genera la migración con los cambios de las entidades (solo crea archivos)
dotnet ef migrations add <Nombre> --project Mixtape.Data --startup-project Mixtape
```

Al depurar con Visual Studio, el depurador de JS se para en excepciones internas del iframe de
YouTube (`Permissions.query`). No son de la app: Continue, o arrancar con Ctrl+F5. Fuera del
depurador (y en producción) el navegador solo los anota en la consola.

El service worker (`sw.js`) también se registra en local. Para probar la página sin conexión:
DevTools → Application → Service workers → *Offline*, y recargar. Para quitarlo: *Unregister*.

## Cómo se despliega

Servicio `mixtape` en `server-infra` (compose y Caddyfile). Tras el push de la app:

```bash
# Entra al servidor
ssh hub
# Solo si el despliegue trae migraciones: copia de seguridad antes
bash ~/server-infra/scripts/backup.sh
# Trae el código nuevo
cd ~/projects/mixtape && git pull
# Reconstruye y levanta el contenedor
cd ~/server-infra && docker compose up -d --build mixtape
# Mira el arranque: migraciones y "Catalog synced"
docker compose logs mixtape --tail 30
```

En el servidor la base de datos es `mixtape`, usuario `mixtape`, en el Postgres compartido
(`postgres:5432`); el contenedor recibe la cadena en `ConnectionStrings__Default`. Para abrirla
desde el PC: túnel `ssh -L 5433:localhost:5432 hub` y pgAdmin en `localhost:5433`.

El build tarda unos 2–3 minutos. En el log del arranque salen cosas que no son fallos:

- `Cannot load library libgssapi_krb5.so.2`: el conector de Postgres busca una librería de
  Kerberos que la imagen no trae y que no se usa (se entra con usuario y contraseña).
- Los avisos de que las claves de Data Protection viven dentro del contenedor: se arreglan en
  el paso 2, con la carpeta en el `Dockerfile` y el volumen.
- Solo con la base de datos vacía: un `fail` al leer `__EFMigrationsHistory`. La tabla aún no
  existe; EF la crea a continuación.

Al recrear el contenedor, una pestaña que estaba abierta pierde la conexión y pide recargar;
no hay sesiones ni datos que perder.

## Letras: lo investigado (2026-10-20)

Por si algún día vuelven, solo con letras propias, con permiso o de dominio público.

| Vía | Qué da | Problema |
|---|---|---|
| *Lyric Video* oficial | La letra dentro del video, puesta por el artista. | Ninguno. |
| Subtítulos de YouTube (`cc_load_policy=1`, `cc_lang_pref=es`) | YouTube muestra los subtítulos del video en su reproductor. | Solo si el video los tiene; no se pueden estilar ni sacar del video. |
| API de Musixmatch, plan gratis | ~30 % de la letra, uso no comercial. | Fragmento sin sincronizar; completo y sincronizado = licencia de pago. |
| LyricFind | Letras con licencia. | Solo para empresas, por contrato. |
| Genius | Su API no da letras. | Sacarlas de su web viola sus términos. |
| Permiso del artista o de su editora | Lo que autoricen. | Canción por canción. |

## Bitácora

### 2026-11-13 (madrugada): commit 2 desplegado, commit 3 escrito
- Commit 2 desplegado con copia antes. En el servidor: migración `InitialCreate` aplicada y
  `Catalog synced: 1 artists, 5 tracks, 11 changes`.
- Alexi dio el segundo artista: Moneda Dura, con "Comienzo y Final de una Verde Mañana",
  "Al Sudeste", "Los Ojos de Aitana" y "B".
- Demos: primero secciones por artista (portadas grandes o compactas); Alexi propuso una
  carátula por artista que abre sus canciones, y es lo que quedó. Portadas elegidas por él:
  Los Ojos de Aitana con dos soles sobre La Habana (a partir del estribillo), "B" con la letra
  y sus ecos, y Moneda Dura con un puñado de monedas. Para la mañana verde entró un tono menta
  que no está en la paleta.
- Commit 3 (escrito): `Artist.CoverKey`, `ArtistManager`, `IArtistsService`, `ArtistPage`,
  `Home` con artistas, el volver de la canción a su artista, y fuera `GetTracksAsync`.
- Acordado para el paso 2: los grupos son de cada usuario y un artista hace de grupo.

### 2026-11-13: commit 2 probado
- Alexi lo probó en local: primer arranque `Catalog synced: 1 artists, 5 tracks, 11 changes`;
  los siguientes, `0 changes` (el volcado no repite nada).
- Paquetes de ASP.NET de 10.0.13 a 10.0.14 (`Components.WebAssembly.Server`,
  `Components.WebAssembly`, `Extensions.Localization`). El build avisaba (`NU1903`) de un fallo
  de seguridad en el depurador de WebAssembly (GHSA-g27p-qf6j-c96c); afecta al PC de quien
  depura, no al servidor. Con la 10.0.14 compila sin avisos. `EntityFrameworkCore.Design` sigue
  en 10.0.13.
- Las consultas SQL de EF ya no salen en el log (ver decisiones).

### 2026-11-12 (noche): v2, paso 1
- El chat de infra dejó PostgreSQL 18 en el servidor (base y usuario `mixtape`, copias cada
  noche). Acordado con Alexi: tablas con mayúscula inicial, ⏮ ⏭ dentro del artista y el
  catálogo en `catalog.json`, volcado al arrancar.
- Commit 1 (`feat: PostgreSQL with artists, tracks and covers tables`): proyecto `Mixtape.Data`,
  migración `InitialCreate` (creada por Alexi con `dotnet ef`), migrar al arrancar. Probado en
  local: tablas creadas.
- Commit 2 (escrito): `Mixtape.Shared` y `Mixtape.Application`; `TrackManager` y
  `CatalogManager`; `catalog.json` sustituye a `tracks.json`; `CoverStore` sustituye a
  `TrackCatalog`; el catálogo y la página de la canción leen de la base de datos.

### 2026-11-12
- La v1.1 está desplegada y probada: todo bien. `favicon-180.png` ya está fuera (`508253f`).
- Acordada la v2 con Alexi: segundo artista y agrupación, usuarios sin contraseña (nombre + PIN,
  sin guardar datos de nadie) con sus canciones de YouTube y 3–5 portadas genéricas, PostgreSQL, y
  una explicación breve del proyecto. Lo que afecta a toda la plataforma (PostgreSQL en el
  servidor, regla de usuarios, formato de la explicación) lo cierra antes en el chat de
  configuración.

### 2026-10-21 (casa, noche)
- Leída la regla nueva de `dev-conventions` (§11 "Instalable como app", carpeta `pwa/`, icono del
  hub). Demo de la próxima versión aprobada; icono elegido: vinilo con los colores del casete.
- Escrito: instalable (manifiesto, iconos, `sw.js`, `offline.html`, `app-shell.js`, barra con
  Instalar/Atrás/Compartir/Recargar, diálogo de iPhone, zonas seguras, enlace al hub al final en el
  móvil), aviso de carga, ⏮ ⏭ con paso automático, arreglo del vinilo. `favicon-180.png` deja
  de usarse (lo sustituye `apple-touch-icon.png`, con fondo), pero sigue en el repo.
- Quinta canción: *Pruébame a Ti* (`TR4i7j9Ry74`, video oficial del canal de Jósean Log). Portada
  V: dos cerezas que se balancean sobre el degradado "sol de la ciudad", el mismo del icono.
- Subida a GitHub como v1.1 (`f6302b0`).
- Probado sin compilar: el módulo JS con un YouTube simulado (espera sin aviso, arranca solo si se
  ve, espera si el video está fuera de pantalla, pasa a la siguiente al terminar) y el ancho de la
  página con el disco girando (fijo).

### 2026-10-21
- **Desplegada** en https://mixtape.alexitp1125.com: servicio `mixtape` en el compose y sitio
  en el Caddyfile de `server-infra`; Caddy sacó el certificado de Let's Encrypt solo. Antes se
  probó la versión publicada (Release) en local.
- Visto en el móvil ya desplegada: falta un aviso de carga; la barra de abajo brinca y hay
  scroll lateral mientras suena (causa y arreglo en "Qué sigue"). Tampoco se puede instalar
  como app; Alexi lo pide como regla para todas las apps y se lo plantea al chat de
  configuración.
- Quitado `LyricLine.cs`, que se había quedado sin usar.
- Corregido: la música no arrancaba en la página nueva (el servidor cortaba la conexión de Blazor
  al empezar, sin dejar error en el log). La portada dejó de ser parámetro de `SongPlayer`: ahora
  la pinta `TrackPage` y el vinilo lo mueve el módulo JS.
- Página de la canción rediseñada: portada con vinilo, título, artista y video; fondo con la
  portada desenfocada; una columna en tablet y móvil; más ancho en pantallas grandes y catálogo a
  4 columnas. Fuera todo lo de las letras (`LyricLine`, sincronización, textos y las líneas de
  relleno de `tracks.json`).
- Tema claro: botón ◐ en la barra, `js/theme.js`, script en el `<head>` y texto "Cambiar tema"
  en EN/ES. En el tema claro, el círculo de la barra de avance usa `--text` (el crema no se
  veía) y la barra de error usa `--ink`.

### 2026-10-20 (casa, noche)
- Paleta v3 de `dev-conventions` aplicada con el lenguaje del hub v5: degradado de cielo de fondo,
  barra translúcida, idioma activo subrayado en `--accent-3`, tarjetas con borde `--line` y
  sombra dura `--accent-2`, botón de play y barra de avance con `--accent`, Jost para el texto y
  Space Mono para etiquetas. Chachachá y La Luna pasan al cielo nocturno v3; Doma al de día.
- Aprobados (pendientes de código): tema claro y la nueva página de la canción (ver "Qué sigue").

### 2026-10-20 (casa)
- Nombre definitivo: **Mixtape** (`mixtape.alexitp1125.com`, prefijo `mx-`). Renombrado antes
  del primer commit: proyectos, namespaces, CSS, `LyricsPlayer` → `SongPlayer`, Dockerfile y
  documentos.
- Dirección: reproductor sin letras, con mejor experiencia visual por pantalla. La v2 son las
  listas por enlace; la v3, las portadas generadas.
- `dev-conventions` trae la paleta v3 (noche `#110B4A`); se aplica en el siguiente paso.

### 2026-10-20 (trabajo)
- Alcance de la v1 y solución creada: host + `.Client`, barra de marca con Rako, EN/ES con
  cookie, 404/500 y estado vacío con Rako, `Dockerfile`.
- Portadas rediseñadas (paleta de la marca, Jost + Space Mono) y aprobadas.
- Prototipo pasado a Blazor: `TrackCatalog`, catálogo, `/tracks/{slug}` con 404, reproductor +
  módulo JS.
- Videos (canal oficial de Jósean Log): Doma `pCPYb7KPoUc`, Chachachá `sDMxQF18yvA`, Beso
  `ntdwWKaGaPQ`, La Luna `oJ7WZ_ZS2JA`.
- Corregido: la ruta del JS colocado de `.Client`.
