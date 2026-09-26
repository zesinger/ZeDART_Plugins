// ============================================================================
// FR : FOURNISSEUR DE TRAFIC
//      Ce fichier contient le cœur du plugin. Le fournisseur démarre et arrête
//      la source de trafic, transforme les données reçues en TrafficUpdate et
//      les transmet à ZeDART. Il gère également la réinitialisation du trafic
//      et l'état de la source.
//      Voir ci-dessous pour le détail des interfaces à implémenter.
//
// EN : TRAFFIC PROVIDER
//      This file contains the core of the plugin. The provider starts and stops
//      the traffic source, converts received data into TrafficUpdate objects
//      and sends them to ZeDART. It also handles traffic reset and source status.
//      See below for details about the interfaces to implement.
//
// ES : PROVEEDOR DE TRÁFICO
//      Este archivo contiene el núcleo del plugin. El proveedor inicia y detiene
//      la fuente de tráfico, convierte los datos recibidos en objetos
//      TrafficUpdate y los envía a ZeDART. También gestiona la reinicialización
//      del tráfico y el estado de la fuente.
//      Véase más abajo el detalle de las interfaces que deben implementarse.
//
// DE : VERKEHRSDATENANBIETER
//      Diese Datei enthält den Kern des Plugins. Der Provider startet und stoppt
//      die Verkehrsquelle, wandelt empfangene Daten in TrafficUpdate-Objekte um
//      und übergibt sie an ZeDART. Außerdem verwaltet er das Zurücksetzen der
//      Verkehrsdaten und den Status der Quelle.
//      Einzelheiten zu den zu implementierenden Schnittstellen siehe unten.
// ============================================================================

using Avalonia.Controls;
using ZeDART.Contracts.Plugins;
using ZeDART.Contracts.Traffic;

namespace TemplateTrafficPlugin;

// FR : OBLIGATOIRE — ITrafficProvider définit un fournisseur de trafic ZeDART.
//      Il permet de démarrer et arrêter la source, d'envoyer les mises à jour
//      de trafic et d'informer ZeDART de l'état de la source.
//
// EN : MANDATORY — ITrafficProvider defines a ZeDART traffic provider.
//      It starts and stops the source, sends traffic updates and informs
//      ZeDART about the status of the source.
//
// ES : OBLIGATORIO — ITrafficProvider define un proveedor de tráfico ZeDART.
//      Permite iniciar y detener la fuente, enviar actualizaciones de tráfico
//      e informar a ZeDART sobre el estado de la fuente.
//
// DE : ERFORDERLICH — ITrafficProvider definiert einen ZeDART-Verkehrsdatenanbieter.
//      Die Schnittstelle startet und stoppt die Quelle, sendet Verkehrsdaten
//      und informiert ZeDART über den Status der Quelle.
//
// FR : OPTIONNEL — IZeDartRuntimeUI étend ITrafficProvider et permet au fournisseur
//      d'afficher une fenêtre pendant l'exécution de ZeDART.
//      Si aucune interface runtime n'est nécessaire, remplacez IZeDartRuntimeUI
//      par ITrafficProvider et supprimez ShowUI(), CloseUI() ainsi que les fichiers
//      TemplateRuntimeWindow.axaml et TemplateRuntimeWindow.axaml.cs.
//
// EN : OPTIONAL — IZeDartRuntimeUI extends ITrafficProvider and allows the provider
//      to display a window while ZeDART is running.
//      If no runtime UI is required, replace IZeDartRuntimeUI with ITrafficProvider
//      and remove ShowUI(), CloseUI() and the TemplateRuntimeWindow files.
//
// ES : OPCIONAL — IZeDartRuntimeUI extiende ITrafficProvider y permite al proveedor
//      mostrar una ventana mientras ZeDART está en ejecución.
//      Si no se necesita una interfaz runtime, sustituya IZeDartRuntimeUI por
//      ITrafficProvider y elimine ShowUI(), CloseUI() y TemplateRuntimeWindow.
//
// DE : OPTIONAL — IZeDartRuntimeUI erweitert ITrafficProvider und ermöglicht dem
//      Provider, während der Ausführung von ZeDART ein Fenster anzuzeigen.
//      Wenn keine Runtime-Oberfläche benötigt wird, ersetzen Sie
//      IZeDartRuntimeUI durch ITrafficProvider und entfernen Sie ShowUI(),
//      CloseUI() sowie die TemplateRuntimeWindow-Dateien.
public sealed class TemplateTrafficProvider : IZeDartRuntimeUI
{
    private readonly string _trafficSource;
    private TemplateRuntimeWindow? _runtimeWindow;

