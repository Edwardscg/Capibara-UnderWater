# UnderwaterBoss — entrega universitaria

## Abrir y jugar

En Unity 6000.5.9f1, abrir `Assets/Scenes/UnderwaterBoss.unity`, pulsar **Play** y hacer clic dentro de **Game**. Usar preferentemente relación 16:9 y Scale 1x. La escena también es la primera de la lista de escenas de compilación.

| Control | Acción |
|---|---|
| WASD o flechas | Nadar y orientar el disparo, incluidas diagonales |
| Espacio, mantener o pulsar | Disparar un arpón cada 0,45 segundos |
| Sin dirección pulsada | Mantener la última dirección de disparo y descender lentamente |
| R, después del resultado | Reiniciar |
| Botón REINTENTAR | Reiniciar después de victoria o derrota |

## 1. Nivel construido

Una arena completamente sumergida de aproximadamente 25,6 × 12,4 unidades libres, cámara ortográfica fija que adapta su encuadre a la relación de aspecto, cuatro rocas sólidas, paredes, techo acuático y suelo con colisión. Vegetación, coral decorativo, una estructura sumergida, profundidad de color, rayos de luz y burbujas. El combate incluye salud, invulnerabilidad, arpones, aviso de embestida, recuperación, segunda fase, victoria, derrota y reinicio.

## 2. Archivos creados

- `Assets/Scenes/UnderwaterBoss.unity`: escena jugable serializada; no requiere ejecutar un generador durante Play.
- `Assets/Scenes/BeforeUnderwater_20261003_203909.unity`: copia de los cambios que estaban sin guardar en SampleScene al comenzar la construcción.
- Diez scripts de juego bajo `Assets/Scripts`, descritos abajo.
- `Assets/Editor/UnderwaterLevelBuilder.cs`: herramienta de construcción y ajustes del escenario; no sobrescribe una escena existente al crear.
- `Assets/Editor/UnderwaterPlayTests.cs`: auditoría y pruebas de integración en el motor real, disponibles desde el menú **Underwater**.
- Ocho prefabs, descritos abajo.
- Once Animation Clips y dos Animator Controllers en `Assets/Animations/Player` y `Assets/Animations/Boss`.
- `Assets/Art/Generated`: formas gráficas de los personajes provisionales, recortes de sprites del entorno, materiales y shader de vegetación. Los recortes referencian las texturas existentes y no alteran los PNG originales.
- Esta guía, inventario de archivos, resultados de Play, auditoría de referencias, capturas y comprobantes SHA-256 de los JPEG en `Documentation`.
- Unity creó los archivos `.meta` correspondientes.

## 3. Archivos existentes modificados

- `ProjectSettings/EditorBuildSettings.asset`: UnderwaterBoss añadida al principio, conservando SampleScene. Copia del archivo anterior en `Documentation/EditorBuildSettings.before.txt`.
- `ProjectSettings/TagManager.asset`: añadida la capa física **Arena** y las Sorting Layers **Background**, **EnvironmentBack**, **Characters**, **EnvironmentFront**, **Effects**, **UI**; conservadas las capas existentes.

No se cambió la versión de Unity, URP, New Input System ni la lista de paquetes. Los JPEG originales se conservaron byte por byte. No se hicieron commits ni push. SampleScene se conservó; sus cambios sin guardar también están en la copia BeforeUnderwater.

## 4. Prefabs

| Prefab | Propósito |
|---|---|
| `Prefabs/Player/Player.prefab` | Capibara con física, controles, salud, arma y animación |
| `Prefabs/Boss/CaimanBoss.prefab` | Caimán con física, salud, IA y animación |
| `Prefabs/Combat/PlayerProjectile.prefab` | Arpón con Collider2D Trigger y Rigidbody2D |
| `Prefabs/Combat/ProjectileImpact.prefab` | Partículas de impacto |
| `Prefabs/Environment/Rock1.prefab` a `Rock4.prefab` | Obstáculos con PolygonCollider2D |

Las rutas de esta tabla están dentro de `Assets`. Al colocar el prefab del caimán en otra escena hay que asignar su Target; la instancia de UnderwaterBoss ya lo tiene asignado.

## 5. GameObjects importantes

- **Main Camera**: cámara ortográfica y adaptación del encuadre.
- **Arena · arrecife sumergido**: agua, decoración, paredes, límites, suelo y rocas.
- **Player / Visual / Pose**: personaje y elementos gráficos; **Player / FirePoint**: salida del proyectil.
- **CaimanBoss / Visual / Pose**: jefe y elementos gráficos.
- **GameManager**: coordina el final y el reinicio.
- **Canvas**: vida del jugador, nombre CAIMÁN, vida/estado del jefe, controles y panel Resultado.
- **EventSystem**: InputSystemUIInputModule para el botón de reintento.

## 6. Scripts de juego

