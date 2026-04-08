-- ============================================================
-- AuditorPRO TI — Esquema de Base de Datos Completo
-- Azure SQL Database — Compatible con SQL Server 2022+
-- Versión: 1.0 | Fecha: Abril 2026
-- ILG Logistics — Corporación
-- ============================================================

-- ============================================================
-- BLOQUE 1: MAESTROS CORPORATIVOS
-- ============================================================

CREATE TABLE sociedades (
    id              INT IDENTITY(1,1) PRIMARY KEY,
    codigo          VARCHAR(10)     NOT NULL UNIQUE,
    nombre          VARCHAR(200)    NOT NULL,
    pais            VARCHAR(100),
    activa          BIT             NOT NULL DEFAULT 1,
    created_at      DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    updated_at      DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    created_by      VARCHAR(200)
);
GO

CREATE TABLE departamentos (
    id              INT IDENTITY(1,1) PRIMARY KEY,
    sociedad_id     INT             NOT NULL REFERENCES sociedades(id),
    codigo          VARCHAR(20)     NOT NULL,
    nombre          VARCHAR(200)    NOT NULL,
    activo          BIT             NOT NULL DEFAULT 1,
    created_at      DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    updated_at      DATETIME2       NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TABLE puestos (
    id              INT IDENTITY(1,1) PRIMARY KEY,
    sociedad_id     INT             NOT NULL REFERENCES sociedades(id),
    codigo          VARCHAR(20)     NOT NULL,
    nombre          VARCHAR(200)    NOT NULL,
    nivel_riesgo    VARCHAR(20)     CHECK (nivel_riesgo IN ('ALTO','MEDIO','BAJO')),
    activo          BIT             NOT NULL DEFAULT 1,
    created_at      DATETIME2       NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TABLE empleados_maestro (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    numero_empleado     VARCHAR(30)     NOT NULL UNIQUE,
    nombre_completo     VARCHAR(300)    NOT NULL,
    correo_corporativo  VARCHAR(200),
    entra_id_object     VARCHAR(100),
    sociedad_id         INT             REFERENCES sociedades(id),
    departamento_id     INT             REFERENCES departamentos(id),
    puesto_id           INT             REFERENCES puestos(id),
    jefe_empleado_id    UNIQUEIDENTIFIER REFERENCES empleados_maestro(id),
    estado_laboral      VARCHAR(30)     NOT NULL CHECK (estado_laboral IN ('ACTIVO','INACTIVO','SUSPENDIDO','BAJA_PROCESADA')),
    fecha_ingreso       DATE,
    fecha_baja          DATE,
    fuente_origen       VARCHAR(50),
    lote_carga_id       UNIQUEIDENTIFIER,
    created_at          DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    updated_at          DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    is_deleted          BIT             NOT NULL DEFAULT 0
);
CREATE INDEX IX_empleados_estado  ON empleados_maestro (estado_laboral, sociedad_id);
CREATE INDEX IX_empleados_entra   ON empleados_maestro (entra_id_object);
GO

CREATE TABLE usuarios_sistema (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    sistema             VARCHAR(50)     NOT NULL,
    nombre_usuario      VARCHAR(100)    NOT NULL,
    empleado_id         UNIQUEIDENTIFIER REFERENCES empleados_maestro(id),
    estado              VARCHAR(30)     NOT NULL CHECK (estado IN ('ACTIVO','BLOQUEADO','ELIMINADO','SIN_CORRESPONDENCIA')),
    tipo_usuario        VARCHAR(30),
    fecha_ultimo_acceso DATETIME2,
    fuente_origen       VARCHAR(50),
    created_at          DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    updated_at          DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    is_deleted          BIT             NOT NULL DEFAULT 0
);
CREATE INDEX IX_usuarios_sistema_estado ON usuarios_sistema (sistema, estado);
GO

CREATE TABLE roles_sistema (
    id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    sistema         VARCHAR(50)     NOT NULL,
    nombre_rol      VARCHAR(200)    NOT NULL,
    descripcion     NVARCHAR(MAX),
    nivel_riesgo    VARCHAR(20)     CHECK (nivel_riesgo IN ('CRITICO','ALTO','MEDIO','BAJO')),
    es_critico      BIT             NOT NULL DEFAULT 0,
    activo          BIT             NOT NULL DEFAULT 1,
    created_at      DATETIME2       NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TABLE asignaciones_rol_usuario (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    usuario_id          UNIQUEIDENTIFIER NOT NULL REFERENCES usuarios_sistema(id),
    rol_id              UNIQUEIDENTIFIER NOT NULL REFERENCES roles_sistema(id),
    fecha_asignacion    DATE,
    fecha_vencimiento   DATE,
    asignado_por        VARCHAR(200),
    expediente_ref      VARCHAR(100),
    activa              BIT             NOT NULL DEFAULT 1,
    created_at          DATETIME2       NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TABLE matriz_puesto_rol (
    id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    puesto_id       INT             NOT NULL REFERENCES puestos(id),
    rol_id          UNIQUEIDENTIFIER NOT NULL REFERENCES roles_sistema(id),
    tipo            VARCHAR(20)     CHECK (tipo IN ('REQUERIDO','OPCIONAL','PROHIBIDO')),
    justificacion   NVARCHAR(1000),
    vigente_desde   DATE,
    created_at      DATETIME2       NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TABLE conflictos_sod (
    id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    sistema         VARCHAR(50),
    rol_a_id        UNIQUEIDENTIFIER REFERENCES roles_sistema(id),
    rol_b_id        UNIQUEIDENTIFIER REFERENCES roles_sistema(id),
    descripcion     NVARCHAR(MAX),
    riesgo          VARCHAR(20)     CHECK (riesgo IN ('CRITICO','ALTO','MEDIO','BAJO')),
    mitigacion_doc  NVARCHAR(MAX),
    activo          BIT             NOT NULL DEFAULT 1,
    created_at      DATETIME2       NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ============================================================
-- BLOQUE 2: MOTOR DE AUDITORÍA
-- ============================================================

CREATE TABLE dominios_auditoria (
    id          INT IDENTITY(1,1) PRIMARY KEY,
    codigo      VARCHAR(30)     NOT NULL UNIQUE,
    nombre      VARCHAR(200)    NOT NULL,
    descripcion NVARCHAR(MAX),
    activo      BIT             NOT NULL DEFAULT 1
);
GO

CREATE TABLE puntos_control (
    id                  INT IDENTITY(1,1) PRIMARY KEY,
    dominio_id          INT             NOT NULL REFERENCES dominios_auditoria(id),
    codigo              VARCHAR(50)     NOT NULL UNIQUE,
    nombre              VARCHAR(300)    NOT NULL,
    descripcion         NVARCHAR(MAX),
    tipo_evaluacion     VARCHAR(30)     NOT NULL CHECK (tipo_evaluacion IN ('AUTOMATICO','SEMI_AUTOMATICO','MANUAL')),
    criticidad_base     VARCHAR(20)     NOT NULL CHECK (criticidad_base IN ('CRITICA','MEDIA','BAJA')),
    norma_referencia    VARCHAR(200),
    query_sql           NVARCHAR(MAX),
    condicion_verde     NVARCHAR(MAX),
    condicion_amarillo  NVARCHAR(MAX),
    condicion_rojo      NVARCHAR(MAX),
    evidencia_requerida NVARCHAR(MAX),
    activo              BIT             NOT NULL DEFAULT 1,
    version_regla       INT             NOT NULL DEFAULT 1,
    created_at          DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    updated_at          DATETIME2       NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TABLE simulaciones_auditoria (
    id                      UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    nombre                  VARCHAR(300)    NOT NULL,
    descripcion             NVARCHAR(MAX),
    tipo                    VARCHAR(30)     CHECK (tipo IN ('MANUAL','PROGRAMADA','BAJO_DEMANDA')),
    periodicidad            VARCHAR(30)     CHECK (periodicidad IN ('MENSUAL','TRIMESTRAL','SEMESTRAL','ANUAL','UNICA')),
    estado                  VARCHAR(30)     NOT NULL CHECK (estado IN ('PENDIENTE','EN_PROCESO','COMPLETADA','ERROR','CANCELADA')) DEFAULT 'PENDIENTE',
    sociedad_ids            NVARCHAR(MAX),
    periodo_inicio          DATE            NOT NULL,
    periodo_fin             DATE            NOT NULL,
    dominio_ids             NVARCHAR(MAX),
    puntos_control_ids      NVARCHAR(MAX),
    score_madurez           DECIMAL(4,2),
    porcentaje_cumplimiento DECIMAL(5,2),
    total_controles         INT,
    controles_verde         INT,
    controles_amarillo      INT,
    controles_rojo          INT,
    iniciada_por            VARCHAR(200)    NOT NULL,
    iniciada_at             DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    completada_at           DATETIME2,
    duracion_segundos       INT,
    error_detalle           NVARCHAR(MAX),
    created_at              DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    updated_at              DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    is_deleted              BIT             NOT NULL DEFAULT 0
);
CREATE INDEX IX_sim_estado ON simulaciones_auditoria (estado, iniciada_at DESC);
GO

CREATE TABLE resultados_control (
    id                      UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    simulacion_id           UNIQUEIDENTIFIER NOT NULL REFERENCES simulaciones_auditoria(id),
    punto_control_id        INT             NOT NULL REFERENCES puntos_control(id),
    sociedad_id             INT             REFERENCES sociedades(id),
    semaforo                VARCHAR(10)     NOT NULL CHECK (semaforo IN ('VERDE','AMARILLO','ROJO','NO_EVALUADO')),
    criticidad              VARCHAR(20)     NOT NULL CHECK (criticidad IN ('CRITICA','MEDIA','BAJA')),
    resultado_detalle       NVARCHAR(MAX),
    datos_evaluados         NVARCHAR(MAX),
    evidencia_encontrada    NVARCHAR(MAX),
    evidencia_faltante      NVARCHAR(MAX),
    analisis_ia             NVARCHAR(MAX),
    recomendacion           NVARCHAR(MAX),
    responsable_sugerido    VARCHAR(200),
    fecha_compromiso_sug    DATE,
    evaluado_at             DATETIME2       NOT NULL DEFAULT GETUTCDATE()
);
CREATE INDEX IX_resultados_sim ON resultados_control (simulacion_id, semaforo);
GO

CREATE TABLE hallazgos (
    id                      UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    simulacion_id           UNIQUEIDENTIFIER REFERENCES simulaciones_auditoria(id),
    resultado_control_id    UNIQUEIDENTIFIER REFERENCES resultados_control(id),
    punto_control_id        INT             REFERENCES puntos_control(id),
    sociedad_id             INT             REFERENCES sociedades(id),
    tipo                    VARCHAR(30)     CHECK (tipo IN ('PREVENTIVO','REAL','HISTORICO')),
    titulo                  VARCHAR(500)    NOT NULL,
    descripcion             NVARCHAR(MAX)   NOT NULL,
    causa_probable          NVARCHAR(MAX),
    impacto                 NVARCHAR(MAX),
    criticidad              VARCHAR(20)     NOT NULL CHECK (criticidad IN ('CRITICA','MEDIA','BAJA')),
    semaforo                VARCHAR(10)     NOT NULL,
    estado                  VARCHAR(30)     NOT NULL CHECK (estado IN ('ABIERTO','EN_PROCESO','CERRADO','ACEPTADO','RECURRENTE')) DEFAULT 'ABIERTO',
    recurrente              BIT             NOT NULL DEFAULT 0,
    hallazgo_previo_id      UNIQUEIDENTIFIER REFERENCES hallazgos(id),
    norma_violada           VARCHAR(200),
    created_at              DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    updated_at              DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    is_deleted              BIT             NOT NULL DEFAULT 0
);
CREATE INDEX IX_hallazgos_estado ON hallazgos (estado, criticidad, created_at DESC);
GO

CREATE TABLE planes_accion (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    hallazgo_id         UNIQUEIDENTIFIER NOT NULL REFERENCES hallazgos(id),
    descripcion_accion  NVARCHAR(MAX)   NOT NULL,
    causa_raiz          NVARCHAR(MAX),
    recomendacion_ia    NVARCHAR(MAX),
    responsable_email   VARCHAR(200),
    responsable_nombre  VARCHAR(300),
    fecha_compromiso    DATE            NOT NULL,
    fecha_cierre_real   DATE,
    prioridad           VARCHAR(20)     CHECK (prioridad IN ('INMEDIATA','ALTA','MEDIA','BAJA')),
    estado              VARCHAR(30)     NOT NULL CHECK (estado IN ('PENDIENTE','EN_PROCESO','COMPLETADO','REQUIERE_VALIDACION','CERRADO','VENCIDO')) DEFAULT 'PENDIENTE',
    evidencia_cierre    NVARCHAR(MAX),
    validado_por        VARCHAR(200),
    politica_afectada   NVARCHAR(MAX),
    created_at          DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    updated_at          DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    created_by          VARCHAR(200)
);
GO

-- ============================================================
-- BLOQUE 3: GESTIÓN DOCUMENTAL Y EVIDENCIAS
-- ============================================================

CREATE TABLE evidencias (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    nombre_archivo      VARCHAR(500)    NOT NULL,
    descripcion         NVARCHAR(MAX),
    tipo_documento      VARCHAR(30)     CHECK (tipo_documento IN ('PDF','WORD','EXCEL','CSV','IMAGEN','SCREENSHOT','POWERPOINT','OTRO')),
    blob_url            VARCHAR(1000),
    blob_container      VARCHAR(200),
    tamano_bytes        BIGINT,
    hash_sha256         VARCHAR(64),
    fuente              VARCHAR(50),
    estado_ocr          VARCHAR(20)     CHECK (estado_ocr IN ('PENDIENTE','PROCESADO','ERROR','NO_APLICA')),
    texto_extraido      NVARCHAR(MAX),
    simulacion_id       UNIQUEIDENTIFIER REFERENCES simulaciones_auditoria(id),
    punto_control_id    INT             REFERENCES puntos_control(id),
    hallazgo_id         UNIQUEIDENTIFIER REFERENCES hallazgos(id),
    sociedad_id         INT             REFERENCES sociedades(id),
    periodo_referencia  VARCHAR(20),
    vigente_hasta       DATE,
    estado_revision     VARCHAR(30)     CHECK (estado_revision IN ('PENDIENTE','REVISADA','APROBADA','RECHAZADA','VENCIDA')),
    cargado_por         VARCHAR(200),
    created_at          DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    is_deleted          BIT             NOT NULL DEFAULT 0
);
CREATE INDEX IX_evidencias_control ON evidencias (punto_control_id, simulacion_id);
GO

CREATE TABLE politicas (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    codigo              VARCHAR(50)     NOT NULL UNIQUE,
    nombre              VARCHAR(300)    NOT NULL,
    version             VARCHAR(20),
    fecha_aprobacion    DATE,
    fecha_vencimiento   DATE,
    responsable_email   VARCHAR(200),
    estado              VARCHAR(30)     CHECK (estado IN ('VIGENTE','VENCIDA','EN_REVISION','OBSOLETA','BORRADOR')),
    texto_completo      NVARCHAR(MAX),
    blob_url            VARCHAR(1000),
    score_calidad_ia    DECIMAL(4,2),
    observaciones_ia    NVARCHAR(MAX),
    gaps_detectados     NVARCHAR(MAX),
    ultima_revision_ia  DATETIME2,
    dominio_id          INT             REFERENCES dominios_auditoria(id),
    created_at          DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    updated_at          DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    is_deleted          BIT             NOT NULL DEFAULT 0
);
GO

CREATE TABLE procedimientos (
    id                      UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    codigo                  VARCHAR(50)     NOT NULL UNIQUE,
    nombre                  VARCHAR(300)    NOT NULL,
    politica_id             UNIQUEIDENTIFIER REFERENCES politicas(id),
    version                 VARCHAR(20),
    fecha_aprobacion        DATE,
    responsable_email       VARCHAR(200),
    estado                  VARCHAR(30)     CHECK (estado IN ('VIGENTE','VENCIDO','EN_REVISION','OBSOLETO','BORRADOR')),
    texto_completo          NVARCHAR(MAX),
    blob_url                VARCHAR(1000),
    score_claridad_ia       DECIMAL(4,2),
    score_ejecutabilidad_ia DECIMAL(4,2),
    resultado_revision      VARCHAR(50),
    recomendaciones_ia      NVARCHAR(MAX),
    ultima_revision_ia      DATETIME2,
    created_at              DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    updated_at              DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    is_deleted              BIT             NOT NULL DEFAULT 0
);
GO

-- ============================================================
-- BLOQUE 4: INTEGRACIONES Y CONECTORES
-- ============================================================

CREATE TABLE conectores (
    id                      UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    nombre                  VARCHAR(200)    NOT NULL,
    sistema_destino         VARCHAR(100)    NOT NULL,
    descripcion             NVARCHAR(MAX),
    tipo_conexion           VARCHAR(30)     NOT NULL CHECK (tipo_conexion IN ('REST_API','SOAP_API','SQL_DIRECT','SQL_VIEW','SFTP','EXCEL_CSV','WEBHOOK')),
    url_base                VARCHAR(500),
    metodo_auth             VARCHAR(30)     CHECK (metodo_auth IN ('OAUTH2','BASIC','API_KEY','ENTRA_ID_APP','WINDOWS_AUTH','NONE')),
    key_vault_secret_name   VARCHAR(200),
    headers_adicionales     NVARCHAR(MAX),
    timeout_segundos        INT             NOT NULL DEFAULT 30,
    reintentos              INT             NOT NULL DEFAULT 3,
    esquema_mapeo           NVARCHAR(MAX),
    sql_query               NVARCHAR(MAX),
    connection_string_ref   VARCHAR(200),
    activo                  BIT             NOT NULL DEFAULT 1,
    estado_actual           VARCHAR(30)     CHECK (estado_actual IN ('VERDE','AMARILLO','ROJO','DESCONOCIDO')) DEFAULT 'DESCONOCIDO',
    ultimo_test             DATETIME2,
    ultimo_test_resultado   NVARCHAR(500),
    created_at              DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    updated_at              DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    created_by              VARCHAR(200)
);
GO

CREATE TABLE endpoints_conector (
    id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    conector_id     UNIQUEIDENTIFIER NOT NULL REFERENCES conectores(id),
    nombre          VARCHAR(200)    NOT NULL,
    descripcion     NVARCHAR(MAX),
    metodo_http     VARCHAR(10),
    path_relativo   VARCHAR(500),
    proposito       VARCHAR(100),
    parametros      NVARCHAR(MAX),
    respuesta_esquema NVARCHAR(MAX),
    activo          BIT             NOT NULL DEFAULT 1,
    created_at      DATETIME2       NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE TABLE ejecuciones_conector (
    id                  UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    conector_id         UNIQUEIDENTIFIER NOT NULL REFERENCES conectores(id),
    endpoint_id         UNIQUEIDENTIFIER REFERENCES endpoints_conector(id),
    simulacion_id       UNIQUEIDENTIFIER REFERENCES simulaciones_auditoria(id),
    tipo_ejecucion      VARCHAR(30),
    estado              VARCHAR(20)     NOT NULL CHECK (estado IN ('EXITOSO','ERROR','TIMEOUT','PARCIAL')),
    inicio_at           DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    fin_at              DATETIME2,
    duracion_ms         INT,
    registros_obtenidos INT,
    registros_procesados INT,
    error_codigo        VARCHAR(50),
    error_detalle       NVARCHAR(MAX),
    ejecutado_por       VARCHAR(200)
);
CREATE INDEX IX_ejecuciones_conector ON ejecuciones_conector (conector_id, inicio_at DESC);
GO

CREATE TABLE lotes_carga (
    id              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tipo_carga      VARCHAR(50)     NOT NULL,
    nombre_archivo  VARCHAR(500),
    total_filas     INT,
    filas_procesadas INT,
    filas_error     INT,
    estado          VARCHAR(30)     CHECK (estado IN ('PROCESANDO','COMPLETADO','ERROR_PARCIAL','ERROR_TOTAL')),
    errores_detalle NVARCHAR(MAX),
    cargado_por     VARCHAR(200),
    created_at      DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    completado_at   DATETIME2
);
GO

-- ============================================================
-- BLOQUE 5: BITÁCORA DE AUDITORÍA (APPEND-ONLY)
-- ============================================================

CREATE TABLE bitacora_auditoria (
    id                  BIGINT IDENTITY(1,1) PRIMARY KEY,
    timestamp_utc       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    usuario_email       VARCHAR(200)    NOT NULL,
    usuario_nombre      VARCHAR(300),
    entra_id_object     VARCHAR(100),
    ip_address          VARCHAR(45),
    user_agent          VARCHAR(500),
    accion              VARCHAR(100)    NOT NULL,
    -- LOGIN, LOGOUT, VER_DASHBOARD, EJECUTAR_SIMULACION, VER_HALLAZGO, EDITAR_PLAN,
    -- CARGAR_EVIDENCIA, EXPORTAR_REPORTE, CONFIG_CONECTOR, ELIMINAR (etc.)
    modulo              VARCHAR(50),
    recurso_tipo        VARCHAR(50),
    recurso_id          VARCHAR(100),
    descripcion         NVARCHAR(1000),
    datos_antes         NVARCHAR(MAX),
    datos_despues       NVARCHAR(MAX),
    exitoso             BIT             NOT NULL DEFAULT 1,
    error_detalle       NVARCHAR(MAX),
    duracion_ms         INT,
    sociedad_id         INT             REFERENCES sociedades(id)
);
CREATE INDEX IX_bitacora_fecha    ON bitacora_auditoria (timestamp_utc DESC);
CREATE INDEX IX_bitacora_usuario  ON bitacora_auditoria (usuario_email, timestamp_utc DESC);
CREATE INDEX IX_bitacora_accion   ON bitacora_auditoria (accion, timestamp_utc DESC);
GO

-- ============================================================
-- DATOS SEMILLA — DOMINIOS Y CONTROLES BASE
-- ============================================================

INSERT INTO dominios_auditoria (codigo, nombre, descripcion) VALUES
('IDENTIDAD',        'Gestión de Identidad',             'Altas, bajas y modificaciones de usuarios en todos los sistemas'),
('RECERTIFICACION',  'Recertificación de Accesos',       'Revisión periódica de accesos vigentes vs requeridos'),
('SEG_SAP',          'Seguridad SAP',                    'Roles SAP, SoD, usuarios de diálogo vs sistema vs RFC'),
('ALTAS_BAJAS',      'Altas y Bajas de Personal',        'Sincronía entre RRHH, sistemas de TI y Active Directory'),
('CAMBIOS_APP',      'Cambios en Aplicaciones',          'Control de cambios en sistemas productivos'),
('EVIDENCIA_DOC',    'Evidencia Documental',             'Respaldo de controles con documentación trazable'),
('POLITICAS',        'Políticas y Procedimientos',       'Vigencia, calidad y cobertura de políticas de TI'),
('SOD',              'Segregación de Funciones',         'Conflictos de acceso que violan la segregación de funciones');
GO

INSERT INTO puntos_control (dominio_id, codigo, nombre, tipo_evaluacion, criticidad_base, norma_referencia, condicion_verde, condicion_amarillo, condicion_rojo) VALUES
(1, 'ID-001', 'Usuarios activos sin empleado asociado',          'AUTOMATICO',     'CRITICA', 'ISO 27001 A.9.2.1', 'count=0', '1-5',   '>5'),
(1, 'ID-002', 'Empleados dados de baja con acceso activo',       'AUTOMATICO',     'CRITICA', 'ISO 27001 A.9.2.6', 'count=0', '1-2',   '>2'),
(1, 'ID-003', 'Usuarios sin acceso por más de 90 días',          'AUTOMATICO',     'MEDIA',   'ISO 27001 A.9.2.5', 'count=0', '1-10',  '>10'),
(2, 'RECERT-001', 'Recertificación trimestral completada',       'SEMI_AUTOMATICO','CRITICA', 'COBIT DSS05.04',    'pct>=95', '80-94', '<80'),
(2, 'RECERT-002', 'Accesos sin recertificación vencidos',        'AUTOMATICO',     'ALTA',    'COBIT DSS05.04',    'count=0', '1-5',   '>5'),
(3, 'SAP-001',  'Usuarios SAP con rol SUPER / SAP_ALL',          'AUTOMATICO',     'CRITICA', 'SOX ITGC',          'count=0', 'N/A',   '>0'),
(3, 'SAP-002',  'Usuarios SAP de diálogo sin fecha vencimiento', 'AUTOMATICO',     'MEDIA',   'ISO 27001 A.9.2.5', 'count=0', '1-5',   '>5'),
(4, 'ALTA-001', 'Nuevos empleados sin usuario en sistemas clave','AUTOMATICO',     'MEDIA',   'ISO 27001 A.9.2.1', 'count=0', '1-3',   '>3'),
(4, 'ALTA-002', 'Bajas procesadas con sesión activa <48h',       'AUTOMATICO',     'CRITICA', 'ISO 27001 A.9.2.6', 'count=0', 'N/A',   '>0'),
(7, 'POL-001',  'Políticas de seguridad vigentes y aprobadas',   'MANUAL',         'MEDIA',   'ISO 27001 A.5.1.1', 'pct=100', '90-99', '<90'),
(8, 'SOD-001',  'Conflictos SoD críticos activos',               'AUTOMATICO',     'CRITICA', 'COSO / SOX',        'count=0', '1-2',   '>2');
GO

-- ============================================================
-- DATOS SEMILLA — SOCIEDADES ILG
-- ============================================================

INSERT INTO sociedades (codigo, nombre, pais) VALUES
('ILG-CR', 'ILG Logistics Costa Rica', 'Costa Rica'),
('ILG-SV', 'ILG Logistics El Salvador', 'El Salvador'),
('ILG-GT', 'ILG Logistics Guatemala',   'Guatemala'),
('ILG-HN', 'ILG Logistics Honduras',    'Honduras'),
('ILG-NI', 'ILG Logistics Nicaragua',   'Nicaragua'),
('ILG-MX', 'ILG Logistics México',      'México');
GO
