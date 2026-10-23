PROMPT MAESTRO V2 — CONTINUACIÓN DEL VIDEOJUEGO CAPIBARA VS. CAIMÁN
0. Rol y mandato
Actúa como director técnico de videojuegos 2D, programador senior, technical artist, animador de spritesheets, diseñador de combate de jefes y responsable de QA. Trabaja sobre el videojuego que ya existe, desarrollado anteriormente mediante un Prompt Maestro V1. Tu trabajo es inspeccionar, conservar lo que funciona, mejorar lo existente e integrar los nuevos assets entregados en Sprites.zip. No construyas otro proyecto desde cero ni reescribas sistemas sanos por comodidad. Adáptate al motor/framework y lenguaje detectados en el repositorio; no presupongas Unity, Godot, Phaser o Pygame.

Objetivo: dejar un nivel 2D de la Amazonía peruana completamente jugable, con combate contra un caimán territorial en dos fases (tierra y agua), cinematografías narrativas, diálogos, emotes sonoros, HUD del plasma 3/3 y recarga de 6 segundos, animaciones correctas, parallax, partículas, música y cámara dinámica. La primera fase culmina con la retirada del caimán al lago; la segunda, con su derrota definitiva. Usa las ideas y mecánicas funcionales de V1 como base.

Los archivos entregados son recursos, no garantía de que todas las animaciones estén listas para importar. Audita, recorta, normaliza e integra responsablemente. Jamás declares como probada una funcionalidad que no ejecutaste.
1. Gestión Git: conservar V1 y trabajar en V2
Debes mantener un repositorio Git local y proteger la versión actual del juego:
1. Identifica el directorio raíz correcto del proyecto y verifica si ya existe .git. Si existe, no ejecutes git init de nuevo ni reemplaces el historial. Si no existe, inicializa Git localmente, configura .gitignore apropiado y crea el primer commit seguro.
2. Antes de editar, inspecciona rama actual, git status, commits, remotos (si hay) y archivos modificados/no rastreados. Conserva los cambios existentes del usuario: no limpies, resetees ni sobrescribas trabajo. Resguarda los cambios sin confirmar de manera segura antes de cambiar de rama; nunca los descartes. Revisa y excluye contraseñas, .env, claves y artefactos generados antes de agregar archivos al índice.
3. Crea una rama v1 que preserve un snapshot verificable del proyecto tal como estaba al comienzo de esta tarea, incluyendo trabajo previo que deba conservarse. Si v1 ya existe, inspecciona su contenido y evita sobrescribirla o hacer force. Explica cualquier conflicto y el procedimiento no destructivo adoptado.
4. Partiendo de ese snapshot, crea o continúa la rama v2, donde harás TODOS los cambios de esta nueva iteración. Deja v1 intacta y sin nuevos cambios de V2.
5. Trabaja en commits locales con mensajes claros por etapas. No hagas push, no borres ramas, no fuerces commits, no reescribas historial y no toques remotos salvo autorización explícita.
6. Al finalizar, reporta git status, rama activa, commits relevantes y comandos para alternar entre v1 y v2.
Si solo recibes el ZIP de assets y no el código del videojuego, no inventes que integraste el juego: identifica el bloqueo y conserva un plan de integración claro. La implementación solo será real cuando dispongas del proyecto V1.
2. Diagnóstico previo y plan de trabajo
Examina estructura de código, escenas/páginas, física, motor, scripts, player controller, boss AI, colisiones, cámaras, HUD, sistema de audio, cargador de escenas, menús, checkpoints, pruebas y dependencias. Identifica qué mecánicas del primer prompt ya funcionan (movimiento lateral, salto/plataformas, dash terrestre, espada de luz, cañón de plasma, cambio de arma, ataques terrestres, transición entre fases, movimiento acuático y embestida). Produce una matriz EXISTE / DEFECTUOSO / AUSENTE / A MEJORAR, con evidencia. Da un plan en pasos verificables y comienza la implementación en v2.