    // FR : OBLIGATOIRE — Nom de la source de trafic affiché par ZeDART.
    // EN : MANDATORY — Name of the traffic source displayed by ZeDART.
    // ES : OBLIGATORIO — Nombre de la fuente de tráfico mostrado por ZeDART.
    // DE : ERFORDERLICH — Name der von ZeDART angezeigten Verkehrsquelle.
    public string Name => "Template Traffic Provider";

    // FR : OBLIGATOIRE — Indique si le trafic est en temps réel ou en rejeu.
    // EN : MANDATORY — Indicates whether traffic is live or replayed.
    // ES : OBLIGATORIO — Indica si el tráfico es en tiempo real o reproducido.
    // DE : ERFORDERLICH — Gibt an, ob die Verkehrsdaten live oder wiedergegeben sind.
    public TrafficTimeMode TimeMode => TrafficTimeMode.RealTime;

    // FR : OBLIGATOIRE — Heure UTC correspondant aux données de trafic actuelles.
    // EN : MANDATORY — UTC time corresponding to the current traffic data.
    // ES : OBLIGATORIO — Hora UTC correspondiente a los datos de tráfico actuales.
    // DE : ERFORDERLICH — UTC-Zeit der aktuellen Verkehrsdaten.
    public DateTime CurrentTrafficTimeUtc => DateTime.UtcNow;

    // FR : OBLIGATOIRE — État actuel de la source de trafic.
    //      Color définit la couleur de l'indicateur de la source dans ZeDART.
    //      Text fournit un message affiché en info-bulle, par exemple pour décrire
    //      une panne ou une situation dégradée.
    //      OptionalMapLayerId peut demander l'affichage automatique d'une couche
    //      de carte optionnelle adaptée à cette situation. Utilisez null si
    //      aucune couche de carte ne doit être affichée.
    //
    // EN : MANDATORY — Current status of the traffic source.
    //      Color defines the color of the source indicator in ZeDART.
    //      Text provides a tooltip message, for example to describe
    //      a failure or degraded situation.
    //      OptionalMapLayerId can request the automatic display of an optional
    //      map layer appropriate to this situation. Use null if no map layer
    //      should be displayed.
    //
    // ES : OBLIGATORIO — Estado actual de la fuente de tráfico.
    //      Color define el color del indicador de la fuente en ZeDART.
    //      Text proporciona un mensaje informativo, por ejemplo para describir
    //      un fallo o una situación degradada.
    //      OptionalMapLayerId puede solicitar la visualización automática de una
    //      capa de mapa opcional adaptada a esta situación. Use null si no debe
    //      mostrarse ninguna capa de mapa.
    //
    // DE : ERFORDERLICH — Aktueller Status der Verkehrsquelle.
    //      Color bestimmt die Farbe der Quellenanzeige in ZeDART.
    //      Text liefert einen Tooltip, zum Beispiel zur Beschreibung
    //      eines Fehlers oder einer eingeschränkten Situation.
    //      OptionalMapLayerId kann die automatische Anzeige einer passenden
    //      optionalen Kartenebene anfordern. Verwenden Sie null, wenn keine
    //      Kartenebene angezeigt werden soll.
    public TrafficProviderStatus SourceStatus { get; private set; }
        = TrafficProviderStatus.Normal;

