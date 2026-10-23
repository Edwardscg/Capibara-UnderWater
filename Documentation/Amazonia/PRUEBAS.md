# Pruebas manuales
1. Abrir AmazoniaTerrestre y Play. Verificar 8 HP, espada, fondo y HUD.
2. Pulsar A/D y flechas; comprobar orientación y frenado. Soltar: no flota.
3. Saltar con Espacio sobre el tronco inicial y la raíz siguiente. Pasar desde abajo y aterrizar encima.
4. Usar Shift hacia ambos lados; repetir inmediatamente para comprobar cooldown. Probarlo contra pared izquierda.
5. Avanzar hacia el caimán. Verificar que la cámara sigue sin mostrar todo el mapa.
6. Acercarse y pulsar J con espada: corte visible y 3 de daño por acción. Comprobar que mantener J respeta el intervalo.
7. Cambiar con Q y disparar hacia ambos lados. Verificar origen cerca del arma, daño 1 y desaparición por impacto/tiempo.
8. Observar preparación, arañazo y empujón. Alejarse o subir a una plataforma para provocar lanzamiento de piedras.
9. Dejarse golpear: pérdida de vida, reacción y breve invulnerabilidad; no drena vida continuamente por contacto.
10. Intentar E en la orilla antes de vencer: no debe cargar el agua.
11. Agotar los 18 HP del jefe: ver aturdimiento, huida hacia la derecha e inmersión. No debe aparecer victoria.
12. Acercarse a la orilla: debe aparecer E. Esperar sin pulsarla para verificar que no entra automáticamente.
13. Pulsar E: ver salto, salpicadura, fundido y carga del lago; misma vida del capibara.
14. Nadar en cuatro direcciones y diagonales. Soltar: descenso lento. Probar límites y obstáculos.
15. Pulsar Q y Shift: no hay espada ni dash terrestre. Disparar J hacia la última dirección de natación.
16. Acercarse al jefe. Identificar posicionamiento, aviso y embestida. Cambiar de altura durante el aviso; la trayectoria no debe girar después.
17. Atacar durante recuperación. Reducir a 11 HP o menos: aviso de furia y mayor velocidad con preparación todavía visible.
18. Agotar la vida: animación final, hundimiento, ausencia de daño y victoria. Probar REINTENTAR.
19. Perder toda la vida en tierra y en agua; comprobar derrota, bloqueo de movimiento/ataques y reinicio con R.
20. Probar Escape durante ambas fases: detiene y reanuda; no altera progreso. Cambiar tamaño de Game y revisar HUD/cámara.

Automatización: menú Amazonia > Probar aventura en Play. Ejecuta Unity y su física, no solo análisis estático. Resultados en Resultados.txt; auditoría de referencias en Auditoria.txt; capturas PNG en esta carpeta. Las pruebas teletransportan actores entre casos para aislar mecánicas; no equivalen a una sesión humana completa de dificultad/balance.
