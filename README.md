# Mixtape

A music player with a city-pop look: favorite songs grouped by artist, each with its own page and cover, ready to share with a link. Part of [alexitp1125.com](https://alexitp1125.com).

- Audio always plays through the official YouTube player.
- Covers are original designs.
- Installable as an app on phones and computers.

**Status:** live at [mixtape.alexitp1125.com](https://mixtape.alexitp1125.com).

## Stack

.NET 10 · Blazor Web App (static SSR + WebAssembly) · EF Core · PostgreSQL · Docker

## Run locally

Needs PostgreSQL. Save the connection string once (the app creates the database and its tables):

```
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Database=mixtape;Username=postgres;Password=<yours>" --project Mixtape
dotnet run --project Mixtape
```

Then open http://localhost:5080.

---

Rako and the other brand assets are original designs by Alexi Torres. All rights reserved.