    // FR : OBLIGATOIRE — Événement utilisé pour envoyer une mise à jour de trafic à ZeDART.
    // EN : MANDATORY — Event used to send a traffic update to ZeDART.
    // ES : OBLIGATORIO — Evento utilizado para enviar una actualización de tráfico a ZeDART.
    // DE : ERFORDERLICH — Ereignis zum Senden einer Verkehrsaktualisierung an ZeDART.
    public event Action<TrafficUpdate>? TrafficAvailable;

    // FR : OBLIGATOIRE — Demande à ZeDART d'effacer le trafic fourni par ce plugin.
    // EN : MANDATORY — Tells ZeDART to clear the traffic supplied by this plugin.
    // ES : OBLIGATORIO — Indica a ZeDART que borre el tráfico suministrado por este plugin.
    // DE : ERFORDERLICH — Fordert ZeDART auf, den von diesem Plugin gelieferten Verkehr zu löschen.
    public event Action? TrafficReset;

    // FR : OBLIGATOIRE — Informe ZeDART lorsque SourceStatus a changé.
    // EN : MANDATORY — Notifies ZeDART when SourceStatus has changed.
    // ES : OBLIGATORIO — Notifica a ZeDART cuando SourceStatus ha cambiado.
    // DE : ERFORDERLICH — Informiert ZeDART, wenn sich SourceStatus geändert hat.
    public event Action<TrafficProviderStatus>? SourceStatusChanged;

    public TemplateTrafficProvider(PluginContext context)
    {
        // FR : Récupère la valeur définie dans la fenêtre de configuration.
        // EN : Retrieves the value set in the configuration window.
        // ES : Recupera el valor definido en la ventana de configuración.
        // DE : Liest den im Konfigurationsfenster festgelegten Wert.
        _trafficSource = context.Settings?.TryGetValue("TrafficSource", out var value) == true
                ? value?.ToString() ?? "" : "";
    }

    // FR : OBLIGATOIRE — ZeDART appelle cette méthode pour démarrer la source de trafic.
    //      Démarrez ici votre réception, lecture de fichier, connexion réseau, etc.
    //
    // EN : MANDATORY — ZeDART calls this method to start the traffic source.
    //      Start your receiver, file reader, network connection, etc. here.
    //
    // ES : OBLIGATORIO — ZeDART llama a este método para iniciar la fuente de tráfico.
    //      Inicie aquí su receptor, lectura de archivo, conexión de red, etc.
    //
    // DE : ERFORDERLICH — ZeDART ruft diese Methode zum Starten der Verkehrsquelle auf.
    //      Starten Sie hier Empfänger, Dateileser, Netzwerkverbindung usw.
    public void Start()
    {
        // ====================================================================
        // FR : VOTRE CODE — Démarrez ici la récupération des données de trafic.
        //      Appelez SendTrafficUpdate(...) pour envoyer les données à ZeDART.
        // EN : YOUR CODE — Start retrieving traffic data here.
        //      Call SendTrafficUpdate(...) to send data to ZeDART.
        // ES : SU CÓDIGO — Inicie aquí la obtención de los datos de tráfico.
        //      Llame a SendTrafficUpdate(...) para enviar los datos a ZeDART.
        // DE : IHR CODE — Starten Sie hier den Empfang der Verkehrsdaten.
        //      Rufen Sie SendTrafficUpdate(...) auf, um Daten an ZeDART zu senden.
        // ====================================================================
    }

    // FR : OBLIGATOIRE — ZeDART appelle cette méthode pour arrêter la source de trafic.
    // EN : MANDATORY — ZeDART calls this method to stop the traffic source.
    // ES : OBLIGATORIO — ZeDART llama a este método para detener la fuente de tráfico.
    // DE : ERFORDERLICH — ZeDART ruft diese Methode zum Stoppen der Verkehrsquelle auf.
    public void Stop()
    {
        // ====================================================================
        // FR : VOTRE CODE — Arrêtez et libérez ici les ressources utilisées.
        // EN : YOUR CODE — Stop and release the resources used here.
        // ES : SU CÓDIGO — Detenga y libere aquí los recursos utilizados.
        // DE : IHR CODE — Stoppen und geben Sie hier verwendete Ressourcen frei.
        // ====================================================================
    }