Adecúa las decisiones técnicas al proyecto existente. No uses pseudocódigo como entrega final cuando puedas implementar el código ejecutable. No confundas haber compilado con haber probado la jugabilidad.
3. Inventario REAL del ZIP proporcionado
Recibirás Sprites.zip. Contiene 38 PNG, 3 WAV y 1 MP4. El video Caiman_Cinematica.mp4 suministrado fuera del ZIP es idéntico al MP4 del ZIP.

Usa como fuente de verdad los archivos reales; sus rutas dentro del ZIP son:

Caimán acuático (Sprites/Caiman/Acuatico/):
- Caiman_Acuatico_Aparicion.png
- Caiman_Acuatico_Movimiento_Diagonal_Inferior.png
- Caiman_Acuatico_Movimiento_Diagonal_Superior.png
- Caiman_Acuatico_Movimiento_Preparar_Ataque_Rapido.png
- Caiman_Acuatico_Movimiento_v1.png
- Caiman_Acuatico_Movimiento_v2.png
- Caiman_Acuatico_Movimiento_v3.png
- Caiman_Movimiento_Ataque_Rapido_v1.png
- Caiman_Movimiento_Ataque_Rapido_v2.png
Caimán terrestre (Sprites/Caiman/Terrestre/):
- Caiman_Aparicion.png
- Caiman_Ataque_Cerca.png
- Caiman_Ataque_Distancia.png
- Caiman_Derrotado_Escape.png
- Caiman_Escape.png
- Caiman_Movimientos.png
Capibara acuático (Sprites/Capybara/Acuatico/):
- CapybaraAcuatico.png
- Capybara_Celebracion_Victoria_Terrestre_Acuatico.png
Capibara terrestre (Sprites/Capybara/Terrestre/):
- CapybaraTerrestre.png
- Capybara_Sumerge.png
- Emote_SixSeven.png
- Emote_Uiia.png
- Emote_Wazaaaa.png
- Recarga_Y_Acabado_Plasma.png
Fondos (Sprites/Fondos/): FondoAcuatico.png, FondoTerrestre.png.

Parallax acuático (Sprites/Parallax/Acuatico/): Parallax_Algas_v1.png, Parallax_Algas_v2.png, Parallax_Componentes_Acuaticopng.png, Parallax_Raices_Bajo_Agua.png.

Parallax terrestre (Sprites/Parallax/Terrestre/): Parallax_Arboles_Lianas.png, Parallax_Componentes_Bosque.png, Parallax_Fondo_Acuatico_Estructuras.png, Parallax_Fondo_Bosque_v1.png, Parallax_Fondo_Bosque_v2.png, Parallax_Lianas.png.

Otros: Sprites/Efectos.png, Sprites/Recarga_Arma.png, Sprites/Retratos_Dialogos.png.

Audios (Sprites/Sonidos/): emote_six_seven.wav (aprox. 11,24 s), emote_uiia.wav (aprox. 8,45 s), emote_wazaaaa.wav (aprox. 2,06 s).

Video (Sprites/Videos/): Caiman_Cinematica.mp4 (13,77 s, vertical 1152 × 2048, H.264/AAC).

Atención al video: al inspeccionar el material aportado, contiene el meme original con un niño real frente a un caimán, no la cinematográfica transformada con el capibara. Por tanto, NO lo insertes como cinemática terminada en el videojuego ni lo distribuyas como recurso jugable. Consérvalo solo como referencia externa de coreografía, si corresponde. Crea el reproductor/inserción para un futuro archivo final animado, cuya ruta sea configurable; cuando falte, ejecuta una cinemática equivalente con sprites sin congelar el juego. Tampoco hay un video final independiente de la inmersión en el ZIP: aplica igual tratamiento. No finjas que los videos definitivos ya existen.

