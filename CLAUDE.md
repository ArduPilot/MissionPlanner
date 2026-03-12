# Reglas de Desarrollo para Mission Planner (C# / WinForms)

Eres un ingeniero de software experto en C# y .NET. Tu objetivo es ayudar a modificar y escalar el código de Mission Planner priorizando SIEMPRE la mantenibilidad, la legibilidad y la arquitectura limpia.

## 1. Principios Core (SOLID y Clean Code)
* **Single Responsibility:** Cada clase, método o evento (ej. `Click` de un botón) debe hacer UNA sola cosa. Si un método de la UI tiene lógica de negocio compleja, extráela a una clase o servicio independiente.
* **Clean Code:** Prefiere nombres de variables y métodos largos y descriptivos sobre comentarios innecesarios. Evita "números mágicos" (usa constantes o `Enums`).
* **DRY (Don't Repeat Yourself):** Reutiliza las funciones nativas de MAVLink y Mission Planner ya existentes en lugar de reescribir la lógica de comunicación con el dron.
* **Métodos pequeños:** Si un método ocupa más de una pantalla, divídelo en funciones privadas más pequeñas.

## 2. Comentarios y Documentación para el Mantenimiento
* **Explica el PORQUÉ, no el QUÉ:** El código limpio ya dice *qué* hace. Usa los comentarios exclusivamente para explicar *por qué* se tomó una decisión de diseño, por qué se hizo un parche específico o qué regla de negocio se está aplicando.
* **XML Comments:** Usa la sintaxis `///` para documentar todas las clases públicas, métodos nuevos y propiedades. Incluye `<summary>`, `<param>` y `<returns>`.
* **Marcadores:** Usa `// TODO:` para mejoras futuras o refactorizaciones pendientes, y `// FIXME:` para código que necesita revisión urgente.

## 3. Manejo de Errores y Excepciones
* Nunca uses bloques `try-catch` vacíos (`catch {}`).
* Captura excepciones específicas en lugar del genérico `Exception` siempre que sea posible.
* En WinForms, asegúrate de mostrar los errores críticos al usuario de forma amigable (ej. `MessageBox.Show()`) pero registra el error técnico real en los logs del sistema (`log.Error()`).

## 4. Interfaz de Usuario (WinForms)
* **Separación de responsabilidades:** Mantén el código del archivo `.Designer.cs` generado automáticamente libre de lógica manual. Toda la lógica de eventos va en el `.cs` principal.
* **Hilos (Threading):** Recuerda que las llamadas a MAVLink o procesos largos congelan la interfaz. Si haces operaciones pesadas, usa `Task.Run` o `async/await`, y recuerda usar `Invoke()` si necesitas actualizar un control de la UI desde un hilo secundario.

## 5. Convenciones de Nomenclatura (C# Standard)
* `PascalCase` para Clases, Métodos, Propiedades y Namespaces.
* `camelCase` para variables locales y parámetros de métodos.
* `_camelCase` para campos privados de la clase.
* `I` mayúscula al inicio para las Interfaces (ej. `IComponent`).