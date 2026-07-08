# PromptingDemo

Proyecto base en ASP.NET Core MVC para explorar técnicas de prompt engineering con un modelo de IA local mediante Ollama.

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) o superior
- [Ollama](https://ollama.com/download) corriendo en `localhost:11434`
- Un modelo descargado en Ollama (por defecto: `llama3.2:1b`)

## Configuración

Editar `src/PromptingDemo/appsettings.json`:

```json
{
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "llama3.2:1b"
  },
  "Assistant": {
    "Domain": "soporte técnico de software general (Windows, internet, hardware básico, aplicaciones de uso común)"
  }
}
```

- `BaseUrl`: URL donde corre Ollama.
- `Model`: nombre del modelo descargado (`ollama list` para ver los disponibles).
- `Domain`: descripción del dominio del asistente.

## Ejecución

```bash
dotnet run --project src/PromptingDemo
```

Abrir en el navegador la URL que muestre la consola (por defecto `http://localhost:5197`).

En la página principal seleccione una técnica de prompting, escriba una consulta y presione **Ejecutar**.

## Técnicas disponibles

1. Zero-shot
2. Few-shot
3. Chain-of-Thought
4. Role Prompting

## Estructura

```
src/PromptingDemo/
├── Program.cs
├── appsettings.json
├── Controllers/
│   └── HomeController.cs
├── Models/
│   └── PromptViewModel.cs
├── Views/
│   ├── Home/Index.cshtml
│   └── Shared/_Layout.cshtml
├── Configuration/
│   └── AppSettings.cs
└── Techniques/
    ├── IPromptingTechnique.cs
    ├── ZeroShot.cs
    ├── FewShot.cs
    ├── ChainOfThought.cs
    └── RolePrompting.cs
```