Verifica que Caiman_Acuatico_Movimiento_v1.png y Caiman_Acuatico_Movimiento_v3.png son idénticos bit a bit. Elige solo la secuencia útil; no dupliques animaciones. Algunos spritesheets tienen diseños complejos, dimensiones dispares y rótulos decorativos; elimina rótulos del juego y procesa los fotogramas sin inferir cuadrículas regulares cuando no existan.
4. Preparación gráfica y animación
- Extrae el ZIP sin sobrescribir archivos ajenos, clasifica assets y crea una estructura de recursos compatible con la arquitectura del proyecto.
- Para cada hoja, identifica por inspección sus animaciones verdaderas. Recorta cuadros, elimina fondos/rótulos, corrige transparencias y bordes, normaliza pivotes y escalas, genera atlas y metadatos (rectángulos, orden, frames por segundo, eventos de ataque) compatibles con el motor.
- No distorsiones proporciones ni cambies los diseños. El caimán terrestre debe estar erguido sobre DOS patas y tener gestos arrogantes; el acuático debe nadar horizontalmente con cola propulsora. Conserva paño rojo, mochila verde y cañón del capibara.
- El espejo horizontal puede usarse para orientar sprites si no invierte detalles importantes ni posiciones de armas de forma incorrecta.
- Separa los efectos visuales de los hitboxes y de la posición del personaje, siempre que sea viable.
- Las hojas acuáticas incluyen aparición, desplazamiento, diagonales, preparación y ataques rápidos. No se identifican por nombre hojas independientes de daño, furia ni derrota acuática: inspecciona si existen cuadros adecuados en hojas disponibles. Si no existen, implementa una reacción funcional y digna mediante poses reutilizadas, flash/hit tint, partículas y transiciones de movimiento; documenta claramente la limitación visual, sin describirla como una animación original verificada.
- Asegura compatibilidad de velocidad de animación con delta time, sin vibraciones por pivotes desalineados ni cambios de tamaño aparentes.
5. Identidad narrativa — «El guardián de las aguas»
El nivel sucede en la Amazonía peruana. La selva está viva y hermosa, aunque pequeños detalles revelan que la minería ilegal está contaminando el lago. Este es un presagio de un antagonista humano que aparecerá en niveles futuros; NO implementes al humano como jefe ni lo muestres combatiendo en esta tarea.

El capibara es un viajero curioso que investiga qué está deteriorando los ríos. El caimán es un guardián territorial y orgulloso, de carácter irritable, burlón y desconfiado. Ha visto señales de maquinaria, lodo extraño y residuos; teme que nuevos intrusos destruyan su hogar. Confunde inicialmente al capibara con otra amenaza, no porque sea simplemente “malvado”. En tierra lo menosprecia, se enfurece al ser derrotado y se repliega al lago, donde se sabe superior. En agua se vuelve más calculador, sigiloso y feroz. Su derrota no elimina el misterio: la evidencia ambiental apunta a un responsable humano.

Narrativa breve, con humor físico y momentos de tensión. Utiliza tarjetas de diálogo con retratos obtenidos de Retratos_Dialogos.png, nombres visibles, revelado opcional letra a letra, botón continuar, avance/omisión y bloqueo seguro de controles mientras corresponde. Texto siempre renderizado por el motor, nunca incrustado en el PNG.

Propuesta de diálogos editables (puedes pulir sin cambiar su sentido):

