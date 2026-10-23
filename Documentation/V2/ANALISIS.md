# Auditoría V2
V1 preservada en b3fe937 (rama v1). El commit anterior solo contenía README; el snapshot añade el proyecto real, sin Library/Temp/Logs/.vs. Rama de desarrollo v2. No se modificó el remoto.

| Área | Estado inicial | Evidencia / decisión |
|---|---|---|
| Movimiento, salto, dash, espada, plasma | EXISTE | AdventurePlayer y pruebas V1, conservar física |
| Dos fases, daño, embestida, victoria | EXISTE | AdventureBoss/Flow, escenas Amazonia |
| Caimán terrestre bípedo | AUSENTE | Sustituir clips cuadrúpedos por hojas nuevas |
| Plasma 3/3 y recarga | AUSENTE | Añadir componente de munición con reloj jugable |
| Diálogos, retratos, introducciones | AUSENTE | Director de secuencias sobre Flow existente |
| Video | AUSENTE | Integrar MP4 autorizado y reproductor configurable con fallback |
| Emotes con WAV | AUSENTE | Rueda radial y cancelación por contexto |
| Parallax y mezcla audio | A MEJORAR | Cámara ya existe; añadir capas y slots de música |
| Checkpoint acuático | DEFECTUOSO para V2 | V1 reinicia tierra; V2 reintenta fase actual |
| QA | EXISTE | Extender pruebas reales de Play, conservar resultados históricos |

Orden: assets/clips; munición y HUD; secuencias/video/checkpoints; emotes/audio/parallax; QA y documentación. Unity 6000.5.9f1 y paquetes se mantienen.
La instrucción directa más reciente autoriza Caiman_Cinematica.mp4 y sustituye la prohibición que figura en el documento adjunto. Inmersión usa fallback porque no se entregó segundo video.