    // FR : VOTRE CODE — Cette méthode est seulement un exemple.
    //      Son nom, ses paramètres et son fonctionnement peuvent être entièrement modifiés.
    //      Créez un TrafficUpdate à partir des données reçues de votre propre source.
    //
    // EN : YOUR CODE — This method is only an example.
    //      Its name, parameters and implementation can be completely changed.
    //      Create a TrafficUpdate from the data received from your own traffic source.
    //
    // ES : SU CÓDIGO — Este método es solo un ejemplo.
    //      Su nombre, parámetros y funcionamiento pueden modificarse completamente.
    //      Cree un TrafficUpdate a partir de los datos recibidos de su propia fuente.
    //
    // DE : IHR CODE — Diese Methode ist nur ein Beispiel.
    //      Name, Parameter und Implementierung können vollständig geändert werden.
    //      Erstellen Sie ein TrafficUpdate aus den Daten Ihrer eigenen Verkehrsquelle.
    private void ProcessTrafficData(string trackId, double latitude, double longitude, double altitudeFt)
    {
        var update = new TrafficUpdate(

            // FR : Identifiant unique et stable de la cible.
            //      Le même TrackId mettra à jour la même cible dans ZeDART.
            //      Exemple : "RADAR1:1234" ou "ADSB:39ABCD".
            // EN : Unique and stable identifier of the target.
            //      The same TrackId will update the same target in ZeDART.
            //      Example: "RADAR1:1234" or "ADSB:39ABCD".
            // ES : Identificador único y estable del blanco.
            //      El mismo TrackId actualizará el mismo blanco en ZeDART.
            //      Ejemplo: "RADAR1:1234" o "ADSB:39ABCD".
            // DE : Eindeutige und stabile Kennung des Ziels.
            //      Dieselbe TrackId aktualisiert dasselbe Ziel in ZeDART.
            //      Beispiel: "RADAR1:1234" oder "ADSB:39ABCD".
            TrackId: trackId,

            // FR : Indicatif de l'aéronef, s'il est connu.
            // EN : Aircraft callsign, if known.
            // ES : Indicativo de la aeronave, si se conoce.
            // DE : Rufzeichen des Luftfahrzeugs, falls bekannt.
            Callsign: null,

            // FR : Latitude en degrés décimaux, si connue.
            // EN : Latitude in decimal degrees, if known.
            // ES : Latitud en grados decimales, si se conoce.
            // DE : Breitengrad in Dezimalgrad, falls bekannt.
            Latitude: latitude,

            // FR : Longitude en degrés décimaux, si connue.
            // EN : Longitude in decimal degrees, if known.
            // ES : Longitud en grados decimales, si se conoce.
            // DE : Längengrad in Dezimalgrad, falls bekannt.
            Longitude: longitude,

            // FR : Altitude géométrique en pieds, si connue.
            //      ZeDART lui donne priorité sur l'altitude barométrique.
            // EN : Geometric altitude in feet, if known.
            //      ZeDART gives it priority over barometric altitude.
            // ES : Altitud geométrica en pies, si se conoce.
            //      ZeDART le da prioridad sobre la altitud barométrica.
            // DE : Geometrische Höhe in Fuß, falls bekannt.
            //      ZeDART bevorzugt sie gegenüber der barometrischen Höhe.
            GeoAltitudeFt: null,

            // FR : Altitude barométrique en pieds, si connue.
            // EN : Barometric altitude in feet, if known.
            // ES : Altitud barométrica en pies, si se conoce.
            // DE : Barometrische Höhe in Fuß, falls bekannt.
            BaroAltitudeFt: altitudeFt,

            // FR : Altitude sélectionnée par l'équipage, en pieds, si disponible.
            //      Ce champ est prévu par le contrat mais n'est pas encore utilisé par ZeDART.
            // EN : Altitude selected by the crew, in feet, if available.
            //      This field is defined by the contract but is not yet used by ZeDART.
            // ES : Altitud seleccionada por la tripulación, en pies, si está disponible.
            //      Este campo está definido por el contrato pero ZeDART aún no lo utiliza.
            // DE : Von der Besatzung gewählte Höhe in Fuß, falls verfügbar.
            //      Dieses Feld ist im Vertrag vorgesehen, wird aber noch nicht von ZeDART verwendet.
            SelectedAltitudeFt: null,

            // FR : Altitude autorisée par le contrôle, en pieds, si disponible.
            //      Ce champ est prévu par le contrat mais n'est pas encore utilisé par ZeDART.
            // EN : ATC-cleared altitude, in feet, if available.
            //      This field is defined by the contract but is not yet used by ZeDART.
            // ES : Altitud autorizada por ATC, en pies, si está disponible.
            //      Este campo está definido por el contrato pero ZeDART aún no lo utiliza.
            // DE : Von ATC freigegebene Höhe in Fuß, falls verfügbar.
            //      Dieses Feld ist im Vertrag vorgesehen, wird aber noch nicht von ZeDART verwendet.
            ClearedAltitudeFt: null,

            // FR : Vitesse sol en nœuds, si connue.
            // EN : Ground speed in knots, if known.
            // ES : Velocidad respecto al suelo en nudos, si se conoce.
            // DE : Geschwindigkeit über Grund in Knoten, falls bekannt.
            GroundSpeedKt: null,

            // FR : Route/cap de déplacement en degrés, si connu.
            // EN : Track/heading in degrees, if known.
            // ES : Rumbo/trayectoria en grados, si se conoce.
            // DE : Kurs/Bewegungsrichtung in Grad, falls bekannt.
            HeadingDeg: null,

            // FR : Vitesse verticale en pieds par minute, si connue.
            // EN : Vertical rate in feet per minute, if known.
            // ES : Velocidad vertical en pies por minuto, si se conoce.
            // DE : Vertikalgeschwindigkeit in Fuß pro Minute, falls bekannt.
            VerticalRateFpm: null,

            // FR : Code transpondeur SSR (squawk), si connu. Exemple : "7000".
            // EN : SSR transponder code (squawk), if known. Example: "7000".
            // ES : Código SSR del transpondedor (squawk), si se conoce. Ejemplo: "7000".
            // DE : SSR-Transpondercode (Squawk), falls bekannt. Beispiel: "7000".
            SsrCode: null,

            // FR : Type OACI de l'aéronef, si connu. Exemple : "A320".
            // EN : ICAO aircraft type, if known. Example: "A320".
            // ES : Tipo OACI de la aeronave, si se conoce. Ejemplo: "A320".
            // DE : ICAO-Luftfahrzeugtyp, falls bekannt. Beispiel: "A320".
            AircraftType: null,

            // FR : Catégorie de turbulence de sillage, si connue.
            // EN : Wake turbulence category, if known.
            // ES : Categoría de turbulencia de estela, si se conoce.
            // DE : Wirbelschleppenkategorie, falls bekannt.
            WakeTurbulence: null,

            // FR : Aérodrome de départ, code OACI, si connu. Exemple : "LFPG".
            // EN : Departure aerodrome, ICAO code, if known. Example: "LFPG".
            // ES : Aeródromo de salida, código OACI, si se conoce. Ejemplo: "LFPG".
            // DE : Abflugplatz, ICAO-Code, falls bekannt. Beispiel: "LFPG".
            DepartureIcao: null,

            // FR : Aérodrome de destination, code OACI, si connu. Exemple : "LFPO".
            // EN : Destination aerodrome, ICAO code, if known. Example: "LFPO".
            // ES : Aeródromo de destino, código OACI, si se conoce. Ejemplo: "LFPO".
            // DE : Zielflugplatz, ICAO-Code, falls bekannt. Beispiel: "LFPO".
            DestinationIcao: null,

            // FR : Type de cible utilisé notamment pour son apparence en mode radar sol.
            //      Utilisez "Aircraft" pour un avion. D'autres types peuvent être
            //      définis dans la configuration du projet ZeDART_Config.
            // EN : Target type used notably for its appearance in ground radar mode.
            //      Use "Aircraft" for an aircraft. Other types can be defined
            //      in the ZeDART_Config project configuration.
            // ES : Tipo de blanco utilizado especialmente para su apariencia en radar de superficie.
            //      Use "Aircraft" para una aeronave. Se pueden definir otros tipos
            //      en la configuración del proyecto ZeDART_Config.
            // DE : Zieltyp, der insbesondere die Darstellung im Bodenradarmodus bestimmt.
            //      Verwenden Sie "Aircraft" für ein Luftfahrzeug. Weitere Typen können
            //      in der ZeDART_Config-Projektkonfiguration definiert werden.
            TargetTypeId: "Aircraft",

            // FR : Temps logique de cette mise à jour, exprimé en secondes Unix UTC.
            //      Pour une source de trafic en temps réel, utilisez l'heure UTC actuelle
            //      (ou le timestamp de la mesure s'il est fourni par la source).
            //      Pour un rejeu, utilisez l'heure de la séquence rejouée : elle détermine
            //      l'heure courante du replay dans ZeDART.
            //
            // EN : Logical time of this update, expressed as Unix UTC seconds.
            //      For a real-time traffic source, use the current UTC time
            //      (or the measurement timestamp if supplied by the source).
            //      For replay, use the time of the replayed sequence: it determines
            //      the current replay time in ZeDART.
            //
            // ES : Tiempo lógico de esta actualización, expresado en segundos Unix UTC.
            //      Para una fuente de tráfico en tiempo real, use la hora UTC actual
            //      (o la marca de tiempo de la medición si la proporciona la fuente).
            //      Para una reproducción, use la hora de la secuencia reproducida: esta
            //      determina la hora actual del replay en ZeDART.
            //
            // DE : Logische Zeit dieser Aktualisierung, angegeben als Unix-UTC-Sekunden.
            //      Verwenden Sie für eine Echtzeit-Verkehrsquelle die aktuelle UTC-Zeit
            //      (oder den Zeitstempel der Messung, falls die Quelle ihn liefert).
            //      Verwenden Sie bei einem Replay die Zeit der wiedergegebenen Sequenz:
            //      Sie bestimmt die aktuelle Replay-Zeit in ZeDART.
            AgeSeconds: DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        );

        SendTrafficUpdate(update);
    }