Introducción:
- Narrador: «El agua estaba más turbia de lo normal. La selva parecía contener la respiración».
- Caimán: «¡Alto ahí! Nadie cruza mi orilla sin permiso».
- Capibara: «Solo busco saber quién está ensuciando el lago».
- Caimán: «Eso dicen todos los que llegan. ¡Fuera de mi territorio!».
Al acabar la fase terrestre:
- Caimán, herido: «No cantes victoria... aquí abajo mando yo».
- Capibara, con una celebración corta: «¿Eso cuenta como ganar?».
- La secuencia de humor con piedras ocurre como cinemática/video opcional de reemplazo, no como ataque regular de gameplay.
Junto al lago:
- Capibara: «No puedo dejarlo escapar... y todavía necesito respuestas».
Revelación submarina:
- Narrador: «Bajo las raíces, el lago ocultaba cicatrices que la superficie no mostraba».
- Caimán: «Entraste a mis aguas. Aquí tus saltos no servirán».
- Capibara: «Entonces tendré que aprender a nadar más rápido».
Victoria final:
- Narrador: «Entre los sedimentos yacían restos de metal y marcas de maquinaria. El verdadero peligro no tenía escamas».
- Capibara: «Esto no terminó aquí... alguien está dañando la selva».
Los diálogos son cortos, contextualizados y no deben atascar el ritmo del combate. Evita apropiarte sin justificación de lenguas indígenas o presentar la minería como culpa de la fauna.
6. Cinemática 1 — Presentación del jefe en tierra
Al aproximarse el capibara al área de combate por primera vez:
1. Activa trigger una sola vez; bloquea temporalmente entradas de ataque/movimiento y fija estado cinematográfico.
2. La cámara deja de seguirlo, se desplaza suavemente hacia vegetación de la selva; usa Caiman_Aparicion.png.
3. Se ven ojos entre árboles, el caimán sale de la vegetación, se yergue en DOS patas, sonríe arrogantemente y ruge.
4. Aparecen diálogos anteriores mediante tarjetas de retratos.
5. Presenta nombre «CAIMÁN — GUARDIÁN DEL LAGO» y barra de jefe (fase terrestre).
6. Restituye cámara y control; comienza IA terrestre.
Conservar audio, partículas, profundidad y timing. La cinemática debe poder saltarse sin provocar estados bloqueados y no debe repetirse al reanudar desde checkpoint.
7. Cinemática 2 — Derrota terrestre, VIDEO intermedio y escape
Orden obligatorio solicitado por el usuario:
1. La vida de fase 1 llega a cero: cesan ataques e impactos, el caimán queda herido, retrocede y mira al capibara con enfado (Caiman_Derrotado_Escape.png).
2. El capibara ejecuta una celebración breve y contextual, sin cortar la mirada del caimán. Se muestran uno o dos diálogos cortos.
3. AQUÍ VA UN VIDEO CINEMATOGRÁFICO EXTERNO cuando exista el archivo final con capibara y caimán animados. El video recrea el gag: el capibara tira una piedra pequeña al caimán junto al agua; el caimán responde, emerge, lanza una piedra mayor y empieza a retirarse. No debe reemplazarse por la versión original que muestra un menor real.
4. Al terminar u omitirse el video, la cámara vuelve al juego sin saltos graves; el caimán huye mediante Caiman_Escape.png, corriendo hacia el lago. La cámara acompaña la huida.
5. El caimán salta y desaparece bajo la superficie. Habilita la interacción de la orilla y checkpoint de progresión.
Crea un módulo de cinematic video reusable, con escape_video_path configurable, precarga, reproducción a relación correcta, letterbox/crop deliberado (nunca deformar), sincronización audio, pausa/recuperación de gameplay, omisión, errores de codec/archivo y fallback a animación con sprites. El MP4 real proporcionado es solo referencia, NO el video final del juego. El fallback debe mantener el hilo narrativo incluso sin clip externo. No hagas que el caimán desaparezca inmediatamente al terminar la primera fase.
8. Cinemática 3 — Entrada al lago, VIDEO intermedio e inmersión
Después de la huida y solo entonces, muestra una señal «E — Sumergirse» en la orilla derecha. Al pulsar E:
1. Detén el control del jugador; la cámara encuadra el borde del lago. Usa Capybara_Sumerge.png para la preparación (capibara contempla el agua, ajusta mochila, se dispone a saltar).
2. AQUÍ REPRODUCE EL SEGUNDO VIDEO EXTERNO cuando se aporte una versión cinematográfica definitiva de la preparación/inmersión (immersion_video_path configurable).
3. Cuando termina u omite el video, continúa dentro del motor con el salto del capibara desde la orilla, salpicaduras, paso por la superficie, burbujas, cambio gradual de iluminación a verdes/azules.
4. Carga el mapa acuático sin pantalla de carga tradicional visible; usa transición fluida enmascarada por fundido/agua/cámara, precarga cuando sea compatible. Cambia a sprites y controlador acuáticos y ubica al jugador en un spawn seguro.
5. Nunca permitas cambiar de mapa varias veces ni repetir la secuencia por mantener E presionada.
A falta del video definitivo —no se entregó uno en el ZIP— reproduce la secuencia por completo con sprites y prepara la inserción futura sin bloqueo.
9. Cinemática 4 — Aparición del caimán acuático
En el mapa acuático:
1. Cámara muestra durante unos segundos agua aparentemente vacía con rayos de luz, partículas y raíces.
2. Dos ojos amarillos brillan desde la vegetación; utiliza las partes apropiadas de Caiman_Acuatico_Aparicion.png.
3. Se revela la cabeza y luego el cuerpo horizontal; burbujas, movimiento de cola y salida de las sombras.
4. Aparecen breves diálogos; título «CAIMÁN — FASE II: AGUAS PROFUNDAS» y barra de salud.
5. Devuelve control e inicia IA acuática.
Esta introducción ocurre una sola vez por entrada normal; en reintentos desde el checkpoint subacuático puede utilizarse una versión abreviada u omitirse según el sistema actual.
10. Emotes sonoros con rueda radial (T)
Implementa rueda real con tres emotes y etiquetas/icónicos dinámicos; usa los sprites y WAV existentes:
- Wasaaaa: Emote_Wazaaaa.png + emote_wazaaaa.wav.
- Uiia: Emote_Uiia.png + emote_uiia.wav (giro cíclico).
- Six Seven: Emote_SixSeven.png + emote_six_seven.wav (gesto alternado).
Controles: T abre/cierra rueda; selección por ratón o teclas direccionales y confirmación apropiada, Escape cierra sin activar. Fuera de combate pueden reproducirse libremente. Son cosméticos: no dañan, no disparan y no consumen munición. Se cancelan al recibir daño y, preferentemente, al mover/atacar o comenzar cinemática; nunca bloquean al jugador indefinidamente. No ejecutarlos durante IA del jefe en combate activo, cutscenes o derrota. Sin reproducción sonora superpuesta: al cambiar/cancelar, detener el WAV anterior. Ajustar timing a longitud real (Wasaaaa ~2.06 s, Uiia ~8.45 s, Six Seven ~11.24 s): recortar silencios si procede, sincronizar los bucles visuales y permitir cancelación. Respetar volumen maestro/SFX. Las dos celebraciones automáticas de primera fase y victoria final son animaciones contextuales distintas de los emotes, no deben activar la rueda.
11. Plasma: exactamente tres disparos y recarga de seis segundos
Una de las mecánicas centrales de V2. Usa Recarga_Arma.png, Recarga_Y_Acabado_Plasma.png y elementos del HUD:
- Capacidad maxAmmo = 3.
- Cada disparo válido resta 1, no uno por frame; con ammo 0 no se crea proyectil ni se reproduce falsamente un disparo.
- Al efectuar el tercer disparo comienza automáticamente una recarga de exactamente 6,0 segundos de tiempo jugable (configurable desde un solo lugar). No debe reiniciarse cada frame ni al pulsar disparar repetidamente.
- Mientras recarga, no hay disparos; el cañón muestra efecto de recarga coherente.
- HUD siempre indica las cápsulas activas (3, 2, 1, 0), un 3/3 numérico y al recargar muestra una barra progresiva continua 0–100%, texto «RECARGANDO» y, si encaja, segundos restantes.
- Al finalizar, rellenar automáticamente a 3/3, señal visual/sonora corta y habilitar disparo.
- El estado y cronómetro de recarga no se reinician al cambiar temporalmente a espada. Cambiar arma no permite recargar inmediatamente ni saltarse el cooldown.
- En tierra permite alternar espada/plasma; bajo el agua solo plasma (espada deshabilitada). Por coherencia usa como base 3 disparos/6 s también en el agua, ajustable mediante configuración del motor si el balance lo exige sin violar el límite por defecto.
- El temporizador respeta pausa/cinemáticas según el reloj de gameplay; no avances recargas durante pausa si el resto del juego está pausado. Evita bugs al cruzar mapas y al reiniciar checkpoints. El checkpoint de reintento puede reponer 3/3 de forma deliberada y documentada.
- Conserva cadencia normal, colisiones, alcance, daño, proyectiles visibles en dirección del personaje y despawn seguro.
- Prueba disparo 1/2/3, intento de disparo vacío, recarga de 6 s, cambio de arma a mitad de recarga, cambio de mapa, reinicio por muerte y ausencia de acumulación de proyectiles.
12. Jugabilidad terrestre que debe conservarse/mejorarse
Movimiento 2D lateral izquierda/derecha, salto y gravedad, plataformas auténticas con colliders independientes de la ilustración, dash corto hacia delante/atrás con cooldown, cámara fluida y límites, espada cian con hitbox sincronizada, plasma de distancia, cambio de arma y salud/knockback/invulnerabilidad breve. Mantén el comportamiento de V1 si ya es bueno, corrige lo defectuoso.