| Script | Responsabilidad |
|---|---|
| `Player/PlayerController.cs` | Lee New Input System en Update; aplica aceleración y límite de velocidad en FixedUpdate |
| `Player/PlayerHealth.cs` | 5 HP, TakeDamage, 0,7 s de invulnerabilidad y muerte |
| `Player/PlayerAttack.cs` | Dirección, FirePoint, creación del arpón y cooldown |
| `Combat/PlayerProjectile.cs` | Movimiento, impacto contra escenario o jefe, daño exclusivo al jefe y caducidad |
| `Boss/BossController.cs` | Máquina de estados, rodeo local de obstáculos y daño de embestida |
| `Boss/BossHealth.cs` | 12 HP, TakeDamage, umbral del 50 % y muerte |
| `Game/ActorVisual.cs` | Orientación, clips, flash de daño, aviso de ataque y muerte |
| `Game/GameManager.cs` | Estado de partida, resultado, interrupción del combate y recarga de escena |
| `Game/ArenaCamera.cs` | Mantiene la arena dentro del encuadre |
| `UI/HealthBar.cs` | Actualiza barra y valor numérico de salud |

Las rutas de la tabla son relativas a `Assets/Scripts`. Los valores de ajuste están expuestos mediante SerializeField. No se buscan objetos continuamente dentro de Update. Los barridos de objetos sólo se utilizan al finalizar la partida y en las herramientas de prueba.

## 7. Player Rigidbody2D

Dynamic; masa 1; Gravity Scale **0,16**; Linear Damping **3**; Freeze Rotation Z; Interpolate; detección Continuous. CapsuleCollider2D horizontal de **1,35 × 0,95**. Aceleración **18** y velocidad objetivo máxima **4,2** unidades/s. La resistencia reduce progresivamente el movimiento y la gravedad produce un descenso suave al soltar las teclas. La prueba midió aproximadamente **0,56 unidades en 1,4 segundos**, partiendo del reposo.

## 8. Boss Rigidbody2D

Dynamic; masa **3**; Gravity Scale **0**; Freeze Rotation Z; Interpolate; detección Continuous. CapsuleCollider2D horizontal de **2,7 × 0,85**, offset X **0,22**. Su velocidad la controla la IA en FixedUpdate. Su silueta tiene aproximadamente el doble de anchura que la del capibara.

## 9–11. Combate, IA y segunda fase

**CHASE:** persigue en dos dimensiones a 1,65 unidades/s. CircleCast evita en lo posible los obstáculos inmediatos. Al acercarse a 6 unidades, o al transcurrir 4 segundos de persecución, prepara el ataque. Este rodeo es local, no un sistema de navegación completo.

**TELEGRAPH:** se detiene, cambia de color y la interfaz indica que hay que esquivar. Dura 0,9 s en fase 1.

**CHARGE:** toma la dirección del jugador una sola vez al empezar. Se mueve a 9 unidades/s durante un máximo de 0,65 s, sin volver a apuntar. Un impacto contra Arena interrumpe la embestida y pasa a recuperación. El contacto con Player causa 1 HP como máximo una vez por embestida, además de respetar la invulnerabilidad del jugador. Fuera de Charge no produce daño de contacto.

**RECOVERY:** permanece detenido 1,3 s; la interfaz invita a atacar. Después vuelve a Chase.

**Fase 2:** comienza con 6 HP o menos. La persecución sube a 2,25 unidades/s y el aviso baja a 0,65 s. No hay doble embestida: se conserva una dificultad moderada. La velocidad de embestida y la recuperación no cambian.

**Arpón:** 11 unidades/s; 1 HP; 2,5 s de vida; cooldown 0,45 s. Colisiona con el entorno y sólo hace daño al caimán. No daña al jugador ni a la decoración.

**Final:** al llegar cualquiera a cero se detienen movimiento, IA y ataques, se retiran los proyectiles y aparece el resultado con REINTENTAR. No se congela Time.timeScale, de modo que la animación de muerte y la UI siguen funcionando.

## 12–13. Recursos gráficos y animaciones

Usados del paquete existente: Day/Middle.png para suelo y vegetación; Day/Tile.png para paredes; Day/props/dome-1.png para una estructura de fondo. Se inspeccionaron las variantes Day, Noon y Night, los previews, dimensiones y los recursos disponibles. Se conservaron los demás recursos y documentos del paquete. Nubes y superficie no se colocaron porque el combate debe estar completamente sumergido.

El material **OceanVegetation** oculta al renderizar el azul uniforme de la capa Middle entre las plantas. La textura original permanece intacta. Este shader está pensado para esa textura concreta.

Los JPEG de capibara y caimán son de **2816 × 1536**. Contienen rótulos, un damero gris incrustado, tamaños desiguales y distribución irregular. En el capibara se distinguen grupos Idle/Floating, Swim, Swim Up, Swim Down, Attack, Hurt y Death; en el caimán, Idle, Swim, Chase, Telegraph, Charge, Hurt y Death. No son hojas en una cuadrícula uniforme ni tienen transparencia real.

