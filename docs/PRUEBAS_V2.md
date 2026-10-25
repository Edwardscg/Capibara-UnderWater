# Pruebas V2

## Resultado comprobado

Última suite completa: 22 de octubre de 2026, Unity 6000.5.9f1 en Windows Editor. Archivo de evidencia: `Documentation/V2/Resultados.txt`: **74 líneas PASS, ninguna FAIL**, incluyendo el resumen de cero errores en Play. La auditoría de ambas escenas reportó **0 Missing Scripts y 0 Missing References**. Capturas reales en `Documentation/V2/01-Exploracion.png` a `09-Rueda.png`.

La suite controla un teclado virtual del New Input System, ejecuta físicas reales, prueba impactos de proyectiles y cambia escenas realmente. Para aislar algunos casos coloca personajes en posiciones controladas o detiene la IA; por tanto no equivale a una partida completa jugada por una persona ni a una certificación de dificultad.

Comprobado: movimiento terrestre, salto, dash, espada, plasma, tres disparos y cuarto bloqueado, reloj de seis segundos, pausa/cinemática/cambio de arma, rueda T y cancelación sin pausa, selección por teclado, tres WAV, cancelación por movimiento y daño, bloqueo de emotes en combate, garra/empujón/piedra con daño real, video decodificado con fin natural, escape al lago, transición única, conservación de HP/munición/reloj, ocho teclas de natación, descenso suave, límite acuático, ciclo de IA y trayectoria fija, furia, daño por embestida, victoria, derrota, checkpoint y reintentos repetidos, ausencia de video y omisión del reproductor reusable.

## Reproducir

Guardar primero el trabajo de escena: las pruebas abren otras escenas y entran/salen de Play. Menú **Amazonia > V2 > Pruebas de integración**. Al finalizar consultar Resultados.txt y Auditoria.txt. No ejecutar el constructor V1 para probar V2.

Las pruebas no requieren instalar paquetes adicionales. Los avisos y errores históricos del log durante el desarrollo no representan el resultado final: se corrigieron un enlace de Animator y un literal escapado del generador antes de la suite satisfactoria.

## Prueba manual paso a paso

1. Abrir AmazoniaTerrestre y Play: 8 HP, espada y tres cápsulas; jefe oculto al inicio.
2. Caminar A/D y flechas; saltar sobre troncos con Espacio; probar Shift a ambos lados y su cooldown.
3. T abre rueda. Seleccionar cada emote por clic y luego por flechas/Enter; comprobar gesto y sonido. Escape cierra sin pausar. Moverse cancela. No debe gastar munición.
4. Q equipa plasma. Pulsar J tres veces separadas: 2, 1, 0. Mantener J vacío no produce otro disparo. Medir seis segundos de tiempo jugable y observar relleno a tres.
5. Durante recarga cambiar a espada y volver: conservar tiempo restante. Pausar con Escape, esperar y reanudar: el contador no debe avanzar durante pausa.
6. Avanzar hasta la presentación. Leer con Enter o Continuar. Repetir una partida omitiendo con Escape: recuperar movimiento y combate, sin volver a mostrar la intro al retroceder.
7. Combatir: cerca, observar garras y empujón; lejos, piedras. Saltar/esquivar y usar recuperación para atacar. Confirmar invulnerabilidad breve y feedback de daño.
8. Derrotar en tierra: herida, celebración, diálogos, video vertical y huida. Dejar terminar el video una vez y omitirlo otra. El caimán debe llegar al lago aunque el jugador esté en su camino.
9. En la orilla pulsar E una vez. Preparación e inmersión por sprites, fundido, mapa acuático. Mantener E no debe cargar dos veces.
10. Leer/omitir presentación acuática. Nadar en cuatro direcciones y diagonales. Soltar teclas: descenso lento. Chocar con suelo, techo, paredes y rocas.
11. Sólo plasma bajo el agua. Esquivar después del aviso: la embestida mantiene dirección. Reducir vida a la mitad: comprobar furia y ritmo más rápido, con recuperación todavía visible.
12. Perder en agua y pulsar Reintentar: comenzar en agua con 8 HP, tres cápsulas e intro omitida. Repetir una segunda vez.
13. Ganar: celebración, diálogo sobre residuos y victoria. No poder atacar después del final.
14. En copia de prueba de la escena, vaciar escapeClip y ruta: el flujo debe seguir mediante poses/efectos/escape. Restaurar el clip original después.
15. Redimensionar Game View y revisar HUD, retratos, rueda y bandas del video. Escuchar emotes y video con master/SFX distintos desde AdventureAudio.

## Alcance y límites

- Comprobado en Windows Editor; no se generó ni probó un ejecutable distribuible, WebGL, móvil ni otros codecs/plataformas.
- Selección de rueda por teclado automatizada; clic y botón Continuar conectados e inspeccionados, pendientes de recorrido manual completo con ratón.
- Video ausente y skip ejecutados. La rama de error de codec/archivo tiene timeout y retorno, pero no se simuló cada fallo de decodificador.
- No se realizó perfilado prolongado de memoria. Los proyectiles tienen tiempo de vida y la RenderTexture se libera al terminar/omitir/destruir el reproductor.
- El cierre del 24 de octubre completó catálogo y segunda fila de emotes sin reconstruir las escenas: Unity tenía cambios de escena sin guardar. La suite completa anterior sigue siendo la evidencia de gameplay; la ampliación visual de los emotes se inspecciona separadamente.
- Música y video de inmersión no incluidos: slots preparados y secuencia de sprites operativa. Los estados acuáticos sin hojas específicas reutilizan poses. Ver ASSETS_V2.md.