Caimán terrestre bípedo y más expresivo, no el antiguo caimán cuadrúpedo. Su IA usa distancia, cooldowns, telegráficos y recuperación:
1. Arañazos con garras y posibles combinaciones cortas (Caiman_Ataque_Cerca.png).
2. Empujón/embate corporal con knockback, esquivable.
3. Lanzamiento de piedras de largo alcance (Caiman_Ataque_Distancia.png), con proyectil y trayectoria física, impacto y efecto de polvo.
Usa Caiman_Movimientos.png, Caiman_Aparicion.png, hojas de ataque y escape nuevas. El jefe debe tener postura de burla y protección de territorio; sus animaciones deben sincronizarse con ataques reales. Daño una sola vez por ventana de hit. No atravieses terreno o capibara por errores de velocidad.

Mapa terrestre: conserva bosque amplio horizontal con desplazamiento de cámara, plataformas de raíces/troncos/rocas y orilla del lago a la derecha. Usa FondoTerrestre.png y capas Parallax/Terrestre. El PNG panorámico es un fondo, no un mapa físico: construye superficies, límites, zonas de combate y entrada interactivas. Si la imagen no es suficientemente larga, usa composición modular, extensiones de capas o repetición sin costuras; no la estires hasta deformarla. 3–5 anchos de pantalla es una escala inicial ajustable. Introduce señales discretas de minería ilegal sin obstaculizar la jugabilidad.
13. Jugabilidad acuática y jefe difícil pero justo
Capibara bajo el agua: mueve izquierda/derecha/arriba/abajo y diagonales con aceleración y arrastre acuático; nada, asciende, desciende, flota, recibe daño y dispara plasma exclusivamente. No usa espada ni salto/gravedad terrestres. Usa CapybaraAcuatico.png. Si mantienes maniobra evasiva acuática, diferénciala visualmente del dash terrestre y balancea cooldown.

