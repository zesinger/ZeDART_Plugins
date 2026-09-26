// ============================================================================
// FR : POINT D'ENTRÉE DU PLUGIN
//      Ce fichier définit le plugin chargé par ZeDART. Il fournit son identité,
//      son type et sa version, puis crée le fournisseur de trafic utilisé
//      pendant l'exécution. Il peut également afficher une fenêtre de
//      configuration avant le démarrage du radar.
//      Voir ci-dessous pour le détail des interfaces à implémenter.
//
// EN : PLUGIN ENTRY POINT
//      This file defines the plugin loaded by ZeDART. It provides its identity,
//      type and version, then creates the traffic provider used at runtime.
//      It can also display a configuration window before the radar starts.
//      See below for details about the interfaces to implement.
//
// ES : PUNTO DE ENTRADA DEL PLUGIN
//      Este archivo define el plugin cargado por ZeDART. Proporciona su
//      identidad, tipo y versión, y crea el proveedor de tráfico utilizado
//      durante la ejecución. También puede mostrar una ventana de configuración
//      antes del inicio del radar.
//      Véase más abajo el detalle de las interfaces que deben implementarse.
//
// DE : EINSTIEGSPUNKT DES PLUGINS
//      Diese Datei definiert das von ZeDART geladene Plugin. Sie stellt
//      Kennung, Typ und Version bereit und erstellt anschließend den zur
//      Laufzeit verwendeten Verkehrsdatenanbieter. Optional kann vor dem
//      Start des Radars ein Konfigurationsfenster angezeigt werden.
//      Einzelheiten zu den zu implementierenden Schnittstellen siehe unten.
// ============================================================================

using Avalonia.Controls;
using Avalonia.Threading;
using ZeDART.Contracts.Plugins;

namespace TemplateTrafficPlugin;

// FR : OBLIGATOIRE — IZeDartPlugin est l'interface principale d'un plugin ZeDART.
//      Elle fournit à ZeDART l'identité, le type et la version du plugin et
//      permet de créer son instance.
//
// EN : MANDATORY — IZeDartPlugin is the main interface of a ZeDART plugin.
//      It provides ZeDART with the plugin identity, type and version and
//      allows its instance to be created.
//
// ES : OBLIGATORIO — IZeDartPlugin es la interfaz principal de un plugin ZeDART.
//      Proporciona a ZeDART la identidad, el tipo y la versión del plugin y
//      permite crear su instancia.
//
// DE : ERFORDERLICH — IZeDartPlugin ist die Hauptschnittstelle eines ZeDART-Plugins.
//      Sie stellt ZeDART Kennung, Typ und Version des Plugins bereit und
//      ermöglicht die Erstellung seiner Instanz.
//
// FR : OPTIONNEL — IZeDartConfigurablePlugin permet d'afficher une fenêtre de
//      configuration avant la création et le démarrage du fournisseur de trafic.
//      Supprimez cette interface si votre plugin ne nécessite aucune configuration.
//
// EN : OPTIONAL — IZeDartConfigurablePlugin allows a configuration window to be
//      displayed before the traffic provider is created and started.
//      Remove this interface if your plugin does not require configuration.
//
// ES : OPCIONAL — IZeDartConfigurablePlugin permite mostrar una ventana de
//      configuración antes de crear e iniciar el proveedor de tráfico.
//      Elimine esta interfaz si su plugin no necesita configuración.
//
// DE : OPTIONAL — IZeDartConfigurablePlugin ermöglicht die Anzeige eines
//      Konfigurationsfensters, bevor der Verkehrsdatenanbieter erstellt und
//      gestartet wird. Entfernen Sie diese Schnittstelle, wenn keine
//      Konfiguration erforderlich ist.
public sealed class TemplateTrafficPlugin : IZeDartPlugin, IZeDartConfigurablePlugin
{


    // FR : OBLIGATOIRE — Identifiant unique du plugin.
    // EN : MANDATORY — Unique identifier of the plugin.
    // ES : OBLIGATORIO — Identificador único del plugin.
    // DE : ERFORDERLICH — Eindeutige Kennung des Plugins.
    public string Id => "TemplateTrafficPlugin";

    // FR : OBLIGATOIRE — Nom du plugin affiché par ZeDART.
    // EN : MANDATORY — Plugin name displayed by ZeDART.
    // ES : OBLIGATORIO — Nombre del plugin mostrado por ZeDART.
    // DE : ERFORDERLICH — Von ZeDART angezeigter Name des Plugins.
    public string DisplayName => "Template Traffic Plugin";

    // FR : OBLIGATOIRE — Type de plugin. Ne pas modifier pour un fournisseur de trafic.
    // EN : MANDATORY — Plugin type. Do not change for a traffic provider.
    // ES : OBLIGATORIO — Tipo de plugin. No modificar para un proveedor de tráfico.
    // DE : ERFORDERLICH — Plugin-Typ. Für einen Verkehrsdatenanbieter nicht ändern.
    public ZeDartPluginKind Kind => ZeDartPluginKind.TrafficProvider;

    // FR : OBLIGATOIRE — Version du plugin.
    // EN : MANDATORY — Plugin version.
    // ES : OBLIGATORIO — Versión del plugin.
    // DE : ERFORDERLICH — Version des Plugins.
    public string Version => "1.0.0";

    // FR : OPTIONNEL — Affiche une fenêtre de configuration avant le démarrage.
    //      Supprimez IZeDartConfigurablePlugin, cette méthode et les fichiers
    //      TemplateConfigWindow.* si votre plugin n'a pas besoin de configuration.
    //
    // EN : OPTIONAL — Displays a configuration window before startup.
    //      Remove IZeDartConfigurablePlugin, this method and the
    //      TemplateConfigWindow.* files if your plugin does not need configuration.
    //
    // ES : OPCIONAL — Muestra una ventana de configuración antes del inicio.
    //      Elimine IZeDartConfigurablePlugin, este método y los archivos
    //      TemplateConfigWindow.* si su plugin no necesita configuración.
    //
    // DE : OPTIONAL — Zeigt vor dem Start ein Konfigurationsfenster an.
    //      Entfernen Sie IZeDartConfigurablePlugin, diese Methode und die
    //      TemplateConfigWindow.* Dateien, wenn Ihr Plugin keine Konfiguration benötigt.
    public bool Configure(PluginConfigContext context)
    {
        var owner = context.OwnerWindow as Window ?? throw new InvalidOperationException("A parent window is required.");
        var window = new TemplateConfigWindow();
        var frame = new DispatcherFrame();
        window.Closed += (_, _) =>
        {
            frame.Continue = false;
        };
        window.ShowDialog(owner);
        Dispatcher.UIThread.PushFrame(frame);
        if (!window.Result) return false;
        context.Settings["TrafficSource"] = window.TrafficSource;
        return true;
    }

    // FR : OBLIGATOIRE — Crée le fournisseur de trafic utilisé par ZeDART.
    // EN : MANDATORY — Creates the traffic provider used by ZeDART.
    // ES : OBLIGATORIO — Crea el proveedor de tráfico utilizado por ZeDART.
    // DE : ERFORDERLICH — Erstellt den von ZeDART verwendeten Verkehrsdatenanbieter.
    public object CreateInstance(PluginContext context)
    {
        return new TemplateTrafficProvider(context);
    }
}
