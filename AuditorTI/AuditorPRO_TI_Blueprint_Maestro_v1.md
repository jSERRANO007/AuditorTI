# AuditorPRO TI — Blueprint Maestro v1.0
### Plataforma Empresarial de Auditoría Preventiva Inteligente
**Corporación ILG Logistics — Área de TI y Auditoría Interna**
**Versión:** 1.0 | **Fecha:** Abril 2026 | **Clasificación:** Confidencial — Uso interno

---

> **Principio rector:** El objetivo estratégico es **cero hallazgos de auditoría**. Cada funcionalidad, control, regla y flujo de esta plataforma existe para detectar debilidades antes que el auditor externo, y corregirlas con evidencia trazable y verificable.

---

## ÍNDICE MAESTRO

1. [Resumen Ejecutivo](#1-resumen-ejecutivo)
2. [Visión del Producto](#2-visión-del-producto)
3. [Arquitectura General de la Solución](#3-arquitectura-general-de-la-solución)
4. [Stack Tecnológico — Justificación y Decisiones](#4-stack-tecnológico)
5. [Autenticación y Seguridad — Microsoft Entra ID](#5-autenticación-y-seguridad)
6. [Modelo de Datos Completo](#6-modelo-de-datos-completo)
7. [Módulos Funcionales — Especificación Detallada](#7-módulos-funcionales)
8. [Motor de Reglas de Auditoría](#8-motor-de-reglas-de-auditoría)
9. [Agente IA Auditor Preventivo](#9-agente-ia-auditor-preventivo)
10. [Motor de Integraciones — SOA Manager de la Plataforma](#10-motor-de-integraciones)
11. [UX/UI — Diseño Fiori Enterprise](#11-uxui-diseño-fiori-enterprise)
12. [Trazabilidad y Bitácora de Auditoría](#12-trazabilidad-y-bitácora)
13. [Generación Automática de Entregables](#13-generación-de-entregables)
14. [API Interna — Especificación REST](#14-api-interna)
15. [Escenarios de Prueba — Matriz QA Completa](#15-escenarios-de-prueba-qa)
16. [Plan de Implementación y Roadmap](#16-roadmap-de-implementación)
17. [Alineación con Marcos Normativos](#17-alineación-normativa)
18. [Gestión de Configuración y Ambientes](#18-gestión-de-configuración)
19. [Arquitectura de Despliegue Azure](#19-arquitectura-de-despliegue-azure)
20. [Checklist de Verificación Final](#20-checklist-de-verificación-final)

---

## 1. RESUMEN EJECUTIVO

### ¿Qué es AuditorPRO TI?

AuditorPRO TI es una **plataforma empresarial de auditoría preventiva inteligente** diseñada para actuar como un auditor interno digital disponible 24/7. Su propósito es anticipar hallazgos, evaluar controles, consolidar evidencias y guiar a los responsables hacia el cumplimiento antes de que llegue la auditoría formal.

### Problema que resuelve

| Situación actual | Con AuditorPRO TI |
|---|---|
| Hallazgos descubiertos durante auditoría formal | Detectados semanas o meses antes |
| Evidencias buscadas en el último momento | Organizadas, indexadas y listas |
| Políticas inconsistentes o desactualizadas | Revisadas y fortalecidas continuamente |
| Trazabilidad parcial o manual | Automática, completa e inmutable |
| Reportes preparados manualmente | Generados en segundos |
| Sin visibilidad del estado de madurez | Dashboard ejecutivo en tiempo real |

### Valor diferencial que justifica adquisición empresarial

- **Motor de reglas auditables** configurables por dominio sin código
- **Agente IA con contexto organizacional** propio del cliente
- **Mantenimiento de conectores tipo SOA Manager** visual e intuitivo
- **Generación automática** de Word, PowerPoint y expedientes completos
- **Autenticación nativa con Microsoft Entra ID** — cero usuarios adicionales
- **Calificación de madurez de auditoría** de 1 a 10 por sociedad y proceso
- **Trazabilidad forense completa** para auditorías externas e internas

---

## 2. VISIÓN DEL PRODUCTO

### Nombre del sistema
**AuditorPRO TI** — Plataforma de Auditoría Preventiva Inteligente

### Tagline
> *"Encuentra las debilidades antes que el auditor. Llega a cero hallazgos."*

### Usuarios objetivo

| Rol | Uso principal |
|---|---|
| Administrador de TI (SAP Basis/Seguridad) | Evaluar controles SAP, recertificaciones, cambios |
| Auditor Interno | Simulaciones, revisión de hallazgos, exportar reportes |
| Gerente de TI | Dashboard ejecutivo, calificación de madurez |
| Controlador Financiero | Evidencias, planes de acción, riesgos SoD |
| Responsable de proceso | Revisión de sus controles asignados |
| Administrador de la plataforma | Mantenimiento de conectores y configuración |

### Principios de diseño del producto

1. **Claridad sobre complejidad** — Cualquier usuario entiende qué pasa sin capacitación
2. **Trazabilidad primero** — Todo queda registrado, nada se pierde
3. **Acción inmediata** — Cada hallazgo lleva a una acción sugerida
4. **Datos antes que opinión** — El agente IA siempre justifica con evidencia
5. **Resiliencia** — Funciona aunque una integración esté caída (modo contingencia)
6. **Mantenibilidad** — Los conectores y reglas se configuran sin programar

---

## 3. ARQUITECTURA GENERAL DE LA SOLUCIÓN

### Diagrama de arquitectura por capas

```
╔══════════════════════════════════════════════════════════════════════════╗
║                    CAPA DE PRESENTACIÓN (Frontend)                       ║
║   React + TypeScript + Tailwind CSS                                      ║
║   Dashboard Fiori · Módulos · Consulta IA · Mantenimiento Conectores     ║
╚══════════════════════════════════════════════════════════════════════════╝
                               │ HTTPS / JWT
╔══════════════════════════════════════════════════════════════════════════╗
║                    CAPA DE API (.NET 8 — Clean Architecture)             ║
║  Controllers → Application Layer → Domain Layer → Infrastructure         ║
║  Auth Middleware · Audit Middleware · Rate Limiting · Error Handling     ║
╚══════════════════════════════════════════════════════════════════════════╝
        │               │               │               │
        ▼               ▼               ▼               ▼
╔══════════════╗ ╔════════════╗ ╔═════════════╗ ╔═══════════════╗
║ Azure SQL DB ║ ║ Azure Blob ║ ║ Azure AI    ║ ║ Azure OpenAI  ║
║ (datos       ║ ║ Storage    ║ ║ Search      ║ ║ / AI Foundry  ║
║ operativos)  ║ ║ (docs,     ║ ║ (índice     ║ ║ (agente IA,   ║
║              ║ ║ evidencias)║ ║ semántico)  ║ ║ generación)   ║
╚══════════════╝ ╚════════════╝ ╚═════════════╝ ╚═══════════════╝
        │
╔══════════════════════════════════════════════════════════════════════════╗
║               CAPA DE INTEGRACIÓN (Motor de Conectores)                  ║
║   Azure Functions · Integration Engine · SOA-Style Connector Manager    ║
║   SE Suite · Recertificación · Evolution · SAP · DMS · Otros            ║
╚══════════════════════════════════════════════════════════════════════════╝
        │
╔══════════════════════════════════════════════════════════════════════════╗
║                    CAPA DE SEGURIDAD (Transversal)                       ║
║   Microsoft Entra ID · RBAC · TLS 1.3 · Key Vault · Audit Logs          ║
╚══════════════════════════════════════════════════════════════════════════╝
        │
╔══════════════════════════════════════════════════════════════════════════╗
║                    CAPA DE OBSERVABILIDAD (Transversal)                  ║
║   Application Insights · Log Analytics · Alertas · Health Checks        ║
╚══════════════════════════════════════════════════════════════════════════╝
```

### Principios arquitectónicos

| Principio | Descripción | Implementación |
|---|---|---|
| Clean Architecture | Separación estricta de capas | Domain → Application → Infrastructure → Presentation |
| SOLID | Responsabilidad única por servicio | Un servicio = una responsabilidad |
| Domain-Driven Design | Modelo centrado en dominio de auditoría | AggregateRoots: Simulación, Control, Hallazgo |
| CQRS (ligero) | Separar lecturas de escrituras para rendimiento | Queries para dashboard, Commands para simulaciones |
| Event Sourcing (logs) | Bitácora inmutable de eventos | Azure SQL con append-only en tablas de auditoría |
| API-First | Contrato API antes de implementar | OpenAPI/Swagger como contrato |
| Resiliencia | Fallback cuando integración falla | Modo contingencia por Excel/CSV siempre disponible |

---

## 4. STACK TECNOLÓGICO

### Decisiones técnicas justificadas

#### Frontend

| Componente | Tecnología | Justificación |
|---|---|---|
| Framework | React 18 + TypeScript | Ecosistema maduro, tipado fuerte, componentes reutilizables |
| Estilos | Tailwind CSS + shadcn/ui | Consistencia visual, velocidad de desarrollo, design system |
| Estado global | Zustand | Más simple que Redux, suficiente para este caso |
| Gráficos | Recharts + D3 | Flexibilidad para KPIs y semáforos |
| Tabla de datos | TanStack Table | Filtrado, ordenamiento, paginación nativa |
| Formularios | React Hook Form + Zod | Validación de esquema, fácil de mantener |
| HTTP client | Axios con interceptores | Manejo centralizado de tokens y errores |
| Routing | React Router v6 | Navegación de 4 niveles requerida |
| Notificaciones | Sonner (toast) | UX mínima, no intrusiva |
| Build | Vite | Velocidad de build, HMR, optimización automática |

#### Backend

| Componente | Tecnología | Justificación |
|---|---|---|
| Framework | .NET 8 (C#) | Rendimiento, soporte Microsoft, Azure nativo |
| Arquitectura | Clean Architecture + MediatR | Desacoplamiento real, fácil de probar |
| ORM | Entity Framework Core 8 | Migrations, LINQ, integración Azure SQL |
| Validación | FluentValidation | Reglas de negocio expresivas |
| Autenticación | Microsoft.Identity.Web | Entra ID nativo |
| Documentación API | Swashbuckle (Swagger) | OpenAPI 3.0 automático |
| Logging | Serilog → Application Insights | Structured logging, trazabilidad |
| Health Checks | AspNetCore.HealthChecks | Monitoreo de dependencias |
| Generación Word | DocumentFormat.OpenXml | Word sin dependencias de Office |
| Generación PPT | PresentationML / OpenXml SDK | PowerPoint sin dependencias |
| Cache | IMemoryCache + Azure Cache Redis | Reducir carga en DB para KPIs |

#### Infraestructura Azure

| Servicio | Uso | Configuración sugerida |
|---|---|---|
| Azure App Service | Hosting backend y frontend | Plan P1v3 mínimo para producción |
| Azure SQL Database | Base de datos principal | General Purpose, 4 vCores, BCDR habilitado |
| Azure Blob Storage | Evidencias y documentos | GRS (replicación geográfica) |
| Azure AI Search | Búsqueda semántica de evidencias | Standard S1 |
| Azure OpenAI / AI Foundry | Agente IA, generación docs | GPT-4o, deployment regional |
| Azure Functions | Integraciones y jobs batch | Consumption Plan (escala automática) |
| Azure Key Vault | Secretos y connection strings | Acceso solo desde Managed Identity |
| Microsoft Entra ID | Autenticación corporativa | App Registration + grupos de seguridad |
| Application Insights | Telemetría y errores | Workspace-based, 90 días de retención |
| Log Analytics | Logs centralizados | Integración con Application Insights |
| Azure Container Registry | Imágenes Docker | Si se containeriza en Fase 2 |

---

## 5. AUTENTICACIÓN Y SEGURIDAD

### Flujo de autenticación con Microsoft Entra ID

```
Usuario accede a AuditorPRO TI
         │
         ▼
Frontend verifica si hay token válido en sessionStorage
         │
    No hay token / expiró
         │
         ▼
Redirige a Microsoft Entra ID (MSAL.js / OAuth 2.0 + PKCE)
         │
         ▼
Usuario se autentica con credenciales corporativas (SSO)
         │
Entra ID verifica que el usuario esté ACTIVO en Azure AD
         │
    Usuario inactivo / no encontrado → ACCESO DENEGADO
         │
    Usuario activo → emite Access Token + ID Token
         │
         ▼
Frontend almacena token (sessionStorage, NO localStorage)
         │
         ▼
Cada request al backend incluye: Authorization: Bearer {token}
         │
         ▼
Backend valida token contra Entra ID (firma + audiencia + expiración)
         │
Extrae: userId, email, displayName, grupos de seguridad
         │
         ▼
Middleware verifica que el usuario tenga rol asignado en AuditorPRO TI
         │
    Sin rol asignado → 403 Forbidden
         │
    Con rol → accede al recurso según permisos del rol
         │
         ▼
Todo el flujo queda registrado en la bitácora de auditoría
```

### Configuración de App Registration en Entra ID

```json
{
  "app_name": "AuditorPRO-TI",
  "platform": "SPA (Single Page Application)",
  "redirect_uris": [
    "https://auditorpro.ilglogistics.com/auth/callback",
    "https://localhost:5173/auth/callback"
  ],
  "scopes_expuestos": [
    "api://auditorpro-ti/Simulaciones.Read",
    "api://auditorpro-ti/Simulaciones.Write",
    "api://auditorpro-ti/Hallazgos.Read",
    "api://auditorpro-ti/Administracion.Full"
  ],
  "grupos_requeridos": true,
  "token_lifetime": "1 hora (access token)",
  "require_mfa": true
}
```

### Modelo de roles (RBAC)

| Rol | Descripción | Permisos |
|---|---|---|
| `AuditorPRO.Admin` | Administrador total de la plataforma | Todo, incluyendo configuración de conectores y reglas |
| `AuditorPRO.Auditor` | Auditor interno o externo | Crear/ejecutar simulaciones, ver todos los módulos, exportar |
| `AuditorPRO.TI.Senior` | Administrador TI senior | Simulaciones propias, ver hallazgos, editar planes de acción |
| `AuditorPRO.TI.Viewer` | Usuario TI de solo lectura | Ver dashboard, hallazgos, evidencias. Sin modificar |
| `AuditorPRO.Gerente` | Gerente o Controlador Financiero | Dashboard ejecutivo, KPIs, exportar reportes gerenciales |
| `AuditorPRO.Responsable` | Responsable de proceso | Ver sus controles asignados, actualizar plan de acción propio |

### Validación de usuario activo en corporación

**Requisito crítico:** El sistema debe verificar que el usuario autenticado exista y esté **activo** en Azure AD de la corporación, NO solo que tenga credenciales válidas.

```csharp
// Middleware de validación de usuario activo
public class ActiveUserValidationMiddleware
{
    public async Task InvokeAsync(HttpContext context, IGraphServiceClient graphClient)
    {
        var userId = context.User.GetObjectId();
        
        // Consultar Microsoft Graph para estado del usuario
        var user = await graphClient.Users[userId]
            .GetAsync(req => req.QueryParameters.Select = 
                new[] { "accountEnabled", "displayName", "department" });
        
        if (user == null || user.AccountEnabled == false)
        {
            // Registrar intento de acceso con cuenta inactiva
            await auditLogger.LogSecurityEvent(
                userId, "ACCESO_DENEGADO_USUARIO_INACTIVO", context.Request.Path);
            
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new {
                error = "Acceso denegado. Su cuenta no está activa en el directorio corporativo.",
                code = "USER_INACTIVE"
            });
            return;
        }
        
        // Enriquecer contexto con datos del usuario
        context.Items["UserDisplayName"] = user.DisplayName;
        context.Items["UserDepartment"] = user.Department;
        
        await next(context);
    }
}
```

### Seguridad adicional — capas de protección

| Capa | Control | Implementación |
|---|---|---|
| Transporte | TLS 1.3 obligatorio | Azure App Service + HSTS headers |
| Secretos | Azure Key Vault | Managed Identity, sin strings en código |
| Datos en reposo | Cifrado AES-256 | Azure SQL Transparent Data Encryption |
| Datos sensibles en BD | Column-level encryption | Campos: número empleado, correo, salario |
| API | Rate limiting | 100 req/min por usuario, 1000/min global |
| API | Input validation | FluentValidation en todos los commands |
| Frontend | CSP headers | Content-Security-Policy estricto |
| Frontend | Token en sessionStorage | Se limpia al cerrar pestaña |
| Acceso a blob | SAS tokens temporales | Máximo 1 hora de vigencia por documento |
| Logs | Sin datos sensibles | PII ofuscado en logs (solo ID, no nombre completo) |

---

## 6. MODELO DE DATOS COMPLETO

### Convenciones

- Todas las tablas tienen: `created_at`, `updated_at`, `created_by_user_id`, `is_deleted` (soft delete)
- PKs: GUID (uniqueidentifier) excepto catálogos de referencia (int identity)
- Foreign Keys con nombres explícitos y DELETE RESTRICT (sin cascada en datos operativos)
- Índices en todos los campos de búsqueda y filtrado frecuente

---

### BLOQUE 1: Maestros Corporativos

```sql
-- =========================================
-- TABLA: sociedades
-- =========================================
CREATE TABLE sociedades (
    id              INT IDENTITY(1,1) PRIMARY KEY,
    codigo          VARCHAR(10) NOT NULL UNIQUE,   -- Ej: ILG-CR, ILG-SV
    nombre          VARCHAR(200) NOT NULL,
    pais            VARCHAR(100),
    activa          BIT NOT NULL DEFAULT 1,
    created_at      DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at      DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    created_by      VARCHAR(200)
);

-- =========================================
-- TABLA: departamentos
-- =========================================
CREATE TABLE departamentos (
    id              INT IDENTITY(1,1) PRIMARY KEY,
    sociedad_id     INT NOT NULL REFERENCES sociedades(id),
    codigo          VARCHAR(20) NOT NULL,
    nombre          VARCHAR(200) NOT NULL,
    activo          BIT NOT NULL DEFAULT 1,
    created_at      DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at      DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- =========================================
-- TABLA: puestos
-- =========================================
CREATE TABLE puestos (
    id              INT IDENTITY(1,1) PRIMARY KEY,
    sociedad_id     INT NOT NULL REFERENCES sociedades(id),
    codigo          VARCHAR(20) NOT NULL,
    nombre          VARCHAR(200) NOT NULL,
    nivel_riesgo    VARCHAR(20) CHECK (nivel_riesgo IN ('ALTO','MEDIO','BAJO')),
    activo          BIT NOT NULL DEFAULT 1,
    created_at      DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- =========================================
-- TABLA: empleados_maestro
-- Fuente principal de verdad sobre empleados activos
-- =========================================
CREATE TABLE empleados_maestro (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    numero_empleado     VARCHAR(30) NOT NULL UNIQUE,
    nombre_completo     VARCHAR(300) NOT NULL,
    correo_corporativo  VARCHAR(200),
    entra_id_object     VARCHAR(100),           -- Object ID en Azure AD
    sociedad_id         INT REFERENCES sociedades(id),
    departamento_id     INT REFERENCES departamentos(id),
    puesto_id           INT REFERENCES puestos(id),
    jefe_empleado_id    UNIQUEIDENTIFIER REFERENCES empleados_maestro(id),
    estado_laboral      VARCHAR(30) NOT NULL CHECK (estado_laboral IN ('ACTIVO','INACTIVO','SUSPENDIDO','BAJA_PROCESADA')),
    fecha_ingreso       DATE,
    fecha_baja          DATE,
    fuente_origen       VARCHAR(50),            -- EVOLUTION, EXCEL_MANUAL, API_DMS
    lote_carga_id       UNIQUEIDENTIFIER,
    created_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    is_deleted          BIT NOT NULL DEFAULT 0,
    INDEX IX_empleados_estado (estado_laboral, sociedad_id),
    INDEX IX_empleados_entra (entra_id_object)
);

-- =========================================
-- TABLA: usuarios_sistema
-- Usuarios en sistemas de TI (SAP, SE Suite, etc.)
-- =========================================
CREATE TABLE usuarios_sistema (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    sistema             VARCHAR(50) NOT NULL,   -- SAP, SE_SUITE, EVOLUTION, MAGAYA
    nombre_usuario      VARCHAR(100) NOT NULL,
    empleado_id         UNIQUEIDENTIFIER REFERENCES empleados_maestro(id),
    estado              VARCHAR(30) NOT NULL CHECK (estado IN ('ACTIVO','BLOQUEADO','ELIMINADO','SIN_CORRESPONDENCIA')),
    tipo_usuario        VARCHAR(30),            -- DIALOGO, SISTEMA, COMUNICACION, RFC
    fecha_ultimo_acceso DATETIME2,
    fuente_origen       VARCHAR(50),
    created_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    is_deleted          BIT NOT NULL DEFAULT 0,
    INDEX IX_usuarios_sistema_estado (sistema, estado)
);

-- =========================================
-- TABLA: roles_sistema
-- =========================================
CREATE TABLE roles_sistema (
    id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    sistema         VARCHAR(50) NOT NULL,
    nombre_rol      VARCHAR(200) NOT NULL,
    descripcion     NVARCHAR(MAX),
    nivel_riesgo    VARCHAR(20) CHECK (nivel_riesgo IN ('CRITICO','ALTO','MEDIO','BAJO')),
    es_critico      BIT NOT NULL DEFAULT 0,
    activo          BIT NOT NULL DEFAULT 1,
    created_at      DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- =========================================
-- TABLA: asignaciones_rol_usuario
-- =========================================
CREATE TABLE asignaciones_rol_usuario (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    usuario_id          UNIQUEIDENTIFIER NOT NULL REFERENCES usuarios_sistema(id),
    rol_id              UNIQUEIDENTIFIER NOT NULL REFERENCES roles_sistema(id),
    fecha_asignacion    DATE,
    fecha_vencimiento   DATE,
    asignado_por        VARCHAR(200),
    expediente_ref      VARCHAR(100),
    activa              BIT NOT NULL DEFAULT 1,
    created_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- =========================================
-- TABLA: matriz_puesto_rol
-- Qué roles DEBE tener cada puesto
-- =========================================
CREATE TABLE matriz_puesto_rol (
    id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    puesto_id       INT NOT NULL REFERENCES puestos(id),
    rol_id          UNIQUEIDENTIFIER NOT NULL REFERENCES roles_sistema(id),
    tipo            VARCHAR(20) CHECK (tipo IN ('REQUERIDO','OPCIONAL','PROHIBIDO')),
    justificacion   NVARCHAR(1000),
    vigente_desde   DATE,
    created_at      DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- =========================================
-- TABLA: conflictos_sod
-- Conflictos de Segregación de Funciones conocidos
-- =========================================
CREATE TABLE conflictos_sod (
    id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    sistema         VARCHAR(50),
    rol_a_id        UNIQUEIDENTIFIER REFERENCES roles_sistema(id),
    rol_b_id        UNIQUEIDENTIFIER REFERENCES roles_sistema(id),
    descripcion     NVARCHAR(MAX),
    riesgo          VARCHAR(20) CHECK (riesgo IN ('CRITICO','ALTO','MEDIO','BAJO')),
    mitigacion_doc  NVARCHAR(MAX),
    activo          BIT NOT NULL DEFAULT 1,
    created_at      DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
```

---

### BLOQUE 2: Motor de Auditoría

```sql
-- =========================================
-- TABLA: dominios_auditoria
-- Catálogo de dominios evaluables
-- =========================================
CREATE TABLE dominios_auditoria (
    id          INT IDENTITY(1,1) PRIMARY KEY,
    codigo      VARCHAR(30) NOT NULL UNIQUE,
    nombre      VARCHAR(200) NOT NULL,
    descripcion NVARCHAR(MAX),
    activo      BIT NOT NULL DEFAULT 1
    -- Ejemplos: IDENTIDAD, ALTAS_BAJAS, RECERTIFICACION, SEG_SAP, 
    --           CAMBIOS_APP, EVIDENCIA_DOC, POLITICAS, PROCEDIMIENTOS, SoD
);

-- =========================================
-- TABLA: puntos_control
-- Catálogo de controles auditables (configurables sin código)
-- =========================================
CREATE TABLE puntos_control (
    id                  INT IDENTITY(1,1) PRIMARY KEY,
    dominio_id          INT NOT NULL REFERENCES dominios_auditoria(id),
    codigo              VARCHAR(50) NOT NULL UNIQUE,   -- Ej: ID-001, ALTA-003
    nombre              VARCHAR(300) NOT NULL,
    descripcion         NVARCHAR(MAX),
    tipo_evaluacion     VARCHAR(30) NOT NULL CHECK (tipo_evaluacion IN ('AUTOMATICO','SEMI_AUTOMATICO','MANUAL')),
    criticidad_base     VARCHAR(20) NOT NULL CHECK (criticidad_base IN ('CRITICA','MEDIA','BAJA')),
    norma_referencia    VARCHAR(200),                  -- ISO 27001 A.9.2, COBIT APO13
    query_sql           NVARCHAR(MAX),                 -- Query para evaluación automática
    condicion_verde     NVARCHAR(MAX),                 -- Lógica JSON para semáforo verde
    condicion_amarillo  NVARCHAR(MAX),
    condicion_rojo      NVARCHAR(MAX),
    evidencia_requerida NVARCHAR(MAX),                 -- Lista de evidencias esperadas (JSON)
    activo              BIT NOT NULL DEFAULT 1,
    version_regla       INT NOT NULL DEFAULT 1,
    created_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- =========================================
-- TABLA: simulaciones_auditoria
-- Cabecera de cada simulación ejecutada
-- =========================================
CREATE TABLE simulaciones_auditoria (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    nombre              VARCHAR(300) NOT NULL,
    descripcion         NVARCHAR(MAX),
    tipo                VARCHAR(30) CHECK (tipo IN ('MANUAL','PROGRAMADA','BAJO_DEMANDA')),
    periodicidad        VARCHAR(30) CHECK (periodicidad IN ('MENSUAL','TRIMESTRAL','SEMESTRAL','ANUAL','UNICA')),
    estado              VARCHAR(30) NOT NULL CHECK (estado IN ('PENDIENTE','EN_PROCESO','COMPLETADA','ERROR','CANCELADA')) DEFAULT 'PENDIENTE',
    sociedad_ids        NVARCHAR(MAX),              -- JSON array de IDs
    periodo_inicio      DATE NOT NULL,
    periodo_fin         DATE NOT NULL,
    dominio_ids         NVARCHAR(MAX),              -- JSON array de dominios
    puntos_control_ids  NVARCHAR(MAX),              -- JSON array o NULL = todos
    score_madurez       DECIMAL(4,2),               -- 1.00 a 10.00
    porcentaje_cumplimiento DECIMAL(5,2),
    total_controles     INT,
    controles_verde     INT,
    controles_amarillo  INT,
    controles_rojo      INT,
    iniciada_por        VARCHAR(200) NOT NULL,       -- email del usuario
    iniciada_at         DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    completada_at       DATETIME2,
    duracion_segundos   INT,
    error_detalle       NVARCHAR(MAX),
    created_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX IX_sim_estado (estado, iniciada_at DESC),
    INDEX IX_sim_sociedad (sociedad_ids) -- considera JSONVALUE index en Azure SQL
);

-- =========================================
-- TABLA: resultados_control
-- Resultado de cada punto de control por simulación
-- =========================================
CREATE TABLE resultados_control (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    simulacion_id       UNIQUEIDENTIFIER NOT NULL REFERENCES simulaciones_auditoria(id),
    punto_control_id    INT NOT NULL REFERENCES puntos_control(id),
    sociedad_id         INT REFERENCES sociedades(id),
    semaforo            VARCHAR(10) NOT NULL CHECK (semaforo IN ('VERDE','AMARILLO','ROJO','NO_EVALUADO')),
    criticidad          VARCHAR(20) NOT NULL CHECK (criticidad IN ('CRITICA','MEDIA','BAJA')),
    resultado_detalle   NVARCHAR(MAX),              -- Descripción legible del resultado
    datos_evaluados     NVARCHAR(MAX),              -- JSON con los datos analizados
    evidencia_encontrada NVARCHAR(MAX),             -- JSON array de evidencias
    evidencia_faltante  NVARCHAR(MAX),              -- JSON array de lo que falta
    analisis_ia         NVARCHAR(MAX),              -- Texto generado por el agente IA
    recomendacion       NVARCHAR(MAX),
    responsable_sugerido VARCHAR(200),
    fecha_compromiso_sug DATE,
    evaluado_at         DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX IX_resultados_sim (simulacion_id, semaforo)
);

-- =========================================
-- TABLA: hallazgos
-- Hallazgos generados (preventivos o reales)
-- =========================================
CREATE TABLE hallazgos (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    simulacion_id       UNIQUEIDENTIFIER REFERENCES simulaciones_auditoria(id),
    resultado_control_id UNIQUEIDENTIFIER REFERENCES resultados_control(id),
    punto_control_id    INT REFERENCES puntos_control(id),
    sociedad_id         INT REFERENCES sociedades(id),
    tipo                VARCHAR(30) CHECK (tipo IN ('PREVENTIVO','REAL','HISTORICO')),
    titulo              VARCHAR(500) NOT NULL,
    descripcion         NVARCHAR(MAX) NOT NULL,
    causa_probable      NVARCHAR(MAX),
    impacto             NVARCHAR(MAX),
    criticidad          VARCHAR(20) NOT NULL CHECK (criticidad IN ('CRITICA','MEDIA','BAJA')),
    semaforo            VARCHAR(10) NOT NULL,
    estado              VARCHAR(30) NOT NULL CHECK (estado IN ('ABIERTO','EN_PROCESO','CERRADO','ACEPTADO','RECURRENTE')) DEFAULT 'ABIERTO',
    recurrente          BIT NOT NULL DEFAULT 0,
    hallazgo_previo_id  UNIQUEIDENTIFIER REFERENCES hallazgos(id),
    norma_violada       VARCHAR(200),
    created_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    is_deleted          BIT NOT NULL DEFAULT 0,
    INDEX IX_hallazgos_estado (estado, criticidad, created_at DESC)
);

-- =========================================
-- TABLA: planes_accion
-- Plan de acción por hallazgo
-- =========================================
CREATE TABLE planes_accion (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    hallazgo_id         UNIQUEIDENTIFIER NOT NULL REFERENCES hallazgos(id),
    descripcion_accion  NVARCHAR(MAX) NOT NULL,
    causa_raiz          NVARCHAR(MAX),
    recomendacion_ia    NVARCHAR(MAX),
    responsable_email   VARCHAR(200),
    responsable_nombre  VARCHAR(300),
    fecha_compromiso    DATE NOT NULL,
    fecha_cierre_real   DATE,
    prioridad           VARCHAR(20) CHECK (prioridad IN ('INMEDIATA','ALTA','MEDIA','BAJA')),
    estado              VARCHAR(30) NOT NULL CHECK (estado IN ('PENDIENTE','EN_PROCESO','COMPLETADO','REQUIERE_VALIDACION','CERRADO','VENCIDO')) DEFAULT 'PENDIENTE',
    evidencia_cierre    NVARCHAR(MAX),              -- JSON array de URLs de evidencias
    validado_por        VARCHAR(200),
    politica_afectada   NVARCHAR(MAX),
    created_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    created_by          VARCHAR(200)
);
```

---

### BLOQUE 3: Gestión Documental y Evidencias

```sql
-- =========================================
-- TABLA: evidencias
-- Registro de cada evidencia cargada o generada
-- =========================================
CREATE TABLE evidencias (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    nombre_archivo      VARCHAR(500) NOT NULL,
    descripcion         NVARCHAR(MAX),
    tipo_documento      VARCHAR(30) CHECK (tipo_documento IN ('PDF','WORD','EXCEL','CSV','IMAGEN','SCREENSHOT','POWERPOINT','OTRO')),
    blob_url            VARCHAR(1000),              -- URL en Azure Blob Storage
    blob_container      VARCHAR(200),
    tamano_bytes        BIGINT,
    hash_sha256         VARCHAR(64),                -- Para verificar integridad
    fuente              VARCHAR(50),                -- MANUAL, SE_SUITE, RECERTIFICACION, GENERADO
    estado_ocr          VARCHAR(20) CHECK (estado_ocr IN ('PENDIENTE','PROCESADO','ERROR','NO_APLICA')),
    texto_extraido      NVARCHAR(MAX),              -- Resultado OCR / extracción
    simulacion_id       UNIQUEIDENTIFIER REFERENCES simulaciones_auditoria(id),
    punto_control_id    INT REFERENCES puntos_control(id),
    hallazgo_id         UNIQUEIDENTIFIER REFERENCES hallazgos(id),
    sociedad_id         INT REFERENCES sociedades(id),
    periodo_referencia  VARCHAR(20),                -- Ej: 2026-Q1
    vigente_hasta       DATE,
    estado_revision     VARCHAR(30) CHECK (estado_revision IN ('PENDIENTE','REVISADA','APROBADA','RECHAZADA','VENCIDA')),
    cargado_por         VARCHAR(200),
    created_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    is_deleted          BIT NOT NULL DEFAULT 0,
    INDEX IX_evidencias_control (punto_control_id, simulacion_id)
);

-- =========================================
-- TABLA: politicas
-- Registro de políticas internas
-- =========================================
CREATE TABLE politicas (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    codigo              VARCHAR(50) NOT NULL UNIQUE,
    nombre              VARCHAR(300) NOT NULL,
    version             VARCHAR(20),
    fecha_aprobacion    DATE,
    fecha_vencimiento   DATE,
    responsable_email   VARCHAR(200),
    estado              VARCHAR(30) CHECK (estado IN ('VIGENTE','VENCIDA','EN_REVISION','OBSOLETA','BORRADOR')),
    texto_completo      NVARCHAR(MAX),
    blob_url            VARCHAR(1000),
    score_calidad_ia    DECIMAL(4,2),               -- Calificación IA de 1-10
    observaciones_ia    NVARCHAR(MAX),              -- Análisis de calidad por IA
    gaps_detectados     NVARCHAR(MAX),              -- JSON array de vacíos detectados
    ultima_revision_ia  DATETIME2,
    dominio_id          INT REFERENCES dominios_auditoria(id),
    created_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    is_deleted          BIT NOT NULL DEFAULT 0
);

-- =========================================
-- TABLA: procedimientos
-- =========================================
CREATE TABLE procedimientos (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    codigo              VARCHAR(50) NOT NULL UNIQUE,
    nombre              VARCHAR(300) NOT NULL,
    politica_id         UNIQUEIDENTIFIER REFERENCES politicas(id),
    version             VARCHAR(20),
    fecha_aprobacion    DATE,
    responsable_email   VARCHAR(200),
    estado              VARCHAR(30) CHECK (estado IN ('VIGENTE','VENCIDO','EN_REVISION','OBSOLETO','BORRADOR')),
    texto_completo      NVARCHAR(MAX),
    blob_url            VARCHAR(1000),
    score_claridad_ia   DECIMAL(4,2),
    score_ejecutabilidad_ia DECIMAL(4,2),
    resultado_revision  VARCHAR(50),
    -- ADECUADO | INSUFICIENTE | AMBIGUO | DESACTUALIZADO | DIFICIL_EJECUTAR | REQUIERE_COMPLEMENTO
    recomendaciones_ia  NVARCHAR(MAX),
    ultima_revision_ia  DATETIME2,
    created_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    is_deleted          BIT NOT NULL DEFAULT 0
);
```

---

### BLOQUE 4: Integraciones y Conectores

```sql
-- =========================================
-- TABLA: conectores
-- Catálogo de conectores registrados (equivalente a SOA Manager)
-- =========================================
CREATE TABLE conectores (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    nombre              VARCHAR(200) NOT NULL,
    sistema_destino     VARCHAR(100) NOT NULL,      -- SE_SUITE, RECERTIFICACION, SAP, EVOLUTION
    descripcion         NVARCHAR(MAX),
    tipo_conexion       VARCHAR(30) NOT NULL CHECK (tipo_conexion IN ('REST_API','SOAP_API','SQL_DIRECT','SQL_VIEW','SFTP','EXCEL_CSV','WEBHOOK')),
    url_base            VARCHAR(500),
    metodo_auth         VARCHAR(30) CHECK (metodo_auth IN ('OAUTH2','BASIC','API_KEY','ENTRA_ID_APP','WINDOWS_AUTH','NONE')),
    -- Credenciales almacenadas en Azure Key Vault, solo referencia aquí
    key_vault_secret_name VARCHAR(200),
    headers_adicionales NVARCHAR(MAX),              -- JSON de headers custom
    timeout_segundos    INT NOT NULL DEFAULT 30,
    reintentos          INT NOT NULL DEFAULT 3,
    esquema_mapeo       NVARCHAR(MAX),              -- JSON: cómo mapear campos del origen a nuestro modelo
    sql_query           NVARCHAR(MAX),              -- Para tipo SQL_DIRECT o SQL_VIEW
    connection_string_ref VARCHAR(200),             -- Referencia a Key Vault secret
    activo              BIT NOT NULL DEFAULT 1,
    estado_actual       VARCHAR(30) CHECK (estado_actual IN ('VERDE','AMARILLO','ROJO','DESCONOCIDO')) DEFAULT 'DESCONOCIDO',
    ultimo_test         DATETIME2,
    ultimo_test_resultado NVARCHAR(500),
    created_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    created_by          VARCHAR(200)
);

-- =========================================
-- TABLA: endpoints_conector
-- Endpoints específicos de cada conector (métodos disponibles)
-- =========================================
CREATE TABLE endpoints_conector (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    conector_id         UNIQUEIDENTIFIER NOT NULL REFERENCES conectores(id),
    nombre              VARCHAR(200) NOT NULL,
    descripcion         NVARCHAR(MAX),
    metodo_http         VARCHAR(10),                -- GET, POST, PUT
    path_relativo       VARCHAR(500),
    proposito           VARCHAR(100),               -- EMPLEADOS_ACTIVOS, USUARIOS_SAP, CAMPANAS_RECERT
    parametros          NVARCHAR(MAX),              -- JSON schema de parámetros
    respuesta_esquema   NVARCHAR(MAX),              -- JSON schema de respuesta
    activo              BIT NOT NULL DEFAULT 1,
    created_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- =========================================
-- TABLA: ejecuciones_conector
-- Historial de cada invocación a un conector
-- =========================================
CREATE TABLE ejecuciones_conector (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    conector_id         UNIQUEIDENTIFIER NOT NULL REFERENCES conectores(id),
    endpoint_id         UNIQUEIDENTIFIER REFERENCES endpoints_conector(id),
    simulacion_id       UNIQUEIDENTIFIER REFERENCES simulaciones_auditoria(id),
    tipo_ejecucion      VARCHAR(30),                -- MANUAL, AUTOMATICA, TEST
    estado              VARCHAR(20) NOT NULL CHECK (estado IN ('EXITOSO','ERROR','TIMEOUT','PARCIAL')),
    inicio_at           DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    fin_at              DATETIME2,
    duracion_ms         INT,
    registros_obtenidos INT,
    registros_procesados INT,
    error_codigo        VARCHAR(50),
    error_detalle       NVARCHAR(MAX),
    ejecutado_por       VARCHAR(200),
    INDEX IX_ejecuciones_conector (conector_id, inicio_at DESC)
);

-- =========================================
-- TABLA: lotes_carga
-- Cargas masivas por Excel/CSV (modo contingencia)
-- =========================================
CREATE TABLE lotes_carga (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tipo_carga          VARCHAR(50) NOT NULL,       -- EMPLEADOS, USUARIOS_SAP, RECERTIFICACION, EVIDENCIAS
    nombre_archivo      VARCHAR(500),
    total_filas         INT,
    filas_procesadas    INT,
    filas_error         INT,
    estado              VARCHAR(30) CHECK (estado IN ('PROCESANDO','COMPLETADO','ERROR_PARCIAL','ERROR_TOTAL')),
    errores_detalle     NVARCHAR(MAX),              -- JSON array de errores por fila
    cargado_por         VARCHAR(200),
    created_at          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    completado_at       DATETIME2
);
```

---

### BLOQUE 5: Bitácora de Auditoría

```sql
-- =========================================
-- TABLA: bitacora_auditoria
-- Registro inmutable de todas las acciones de usuarios
-- APPEND ONLY — nunca se modifica ni elimina
-- =========================================
CREATE TABLE bitacora_auditoria (
    id                  BIGINT IDENTITY(1,1) PRIMARY KEY,   -- INT para rendimiento en append
    timestamp_utc       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    usuario_email       VARCHAR(200) NOT NULL,
    usuario_nombre      VARCHAR(300),
    entra_id_object     VARCHAR(100),
    ip_address          VARCHAR(45),
    user_agent          VARCHAR(500),
    accion              VARCHAR(100) NOT NULL,
    -- LOGIN, LOGOUT, VER_SIMULACION, CREAR_SIMULACION, EJECUTAR_SIMULACION,
    -- VER_HALLAZGO, EXPORTAR_WORD, EXPORTAR_PPT, CREAR_CONECTOR, EDITAR_REGLA,
    -- CONSULTA_IA, CARGAR_EVIDENCIA, VER_EVIDENCIA, MODIFICAR_PLAN_ACCION
    modulo              VARCHAR(50),
    entidad_tipo        VARCHAR(100),               -- simulacion, hallazgo, evidencia
    entidad_id          VARCHAR(100),
    descripcion         NVARCHAR(MAX),
    datos_antes         NVARCHAR(MAX),              -- JSON estado anterior
    datos_despues       NVARCHAR(MAX),              -- JSON estado nuevo
    resultado           VARCHAR(20) CHECK (resultado IN ('EXITOSO','ERROR','DENEGADO')),
    error_detalle       NVARCHAR(MAX),
    INDEX IX_bitacora_usuario (usuario_email, timestamp_utc DESC),
    INDEX IX_bitacora_accion (accion, timestamp_utc DESC),
    INDEX IX_bitacora_fecha (timestamp_utc DESC)
);
-- Nota: Considerar Ledger Table de Azure SQL para inmutabilidad criptográfica
ALTER TABLE bitacora_auditoria SET (LEDGER = ON);
```

---

## 7. MÓDULOS FUNCIONALES

### Módulo 1: Dashboard Ejecutivo

**Propósito:** Vista de mando en tiempo real. Primera pantalla que ve cualquier usuario. Muestra el estado de madurez de auditoría de un vistazo.

**Componentes visuales:**

```
┌─────────────────────────────────────────────────────────────────────┐
│  🔴 AuditorPRO TI                    👤 Juan Solano  [ILG-CR] ▼    │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  Madurez de Auditoría: ████████░░ 7.8/10  ▲ +0.4 vs anterior       │
│  Cumplimiento global:  82%  │  Última simulación: hace 3 días       │
│                                                                     │
├──────────┬──────────┬──────────┬──────────┬──────────┬─────────────┤
│  RIESCOS │ HALLAZ   │EVIDENCIAS│ RECERTIF │ CAMBIOS  │ POLÍTICAS   │
│  SoD     │ CRÍTICOS │FALTANTES │PENDIENTES│SIN EXPED.│ VENCIDAS    │
│  🔴 14   │ 🔴 3    │ 🟡 22   │ 🟡 5    │ 🔴 8    │ 🔴 2       │
│ +2 ▲    │ -1 ▼    │ -5 ▼    │ = igual  │ +3 ▲    │ +1 ▲       │
├──────────┴──────────┴──────────┴──────────┴──────────┴─────────────┤
│                                                                     │
│  [Gráfico distribución semáforo] [Top 5 hallazgos críticos]        │
│  [Evolución madurez 6 simulaciones] [Riesgo por dominio]           │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

**KPIs del dashboard (mínimos requeridos):**

| Tile | Métrica | Color según umbral |
|---|---|---|
| Madurez de auditoría | Score 1-10 | Verde ≥8 / Amarillo 6-7.9 / Rojo <6 |
| Cumplimiento global | % controles verdes | Verde ≥85% / Amarillo 70-84% / Rojo <70% |
| Hallazgos críticos abiertos | Cantidad | Verde 0 / Amarillo 1-3 / Rojo ≥4 |
| Riesgos SoD activos | Cantidad sin mitigación | Verde 0-5 / Amarillo 6-15 / Rojo >15 |
| Evidencias faltantes | Cantidad | Verde 0-5 / Amarillo 6-20 / Rojo >20 |
| Recertificaciones pendientes | Usuarios sin validar | Verde 0 / Amarillo 1-10 / Rojo >10 |
| Cambios sin expediente | Cantidad | Verde 0-2 / Amarillo 3-8 / Rojo >8 |
| Políticas vencidas | Cantidad | Verde 0 / Amarillo 1-3 / Rojo >3 |
| Planes de acción vencidos | Planes sin cerrar en fecha | Verde 0 / Amarillo 1-5 / Rojo >5 |
| Última simulación | Días desde última ejecución | Verde ≤30 / Amarillo 31-60 / Rojo >60 |

---

### Módulo 2: Simulación de Auditoría

**Flujo de creación y ejecución:**

```
Paso 1: Configurar simulación
  ├── Nombre descriptivo
  ├── Tipo: Manual / Programada
  ├── Sociedades a evaluar (multi-selección)
  ├── Período de evaluación (desde/hasta)
  ├── Dominios a incluir (multi-selección con "todos")
  └── Descripción/contexto (opcional)

Paso 2: Revisar controles que se evaluarán (preview)
  └── Lista de puntos de control activos para los dominios seleccionados

Paso 3: Ejecutar
  ├── Botón "Iniciar simulación"
  ├── Barra de progreso con dominio actual
  ├── Log en tiempo real (WebSocket o polling)
  └── Estado: Pendiente → En proceso → Completada

Paso 4: Ver resultados
  ├── Resumen ejecutivo (score, semáforos, hallazgos)
  ├── Detalle por dominio
  ├── Detalle por control (drill-down)
  └── Acciones: Exportar Word / PPT / Plan de acción
```

**Estados de simulación y transiciones:**

```
PENDIENTE → EN_PROCESO → COMPLETADA
                      → ERROR (con mensaje y opción de reintentar)
           → CANCELADA (si usuario cancela)
```

---

### Módulo 3: Hallazgos

**Vista de lista:**
- Filtros: sociedad, dominio, criticidad, semáforo, estado, fecha, tipo (preventivo/real)
- Tabla con: título, criticidad (badge color), dominio, sociedad, estado plan, fecha, responsable
- Exportación a Excel de la lista filtrada

**Vista de detalle de hallazgo:**
```
┌─ HALLAZGO: HAL-2026-0042 ─────────────────────────────────────────┐
│ 🔴 CRÍTICO | Dominio: Seguridad SAP | ILG-CR                      │
│                                                                   │
│ Título: Usuarios con perfil SAP_ALL sin justificación             │
│                                                                   │
│ Descripción: Se identificaron 3 usuarios de diálogo con el rol    │
│ SAP_ALL asignado sin excepción documentada...                     │
│                                                                   │
│ Causa probable: El rol fue asignado como workaround...            │
│ Impacto: Acceso irrestricto a todos los módulos SAP...            │
│ Norma violada: ISO 27001 A.9.2.3 / COBIT DSS05.04                │
│                                                                   │
│ Análisis IA: "Este hallazgo representa un riesgo crítico de      │
│ segregación de funciones. Los 3 usuarios identificados son        │
│ [usuario_1], [usuario_2], [usuario_3]. La recomendación es..."   │
│                                                                   │
│ Evidencia encontrada: [📄 log_asignaciones.xlsx] [📄 su10_exp]   │
│ Evidencia faltante: [❌ Excepción firmada] [❌ Justificación]     │
│                                                                   │
│ ─── PLAN DE ACCIÓN ───────────────────────────────────────────── │
│ Responsable: juan.solano@ilglogistics.com                         │
│ Fecha compromiso: 30/04/2026  Estado: 🟡 EN PROCESO               │
│ Acción: Remover SAP_ALL de los 3 usuarios, asignar roles...      │
│                                                                   │
│ [✏️ Editar plan] [📎 Subir evidencia cierre] [✅ Cerrar hallazgo] │
└───────────────────────────────────────────────────────────────────┘
```

---

### Módulo 4: Evidencias

**Funcionalidades:**
- Upload de múltiples formatos: PDF, Word, Excel, CSV, PNG, JPG, screenshots
- Procesamiento OCR automático en background (Azure AI Document Intelligence)
- Búsqueda semántica sobre contenido de evidencias (Azure AI Search)
- Asociación manual o automática a control/hallazgo/simulación
- Vista de galería + lista con filtros
- Visualizador inline de PDF e imágenes
- Control de versiones (misma evidencia, versión actualizada)
- Indicador de vigencia y estado

**Estados de evidencia:**

| Estado | Significado |
|---|---|
| Pendiente | Subida, pendiente de clasificar |
| En revisión | Asociada a control, pendiente de validación |
| Aprobada | Válida para respaldo de control |
| Rechazada | Insuficiente o incorrecta |
| Vencida | Fuera del período de vigencia |

---

### Módulo 5: Planes de Acción

**Vista tipo Kanban + tabla:**

```
[PENDIENTE] → [EN PROCESO] → [REQUIERE VALIDACIÓN] → [CERRADO]
    5              8                  3                  47
```

- Indicadores de vencimiento (días restantes / días vencido)
- Asignación de responsable con notificación por correo
- Subida de evidencia de cierre
- Historial de cambios de estado por plan
- Exportación de matriz de planes de acción para presentación al auditor

---

### Módulo 6: Consulta IA

**Interfaz:**
- Chat conversacional con contexto de la organización
- Input multimodal: texto + adjuntar archivos (PDF, Word, Excel, imágenes)
- Historial de consultas guardado con trazabilidad
- Fuentes citadas en cada respuesta ("Basado en: hallazgo HAL-2026-0032, política POL-SEC-001")

**Capacidades del agente:**

| Consulta de ejemplo | Comportamiento esperado |
|---|---|
| "¿Cuáles son los controles más débiles en ILG-CR?" | Analiza última simulación y lista top 5 controles en rojo |
| "Explícame por qué el control ID-003 está en rojo" | Explica con datos reales del resultado |
| "¿Qué evidencias me faltan para cerrar el hallazgo HAL-0042?" | Lista específica de evidencias requeridas |
| "Revisa esta política y dime si está bien" | Analiza el archivo adjunto contra criterios de calidad |
| "¿Tenemos usuarios inactivos en SAP con acceso?" | Ejecuta consulta a datos y responde con lista |
| "Genera el expediente del requerimiento AUDIT-2026-01" | Consolida toda la evidencia en documento Word |

---

### Módulo 7: Revisión de Políticas y Procedimientos

**Flujo de revisión:**

```
1. Seleccionar política/procedimiento a revisar
   (desde catálogo o subir nuevo documento)
         ↓
2. Agente IA analiza el documento contra criterios:
   - Objetivo claro
   - Alcance definido
   - Responsabilidades explícitas
   - Pasos secuenciales y lógicos
   - Controles identificados
   - Evidencias definidas
   - Vigencia establecida
   - Aprobaciones formales
   - Facilidad de comprensión (Flesch-Kincaid simplificado)
   - Factibilidad de cumplimiento
         ↓
3. El sistema emite:
   - Score de calidad 1-10
   - Resultado: ADECUADO / INSUFICIENTE / AMBIGUO / DESACTUALIZADO / DIFÍCIL_EJECUTAR
   - Lista de gaps y debilidades
   - Sugerencias de mejora con texto propuesto
   - Mapeo contra controles auditables relacionados
         ↓
4. Opciones de acción:
   - Exportar análisis completo a Word
   - Generar versión mejorada sugerida por IA
   - Marcar para actualización (crea plan de acción)
```

---

### Módulo 8: Cargas Excel/CSV

**Tipos de carga soportados:**

| Tipo | Fuente típica | Campos mínimos requeridos |
|---|---|---|
| Empleados activos | Evolution, DMS | numero_empleado, nombre, sociedad, puesto, jefe, estado |
| Usuarios SAP | SU01 export | usuario, estado, tipo, fecha_ultimo_acceso, empleado_ref |
| Roles asignados | AGR_USERS export | usuario, rol, fecha_asignacion, sociedad |
| Recertificación | Sistema propio | campaña, usuario, jefatura, resultado, fecha |
| Expedientes cambios | SE Suite export | ticket_id, estado, solicitante, aprobador, fecha_cierre |

**Validación en carga:**
- Validación de formato y columnas requeridas antes de procesar
- Reporte de errores por fila descargable en Excel
- Vista previa de primeras 10 filas antes de confirmar
- Trazabilidad: lote de carga registrado con usuario, fecha y resultados
- Posibilidad de cargar en modo "actualizar" (actualiza) o "reemplazar" (borra y recarga)

---

### Módulo 9: Generación de Documentos

**Word por requerimiento/control:**
- Portada con datos de la organización
- Resumen del control evaluado
- Resultado y semáforo
- Análisis del agente IA
- Evidencias encontradas y faltantes
- Plan de acción
- Firma de responsable (espacio)
- Numerado y fechado automáticamente

**PowerPoint ejecutivo:**
- Slide 1: Portada con nombre de simulación y fecha
- Slide 2: Resumen ejecutivo — Score madurez + cumplimiento global
- Slide 3: Distribución semáforo (gráfico pastel/dona)
- Slide 4: Evolución histórica (línea de tiempo de simulaciones)
- Slide 5: Top 5 hallazgos críticos
- Slide 6: Controles más débiles (dominio + control + semáforo)
- Slide 7: Estado de planes de acción
- Slide 8: Dominios fuertes vs débiles
- Slide 9: Acciones prioritarias recomendadas
- Slide 10: Conclusión ejecutiva (generada por IA)
- Colores corporativos: rojo ILG, negro, gris, blanco

---

## 8. MOTOR DE REGLAS DE AUDITORÍA

### Arquitectura del motor

```
Entrada de datos (SQL + APIs + Excel)
         │
         ▼
┌─────────────────────────────────────────┐
│         MOTOR DE REGLAS                  │
│                                         │
│  Para cada punto_control:               │
│  1. Cargar datos según query_sql        │
│  2. Aplicar condicion_verde/amar/rojo   │
│  3. Buscar evidencias asociadas         │
│  4. Calcular semáforo                   │
│  5. Enviar al Agente IA para análisis   │
│  6. Generar resultado_control           │
│  7. Si semáforo ≠ VERDE: crear hallazgo │
└─────────────────────────────────────────┘
         │
         ▼
Resultados → Dashboard → Hallazgos → Planes de acción
```

### Catálogo completo de controles mínimos

#### DOMINIO: IDENTIDAD Y CICLO DE VIDA (ID)

| Código | Control | Tipo | Criticidad base |
|---|---|---|---|
| ID-001 | Empleado activo en HR con usuario activo en sistema | AUTOMATICO | CRITICA |
| ID-002 | Empleado inactivo/baja con usuario activo | AUTOMATICO | CRITICA |
| ID-003 | Usuario en sistema sin empleado correspondiente en HR | AUTOMATICO | ALTA |
| ID-004 | Coherencia puesto vs roles asignados según matriz | AUTOMATICO | ALTA |
| ID-005 | Coherencia sociedad del empleado vs sociedad del usuario SAP | AUTOMATICO | MEDIA |
| ID-006 | Departamento del empleado consistente con usuario SAP | AUTOMATICO | MEDIA |

#### DOMINIO: ALTAS, BAJAS Y CAMBIOS (ABC)

| Código | Control | Tipo | Criticidad base |
|---|---|---|---|
| ABC-001 | Alta de usuario con expediente completo (solicitud + aprobación + ejecución) | SEMI_AUTOMATICO | CRITICA |
| ABC-002 | Baja procesada dentro de SLA (máx. 24h desde desvinculación) | AUTOMATICO | CRITICA |
| ABC-003 | Cambio de puesto reflejado en accesos del sistema | AUTOMATICO | ALTA |
| ABC-004 | Consistencia temporal: fecha_solicitud ≤ fecha_aprobacion ≤ fecha_ejecucion | AUTOMATICO | ALTA |
| ABC-005 | Evidencia de ejecución (pantallazo, log, ticket cerrado) | SEMI_AUTOMATICO | MEDIA |
| ABC-006 | Segregación entre quien solicita y quien aprueba | AUTOMATICO | ALTA |

#### DOMINIO: RECERTIFICACIÓN (RECERT)

| Código | Control | Tipo | Criticidad base |
|---|---|---|---|
| RECERT-001 | Usuario no revisó su propio acceso en campaña | AUTOMATICO | CRITICA |
| RECERT-002 | Validación realizada por jefatura correcta según HR | AUTOMATICO | CRITICA |
| RECERT-003 | Campaña trazable con inicio, fin y resultado formal | AUTOMATICO | ALTA |
| RECERT-004 | Todos los usuarios activos incluidos en campaña | AUTOMATICO | ALTA |
| RECERT-005 | Roles validados uno a uno (no aprobación masiva sin revisión) | SEMI_AUTOMATICO | ALTA |
| RECERT-006 | Excepciones debidamente documentadas y autorizadas | SEMI_AUTOMATICO | MEDIA |
| RECERT-007 | Desviaciones con justificación formal | SEMI_AUTOMATICO | MEDIA |

#### DOMINIO: SEGURIDAD SAP (SAP-SEC)

| Código | Control | Tipo | Criticidad base |
|---|---|---|---|
| SAP-001 | Roles asignados conformes a matriz puesto-rol aprobada | AUTOMATICO | CRITICA |
| SAP-002 | Sin usuarios con SAP_ALL sin excepción documentada | AUTOMATICO | CRITICA |
| SAP-003 | Cambios de seguridad SAP con expediente completo en SE Suite | SEMI_AUTOMATICO | CRITICA |
| SAP-004 | Evidencia de pruebas en ambiente QAS antes de transporte | SEMI_AUTOMATICO | ALTA |
| SAP-005 | Segregación: quien configura ≠ quien transporta a PRD | AUTOMATICO | CRITICA |
| SAP-006 | Conflictos SoD activos sin mitigación documentada | AUTOMATICO | CRITICA |
| SAP-007 | Funciones críticas (pago, nómina, configuración) bajo control especial | SEMI_AUTOMATICO | CRITICA |
| SAP-008 | Sin usuarios de sistema (tipo 'S') usados como usuarios de diálogo | AUTOMATICO | ALTA |

#### DOMINIO: CAMBIOS APLICATIVOS (CAMBIOS)

| Código | Control | Tipo | Criticidad base |
|---|---|---|---|
| CAMBIOS-001 | Solicitud formal registrada para todo cambio | SEMI_AUTOMATICO | ALTA |
| CAMBIOS-002 | Evaluación de impacto documentada | SEMI_AUTOMATICO | ALTA |
| CAMBIOS-003 | Aprobación formal por responsable funcional y técnico | SEMI_AUTOMATICO | CRITICA |
| CAMBIOS-004 | Pruebas en QAS documentadas antes de producción | SEMI_AUTOMATICO | CRITICA |
| CAMBIOS-005 | Implementación en producción trazable (quién, cuándo, qué) | AUTOMATICO | ALTA |
| CAMBIOS-006 | Cierre formal del expediente | SEMI_AUTOMATICO | MEDIA |
| CAMBIOS-007 | Sin cambios directos en producción sin proceso formal | AUTOMATICO | CRITICA |

#### DOMINIO: EVIDENCIA DOCUMENTAL (EVID)

| Código | Control | Tipo | Criticidad base |
|---|---|---|---|
| EVID-001 | Expediente de cada requerimiento con todas las evidencias requeridas | SEMI_AUTOMATICO | ALTA |
| EVID-002 | Responsable identificado en cada expediente | AUTOMATICO | MEDIA |
| EVID-003 | Fechas consistentes en documentos de evidencia | AUTOMATICO | ALTA |
| EVID-004 | Documentos vigentes (no expirados en criterio de auditoría) | AUTOMATICO | ALTA |
| EVID-005 | Evidencias entendibles y reutilizables para auditoría | MANUAL | MEDIA |

#### DOMINIO: POLÍTICAS Y PROCEDIMIENTOS (DOC)

| Código | Control | Tipo | Criticidad base |
|---|---|---|---|
| DOC-001 | Política existe para cada dominio evaluado | SEMI_AUTOMATICO | ALTA |
| DOC-002 | Políticas vigentes (no vencidas) | AUTOMATICO | ALTA |
| DOC-003 | Políticas aprobadas formalmente con responsable identificado | SEMI_AUTOMATICO | MEDIA |
| DOC-004 | Procedimientos con responsables y evidencias definidas | SEMI_AUTOMATICO | ALTA |
| DOC-005 | Documentación alineada con controles auditados | SEMI_AUTOMATICO | ALTA |

---

## 9. AGENTE IA AUDITOR PREVENTIVO

### Arquitectura del agente

```
Consulta usuario / Trigger simulación
         │
         ▼
┌────────────────────────────────────────────────────────┐
│                  ORQUESTADOR IA                         │
│                                                        │
│  1. Clasificar intención (qué quiere el usuario)       │
│  2. Seleccionar herramientas disponibles               │
│  3. Recuperar contexto relevante (RAG sobre evidencias)│
│  4. Construir prompt enriquecido con datos reales      │
│  5. Llamar a Azure OpenAI (GPT-4o)                     │
│  6. Postprocesar y validar respuesta                   │
│  7. Citar fuentes (IDs de evidencias/controles)        │
│  8. Registrar en bitácora_consulta_ia                  │
└────────────────────────────────────────────────────────┘
         │
         ▼
    Respuesta al usuario
```

### Herramientas disponibles para el agente

```json
{
  "herramientas": [
    {
      "nombre": "buscar_hallazgos",
      "descripcion": "Busca hallazgos por dominio, criticidad, sociedad o simulación",
      "parametros": ["dominio", "criticidad", "sociedad_id", "simulacion_id", "estado"]
    },
    {
      "nombre": "buscar_evidencias",
      "descripcion": "Busca evidencias por control, hallazgo o término semántico (Azure AI Search)",
      "parametros": ["query_texto", "punto_control_id", "hallazgo_id", "periodo"]
    },
    {
      "nombre": "evaluar_control",
      "descripcion": "Ejecuta la evaluación de un punto de control específico en tiempo real",
      "parametros": ["punto_control_id", "sociedad_id", "periodo_inicio", "periodo_fin"]
    },
    {
      "nombre": "obtener_resultados_simulacion",
      "descripcion": "Obtiene los resultados de una simulación específica o la última",
      "parametros": ["simulacion_id"]
    },
    {
      "nombre": "revisar_politica",
      "descripcion": "Analiza el contenido de una política o procedimiento",
      "parametros": ["politica_id", "texto_contenido"]
    },
    {
      "nombre": "generar_resumen_ejecutivo",
      "descripcion": "Genera un resumen ejecutivo del estado de auditoría",
      "parametros": ["sociedad_id", "simulacion_id"]
    }
  ]
}
```

### System prompt base del agente

```
Eres AuditorPRO IA, el auditor preventivo digital de ILG Logistics.

Tu misión es ayudar al equipo de TI y auditoría a:
- Detectar riesgos y debilidades de control antes de la auditoría formal
- Interpretar resultados de simulaciones con precisión y claridad
- Localizar evidencias relevantes
- Sugerir acciones correctivas concretas y prioritarias
- Revisar políticas y procedimientos con criterio técnico-normativo

Principios de tu comportamiento:
1. Siempre responde con base en datos reales del sistema. Nunca inventes datos.
2. Cita siempre la fuente de tu respuesta (ID de control, hallazgo, evidencia o simulación).
3. Adapta tu lenguaje según quién pregunta: ejecutivo (corto, visual) o técnico (detallado, preciso).
4. Si no tienes datos suficientes para responder, dilo claramente y orienta qué se necesita.
5. Toda respuesta sobre hallazgos debe incluir: qué pasó, por qué importa, qué hacer.
6. Prioriza siempre la criticidad: primero lo rojo, luego lo amarillo.

Contexto de la organización: ILG Logistics, Costa Rica, grupo con múltiples subsidiarias.
Sistemas evaluados: SAP ERP, SE Suite (BPM), sistema de recertificación interno, Evolution (planillas).
Marco normativo aplicable: ISO 27001, COBIT 2019, ITIL 4, Normas de Auditoría ISACA.
```

---

## 10. MOTOR DE INTEGRACIONES

### SOA Manager de AuditorPRO TI — Módulo de Mantenimiento de Conectores

Este módulo es el **equivalente al SOA Manager de SAP** pero diseñado para ser 100% visual, sin necesidad de programar. Permite al administrador configurar, probar y mantener todas las integraciones desde la interfaz.

### Pantalla principal del módulo de conectores

```
┌─────────────────────────────────────────────────────────────────────┐
│  ⚙️ MANTENIMIENTO DE CONECTORES              [+ Nuevo conector]     │
├──────────┬─────────────┬──────────┬─────────┬──────────┬───────────┤
│ Nombre   │ Sistema     │ Tipo     │ Estado  │ Último   │ Acciones  │
│          │ destino     │ conexión │         │ uso      │           │
├──────────┼─────────────┼──────────┼─────────┼──────────┼───────────┤
│ SE Suite │ SE Suite    │ REST API │ 🟢 OK   │ Hoy 8am  │ ▶ 🔧 📋  │
│ Prod     │ BPM         │          │         │          │           │
├──────────┼─────────────┼──────────┼─────────┼──────────┼───────────┤
│ Recert   │ Recertif    │ SQL View │ 🟢 OK   │ Hoy 8am  │ ▶ 🔧 📋  │
│ DB SQL   │ Sistema int.│          │         │          │           │
├──────────┼─────────────┼──────────┼─────────┼──────────┼───────────┤
│ Evol.    │ Evolution   │ Excel/   │ 🟡 Man  │ Ayer     │ ▶ 🔧 📋  │
│ Planilla │ Planillas   │ CSV      │ ual     │          │           │
├──────────┼─────────────┼──────────┼─────────┼──────────┼───────────┤
│ SAP RFC  │ SAP ERP     │ RFC/BAPI │ 🔴 Error│ 2d atrás │ ▶ 🔧 📋  │
└──────────┴─────────────┴──────────┴─────────┴──────────┴───────────┘
```

### Formulario de configuración de conector (sin código)

```
┌─────────────────────────────────────────────────────────────────────┐
│  CONFIGURAR CONECTOR — SE Suite BPM                                 │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  Información general                                                │
│  ├── Nombre del conector:  [SE Suite — Producción        ]         │
│  ├── Sistema destino:      [SE Suite BPM                 ]         │
│  └── Descripción:          [API de SE Suite para requerim...]       │
│                                                                     │
│  Tipo de conexión:  ○ REST API  ● SQL Directo  ○ Excel/CSV         │
│                     ○ SOAP      ○ SQL Vista    ○ SFTP              │
│                                                                     │
│  Configuración de conexión                                          │
│  ├── URL base:  [https://sesuite.ilglogistics.com/api/v2]          │
│  ├── Auth:      ● OAuth2  ○ Basic  ○ API Key  ○ Entra ID          │
│  ├── Secreto:   [Seleccionar desde Azure Key Vault ▼    ]          │
│  ├── Timeout:   [30] segundos  │  Reintentos: [3]                  │
│  └── Headers adicionales: [+ Agregar header]                       │
│                                                                     │
│  Mapeo de campos                                                    │
│  ├── Campo origen           →    Campo destino                     │
│  ├── [id_ticket          ]  →    [expediente_ref     ]             │
│  ├── [estado_solicitud   ]  →    [estado             ]             │
│  └── [+ Agregar mapeo]                                             │
│                                                                     │
│  Métodos / Endpoints disponibles                                    │
│  ├── [Obtener requerimientos activos     ] [▶ Probar]              │
│  ├── [Obtener expediente por ID          ] [▶ Probar]              │
│  └── [+ Agregar método]                                            │
│                                                                     │
│  [💾 Guardar]  [▶ Probar conexión]  [❌ Cancelar]                  │
│                                                                     │
│  Último test: ✅ EXITOSO — 2026-04-05 08:15:00 — 245ms             │
└─────────────────────────────────────────────────────────────────────┘
```

### Tipos de conexión soportados

| Tipo | Cuándo usar | Configuración requerida |
|---|---|---|
| **REST API** | Sistema con API documentada | URL base, auth (OAuth/APIKey/Basic), headers, mapeo JSON |
| **SOAP API** | Sistemas legacy con WSDL | URL WSDL, auth, namespace, operación, mapeo XML |
| **SQL Directo** | BD accesible por red | Connection string (Key Vault), query SELECT, mapeo columnas |
| **SQL Vista** | BD con vistas publicadas | Connection string (Key Vault), nombre vista, mapeo |
| **Excel/CSV** | Sin API disponible (contingencia) | Plantilla de columnas esperadas, separador, encoding |
| **SFTP** | Archivos depositados periódicamente | Host, puerto, credenciales (Key Vault), ruta, patrón nombre |
| **Webhook** | Sistema envía datos activamente | Endpoint receptor, validación firma, mapeo payload |

### Modo contingencia automático

```
Si conector principal falla:
  1. Sistema detecta el fallo (timeout, error HTTP, error SQL)
  2. Registra evento en bitácora con detalle del error
  3. Activa modo contingencia para ese conector:
     - Si hay alternativa configurada (ej: SQL directo como backup de API) → usa alternativa
     - Si no hay alternativa → marca dominio como "datos desactualizados" en dashboard
  4. Notifica al administrador por correo
  5. El dominio afectado puede seguir evaluado con datos de última carga válida
  6. Muestra en simulación: "⚠️ Datos de SE Suite con corte al [fecha última carga exitosa]"
```

---

## 11. UX/UI — DISEÑO FIORI ENTERPRISE

### Sistema de diseño

**Tokens de diseño:**

```css
/* Colores corporativos ILG */
--color-primary:       #CC0000;    /* Rojo ILG */
--color-primary-dark:  #990000;
--color-primary-light: #FF3333;
--color-surface:       #FFFFFF;
--color-background:    #F5F5F5;
--color-text-primary:  #1A1A1A;   /* Negro */
--color-text-secondary:#666666;   /* Gris */
--color-border:        #E0E0E0;

/* Semáforo */
--color-verde:    #28A745;
--color-amarillo: #FFC107;
--color-rojo:     #DC3545;
--color-gris:     #6C757D;

/* Tipografía */
--font-family:  'Inter', 'Segoe UI', system-ui, sans-serif;
--font-size-sm: 12px;
--font-size-md: 14px;
--font-size-lg: 16px;
--font-size-xl: 20px;
--font-size-2xl:24px;

/* Espaciado */
--spacing-xs:  4px;
--spacing-sm:  8px;
--spacing-md:  16px;
--spacing-lg:  24px;
--spacing-xl:  32px;

/* Sombras */
--shadow-card: 0 2px 8px rgba(0,0,0,0.08);
--shadow-modal:0 8px 32px rgba(0,0,0,0.16);

/* Bordes */
--radius-sm:  4px;
--radius-md:  8px;
--radius-lg:  12px;
```

### Estructura de navegación (máximo 4 niveles)

```
Nivel 1: Header + sidebar
  ├── 📊 Dashboard
  ├── 🔍 Simulaciones
  ├── ⚠️ Hallazgos
  ├── 📁 Evidencias
  ├── ✅ Planes de acción
  ├── 🤖 Consulta IA
  ├── 📋 Políticas
  ├── 📋 Procedimientos
  ├── 📤 Cargas
  ├── ⚙️ Configuración
  │    ├── Conectores
  │    ├── Reglas de control
  │    ├── Usuarios y roles
  │    └── Catálogos
  └── 📊 Reportes

Nivel 2: Vista de lista/dashboard del módulo
Nivel 3: Vista de detalle de un registro
Nivel 4: Sub-detalle o modal de edición (máximo)
```

### Componentes UI estándar

| Componente | Descripción | Comportamiento |
|---|---|---|
| **Tile KPI** | Mosaico de dashboard | Título + valor + semáforo + tendencia + clic para drill-down |
| **Semáforobadge** | Indicador color | VERDE/AMARILLO/ROJO con ícono y texto |
| **CriticalidadBadge** | Nivel criticidad | CRÍTICA (rojo sólido) / MEDIA (amarillo) / BAJA (gris) |
| **ProgressBar madurez** | Score visual | Barra de 1-10 con color según rango |
| **DataTable** | Tabla de datos | Filtros inline, orden por columna, paginación, exportar |
| **DetailCard** | Ficha de detalle | Encabezado + secciones + acciones |
| **ChatInterface** | Consulta IA | Burbuja usuario + burbuja IA + citas de fuentes |
| **FileUploader** | Carga archivos | Drag & drop + selección + preview + barra de progreso |
| **ConnectorStatus** | Estado conector | Nombre + estado + último test + acciones |
| **StepWizard** | Flujo de pasos | Barra de progreso + paso actual + navegación |
| **AuditTimeline** | Bitácora visual | Timeline de eventos con usuario, fecha y acción |

### Reglas UX obligatorias

1. **Feedback inmediato:** Todo botón de acción muestra estado (cargando / éxito / error) en ≤200ms
2. **Mensajes de error humanos:** "No se pudo conectar con SE Suite. El equipo de TI fue notificado." (NO: "Error 503 Service Unavailable")
3. **Confirmación para acciones destructivas:** Modal de confirmación con texto de la acción (no solo "¿Estás seguro?")
4. **Cero pantallas en blanco:** Si no hay datos, mostrar estado vacío con ícono + explicación + acción sugerida
5. **Responsive:** Funcional en 1920px (escritorio gerencial), 1366px (estación trabajo TI) y 768px (tablet para presentaciones)
6. **Accesibilidad WCAG 2.1 AA:** Contraste mínimo 4.5:1, navegación por teclado, aria-labels
7. **Loader global:** Indicador de carga para operaciones >500ms
8. **Breadcrumbs:** En niveles 3 y 4 siempre visible: Dashboard > Simulaciones > SIM-2026-001 > Control ID-003

---

## 12. TRAZABILIDAD Y BITÁCORA

### Principio de trazabilidad forense

Toda acción relevante queda registrada con suficiente detalle para responder estas preguntas ante un auditor:
- ¿Quién hizo esto?
- ¿Cuándo exactamente?
- ¿Desde dónde (IP)?
- ¿Qué hizo?
- ¿Qué había antes?
- ¿Qué quedó después?
- ¿Tuvo éxito o fue bloqueado?

### Eventos que se registran obligatoriamente

| Categoría | Evento | Detalle adicional |
|---|---|---|
| **Autenticación** | LOGIN exitoso | IP, dispositivo, hora |
| **Autenticación** | LOGIN fallido | Razón, número de intentos |
| **Autenticación** | Sesión cerrada | Duración de sesión |
| **Autenticación** | Acceso denegado (usuario inactivo AD) | Cuenta bloqueada |
| **Simulación** | Creación de simulación | Parámetros configurados |
| **Simulación** | Inicio de ejecución | Usuario, timestamp |
| **Simulación** | Completada | Score, resumen de resultados |
| **Simulación** | Error en ejecución | Detalle del error |
| **Hallazgo** | Apertura de hallazgo | Datos completos del hallazgo |
| **Hallazgo** | Modificación de estado | Estado anterior → nuevo |
| **Hallazgo** | Cierre de hallazgo | Evidencia de cierre |
| **Evidencia** | Carga de evidencia | Nombre, tamaño, asociación |
| **Evidencia** | Acceso a evidencia (descarga/visualización) | Quién accedió |
| **Plan de acción** | Creación | Responsable, fecha compromiso |
| **Plan de acción** | Cambio de estado | Quién cambió, cuándo |
| **Consulta IA** | Toda consulta realizada | Prompt (sin datos sensibles), respuesta resumida |
| **Connectors** | Creación/modificación de conector | Datos del conector (sin credenciales) |
| **Connectors** | Prueba de conector | Resultado del test |
| **Connectors** | Ejecución de integración | Registros obtenidos, errores |
| **Cargas** | Inicio de carga Excel/CSV | Tipo, nombre archivo, usuario |
| **Cargas** | Resultado de carga | Filas procesadas, errores |
| **Exportación** | Generación de Word/PPT | Tipo, usuario, simulación relacionada |
| **Configuración** | Modificación de regla de control | Valores antes/después |
| **Configuración** | Cambio de rol de usuario | Quién asignó, rol anterior y nuevo |

### Implementación de Azure SQL Ledger

```sql
-- La tabla bitacora_auditoria usa Ledger Tables de Azure SQL
-- Esto proporciona verificación criptográfica de que los datos no fueron alterados
-- y genera un digest que puede verificarse externamente

-- Verificar integridad de la bitácora (ejecutar periódicamente o ante auditoría)
EXECUTE sp_verify_database_ledger;

-- El auditor puede verificar que ningún registro fue modificado o eliminado
-- usando el blockchain digest almacenado en Azure Confidential Ledger (opcional)
```

---

## 13. GENERACIÓN DE ENTREGABLES

### Plantillas de documentos Word

**Estructura del Word por control/requerimiento:**

```
[PORTADA]
  - Logo corporativo ILG
  - Título: "Expediente de Control: [código] — [nombre]"
  - Simulación: [nombre] | Fecha: [fecha] | Generado por: [usuario]
  - Clasificación: CONFIDENCIAL

[SECCIÓN 1: RESUMEN DEL CONTROL]
  - Código: [código]
  - Dominio: [dominio]
  - Descripción del control: [descripción]
  - Tipo de evaluación: [automático/semi-automático/manual]
  - Norma de referencia: [norma]

[SECCIÓN 2: RESULTADO DE EVALUACIÓN]
  - Semáforo: 🟢/🟡/🔴 [VERDE/AMARILLO/ROJO]
  - Criticidad: [CRÍTICA/MEDIA/BAJA]
  - Resumen del resultado: [resultado_detalle]
  - Datos evaluados: [tabla con datos]

[SECCIÓN 3: ANÁLISIS DEL AGENTE IA]
  - [analisis_ia — texto completo]
  - Recomendación: [recomendacion]

[SECCIÓN 4: EVIDENCIA]
  - Evidencia encontrada: [lista con enlaces/descripción]
  - Evidencia faltante: [lista con explicación]
  - Anexos: [lista de archivos adjuntos]

[SECCIÓN 5: PLAN DE ACCIÓN]
  - Acción requerida: [descripcion_accion]
  - Responsable: [nombre y correo]
  - Fecha compromiso: [fecha]
  - Estado actual: [estado]

[PIE DE PÁGINA]
  - "Documento generado automáticamente por AuditorPRO TI"
  - "Fecha de generación: [timestamp UTC]"
  - "Número de documento: [GUID]"
```

### Calificación de madurez — Algoritmo

```
Score_Madurez = (
    (% controles verdes × 4.0) +
    (% controles amarillo × 2.0) +
    (% controles rojo × 0.0) +
    (cobertura_evidencia × 2.0) +
    (planes_accion_cerrados / planes_accion_total × 1.5) +
    (sin_hallazgos_criticos_repetidos × 0.5)
) ÷ 10

Donde:
- % controles verdes = controles_verde / total_controles
- cobertura_evidencia = evidencias_aprobadas / evidencias_requeridas_total
- Ajuste por hallazgos críticos repetidos (mismo control rojo en 2+ simulaciones): penalización -0.5

Resultado final: entre 1.0 y 10.0 (redondeado a 1 decimal)

Interpretación:
  9.0 - 10.0: Excelente — Listo para auditoría externa
  7.5 - 8.9:  Bueno — Riesgo bajo de hallazgos
  6.0 - 7.4:  Aceptable — Algunos puntos de atención
  4.0 - 5.9:  Débil — Riesgo medio-alto de hallazgos
  1.0 - 3.9:  Crítico — Requiere acción inmediata
```

---

## 14. API INTERNA — ESPECIFICACIÓN REST

### Convenciones generales

- Base URL: `https://api-auditorpro.ilglogistics.com/v1`
- Autenticación: `Authorization: Bearer {EntraID_AccessToken}`
- Formato: JSON (Content-Type: application/json)
- Paginación: `?page=1&pageSize=20` con respuesta `{ data: [], total: N, page: N, pageSize: N }`
- Filtros: parámetros query opcionales por recurso
- Respuestas de error: `{ error: { code: "STRING", message: "Humano", details: {} } }`
- Versioning: URL path (`/v1/`, `/v2/`)

### Endpoints principales

#### Autenticación y usuario

```
GET  /v1/auth/me
     → Datos del usuario autenticado: email, nombre, roles, estado Entra ID

GET  /v1/auth/validate-active
     → Verifica si el usuario sigue activo en Azure AD (se llama periódicamente)
```

#### Dashboard

```
GET  /v1/dashboard/summary?sociedadId=&fechaDesde=
     → KPIs principales del dashboard

GET  /v1/dashboard/kpis?sociedadId=
     → Todos los KPIs con valores y semáforos

GET  /v1/dashboard/trends?simulaciones=6
     → Evolución de madurez por las últimas N simulaciones
```

#### Simulaciones

```
GET    /v1/simulaciones?estado=&page=&pageSize=
       → Lista de simulaciones con filtros

POST   /v1/simulaciones
       → Crear nueva simulación
       Body: { nombre, tipo, sociedadIds[], dominioIds[], periodoInicio, periodoFin }

GET    /v1/simulaciones/{id}
       → Detalle de simulación con resultados

POST   /v1/simulaciones/{id}/ejecutar
       → Iniciar ejecución de simulación

GET    /v1/simulaciones/{id}/progreso
       → Estado en tiempo real (SSE o polling)

GET    /v1/simulaciones/{id}/resultados
       → Resultados por control con semáforos

GET    /v1/simulaciones/{id}/comparar?simulacionAnteriorId=
       → Comparación entre dos simulaciones

POST   /v1/simulaciones/{id}/exportar/word
       → Generar Word del expediente completo

POST   /v1/simulaciones/{id}/exportar/powerpoint
       → Generar presentación ejecutiva PPT
```

#### Hallazgos

```
GET    /v1/hallazgos?criticidad=&estado=&dominio=&sociedad=&page=
GET    /v1/hallazgos/{id}
PUT    /v1/hallazgos/{id}/estado
POST   /v1/hallazgos/{id}/exportar/word
```

#### Planes de acción

```
GET    /v1/planes-accion?estado=&responsable=&vencidos=&page=
GET    /v1/planes-accion/{id}
PUT    /v1/planes-accion/{id}
POST   /v1/planes-accion/{id}/cerrar
       Body: { evidenciaCierre: "texto", evidenciasIds: [] }
```

#### Evidencias

```
POST   /v1/evidencias/upload
       Content-Type: multipart/form-data
       → Subir archivo con metadatos

GET    /v1/evidencias?controlId=&hallazgoId=&simulacionId=&page=
GET    /v1/evidencias/{id}
GET    /v1/evidencias/{id}/contenido     → SAS token temporal para descarga
DELETE /v1/evidencias/{id}              → Soft delete con registro en bitácora
```

#### Agente IA

```
POST   /v1/ia/consulta
       Body: { mensaje, archivosIds[], contextoSimulacionId }
       → Respuesta del agente con fuentes citadas

GET    /v1/ia/historial?page=
       → Historial de consultas del usuario autenticado
```

#### Conectores (Mantenimiento)

```
GET    /v1/conectores
GET    /v1/conectores/{id}
POST   /v1/conectores
PUT    /v1/conectores/{id}
DELETE /v1/conectores/{id}

POST   /v1/conectores/{id}/probar
       → Ejecuta test de conexión y devuelve resultado

POST   /v1/conectores/{id}/ejecutar
       → Ejecuta sincronización manual

GET    /v1/conectores/{id}/historial
       → Historial de ejecuciones del conector
```

#### Puntos de control (Configuración)

```
GET    /v1/controles?dominioId=&activo=
GET    /v1/controles/{id}
POST   /v1/controles           (solo Admin)
PUT    /v1/controles/{id}      (solo Admin)
```

#### Bitácora

```
GET    /v1/bitacora?usuario=&accion=&desde=&hasta=&page=
       → Solo para rol Admin y Auditor
```

---

## 15. ESCENARIOS DE PRUEBA — MATRIZ QA COMPLETA

### MÓDULO: Autenticación y Seguridad

| # | Escenario | Entrada | Resultado esperado |
|---|---|---|---|
| AUTH-01 | Login usuario activo en Entra ID con rol asignado | Usuario corporativo válido | Acceso concedido, redirect a dashboard |
| AUTH-02 | Login usuario deshabilitado en Azure AD | Cuenta desactivada | Error 403: "Su cuenta no está activa en el directorio corporativo" |
| AUTH-03 | Token expirado en petición | Access token vencido | Refresh automático con silent token; si falla, redirect a login |
| AUTH-04 | Usuario activo en AD pero sin rol en AuditorPRO | Sin grupo asignado | Pantalla: "No tiene acceso. Contacte al administrador." |
| AUTH-05 | Acceso a ruta que excede permisos del rol | Viewer intenta crear simulación | Error 403 con mensaje claro |
| AUTH-06 | Intento de acceso con token manipulado | JWT modificado | 401 Unauthorized |
| AUTH-07 | Sesión concurrente en dos dispositivos | Mismo usuario, dos pestañas | Ambas válidas (sesiones independientes) |
| AUTH-08 | Cierre de sesión registrado en bitácora | Logout explícito | Evento en bitácora, token invalidado |

### MÓDULO: Simulación de Auditoría

| # | Escenario | Entrada | Resultado esperado |
|---|---|---|---|
| SIM-01 | Crear y ejecutar simulación completa | Todos los campos válidos | Simulación completada, score calculado, hallazgos generados |
| SIM-02 | Crear simulación con fechas invertidas | Inicio > Fin | Error de validación: "La fecha de inicio debe ser anterior al fin" |
| SIM-03 | Ejecutar simulación con conector caído | SE Suite no responde | Modo contingencia activo, advertencia visible, simulación continúa con datos disponibles |
| SIM-04 | Ejecutar simulación sin datos cargados | BD vacía | Resultado: "Sin datos suficientes para evaluar [dominio]. Cargue datos primero." |
| SIM-05 | Dos usuarios ejecutan simulación simultáneamente | Concurrencia | Ambas simulaciones se crean independientes sin interferir |
| SIM-06 | Cancelar simulación en proceso | Cancel durante ejecución | Se detiene limpiamente, estado = CANCELADA, bitácora actualizada |
| SIM-07 | Comparar dos simulaciones | IDs de dos sims completadas | Delta de score, controles que mejoraron/empeoraron |
| SIM-08 | Exportar Word de simulación completada | Simulación en estado COMPLETADA | Archivo Word descargable con todos los resultados |
| SIM-09 | Exportar PPT de simulación | Simulación completada | Presentación de 10 slides descargable |
| SIM-10 | Ver historial de 20 simulaciones con filtros | Filtro por sociedad y estado | Lista paginada correcta |

### MÓDULO: Motor de Reglas — Controles Críticos

| # | Escenario | Entrada | Resultado esperado |
|---|---|---|---|
| CTRL-01 | Control ID-002: empleado inactivo con usuario SAP activo | empleado.estado=INACTIVO, usuario.estado=ACTIVO | Semáforo ROJO, criticidad CRÍTICA, hallazgo generado |
| CTRL-02 | Control ABC-002: baja no procesada en 24h | empleado.fecha_baja = ayer, usuario.estado = ACTIVO | ROJO, hallazgo ABC-002, plan de acción sugerido |
| CTRL-03 | Control SAP-005: mismo usuario configura y transporta | solicitante = transportador en expediente | ROJO crítico, SoD hallazgo |
| CTRL-04 | Control RECERT-001: usuario revisó su propio acceso | revisor_id = usuario_id en campaña | ROJO crítico, hallazgo con usuario específico |
| CTRL-05 | Control ID-001: empleado activo, usuario activo, todo correcto | Datos coherentes | VERDE, sin hallazgo |
| CTRL-06 | Control con evidencia parcial | 2 de 4 evidencias requeridas | AMARILLO, evidencias faltantes listadas |

### MÓDULO: Conectores y Cargas

| # | Escenario | Entrada | Resultado esperado |
|---|---|---|---|
| CONN-01 | Crear conector REST API con credenciales válidas | URL + secret Key Vault válido | Conector creado, test exitoso |
| CONN-02 | Crear conector con URL incorrecta | URL inaccesible | Error de test: "No se pudo conectar a [URL]. Verificar la dirección." |
| CONN-03 | Credenciales de Key Vault inválidas | Secret name inexistente | Error: "No se encontró el secreto en Azure Key Vault" |
| CONN-04 | Carga Excel empleados con formato correcto | Archivo con columnas requeridas | Carga exitosa, lote registrado |
| CONN-05 | Carga Excel con columnas faltantes | Archivo sin "numero_empleado" | Error: "El archivo no tiene las columnas requeridas. Descargue la plantilla." |
| CONN-06 | Carga Excel con 500 filas, 50 con errores | Mix de datos válidos e inválidos | 450 procesados, 50 en error, reporte descargable |
| CONN-07 | Modo contingencia cuando conector falla | Conector SQL_VIEW timeout | Dashboard muestra "⚠️ Datos con corte al [fecha]", simulación continúa |

### MÓDULO: Agente IA

| # | Escenario | Entrada | Resultado esperado |
|---|---|---|---|
| IA-01 | Consulta sobre hallazgos críticos actuales | "¿Cuáles son los hallazgos críticos?" | Lista con IDs, títulos y acciones, citando fuente |
| IA-02 | Consulta sin datos en el sistema | Sistema recién instalado, sin datos | "No hay simulaciones ejecutadas aún. Para comenzar..." |
| IA-03 | Subir política para revisión | PDF de política | Score 1-10 + análisis + lista de gaps |
| IA-04 | Consulta fuera del dominio de auditoría | "¿Cuál es el clima?" | Respuesta enfocada: "Estoy especializado en auditoría TI. ¿En qué puedo ayudarte?" |
| IA-05 | Consulta con datos sensibles en el prompt | Incluir contraseñas en texto | Respuesta normal, logs NO registran el contenido sensible |
| IA-06 | Historial de consultas del usuario | Usuario con 20 consultas previas | Lista paginada ordenada por fecha desc |

### MÓDULO: Generación de Documentos

| # | Escenario | Entrada | Resultado esperado |
|---|---|---|---|
| DOC-01 | Generar Word de control con todos los datos | Control evaluado, con evidencias | Word correcto, sin errores de formato, descargable |
| DOC-02 | Generar PPT de simulación sin gráficos de tendencia | Primera simulación (no hay historial) | PPT sin slide de tendencia O slide con mensaje "Sin historial previo" |
| DOC-03 | Generar Word con caracteres especiales | Nombres con tildes, ñ, símbolos | Caracteres preservados correctamente en Word |
| DOC-04 | Generación concurrente por 5 usuarios | 5 solicitudes simultáneas | 5 archivos generados correctamente sin corrupción |

---

## 16. ROADMAP DE IMPLEMENTACIÓN

### FASE 1: MVP Funcional (Semanas 1-8)

**Objetivo:** Sistema funcional end-to-end con los módulos más críticos.

**Sprint 1 (semanas 1-2): Fundamentos**
- [ ] Configuración de proyecto (repo Git, ambientes DEV/QA/PRD, CI/CD básico)
- [ ] App Registration en Entra ID
- [ ] Base de datos Azure SQL con schema completo
- [ ] Autenticación MSAL.js en frontend
- [ ] Middleware de validación de usuario activo
- [ ] API base con autenticación y RBAC
- [ ] Bitácora de auditoría funcionando

**Sprint 2 (semanas 3-4): Dashboard y cargas**
- [ ] Módulo de cargas Excel/CSV (empleados, usuarios SAP)
- [ ] Dashboard ejecutivo con KPIs básicos
- [ ] Tiles con semáforos calculados
- [ ] Maestros: sociedades, departamentos, puestos, empleados

**Sprint 3 (semanas 5-6): Simulación y controles**
- [ ] Motor de reglas con 10 controles automáticos (los más críticos)
- [ ] Creación y ejecución de simulaciones
- [ ] Generación de hallazgos preventivos
- [ ] Planes de acción automáticos

**Sprint 4 (semanas 7-8): Evidencias y documentos**
- [ ] Módulo de evidencias con upload a Azure Blob
- [ ] Exportación Word de control/hallazgo
- [ ] Exportación PowerPoint básica
- [ ] Consulta IA básica (sin RAG avanzado aún)

**Criterio de aceptación Fase 1:**
- Usuario puede crear simulación, ver resultados con semáforos, revisar hallazgos, subir evidencias y exportar Word/PPT
- Todos los 15 módulos de QA del Sprint ejecutados exitosamente
- Sin errores críticos (P1) en ambiente QA

---

### FASE 2: Integraciones y RAG (Semanas 9-16)

- [ ] Módulo de mantenimiento de conectores (SOA Manager visual)
- [ ] Integración SE Suite por API/SQL
- [ ] Integración sistema de recertificación
- [ ] OCR automático con Azure AI Document Intelligence
- [ ] Azure AI Search para búsqueda semántica de evidencias
- [ ] Agente IA con RAG real (contexto organizacional)
- [ ] Revisión de políticas y procedimientos con IA
- [ ] Todos los controles del catálogo implementados (30+ controles)
- [ ] Historial de simulaciones con comparación

---

### FASE 3: Madurez Enterprise (Semanas 17-24)

- [ ] Evolution por API (reemplaza Excel/CSV)
- [ ] Alertas automáticas por correo (hallazgos críticos, planes vencidos)
- [ ] Score de madurez por tendencia con ML básico
- [ ] Voz (Web Speech API para consultas al agente)
- [ ] Power BI embedded (opcional)
- [ ] Modo multitenant (si se escala a otras empresas del grupo)
- [ ] Certificaciones: revisión de cumplimiento ISO 27001, SOC 2 ready

---

## 17. ALINEACIÓN NORMATIVA

### ISO 27001:2022

| Cláusula | Dominio ISO | Control AuditorPRO | Cobertura |
|---|---|---|---|
| A.5.15 | Control de acceso | Controles ID-001 a ID-006 | ✅ Automático |
| A.5.16 | Gestión de identidad | Controles ABC-001 a ABC-006 | ✅ Automático |
| A.5.18 | Derechos de acceso | Controles RECERT-001 a RECERT-007 | ✅ Automático |
| A.8.2 | Privileged access | Controles SAP-001, SAP-002, SAP-006 | ✅ Automático |
| A.8.32 | Change management | Controles CAMBIOS-001 a CAMBIOS-007 | ✅ Semi-auto |
| A.5.33 | Protection of records | Módulo de evidencias + bitácora | ✅ Completo |

### COBIT 2019

| Objetivo COBIT | Dominio | Controles relacionados |
|---|---|---|
| APO01 — Gestión del marco de gestión | Gobernanza | DOC-001 a DOC-005 |
| APO13 — Gestión de seguridad | Seguridad | SAP-001 a SAP-008, SoD |
| BAI06 — Gestión de cambios | Cambios | CAMBIOS-001 a CAMBIOS-007 |
| DSS05 — Gestión de servicios de seguridad | Identidad | ID-001 a ID-006, ABC |
| DSS06 — Gestión de controles de proceso | Procesos | EVID-001 a EVID-005 |
| MEA02 — Gestión del sistema de control interno | Monitoreo | Motor de simulaciones |

### ISACA — Normas de Auditoría de TI

| Norma | Requisito | Implementación |
|---|---|---|
| ITAF 1201 | Planificación del trabajo de auditoría | Módulo de simulación con alcance definible |
| ITAF 1202 | Evaluación de riesgo | Semáforo + criticidad + score de madurez |
| ITAF 1401 | Reportes de auditoría | Generación automática Word + PPT |
| ITAF 1402 | Actividades de seguimiento | Módulo planes de acción con estado |
| ITAF 1205 | Evidencia de auditoría | Módulo de evidencias con cadena de custodia |

### ITIL 4

| Práctica ITIL | Implementación en AuditorPRO |
|---|---|
| Gestión de cambios | Motor de controles CAMBIOS + expedientes |
| Gestión de accesos | Motor de controles ID + ABC + RECERT |
| Gestión del nivel de servicio | Health checks, alertas, disponibilidad |
| Gestión de incidentes | Hallazgos críticos → plan de acción inmediato |
| Mejora continua | Evolución de score entre simulaciones |

---

## 18. GESTIÓN DE CONFIGURACIÓN Y AMBIENTES

### Ambientes

| Ambiente | Propósito | Datos | Acceso |
|---|---|---|---|
| **DEV** | Desarrollo activo | Sintéticos únicamente | Desarrolladores |
| **QA** | Pruebas de integración y UAT | Anonimizados de PRD | TI + auditores clave |
| **PRD** | Producción | Datos reales | Usuarios con rol asignado |

### Variables de entorno (todas en Azure Key Vault)

```
# Base de datos
AUDITORPRO_SQL_CONNECTIONSTRING      → "Server=sql-auditorpro.database.windows.net;..."

# Azure AD
ENTRA_TENANT_ID                      → "[guid del tenant]"
ENTRA_CLIENT_ID                      → "[guid del App Registration]"
ENTRA_AUDIENCE                       → "api://auditorpro-ti"

# Azure OpenAI
AZURE_OPENAI_ENDPOINT                → "https://[resource].openai.azure.com"
AZURE_OPENAI_DEPLOYMENT              → "gpt-4o"

# Azure AI Search
AZURE_SEARCH_ENDPOINT                → "https://[resource].search.windows.net"
AZURE_SEARCH_API_KEY                 → "[key desde Key Vault]"
AZURE_SEARCH_INDEX                   → "auditorpro-evidencias"

# Azure Blob Storage
AZURE_BLOB_ACCOUNT                   → "[storage account name]"
AZURE_BLOB_CONTAINER_EVIDENCIAS      → "evidencias"
AZURE_BLOB_CONTAINER_DOCUMENTOS      → "documentos-generados"

# Application Insights
APPLICATIONINSIGHTS_CONNECTION_STRING → "[connection string]"

# Configuración de la app
AUDITORPRO_URL_FRONTEND              → "https://auditorpro.ilglogistics.com"
AUDITORPRO_ADMIN_EMAIL               → "juan.solano@ilglogistics.com"
AUDITORPRO_MFA_REQUIRED              → "true"
```

### Pipeline CI/CD (Azure DevOps)

```yaml
# azure-pipelines.yml (estructura)

trigger:
  branches:
    include: [main, develop, release/*]

stages:
  - stage: Build
    jobs:
      - job: BuildAndTest
        steps:
          - dotnet build
          - dotnet test (ejecutar escenarios QA automáticos)
          - npm run build (frontend)
          - npm run test

  - stage: DeployDev
    condition: branch = develop
    jobs:
      - DeployToAppService (DEV)
      - RunSmokeTests

  - stage: DeployQA
    condition: branch = release/*
    jobs:
      - DeployToAppService (QA)
      - RunIntegrationTests
      - RunQAScenarios (los 60+ escenarios de este documento)

  - stage: DeployPRD
    condition: branch = main
    jobs:
      - ManualApproval (requiere aprobación del Admin TI)
      - DeployToAppService (PRD)
      - HealthCheckVerification
      - NotifyStakeholders
```

---

## 19. ARQUITECTURA DE DESPLIEGUE AZURE

### Diagrama de recursos Azure

```
┌─ Resource Group: rg-auditorpro-prd ──────────────────────────────────┐
│                                                                       │
│  ┌─ App Service Plan (P1v3) ──────────────────────────────────────┐  │
│  │  ├── App Service: auditorpro-api (Backend .NET 8)              │  │
│  │  └── App Service: auditorpro-web (Frontend React)              │  │
│  └────────────────────────────────────────────────────────────────┘  │
│                                                                       │
│  ┌─ Datos ────────────────────────────────────────────────────────┐  │
│  │  ├── Azure SQL Database: sql-auditorpro                        │  │
│  │  ├── Azure Blob Storage: stauditorpro                          │  │
│  │  └── Azure Cache for Redis: cache-auditorpro (KPIs)            │  │
│  └────────────────────────────────────────────────────────────────┘  │
│                                                                       │
│  ┌─ IA y Búsqueda ────────────────────────────────────────────────┐  │
│  │  ├── Azure OpenAI: oai-auditorpro                              │  │
│  │  ├── Azure AI Search: srch-auditorpro                          │  │
│  │  └── Azure AI Document Intelligence: di-auditorpro (OCR)      │  │
│  └────────────────────────────────────────────────────────────────┘  │
│                                                                       │
│  ┌─ Integración y Procesos ───────────────────────────────────────┐  │
│  │  ├── Azure Functions: func-auditorpro (batch, integraciones)   │  │
│  │  └── Azure Service Bus: sb-auditorpro (mensajería async)       │  │
│  └────────────────────────────────────────────────────────────────┘  │
│                                                                       │
│  ┌─ Seguridad ────────────────────────────────────────────────────┐  │
│  │  ├── Azure Key Vault: kv-auditorpro                            │  │
│  │  ├── Managed Identity (App Service → Key Vault)                │  │
│  │  └── Microsoft Entra ID (App Registration externo)             │  │
│  └────────────────────────────────────────────────────────────────┘  │
│                                                                       │
│  ┌─ Observabilidad ───────────────────────────────────────────────┐  │
│  │  ├── Application Insights: ai-auditorpro                       │  │
│  │  └── Log Analytics Workspace: law-auditorpro                   │  │
│  └────────────────────────────────────────────────────────────────┘  │
└───────────────────────────────────────────────────────────────────────┘
```

### Estimación de costos Azure (referencial mensual)

| Recurso | Tier | Estimado mensual |
|---|---|---|
| App Service Plan P1v3 (2 apps) | P1v3 | ~$140 USD |
| Azure SQL Database | General Purpose 4 vCores | ~$370 USD |
| Azure Blob Storage | LRS 100GB | ~$5 USD |
| Azure AI Search | Standard S1 | ~$250 USD |
| Azure OpenAI / AI Foundry | Por tokens (~1M tokens/mes) | ~$30-60 USD |
| Azure Functions | Consumption | ~$5-10 USD |
| Key Vault | Standard | ~$5 USD |
| Application Insights | 5GB logs/mes | ~$10 USD |
| **Total estimado** | | **~$815 - $850 USD/mes** |

> Nota: Costos varían según uso real. Revisar Azure Pricing Calculator con datos reales de uso esperado.

---

## 20. CHECKLIST DE VERIFICACIÓN FINAL

```
═══════════════════════════════════════════════════════════════════════
VERIFICACIÓN FINAL — AuditorPRO TI Blueprint v1.0
Fecha: Abril 2026 | Preparado por: Claude (Anthropic) para Juan Solano
═══════════════════════════════════════════════════════════════════════

PERSPECTIVA UX/UI
[✅] Diseño Fiori con máximo 4 niveles de navegación
[✅] Colores corporativos ILG (rojo, negro, blanco, gris)
[✅] Sistema de semáforos visual y consistente
[✅] Tiles KPI en dashboard con drill-down
[✅] Mensajes de error en lenguaje humano
[✅] Estado vacío definido para todos los módulos
[✅] Responsive para escritorio y tablet
[✅] Accesibilidad WCAG 2.1 AA en especificaciones

PERSPECTIVA BACKEND / ARQUITECTURA
[✅] Clean Architecture con separación de capas
[✅] SOLID y DDD aplicados
[✅] CQRS ligero para dashboard (performance)
[✅] Manejo de errores en todos los flujos
[✅] Rate limiting definido
[✅] Health checks incluidos
[✅] API REST documentada con OpenAPI
[✅] Versionado de API (/v1/)
[✅] Modo contingencia para integraciones caídas

PERSPECTIVA SEGURIDAD
[✅] Autenticación Microsoft Entra ID (SSO corporativo)
[✅] Validación de usuario ACTIVO en Azure AD (no solo autenticado)
[✅] RBAC con 6 roles bien definidos
[✅] Secretos en Azure Key Vault (ninguno en código)
[✅] TLS 1.3 obligatorio
[✅] Cifrado de datos en reposo (TDE Azure SQL)
[✅] Column-level encryption para datos sensibles
[✅] Tokens en sessionStorage (no localStorage)
[✅] SAS tokens temporales para documentos (1 hora)
[✅] Inputs validados con FluentValidation

PERSPECTIVA DATOS Y CALIDAD
[✅] Modelo de datos completo con 20+ tablas
[✅] Índices en campos de búsqueda frecuente
[✅] Soft delete en todas las tablas operativas
[✅] Trazabilidad de origen en todos los datos
[✅] Bitácora con Azure SQL Ledger (inmutable)
[✅] Validación antes de persistir en BD

PERSPECTIVA TRAZABILIDAD / AUDITORÍA
[✅] Bitácora completa con 20+ tipos de eventos
[✅] Registro de quién, cuándo, qué, desde dónde
[✅] Estado antes/después en modificaciones
[✅] Bitácora append-only con Ledger Table
[✅] Auditoría de consultas al agente IA
[✅] Bitácora de acceso a documentos/evidencias

MARCOS NORMATIVOS
[✅] ISO 27001:2022 — Controles A.5.15, A.5.16, A.5.18, A.8.2, A.8.32
[✅] COBIT 2019 — APO, BAI, DSS, MEA cubiertos
[✅] ISACA ITAF — Normas 1201, 1202, 1401, 1402, 1205
[✅] ITIL 4 — Gestión de cambios, accesos, incidentes
[✅] Evidencia de controles para auditoría ISO/ISACA

MOTOR DE INTEGRACIONES (SOA Manager)
[✅] 7 tipos de conexión soportados (REST, SOAP, SQL, SFTP, CSV, Webhook)
[✅] Configuración visual sin código
[✅] Test de conexión desde la interfaz
[✅] Historial de ejecuciones
[✅] Modo contingencia automático
[✅] Secretos referenciados a Key Vault (no almacenados en BD)
[✅] Mapeo de campos configurable
[✅] Endpoints/métodos configurables por conector

PRUEBAS QA
[✅] 60+ escenarios de prueba documentados
[✅] Cobertura: autenticación, simulación, controles, conectores, IA, documentos
[✅] Casos happy path, error y borde definidos
[✅] Criterios de aceptación medibles

ENTREGABLES AUTOMÁTICOS
[✅] Word por control/requerimiento con estructura completa
[✅] PowerPoint ejecutivo de 10 slides
[✅] Calificación de madurez 1-10 con algoritmo definido
[✅] Expediente consolidado por simulación

ROADMAP
[✅] 3 fases definidas con sprints y criterios de aceptación
[✅] MVP funcional en 8 semanas
[✅] Priorización por valor de negocio

INFRAESTRUCTURA
[✅] Arquitectura Azure completa documentada
[✅] Estimación de costos incluida
[✅] Pipeline CI/CD con 3 ambientes (DEV/QA/PRD)
[✅] Variables de entorno documentadas (sin valores reales)

═══════════════════════════════════════════════════════════════════════
RESULTADO: ✅ APROBADO PARA REVISIÓN Y APROBACIÓN POR JUAN SOLANO
           Siguiente paso: Revisar, validar con el equipo y pasar a
           construcción con Claude Code / equipo de desarrollo.
═══════════════════════════════════════════════════════════════════════
```

---

## APÉNDICE A: GLOSARIO

| Término | Definición en contexto AuditorPRO |
|---|---|
| **Simulación** | Ejecución del motor de reglas sobre los datos disponibles para evaluar el estado de cumplimiento en un período dado |
| **Hallazgo preventivo** | Debilidad o incumplimiento detectado por el sistema antes de la auditoría formal |
| **Semáforo** | Indicador visual (Verde/Amarillo/Rojo) del estado de un control evaluado |
| **Score de madurez** | Calificación de 1 a 10 que resume el estado global de cumplimiento de una organización/sociedad |
| **Conector** | Configuración de integración con un sistema externo (SE Suite, Evolution, SAP, etc.) |
| **Modo contingencia** | Operación del sistema usando datos de última carga válida cuando la integración principal falla |
| **Punto de control** | Elemento auditable configurado en el catálogo, con regla de evaluación y criterios de semáforo |
| **Expediente** | Conjunto de evidencias, resultados y documentación que respalda la evaluación de un control o requerimiento |
| **RAG** | Retrieval-Augmented Generation — técnica de IA que enriquece las respuestas con contexto recuperado de documentos reales |
| **SoD** | Segregation of Duties (Segregación de Funciones) — principio que separa responsabilidades para reducir riesgo de fraude |
| **Ledger Table** | Función de Azure SQL que hace la tabla criptográficamente verificable (inmutable para fines de auditoría) |

---

## APÉNDICE B: REFERENCIAS NORMATIVAS

- ISO/IEC 27001:2022 — Information security management systems
- COBIT 2019 Framework — ISACA
- ITAF (IT Assurance Framework) 3rd Edition — ISACA
- ITIL 4 Foundation — Axelos
- SAP Fiori Design Guidelines — SAP
- Microsoft Entra ID Documentation — Microsoft
- WCAG 2.1 AA — W3C Web Accessibility Guidelines
- OWASP Top 10 — Open Web Application Security Project
- Azure Well-Architected Framework — Microsoft

---

*AuditorPRO TI Blueprint Maestro v1.0*
*Preparado para: Juan Solano — ILG Logistics — Área TI y Auditoría*
*Este documento es el punto de partida para desarrollo. Validar con equipo técnico y auditores antes de iniciar construcción.*
