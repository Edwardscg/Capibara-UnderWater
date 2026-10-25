# Cinemáticas V2

## Decisión sobre el MP4

El documento original pedía excluir `Caiman_Cinematica.mp4`. La instrucción directa más reciente del usuario pidió expresamente incluirlo: esa decisión prevalece. Se conserva e integra el archivo original en la derrota terrestre. No se presenta como una nueva animación de los personajes del juego.

## Orden y control

1. **Presentación terrestre**: al superar x=21 por primera vez, se detiene jugador/IA, se elimina proyectilería residual, la cámara enfoca al jefe, se reproducen ojos/aparición, cuatro diálogos, título y barra. Se devuelve control y comienza combate.
2. **Derrota terrestre**: herida, celebración breve del capibara, dos diálogos, MP4, regreso al escenario, carrera del caimán hacia x=63 y desaparición bajo el agua. La cámara acompaña la huida. El jefe no causa daño ni puede quedar retenido por el collider del jugador. Se habilita E en la orilla.
3. **Inmersión**: E cerca de x=58 inicia una sola transición. Diálogo, preparación con la hoja Capybara_Sumerge, slot de video vacío, pose de inmersión, salto/salpicadura y fundido. Se carga AmazoniaAcuatica conservando HP y cargador. El video ausente no bloquea el flujo.
4. **Presentación acuática**: paneo hacia raíces, ojos/cabeza/cuerpo, diálogo y título de aguas profundas. Control e IA vuelven. El reintento acuático omite esta introducción.
5. **Final**: muerte funcional del jefe, celebración contextual, pistas sobre contaminación y antagonista humano futuro, victoria y Reintentar. No se implementó un enemigo humano.

## Edición sin código

En cada escena seleccionar **Aventura · progreso y transición > AdventureDirector**:

- `landIntro`, `escape`, `immersion`, `waterIntro`, `ending`: nombre, texto y elección de retrato para cada línea. Texto renderizado por Unity.
- `escapeClip`: VideoClip incluido, `Assets/Art/V2/Sprites/Videos/Caiman_Cinematica.mp4`.
- `immersionClip`: vacío hasta recibir material.
- `escape_video_path`, `immersion_video_path`: ruta/URL opcional que prevalece sobre el VideoClip asignado. Para distribuir es preferible asignar un VideoClip dentro de Assets; una ruta absoluta externa sólo funciona en esa computadora.

Importar un MP4 nuevo y arrastrarlo al slot correspondiente. Dejar la ruta vacía para usar el clip. Para activar deliberadamente fallback, dejar vacíos tanto el clip como la ruta.

El reproductor reutilizable está en `AdventureDirector.PlayVideo`: prepara con timeout de 10 s, conserva relación de aspecto y bandas negras, reproduce audio en el bus SFX, permite Escape, detiene el video, libera RenderTexture y devuelve control a la secuencia. Error de preparación/codec continúa por fallback, con advertencia en log. No requiere paquetes externos.

Formato recomendado: MP4 H.264 y audio AAC compatible con el reproductor nativo de Windows. Se conserva el formato vertical del clip entregado; no se estira a pantalla completa. El éxito en Windows Editor no acredita compatibilidad con todos los codecs de otras plataformas.

Enter/clic/botón avanzan diálogo. Escape omite la secuencia actual; la huida/inmersión indispensable se completa para mantener la progresión. Las cinemáticas son temporales, no pausan Time.timeScale: sus scripts bloquean controles, IA, daño y reloj de recarga de forma explícita.

## Alternativas y límites

- Escape usa `Caiman_Derrotado_Escape.png`, que sí tiene transparencia; `Caiman_Escape.png` tiene fondo pintado.
- Sin video de escape: poses de lanzamiento, impactos cosméticos y huida. No se simula un nuevo ataque de combate durante el gag.
- Sin video de inmersión: preparación, salto, efecto de agua y fundido dentro del motor.
- Daño/furia/derrota acuática reutilizan nado con flash, tintado y hundimiento. No son animaciones originales independientes presentes en el ZIP.
- No hay pistas musicales entregadas. AdventureAudio ofrece cinco slots y crossfade; emotes y MP4 sí tienen audio real.
