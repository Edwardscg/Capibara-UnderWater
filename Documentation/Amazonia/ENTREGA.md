# Entrega — Amazonía: capibara contra caimán

Abrir `Assets/Scenes/AmazoniaTerrestre.unity` y pulsar Play. La aventura continúa en `AmazoniaAcuatica` mediante E después de vencer al jefe terrestre. `UnderwaterBoss` conserva el nivel anterior.

## Implementado
Dos mapas de 66 unidades, cámara de seguimiento limitada, plataformas, obstáculos y colisiones. Movimiento terrestre con salto, buffer/coyote y dash físico; espada y plasma con cambio de arma. IA terrestre con arañazo, empujón, piedra y escape visible. Inmersión interactiva, fundido y conservación de vida. Natación, plasma, embestida con dirección fijada, furia, derrota final, victoria, pausa y reinicio.

Se integraron los seis PNG suministrados, 119 sprites extraídos mediante metadatos, 36 clips y cuatro controladores. Se preservaron los JPEG y los recursos anteriores.

## Archivos
Nuevos: dos escenas Amazonia, ocho scripts de ejecución en Assets/Scripts/Amazonia, dos herramientas Editor en Assets/Editor/Amazonia, gráficos en Assets/Art/Amazonia, animaciones en Assets/Animations/Amazonia y documentación en esta carpeta. Inventario de archivos principales: Archivos.txt; inventario de recortes: Recortes.tsv.
Prefabs: CapibaraTerrestre, CapibaraAcuatico, CaimanTerrestre, CaimanAcuatico, Plasma, Piedra; además Balance.asset. GameObjects principales: protagonista, jefe, Main Camera, Selva y orilla / Lago amazónico, Aventura · progreso y transición, HUD, EventSystem, límites, plataformas/rocas y burbujas.
Modificado fuera de estas carpetas: ProjectSettings/EditorBuildSettings.asset para incluir las dos escenas al inicio. No se modificaron la versión, los paquetes, el pipeline ni el Input System. No se hicieron commits ni push.

## Correcciones durante las pruebas
Plataformas convertidas a superficies de una sola cara para permitir saltar desde debajo; impulso ajustado para alcanzarlas. Integración de velocidad acuática corregida para evitar sobrepasar el máximo al aplicar aceleración. Pivotes y recortes por secuencia; rótulos excluidos. Madera tomada del fondo para reemplazar barras temporales. Separación de salto/ataque y bloqueo de espada bajo el agua. Se corrigió la medición de teletransportes en pruebas para tener en cuenta la interpolación física, y se estabilizó la altura del tirador únicamente en el caso aislado de prueba de victoria.

## Validación
21/10/2026, Unity 6000.5.9f1: 84 aserciones aprobadas, 0 fallos, 0 errores/excepciones durante Play. Algunas aserciones repiten la comprobación de trayectoria durante varios pasos; no representan 84 mecánicas distintas. Auditoría: 0 Missing Scripts y 0 referencias rotas en ambas escenas. SHA256 confirma copias exactas de los seis PNG y conservación de ambos JPEG originales.
Resultados detallados: Resultados.txt y Auditoria.txt. Capturas 01 a 06 producidas por el juego en ejecución y revisadas visualmente. Las pruebas usan teclado virtual y física real; colocan actores en posiciones específicas para aislar casos, por lo que no sustituyen una evaluación humana continua de dificultad. No se midieron 60 FPS sostenidos ni se generó un ejecutable independiente.

## Pendientes no bloqueantes
No hay música ni efectos sonoros porque no se encontraron recursos de audio en Assets. Algunas poses y estelas de los sheets IA tienen proporciones variables; se seleccionaron secuencias aprovechables, sin fabricar fotogramas. No se implementa combo de tres cortes: un golpe de espada con su secuencia y ventana de daño. No se añade jefe humano final.

Controles y arranque: CONTROLES.md. Arquitectura y balance: COMBATE_CAIMAN.md. Selección gráfica: ASSETS_ANIMACIONES.md. Pruebas manuales paso a paso: PRUEBAS.md.
