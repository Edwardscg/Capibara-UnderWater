# Arquitectura y combate
La aventura añade el namespace Amazonia y dos escenas. La arena anterior, sus scripts, assets y prefabs permanecen disponibles. No se cambiaron motor, pipeline ni paquetes.

- AdventureBalance: parámetros compartidos en ScriptableObject.
- AdventurePlayer: input en Update, movimiento en FixedUpdate, estados de acción, salto con buffer/coyote, dash físico, armas y hitbox de espada.
- CombatHealth: vida, invulnerabilidad, muerte y respuesta al impacto.
- AdventureProjectile: plasma/piedras; colisión por trigger y barrido para prevenir atravesamientos; daño dirigido según dueño; tiempo de vida.
- AdventureBoss: estados terrestres y acuáticos, decisiones por distancia/altura, escape. Rigidbody dinámico con Z bloqueada; tierra gravedad 2.8, agua 0, masa 3 y colisión continua.
- AdventureVisual: Animator, volteo de sprites, flash y aviso de ataque. Clips de fotogramas reales.
- AdventureCamera: seguimiento amortiguado, encuadre compartido al acercarse al jefe y límites del mapa.
- AdventureFlow: UI, pausa, derrota, victoria, conservación de vida y transición mediante fundido.
- AmazoniaBuilder (Editor): genera recursos serializados, prefabs, clips y escenas. No se necesita en una build.
- AmazoniaTests (Editor): integración con física real, teclado virtual, capturas y auditoría de referencias.

Player: Rigidbody2D dinámico, masa 1, interpolación y detección continua, Z bloqueada. CapsuleCollider 1.2 x 1.55. Tierra: gravedad 2.8 y aceleración horizontal 40; velocidad 6, salto 12.5. Agua: gravedad .12, resistencia 3, aceleración 24, velocidad máxima 5.3. HP 8, invulnerabilidad .7 s. No hay invulnerabilidad especial de dash.

Tierra: el jefe espera y detecta al jugador a 17 unidades. Persigue, prepara un ataque visible y elige piedra si el jugador está lejos o alto. Cerca alterna arañazo y empujón. Cada ataque tiene recuperación. La espada recompensa la proximidad; las plataformas permiten esquivar. Al agotarse sus 18 HP se aturde, corre hasta la orilla, se sumerge y habilita E. No causa daño durante la huida.

Agua: 22 HP. Detecta, persigue en dos ejes, busca espacio, prepara y fija la dirección al terminar el aviso. Embiste durante .7 s sin dirigir la trayectoria hacia un objetivo móvil. Se recupera 1.15 s. A mitad de vida, persecución +.7, embestida +1, preparación al 78% y recuperación al 82%. Sigue existiendo una ventana clara de evasión. La derrota desactiva daño, reproduce los fotogramas de caída y hunde al jefe antes de mostrar victoria.

Cada mapa mide 66 unidades de ancho; la cámara muestra solo una parte. Los colliders son independientes de las ilustraciones. Las plataformas de madera permiten pasar por debajo al saltar; raíces/rocas acuáticas usan polígonos. La contaminación narrativa ya está representada discretamente por barriles, vertidos y maquinaria en los fondos suministrados.
