# Capibara vs. Caimán — V2

Aventura 2D en dos mapas conectados: selva peruana y lago. El caimán es el guardián territorial; los residuos del escenario anticipan una amenaza humana futura.

## Ejecutar

1. Abrir este proyecto con **Unity 6000.5.9f1**. Se conservaron URP 2D 17.6.0 y New Input System 1.20.0.
2. Abrir `Assets/Scenes/AmazoniaTerrestre.unity` y pulsar Play.
3. Explorar hacia la derecha, vencer la fase terrestre, seguir al caimán hasta la orilla y pulsar E.
4. Vencer la fase acuática para llegar al diálogo final y la victoria.

También puede abrirse `AmazoniaAcuatica.unity` para probar el agua directamente. `UnderwaterBoss` es la escena histórica, no el inicio de V2.

## Controles

| Acción | Control |
|---|---|
| Caminar | A/D o flechas |
| Nadar | WASD o flechas |
| Saltar en tierra | Espacio |
| Atacar | J o clic izquierdo |
| Espada/plasma en tierra | Q |
| Dash terrestre | Shift izquierdo |
| Entrar al lago desbloqueado | E |
| Rueda de emotes fuera de combate | T |
| Seleccionar emote | Izquierda/derecha, 1/2/3 y Enter, o clic |
| Continuar diálogo | Enter, clic o botón Continuar |
| Omitir cinemática / cerrar rueda / pausa | Escape según contexto |
| Reintentar después de perder/ganar | R o botón |

Plasma: **3 disparos, recarga automática de 6 segundos jugables**. Pausa y cinemáticas congelan el reloj. Cambiar de arma conserva la recarga. Pasar al agua conserva vida, munición y tiempo restante. Reintentar en agua repone 8 HP y 3 disparos y omite la presentación, sin repetir tierra.

## Organización

- `Assets/Scripts/Amazonia`: movimiento, combate, IA, narrativa, munición, emotes, audio y cámara.
- `Assets/Art/V2/Sprites`: copia íntegra de los 42 archivos entregados. Originales conservados.
- `Assets/Animations/V2`: clips y recortes como assets de Unity; no se repintaron las imágenes.
- `Assets/Prefabs/Amazonia`: jugadores, caimanes, proyectiles y balance.
- `Assets/Editor/Amazonia`: integración de recursos y pruebas en Play.
- `docs/ASSETS_V2.md`, `docs/CINEMATICAS_V2.md`, `docs/PRUEBAS_V2.md`: inventario, configuración y verificación.
- `Documentation/V2`: prompt, hashes, recortes, resultados y capturas reales.

Para editar diálogos/videos/audio, seleccionar **Aventura · progreso y transición** en cada escena. Para balance físico/de combate, editar `Assets/Prefabs/Amazonia/Balance.asset`.

## Git

`v1` conserva el juego previo completo en `b3fe937`. `v2` contiene esta continuación y sus commits locales. No se hizo push ni se alteró el remoto.

Cerrar Unity antes de alternar versiones y comprobar que no haya trabajo local pendiente:

```powershell
git status --short
git switch v1
# Para volver:
git switch v2
```

No usar reset --hard para cambiar de versión. No ejecutar el constructor de V1 sobre V2: recrearía escenas antiguas. La opción `Amazonia > V2 > Integrar recursos y secuencias` es una herramienta de autoría, no un paso necesario para jugar.

## Material pendiente

No se entregó video de inmersión: funciona la alternativa con sprites. El MP4 de escape sí está integrado por la instrucción directa posterior del usuario, que sustituye la exclusión del documento V2. Puede reemplazarse desde Inspector. Faltan música y animaciones acuáticas independientes de daño/furia/muerte; hay slots musicales y respuestas visuales funcionales reutilizadas. Ver inventario para los fondos opacos de dos hojas.