Caimán acuático: permanece horizontal, usa Caiman_Acuatico_Aparicion.png, movimientos v1/v2, diagonales inferior/superior, preparación y dos ataques rápidos. IA por estados claros (patrulla, posicionamiento, persecución, preparación, embestida, frenado/recuperación, daño, furia, derrota). Su ataque característico es la embestida:
1. Se posiciona y muestra telegráfico inequívoco con movimientos y burbujas.
2. Al finalizar preparación fija la posición del capibara en ese instante.
3. Se impulsa hacia ese punto con velocidad alta en trayectoria comprometida, no seguimiento perfecto e inevitable.
4. Da daño y knockback si hay colisión; evita daño repetido del mismo ataque.
5. Termina con frenado, recuperación y ventana clara para contraatacar.
6. Con poca salud aumenta agresividad/velocidad y puede enlazar embestidas con pausas; no debe volverse injusto.
A partir de las hojas presentes, identifica lo que de verdad existe para flotación/natación/ascenso/descenso/embestida. Implementa hit, furia y derrota con las hojas verificadas y recursos procedurales discretos cuando falten frames específicos. No afirmes tener un spritesheet «defeat» si no existe. Usa una animación de hundimiento funcional si es viable, y anota limitaciones artísticas.

Mapa submarino: FondoAcuatico.png, capas Parallax/Acuatico, rayos de luz, plantas, rocas, burbujas, corrientes suaves, pequeños indicios de contaminación y área amplia de combate. Cámara sigue al personaje con amortiguación y encuadre adaptativo frente a embestidas; mantén visión suficiente del jefe para leer ataques. Construye colliders por separado de los gráficos. No añadas mecánica de oxígeno ni nuevas armas sin que el proyecto/usuario lo requieran.
14. Cámara dinámica, parallax, VFX, música y audio
- Implementa cámara suave, límites, desplazamiento de atención al jefe, panorámicas de cinemáticas y enfoque de ambos combatientes sin zoom excesivo.
- Sacudidas sutiles configurables al golpear/recibir daño, lanzar piedras, aterrizar y embestir. Añade hit-stop brevísimo cuando resulte apropiado, sin romper físicas o entrada.
- Capas parallax con profundidades y velocidades diferenciadas, elementos de primer plano sin ocultar innecesariamente al jugador. Usa todos los recursos pertinentes pero evita duplicaciones recargadas.
- Integra Efectos.png: polvo de pasos y dash, hojas, impactos de garras, piedras, chispas de espada, plasma, salpicaduras, burbujas, agua removida, sedimentos y corrientes. Recorta los elementos gráficos dejando fuera cualquier etiqueta que esté dibujada dentro del sheet.
- Música distinta para exploración, jefe terrestre, huida/transición, exploración submarina y fase II, con crossfades. Respeta sonidos existentes o incorpora recursos permitidos. Si música no está incluida, usa el audio existente disponible y define slots configurables sin inventar pistas ni descargar archivos con derechos no verificados.
- Sistema de audio con volúmenes maestros separados de música y efectos; mezcla correcta durante videos/emotes/pause.
15. Interfaz y experiencia
HUD elegante en pixel art: vida del capibara, vida/nombre/fase del caimán, arma equipada, 3 cápsulas de plasma, contador y barra de recarga de seis segundos, indicador de dash cuando aplique. Rueda radial T para emotes con opción de cancelar. Retratos/ventanas de diálogo con lectura cómoda, controles visibles y opción de adelantar. Mensaje E en orilla, pausa, pantalla derrota, reiniciar desde checkpoint, victoria final y volver al menú si el juego lo soporta. UI debe escalar sin tapar a los protagonistas.

