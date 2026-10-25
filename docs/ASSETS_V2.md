# Assets V2

42 originales: 38 PNG, 3 WAV y 1 MP4. Copiados sin sobrescribir los originales externos. Inventario con resolución y SHA-256: `Documentation/V2/Inventario.json`.

## Tratamiento

Importación Point, sin mipmaps, sin compresión, NPOT preservado y máximo 4096. Recortes manuales no uniformes como Sprite assets: no se inventaron cuadros ni se editó la imagen fuente. Las hojas tienen distribución irregular, efectos salientes y escalas distintas: no se usó una grilla universal.

`Documentation/V2/Recortes.tsv` contiene cada cuadro, fuente, rectángulo medido desde arriba, PPU y pivote EN PÍXELES relativo al recorte (API Sprite.pivot). Los pivotes se definieron normalizados, generalmente (0.65,0.5) en tierra y (0.60,0.5) en agua; los PPU cambian por hoja para compensar resolución. Los clips se encuentran en Assets/Animations/V2.

Las poses de daño, furia y muerte acuática son reutilizaciones explícitas, con flash rojo, color cálido y hundimiento físico. No existen hojas nuevas independientes verificadas para esos estados.

## Inventario y uso

| Recurso dentro de Sprites | Resolución | Uso / observación |
|---|---|---|
| Efectos.png | 1536×1024 | Reservado: fondo pintado opaco y rótulos. Se conservan VFX de partículas y efectos de las hojas transparentes; requiere exportación limpia para usar este atlas. |
| Recarga_Arma.png | 1491×1055 | HUD: cápsulas llena y vacía; barra continua dibujada por Unity. |
| Retratos_Dialogos.png | 1536×1024 | Dos retratos enmarcados, capibara y caimán. Fondo opaco aceptable dentro de la tarjeta. |
| Caiman/Acuatico/Caiman_Acuatico_Aparicion.png | 1448×1086 | Aparición: ojos, cabeza y revelación, 5 recortes. |
| Caiman/Acuatico/Caiman_Acuatico_Movimiento_Diagonal_Inferior.png | 2172×724 | Nado diagonal descendente, 6 recortes. |
| Caiman/Acuatico/Caiman_Acuatico_Movimiento_Diagonal_Superior.png | 2172×724 | Nado diagonal ascendente, 6 recortes. |
| Caiman/Acuatico/Caiman_Acuatico_Movimiento_Preparar_Ataque_Rapido.png | 2172×724 | Preparación de embestida, 9 recortes. |
| Caiman/Acuatico/Caiman_Acuatico_Movimiento_v1.png | 2172×724 | Alternativa conservada; se prefiere v2 por cuerpo horizontal más estable. |
| Caiman/Acuatico/Caiman_Acuatico_Movimiento_v2.png | 2172×724 | Idle/nado/recuperación, 8 poses. Reutilizadas para daño/furia/derrota con feedback del motor. |
| Caiman/Acuatico/Caiman_Acuatico_Movimiento_v3.png | 2172×724 | Idéntico byte por byte a v1. Conservado, no duplicado en animaciones. |
| Caiman/Acuatico/Caiman_Movimiento_Ataque_Rapido_v1.png | 2172×724 | Embestida normal, 7 recortes. |
| Caiman/Acuatico/Caiman_Movimiento_Ataque_Rapido_v2.png | 2172×724 | Embestida alternativa de furia, 8 recortes. |
| Caiman/Terrestre/Caiman_Aparicion.png | 1448×1086 | Aparición terrestre de ojos a bípedo, 6 recortes. |
| Caiman/Terrestre/Caiman_Ataque_Cerca.png | 1448×1086 | Garra y empujón, 3 recortes seleccionados por acción. |
| Caiman/Terrestre/Caiman_Ataque_Distancia.png | 1448×1086 | Preparación y lanzamiento de piedra, 3 poses por acción. Piedra física de V1 conservada. |
| Caiman/Terrestre/Caiman_Derrotado_Escape.png | 1448×1086 | Herida, caída alternativa y escape corriendo. Sustituye el atlas de escape opaco. |
| Caiman/Terrestre/Caiman_Escape.png | 1536×1024 | Conservado sin usar: fondo pintado opaco; alternativa transparente disponible en Derrotado_Escape. |
| Caiman/Terrestre/Caiman_Movimientos.png | 1448×1086 | Idle bípedo, andar, telegraph y recuperación. Silueta bípeda sustituye al antiguo cuadrúpedo. |
| Capybara/Acuatico/Capybara_Celebracion_Victoria_Terrestre_Acuatico.png | 1448×1086 | Celebraciones contextuales tierra/agua, 3 poses cada una; no activan rueda. |
| Capybara/Acuatico/CapybaraAcuatico.png | 1448×1086 | Idéntico al original V1: se conservan sus recortes y animaciones funcionales, sin duplicar memoria. |
| Capybara/Terrestre/Capybara_Sumerge.png | 1672×941 | Preparación y salto/inmersión: 8 poses por fila. |
| Capybara/Terrestre/CapybaraTerrestre.png | 1448×1086 | Idéntico al original V1: se conservan sus recortes y animaciones funcionales. |
| Capybara/Terrestre/Emote_SixSeven.png | 1983×793 | Dos filas, 12 poses; bucle mientras dura el WAV. |
| Capybara/Terrestre/Emote_Uiia.png | 1448×1086 | Dos filas, 12 poses de giro; bucle mientras dura el WAV. |
| Capybara/Terrestre/Emote_Wazaaaa.png | 1448×1086 | Dos filas, 10 poses; duración gobernada por WAV real. |
| Capybara/Terrestre/Recarga_Y_Acabado_Plasma.png | 1448×1086 | 6 poses de recarga, sólo al estar inmóvil y con plasma equipado. No interrumpe nado/ataques. |
| Fondos/FondoAcuatico.png | 2172×724 | Panorama acuático de 66 unidades, sin alterar proporción 3:1. |
| Fondos/FondoTerrestre.png | 2172×724 | Panorama terrestre de 66 unidades, sin alterar proporción 3:1. |
| Parallax/Acuatico/Parallax_Algas_v1.png | 2172×724 | Alternativa conservada; se usa v2 para evitar sobrecarga de vegetación. |
| Parallax/Acuatico/Parallax_Algas_v2.png | 2172×724 | Vegetación baja acuática con parallax leve. |
| Parallax/Acuatico/Parallax_Componentes_Acuaticopng.png | 2172×724 | Recorte de roca para los cinco obstáculos físicos acuáticos. |
| Parallax/Acuatico/Parallax_Raices_Bajo_Agua.png | 2172×724 | Primer plano acuático translúcido, centro abierto. |
| Parallax/Terrestre/Parallax_Arboles_Lianas.png | 2172×724 | Primer plano terrestre translúcido. |
| Parallax/Terrestre/Parallax_Componentes_Bosque.png | 2172×724 | Atlas conservado para expansión; plataformas y troncos funcionales V1 reutilizados. |
| Parallax/Terrestre/Parallax_Fondo_Acuatico_Estructuras.png | 2172×724 | Profundidad de ruinas acuáticas, pese a su ubicación en carpeta Terrestre. |
| Parallax/Terrestre/Parallax_Fondo_Bosque_v1.png | 2172×724 | Alternativa conservada; se utiliza v2 por silueta menos recargada. |
| Parallax/Terrestre/Parallax_Fondo_Bosque_v2.png | 2172×724 | Fondo lejano con factor parallax 0.035. |
| Parallax/Terrestre/Parallax_Lianas.png | 2172×724 | Conservado; lianas ya presentes en primer plano y panorama. |
| Sonidos/emote_six_seven.wav | audio/video | Audio real de Six Seven; duración obtenida de AudioClip.length. |
| Sonidos/emote_uiia.wav | audio/video | Audio real de Uiia; una sola voz simultánea. |
| Sonidos/emote_wazaaaa.wav | audio/video | Audio real de Wazaaaa; cancelable. |
| Videos/Caiman_Cinematica.mp4 | audio/video | Video de escape integrado por instrucción directa del usuario; formato vertical y audio original. |