    // FR : Appelez cette méthode lorsque toutes les pistes précédemment envoyées
    //      doivent être supprimées de ZeDART.
    //      Exemple : changement de fichier, redémarrage ou réinitialisation de la source.
    // EN : Call this method when all previously sent tracks must be removed from ZeDART.
    //      Example: file change, restart or reset of the source.
    // ES : Llame a este método cuando todas las pistas enviadas anteriormente
    //      deban eliminarse de ZeDART.
    //      Ejemplo: cambio de archivo, reinicio o reinicialización de la fuente.
    // DE : Rufen Sie diese Methode auf, wenn alle zuvor gesendeten Ziele
    //      aus ZeDART entfernt werden müssen.
    //      Beispiel: Dateiwechsel, Neustart oder Zurücksetzen der Quelle.
    private void ResetTraffic()
    {
        TrafficReset?.Invoke();
    }


    // FR : Appelez cette méthode lorsque l'état de la source de trafic change.
    //      Exemple :
    //      SetSourceStatus(new TrafficProviderStatus(
    //          "#FFFF0000", "Traffic source unavailable", "BackupMap"));
    //
    // EN : Call this method when the traffic source status changes.
    //      Example:
    //      SetSourceStatus(new TrafficProviderStatus(
    //          "#FFFF0000", "Traffic source unavailable", "BackupMap"));
    //
    // ES : Llame a este método cuando cambie el estado de la fuente de tráfico.
    //      Ejemplo:
    //      SetSourceStatus(new TrafficProviderStatus(
    //          "#FFFF0000", "Traffic source unavailable", "BackupMap"));
    //
    // DE : Rufen Sie diese Methode auf, wenn sich der Status der Verkehrsquelle ändert.
    //      Beispiel:
    //      SetSourceStatus(new TrafficProviderStatus(
    //          "#FFFF0000", "Traffic source unavailable", "BackupMap"));
    private void SetSourceStatus(TrafficProviderStatus status)
    {
        SourceStatus = status;
        SourceStatusChanged?.Invoke(status);
    }