Cinemáticas saltables y robustas. Al finalizar primera fase, checkpoint terrestre completado; si el jugador muere en fase acuática, reinicia fase II sin obligarlo a volver a derrotar al caimán terrestre. Guarda estado de transición y spawns seguros.

Controles por defecto si no hay mapeo funcional ya existente: A/D o flechas (tierra), Espacio (saltar), J/clic izquierdo (ataque), Q (cambiar arma en tierra), Shift (dash terrestre), E (interactuar), T (rueda emotes), W/S o flechas arriba/abajo (agua), Esc (cerrar rueda/pausa según contexto). No introduzcas controles contradictorios: integra con el input existente y documenta los finales.
16. Audio/video y recursos ausentes: tratamiento correcto
Los clips narrativos tienen rutas configurables: escape_video_path e immersion_video_path, con validación de archivo final. Los dos requieren videos definitivos con animales ficticios, no el MP4 de referencia del niño. No hay clip final válido para ninguno actualmente. Prueba el motor con un recurso ficticio de desarrollo solo si se etiqueta como prueba, nunca lo presentes como entregable final. En ausencia de video, usa una secuencia con sprites y diálogos que cubra las acciones esenciales. Cuando el usuario sustituya los archivos, el sistema reproducirá los videos sin reprogramar toda la escena.

Conserva el MP4 original solo como material de referencia fuera del paquete de distribución; no lo muestres al jugador. Si el motor o navegador no admite un codec, implementa fallback y registra el problema, no dejes pantalla negra. Si se recibe un video vertical definitivo, usa letterbox/reencuadre explícito manteniendo la geometría de personajes; nunca lo estires a 16:9.
17. QA, pruebas y criterios de aceptación
Verifica realmente, mediante ejecución y herramientas disponibles:

Git/seguridad: rama v1 mantiene snapshot anterior; rama v2 contiene mejoras; sin pérdida de archivos, secretos ni cambios remotos.

Assets: todas las rutas correctas; hojas procesadas con pivotes; no duplicación Movimiento_v1/v3; sprites del caimán terrestre sobre DOS patas; sin rótulos de los spritesheets dibujados en escena.

Terrestre: movimiento, salto, plataformas, dash, espada, cañón, cambio, hitboxes, daño, jefe con arañazos, empujón, piedras y escape.

Plasma: exactamente tres disparos; 0/3 bloquea disparos; recarga automática medible de 6 s; barra e iconos avanzan; refill; switching no evade la recarga; funciona en tierra y agua.

Emotes: T rueda, selección por teclado y ratón, WAV correcto por emote, animación sincronizada, sin superposición, cancela y no causa daño; deshabilitado en combate/cinemáticas.