## Archivos y componentes

Se modificaron las dos escenas Amazonia, cuatro prefabs de personajes, los dos controladores de capibara y scripts existentes de movimiento/IA/cámara/flujo/vida. Se añadieron los controladores del caimán V2, recortes/clips y seis componentes: AdventureDirector, AdventureEmotes, AdventureAudio, AdventureParallax, PlasmaMagazine y herramientas de editor/pruebas. No se movieron recursos de terceros.

GameObjects relevantes: Capibara terrestre/acuático, Caimán terrestre/acuático, Main Camera, Aventura · progreso y transición, HUD · Amazonía, Decoración V2, EventSystem. Diálogos, cargador, rueda y pantalla de video se construyen como hijos del Canvas al iniciar Play.

Rigidbody2D capibara: Dynamic, masa 1, Z congelada, Continuous, Interpolate; gravedad 2.8 tierra / 0.12 agua, damping 0 / 3. Caimán: Dynamic, masa 3, Z congelada, Continuous, Interpolate; gravedad 2.8 tierra / 0 agua. CapsuleCollider bípedo 1.3×1.9 vertical, acuático 2.5×1 horizontal; jugador 1.2×1.55.

## Recursos adicionales

Música: no entregada, cinco slots configurables con crossfade. Video de inmersión: no entregado, fallback operativo. Atlas Efectos y Caiman_Escape: requieren exportación transparente para uso directo; no se intentó borrar fondos pintados degradando las figuras. JPEG históricos conservados intactos. Las animaciones de capibara base se reutilizan porque los PNG nuevos son idénticos a V1.

El listado completo por ruta se encuentra en ARCHIVOS_V2.md. El catálogo final contiene 301 recortes; los emotes incorporan las dos filas de poses.