    // FR : Appelez cette méthode lorsqu'une nouvelle mise à jour de trafic est disponible.
    // EN : Call this method when a new traffic update is available.
    // ES : Llame a este método cuando haya una nueva actualización de tráfico disponible.
    // DE : Rufen Sie diese Methode auf, wenn eine neue Verkehrsaktualisierung verfügbar ist.
    private void SendTrafficUpdate(TrafficUpdate update)
    {
        TrafficAvailable?.Invoke(update);
    }

    // FR : OPTIONNEL — Exemple d'une commande envoyée par la fenêtre runtime
    //      au fournisseur de trafic. Elle demande à ZeDART d'effacer les pistes
    //      actuellement fournies par ce plugin.
    //
    // EN : OPTIONAL — Example of a command sent by the runtime window
    //      to the traffic provider. It asks ZeDART to clear the tracks
    //      currently supplied by this plugin.
    //
    // ES : OPCIONAL — Ejemplo de un comando enviado por la ventana runtime
    //      al proveedor de tráfico. Solicita a ZeDART que borre las pistas
    //      suministradas actualmente por este plugin.
    //
    // DE : OPTIONAL — Beispiel für einen Befehl, den das Runtime-Fenster
    //      an den Verkehrsdatenanbieter sendet. ZeDART wird aufgefordert,
    //      die aktuell von diesem Plugin gelieferten Ziele zu löschen.
    public void ClearTraffic()
    {
        ResetTraffic();
    }
    // FR : OPTIONNEL — ZeDART appelle cette méthode pour afficher l'interface
    //      du plugin pendant son exécution.
    // EN : OPTIONAL — ZeDART calls this method to display the plugin UI
    //      while it is running.
    // ES : OPCIONAL — ZeDART llama a este método para mostrar la interfaz
    //      del plugin durante la ejecución.
    // DE : OPTIONAL — ZeDART ruft diese Methode auf, um die Plugin-Oberfläche
    //      während der Ausführung anzuzeigen.
    public void ShowUI(object ownerWindow)
    {
        if (_runtimeWindow is not null)
            return;

        var owner = ownerWindow as Window
            ?? throw new InvalidOperationException("A parent window is required.");

        _runtimeWindow = new TemplateRuntimeWindow(this, _trafficSource);

        _runtimeWindow.Closed += (_, _) =>
        {
            _runtimeWindow = null;
        };

        _runtimeWindow.Show(owner);
    }


    // FR : OPTIONNEL — ZeDART appelle cette méthode avant l'arrêt du plugin
    //      afin de fermer proprement son interface runtime.
    // EN : OPTIONAL — ZeDART calls this method before stopping the plugin
    //      to properly close its runtime UI.
    // ES : OPCIONAL — ZeDART llama a este método antes de detener el plugin
    //      para cerrar correctamente su interfaz runtime.
    // DE : OPTIONAL — ZeDART ruft diese Methode vor dem Stoppen des Plugins auf,
    //      um dessen Runtime-Oberfläche ordnungsgemäß zu schließen.
    public void CloseUI()
    {
        _runtimeWindow?.Close();
        _runtimeWindow = null;
    }
}
