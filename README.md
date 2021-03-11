# Pathfinding with AStar
 Este es un proyecto con el que pretendo aprender sobre la implementación de AStar en un entorno realista.
 
 [Trello del proyecto](https://trello.com/b/gf9WJnsU/proyecto-de-pathfinding-en-unity)
 
 Todo mi código se encuentra en: [Assets >> Code](Assets/Code/)
 
 Los objetos y tipos de suelos se añaden mediante ScriptableObjects, y se guardan en sus respectivas carpetas:
 * [Assets >> Resources >> ScriptableObjects](Assets/Resources/ScriptableObjects/)
   * [GroundTypes](Assets/Resources/ScriptableObjects/GroundTypes/)
   * [ObjectDefinitions](Assets/Resources/ScriptableObjects/ObjectDefinitions/)
 
 Ahí tengo organizado todo en carpetas y sub carpetas. Aunque la mayor parte solo son cosas necesarias para generar el mapa, etc.
 
# Controles
 Controles de Teclado:
 * WASD:      Movimiento de la cámara
 * Space:     Cambio de herramienta
 * 1-...-0:   Cambio de objeto seleccionado
 * Q & E:     Rotar Objeto en la mano (Para objetos grandes)
 * F4:        Ocultar/Mostrar la interfaz
 * P:         Recalcular el camino desde el punto de inicio al destino
 
 Controles de Ratón:
 * Herramienta de Pathfinding:
   * Clic izquierdo:  Cambiar posición de inicio
   * Clic derecho:    Cambiar posición de destino
 * Herramienta de Terreno:
   * Clic izquierdo:  Cambiar suelo en la posición del ratón (Por defecto es "vacio")
   * Clic derecho:    Nada
 * Herramienta de Objetos:
   * Clic izquierdo:  Poner objeto en la posición del ratón
   * Clic derecho:    Destruir objeto

 Controles adicionales:
 * Introduce una semilla en el campo superior y pulsa el botón "Generate" para regenerar el mapa
 * Si no introduces una semilla, se usará una semilla aleatoria
 * Si introduces la semilla "empty", se generará un mapa totalmente vacío