**No se utilizaron los JPEG como sprites del juego ni se generaron PNG supuestamente transparentes a partir de ellos.** Se conservaron como referencias. Para evitar bordes defectuosos se crearon placeholders reconocibles con formas de Unity: capibara con pañuelo, mochila y arpón, y caimán verde con cola, dientes y escamas.

Los clips **Player_Idle, Player_Swim, Player_Attack, Player_Hurt, Player_Death** y **Caiman_Idle, Caiman_Swim, Caiman_Telegraph, Caiman_Charge, Caiman_Hurt, Caiman_Death** animan transformaciones de los placeholders. No son frames extraídos de los JPEG. No se inventaron frames atribuidos a las hojas originales. Los Animator Controllers usan estados simples activados por código, sin una red compleja de transiciones. El nado arriba/abajo usa el mismo personaje y clip de nado.

## 14–15. Problemas, verificación y pendientes

- Se corrigió una API de inspección obsoleta al adaptar las pruebas a Unity 6.5; también se actualizaron las búsquedas de objetos a sus API vigentes en esta versión.
- Se corrigió el recorte de paredes del tileset y se ajustó la vegetación para integrarla con el agua.
- El editor ya mostraba un aviso de caché Burst bloqueada por la política de seguridad de Windows y mensajes previos de servicios/licencia. No se alteró ninguna configuración de seguridad. Estos avisos no impidieron las pruebas del nivel.
- La auditoría comprueba Missing Scripts y referencias serializadas rotas. El resultado está en `SceneAudit.txt`.
- `PlayTestResults.txt` documenta las pruebas reales del motor, con teclado virtual del New Input System, física, proyectiles, daño, estados y recarga de escena. La comprobación de victoria usa disparos físicos; la de derrota aplica daño repetido respetando la invulnerabilidad. El impacto real de una embestida se comprueba por separado.
- Resultado final: **42 comprobaciones aprobadas**, **174 GameObjects auditados**, **0 referencias rotas**. La consola de Unity al finalizar mostró **0 errores y 0 advertencias**. Se verificaron visualmente las capturas de juego y victoria; la escena queda abierta fuera de Play.
- Los recursos artísticos definitivos del capibara y caimán quedan como mejora visual opcional. El flujo de juego funciona con los placeholders.
- No se ha generado ni probado un ejecutable independiente. La entrega solicitada está preparada y comprobada para Play en el editor local.

## 16. Prueba manual paso a paso

1. Abrir UnderwaterBoss. Seleccionar la pestaña Game, preferentemente 16:9 y Scale 1x, y pulsar Play.
2. Hacer clic en Game. Confirmar capibara, caimán, cuatro obstáculos, barras 5/5 y 12/12 y ayuda inferior.
3. Pulsar A, D, W y S por separado; comprobar las cuatro direcciones y repetir con las flechas.
4. Nadar en diagonal. Confirmar que la velocidad no se dispara y que el personaje mira al lado del movimiento horizontal.
5. Soltar todos los controles lejos del suelo: debe frenar y descender lentamente.
6. Intentar cruzar izquierda, derecha, techo y fondo. Ningún límite debe permitir salir.
7. Intentar atravesar cada roca. Debe bloquear el movimiento; algas y coral decorativo no deben hacerlo.
8. Mirar al caimán y pulsar Espacio. Debe salir un arpón en la última dirección pulsada. Mantener Espacio para comprobar el cooldown.
9. Disparar a una roca; el proyectil debe desaparecer. Acertar al jefe y comprobar que pierde 1 HP y destella.
10. Acercarse al jefe: observar persecución, pausa con aviso rojo, embestida recta y descanso.
11. Durante el aviso, preparar un desplazamiento perpendicular a la embestida; al comenzar Charge, apartarse. El jefe no debe doblar para seguir al jugador.
12. Dejarse alcanzar por Charge. El jugador pierde 1 HP y destella. Permanecer en contacto no debe vaciar toda la barra.
13. Reducir al jefe a 6 HP. Confirmar FASE 2, mayor velocidad de persecución y aviso algo más corto.
14. Aprovechar los descansos para acertar hasta 0 HP. Deben aparecer ¡VICTORIA! y REINTENTAR; movimiento y disparos quedan desactivados.
15. Hacer clic en REINTENTAR. Se reinician posición, vida y estado de ambos.
16. En otra partida, dejarse alcanzar varias veces hasta llegar a cero: debe aparecer DERROTA y detenerse el combate.
17. Pulsar R: debe comenzar otra partida con 5/5 y 12/12.
18. Salir de Play y revisar Console: no deben aparecer errores nuevos de los scripts del nivel.

Para repetir las pruebas automáticas: detener Play y usar **Underwater → Probar nivel automáticamente**. Tardan aproximadamente medio minuto, modifican sólo el estado temporal de Play y dejan la escena fuera de Play al terminar. No interactuar con los controles durante esa prueba.
