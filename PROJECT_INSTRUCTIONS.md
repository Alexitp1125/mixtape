# Mixtape — instrucciones del proyecto

Sigue `dev-conventions`. Aquí solo va lo propio de esta app.

## Resumen

- **Dónde estoy:** v1 en producción en https://mixtape.alexitp1125.com desde el 2026-10-21:
  4 canciones de Jósean Log con portada propia, paleta v3, tema claro, EN/ES. Sin letras.
  Repo en GitHub (`Alexitp1125/mixtape`).
- **Lo último (2026-10-21):** primer despliegue.
- **Qué sigue** (próximo despliegue), un commit por paso:
  1. Aviso de carga hasta que la canción esté lista. Hoy el play sale apagado y el recuadro
     del video vacío, sin decir que está cargando.
  2. Móvil: mientras suena, la barra de abajo brinca y la página tiene scroll lateral. Causa:
     el vinilo gira como un cuadrado y su caja girada se sale de la pantalla (en un Pixel 7 el
     ancho de la página oscila entre 412 y 448 px). Arreglo probado: que gire una capa interior
     recortada por el propio vinilo.
  3. Seguir sin volver atrás: ⏮ ⏭ junto al play, y al terminar una canción pasa sola a la
     siguiente (tras la última, vuelve a la primera). Arranca sola solo si se ve al menos la
     mitad del video (regla de YouTube); en móvil el video queda bajo la portada: verlo en la demo.
  4. Dos canciones más de Jósean Log con su portada (Alexi elige y pasa los enlaces oficiales).
  5. La tarjeta del hub (nombre "Mixtape", enlace y estado) se cambia en el chat del hub, no aquí.

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
  y pantalla grande. Se despliega.
- **v2: listas por enlace.** Cualquiera pega un enlace de YouTube y arma su lista. Sin cuentas:
  al crear una lista se reciben dos enlaces, uno privado para editarla y otro para compartirla.
  Aquí entran PostgreSQL y `.Shared`, `.Application`, `.Data` y `.Tests` (§1.2).
  - Del enlace sale el ID del video. Título y canal, del servicio oEmbed de YouTube (sin clave),
    que además valida si el video existe y **permite incrustarse** (muchos videos oficiales no).
  - Se guarda el ID y el título/artista que confirma el usuario, no los datos de YouTube (sus
    condiciones no permiten guardarlos más de 30 días sin refrescar).
  - Listas no públicas por defecto, sin buscador de listas ajenas, límite de peticiones,
    copias de seguridad de la base de datos y una página corta de privacidad.
- **v3: portadas generadas.** Las 4 portadas actuales como plantillas; el título elige
  plantilla, color y composición.
- **Después, si hace falta:** cuentas de usuario; vista previa al compartir (Open Graph, con la
  portada en PNG).

## Cómo añadir una canción

1. Una entrada en `Mixtape/Content/tracks.json`: `slug`, `title`, `artist` y `videoId` (lo que
   va después de `v=`).
2. La portada en `Mixtape/Content/covers/{slug}.svg` (800×800, colores de la paleta, IDs y
   clases con el prefijo del slug, porque el SVG va incrustado en la página).
3. Reiniciar la app: el catálogo se lee una vez al arrancar.

## Decisiones propias de esta app

