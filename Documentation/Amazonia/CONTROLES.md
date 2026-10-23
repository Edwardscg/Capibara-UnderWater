# Controles y ejecución
Abrir `Assets/Scenes/AmazoniaTerrestre.unity` y pulsar Play. Está primera en Build Settings; AmazoniaAcuatica es la segunda. UnderwaterBoss se conserva como entrega anterior independiente.

| Acción | Tecla |
|---|---|
| Caminar | A/D o flechas izquierda/derecha |
| Saltar (tierra) | Espacio |
| Atacar con arma equipada | J mantenida o clic izquierdo mantenido |
| Cambiar espada/plasma (tierra) | Q |
| Dash físico en dirección pulsada, o hacia donde mira | Shift izquierdo |
| Entrar al lago después de la huida, cerca de la orilla | E |
| Nadar (agua) | WASD o cuatro flechas; diagonales permitidas |
| Apuntar plasma (agua) | Última dirección de natación |
| Pausar/continuar | Escape |
| Reiniciar aventura | Botón REINTENTAR; R después de victoria/derrota |

Tierra: espada 3 de daño, plasma 1. Espada golpea una vez por acción, con activación retrasada 0.14 s. No hay combo de tres ataques: la secuencia seleccionada representa un solo corte.
Agua: solamente plasma, sin dash terrestre. Cambiar de altura permite evitar la trayectoria fijada de las embestidas.
Reintentar comienza una aventura nueva en tierra con 8 HP. La transición al lago conserva la vida, cambia a plasma y reinicia la salud del jefe acuático. Abrir directamente la escena acuática permite practicar con vida completa. La victoria guarda `AmazoniaCompleted` en PlayerPrefs.

El balance se modifica en `Assets/Prefabs/Amazonia/Balance.asset`: movimiento, salto, gravedad, dash, salud, daño, cadencia, persecución, preparación y recuperación.
