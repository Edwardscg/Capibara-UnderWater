# Assets y animaciones
Se copiaron los seis PNG suministrados a `Assets/Art/Amazonia/Originals`. No se alteraron los archivos fuente externos ni los JPEG anteriores.
- Cuatro hojas: RGBA 1448x1086, transparencia real, filas irregulares y efectos unidos a las poses.
- Fondos: RGB 2172x724. Se utilizan completos, sin estirar su proporción 3:1.
- Importación: Point, sin mipmaps, sin compresión, tamaño máximo 4096; conserva píxeles y alfa originales.

Se crean objetos Sprite mediante rectángulos de metadatos. No se modifica ni repinta la textura. Las posiciones de fila, límites horizontales y pivotes están explícitos en AmazoniaBuilder; los sprites resultantes pueden inspeccionarse en Animations/Amazonia. Pixels per unit: capibara terrestre 65, acuático 75, caimanes 70; no se escala cada fotograma por su bounding box, lo que evita que las estelas alteren el tamaño del animal.

Clips reales: capibara terrestre Idle, Run, Jump, Fall, Land, Melee, Ranged, Switch, Dash, Hurt, Death. Capibara acuático Idle, Swim, Up, Down, Ranged, Hurt, Death. Caimán terrestre Idle, Run, Telegraph, Claw, Push, RockPrepare, Rock, Recovery, Hurt, Death. Caimán acuático Idle, Swim, Telegraph, Charge, Recovery, Hurt, Enraged, Death. Cuatro Animator Controllers.

Se seleccionan entre 1 y 7 poses por estado. Fall/Land reutilizan poses aéreas/de aterrizaje; no se inventan imágenes. Los rótulos del caimán acuático quedan fuera de los rectángulos. Se excluyen haces muy largos, fotogramas redundantes y efectos mezclados que dificultan una silueta estable. Los efectos incorporados al dibujo no se separan artificialmente del cuerpo.

Limitaciones del material: las hojas generadas no tienen una silueta idéntica en todas las poses. Los pivotes fijos por secuencia reducen los saltos, pero no sustituyen una limpieza artística fotograma a fotograma. El estilo se conserva según los PNG nuevos, sin repintado ni promesa de pixel art artesanal.

Plataformas terrestres: recorte de madera del fondo, con geometría independiente. Rocas auxiliares: sprite de roca ya existente y capas de color. Se conservan intactos los recursos submarinos anteriores.

No se encontraron WAV, MP3 u OGG en Assets. Música, rugidos y efectos sonoros quedan pendientes de recursos con licencia; la respuesta visual está implementada.
