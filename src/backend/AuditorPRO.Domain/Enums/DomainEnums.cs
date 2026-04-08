namespace AuditorPRO.Domain.Enums;

public enum EstadoLaboral { ACTIVO, INACTIVO, SUSPENDIDO, BAJA_PROCESADA }
public enum EstadoUsuario { ACTIVO, BLOQUEADO, ELIMINADO, SIN_CORRESPONDENCIA }
public enum NivelRiesgo { CRITICO, ALTO, MEDIO, BAJO }
public enum TipoMatrizRol { REQUERIDO, OPCIONAL, PROHIBIDO }
public enum SemaforoColor { VERDE, AMARILLO, ROJO, NO_EVALUADO }
public enum Criticidad { CRITICA, MEDIA, BAJA }
public enum TipoEvaluacion { AUTOMATICO, SEMI_AUTOMATICO, MANUAL }
public enum EstadoSimulacion { PENDIENTE, EN_PROCESO, COMPLETADA, ERROR, CANCELADA }
public enum TipoSimulacion { MANUAL, PROGRAMADA, BAJO_DEMANDA }
public enum PeriodicidadSimulacion { MENSUAL, TRIMESTRAL, SEMESTRAL, ANUAL, UNICA }
public enum EstadoHallazgo { ABIERTO, EN_PROCESO, RESUELTO, ACEPTADO, CERRADO }
public enum EstadoConector { ACTIVO, INACTIVO, ERROR, MANTENIMIENTO }
public enum TipoConector { REST_API, SFTP, BASE_DATOS, EXCEL_CSV, SAP_RFC, WEBHOOK }
public enum TipoEvidencia { ARCHIVO, CAPTURA_PANTALLA, REPORTE_SISTEMA, CORREO, DOCUMENTO_FIRMADO }
public enum EstadoPolitica { VIGENTE, REVISION, OBSOLETA, BORRADOR }
public enum AccionBitacora { CREAR, LEER, ACTUALIZAR, ELIMINAR, EXPORTAR, LOGIN, LOGOUT, EJECUTAR_SIMULACION }