| Decisión | Por qué |
|---|---|
| Nombre **Mixtape** (antes `Lyrics`). | Una mixtape es una lista de canciones que uno hace y comparte: vale para la v1 (las de Alexi) y la v2 (la lista de cada uno). Retro, como la marca. |
| **Sin letras en la v1.** | Las letras de otros necesitan licencia. Mostrar una sola línea no cambia eso. Ver "Letras" más abajo. |
| Portadas propias con la paleta de la marca; sin el nombre del artista en la portada. El crédito va debajo. | Que se vean de la familia de la plataforma y no parezcan las oficiales. |
| **Sin Bootstrap** (excepción a §4). | Nada lo usa: no hay formularios ni tablas, y el diseño es CSS propio. Entra si llega un formulario (la v2). |
| Sin `.Shared` ni API por ahora. | La página SSR le pasa la canción como parámetro al componente interactivo. `.Shared` entra cuando el cliente tenga que pedir datos por su cuenta. |
| Idioma: lo decide el servidor (cookie si el usuario eligió, si no el idioma del navegador) y el WebAssembly arranca con el mismo (`applicationCulture`). | Una sola fuente de verdad. El cambio de idioma recarga la página entera para que el WebAssembly arranque en el nuevo idioma. |
| Los textos de la interfaz (`SharedResource` y sus `.resx`) viven en `.Client`. | El host referencia al cliente, así los dos los usan sin necesitar `.Shared`. |
| Sin `UseHttpsRedirection` ni HSTS en la app. | El HTTPS lo pone Caddy en el servidor; el contenedor solo habla HTTP en el 8080. |
| Canciones y portadas en `Mixtape/Content/`, fuera de `wwwroot`. `TrackCatalog` (singleton) las lee una vez al arrancar. | Las portadas van incrustadas en la página (así cargan las fuentes y corren sus animaciones). Añadir una canción es tocar solo esa carpeta. |
| `TrackDetails` en `.Client/Tracks/`. | Lo recibe el componente interactivo como parámetro. |
| Páginas SSR (catálogo y canción) en el host; un solo componente interactivo, `SongPlayer` (`InteractiveAuto`). La portada, el vinilo y el fondo desenfocado los pinta la página (`TrackPage`); `SongPlayer` recibe solo la canción (título, artista, ID del video). | Solo el reproductor necesita interactividad. Además, los parámetros de un componente interactivo viajan del navegador al servidor al arrancar el circuito, y Blazor Server corta los mensajes de más de 32 KB: con la portada (~10 KB de SVG) como parámetro, el circuito se caía y la música no arrancaba. |
| En `SongPlayer`, Blazor lleva lo que pasa pocas veces (play/pausa, montar y desmontar) y el módulo JS `SongPlayer.razor.js` lo que pasa 4 veces por segundo (tiempo y barra). | En modo Server cada evento cruza SignalR; lo frecuente se queda en el navegador. |
| El JS colocado de `.Client` se importa desde la raíz: `./Tracks/SongPlayer.razor.js`. | Los archivos estáticos de `.Client` se sirven en `/`, no en `_content/`. |
| Página de la canción: la portada manda. Escritorio en dos columnas (portada con vinilo \| título, artista y video); tablet y móvil en una, portada arriba; ≥1600 px más ancho y catálogo a 4 columnas. | Es lo que hace que la app se vea distinta de YouTube; sin letras, la portada ocupa ese sitio. |
| Detrás de la página va una copia de la portada muy desenfocada (`.mx-ambient`), sin animaciones y con sus propios IDs (`amb-…`). | La página toma los colores de cada canción. Sin animaciones, porque el desenfoque redibujado en cada fotograma pesa en móviles; con IDs propios, porque las referencias `url(#…)` se resolverían contra la copia animada. |
| Un vinilo asoma detrás de la portada; al sonar sale un poco más y gira (clase `mx-playing` en `.mx-stage`, que pone `SongPlayer.razor.js` con el estado que manda YouTube). | Refuerza el "Mixtape" y conecta con el vinilo de la tarjeta del hub. Sigue al estado real de YouTube, aunque se use su propio botón. |
| Al salir de la canción se destruye el reproductor. | La música no sigue sonando con el reproductor oculto (regla de YouTube). |
| Sin autoplay: el video se prepara y el usuario pulsa play. | Quien llega por un enlace compartido no ha tocado la página y el navegador bloquearía el sonido; así funciona igual siempre. |
| Sin `DependencyInjection/` vacío: aparece con el primer servicio (`TrackCatalog`). | §0. |
| Tema claro como en el hub: botón ◐ en la barra; se guarda en `localStorage` (`theme`) y un script en el `<head>` lo aplica antes de pintar; por defecto, noche. `js/theme.js` lo cambia y lo vuelve a poner tras la navegación mejorada (`enhancedload`). Con `data-theme`, no `data-bs-theme`. | Mismo comportamiento en toda la plataforma, sin parpadeo. `data-theme` porque aquí no hay Bootstrap (`tokens.css` acepta los dos). Cada subdominio guarda su propia elección; compartirla (cookie del dominio) sería un cambio del hub y de las apps. |

## Cómo se arranca

```powershell
cd C:\dev\mixtape
dotnet build
dotnet run --project Mixtape
```

Se abre en http://localhost:5080.

Al depurar con Visual Studio, el depurador de JS se para en excepciones internas del iframe de
YouTube (`Permissions.query`). No son de la app: Continue, o arrancar con Ctrl+F5. Fuera del
depurador (y en producción) el navegador solo los anota en la consola.

## Cómo se despliega

Servicio `mixtape` en `server-infra` (compose y Caddyfile). Tras el push de la app:

```bash
ssh hub
cd ~/projects/mixtape && git pull
cd ~/server-infra && docker compose up -d --build mixtape
```

El build tarda unos 2–3 minutos. El log avisa de que las claves de Data Protection viven dentro
del contenedor: es esperado. Al recrearlo, una pestaña que estaba abierta pierde la conexión y
pide recargar; no hay sesiones ni datos que perder.

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

### 2026-10-21
- **Desplegada** en https://mixtape.alexitp1125.com: servicio `mixtape` en el compose y sitio
  en el Caddyfile de `server-infra`; Caddy sacó el certificado de Let's Encrypt solo. Antes se
  probó la versión publicada (Release) en local.
- Visto en el móvil ya desplegada: falta un aviso de carga; la barra de abajo brinca y hay
  scroll lateral mientras suena (causa y arreglo en "Qué sigue").
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