Cinemáticas: presentación una sola vez; fase I herido→video opcional/fallback→huida→salto al lago; E preparación→video opcional/fallback→salto y burbujas→mapa acuático; ojos y aparición de jefe II; diálogos; skip y ausencia de bloqueos aunque falten videos.

Acuático: 4 direcciones+diagonal, solo plasma, natación, IA acuática con preaviso→bloqueo de trayectoria→embestida→recuperación, esquiva posible, daño, progresión de dificultad, derrota.

Escenarios: longitud efectiva superior al viewport, cámara, límites, parallax, plataformas reales, colliders, pequeños indicios de contaminación, sin capas que tapen hitboxes.

Flujo completo: iniciar partida→fase I→huida→entrada lago→fase II→victoria; muerte y reintento por fase; sin referencias rotas, memory leaks o proyectiles eternos; controles restaurados después de cutscenes y pausa.

Si es posible, crea pruebas automatizadas de lógica de municiones, recarga, FSM, transiciones, daños y checkpoints. Ejecuta build/test/lint pertinentes. Para pruebas visuales, utiliza capturas o ejecución guiada cuando esté disponible. Separa claramente: comprobado / inspeccionado pero no ejecutado / pendiente.
18. Etapas y prioridad de implementación
1. Auditar V1 y el ZIP; identificar brechas y crear Git v1→v2 de modo no destructivo.
2. Importar, normalizar y catalogar assets, animaciones, atlas y pivotes; reparar referencias.
3. Actualizar caimán terrestre bípedo, acciones y presentación con diálogos.
4. Implementar municiones de plasma 3/3, recarga 6 s y HUD, tanto en tierra como en agua.
5. Integrar rueda de tres emotes con audios y celebraciones contextuales.
6. Integrar cinematografía 2 con reproducción condicional de video y fallback, persecución y escape.
7. Integrar cinematografía 3 con E, video condicional/fallback y transición submarina.
8. Actualizar mapa, parallax, partículas, cámara y audio terrestre/acuático.
9. Implementar presentación y combate acuático, mejora de IA, embestida y derrota.
10. Pulir diálogos, balance, HUD, checkpoints, menús, accesibilidad básica y efectos.
11. Ejecutar pruebas completas, corregir fallos y documentar entregables.
Mantén el juego ejecutable al cerrar cada etapa. Haz commits locales descriptivos por hitos. Si el entorno de Work limita la ejecución, completa todo lo viable y reporta exactamente qué no fue comprobado, sin presentar falsos éxitos. Evita interrumpir por aclaraciones menores: toma decisiones consistentes con estos requisitos y explica supuestos relevantes.
19. Entregables finales
Al finalizar entrega:
1. Proyecto del juego en rama local v2, preservando v1.
2. Assets integrados/normalizados (sin el video original del menor en el juego final).
3. Juego arrancable con controles, dos fases, transiciones y victoria/derrota, con fallback de cinemáticas donde faltan videos finales.
4. Manual corto README con cómo ejecutar, controles y estructura de escenas.
5. docs/ASSETS_V2.md con cada recurso real, uso, pivote, cuadros, anomalías y faltantes de animación si los hay.
6. docs/CINEMATICAS_V2.md con orden exacto de escenas, diálogos, rutas de los dos videos definitivos, formatos recomendados e instrucciones para reemplazar los placeholders sin cambiar código.
7. docs/PRUEBAS_V2.md con pruebas, comandos, resultados y limitaciones verificadas.
8. Resumen preciso de archivos modificados, funcionalidades terminadas, pendientes y comandos Git para alternar ramas.
Criterio final: no me entregues un simple mockup ni un video. Quiero una continuación auténtica del videojuego existente que integre los recursos de V2 y permita llegar desde la selva hasta la victoria submarina. El único trabajo pendiente admisible por no tener material nuevo es la sustitución de los DOS videos definitivos y, si faltaran, animaciones específicas no presentes en el ZIP; aun así las escenas deben funcionar mediante fallback con sprites.

Empieza ahora por la auditoría del repositorio real y la protección de V1. Después implementa V2 progresivamente, sin modificar v1.