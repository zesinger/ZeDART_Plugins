// ============================================================================
// FR : LOGIQUE DE LA FENÊTRE RUNTIME — OPTIONNELLE
//      Ce fichier est le code associé à TemplateRuntimeWindow.axaml.
//      Il permet à la fenêtre d'échanger avec le fournisseur de trafic pendant
//      l'exécution de ZeDART : afficher des informations provenant du provider
//      et lui envoyer des commandes. Adaptez ou remplacez cette logique selon
//      les fonctions runtime nécessaires à votre plugin.
//
// EN : RUNTIME WINDOW LOGIC — OPTIONAL
//      This file is the code behind TemplateRuntimeWindow.axaml.
//      It allows the window to interact with the traffic provider while ZeDART
//      is running: displaying information from the provider and sending commands
//      to it. Adapt or replace this logic according to your plugin's runtime needs.
//
// ES : LÓGICA DE LA VENTANA RUNTIME — OPCIONAL
//      Este archivo contiene el código asociado a TemplateRuntimeWindow.axaml.
//      Permite que la ventana interactúe con el proveedor de tráfico durante
//      la ejecución de ZeDART: mostrar información del proveedor y enviarle
//      comandos. Adapte o sustituya esta lógica según las necesidades del plugin.
//
// DE : LOGIK DES RUNTIME-FENSTERS — OPTIONAL
//      Diese Datei enthält den Code zu TemplateRuntimeWindow.axaml.
//      Sie ermöglicht während der Ausführung von ZeDART den Austausch mit dem
//      Verkehrsdatenanbieter: Informationen des Providers können angezeigt und
//      Befehle an ihn gesendet werden. Passen oder ersetzen Sie diese Logik
//      entsprechend den Anforderungen Ihres Plugins.
// ============================================================================

using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TemplateTrafficPlugin;
public partial class TemplateRuntimeWindow : Window
{
    private readonly TemplateTrafficProvider? _provider;

    // FR : Constructeur requis par le chargeur XAML Avalonia.
    // EN : Constructor required by the Avalonia XAML loader.
    // ES : Constructor requerido por el cargador XAML de Avalonia.
    // DE : Vom Avalonia-XAML-Loader benötigter Konstruktor.
    public TemplateRuntimeWindow()
    {
        InitializeComponent();
    }

    public TemplateRuntimeWindow(TemplateTrafficProvider provider, string trafficSource) : this()
    {
        // FR : Conserve une référence vers le provider afin que la fenêtre
        //      puisse lui envoyer des commandes.
        // EN : Keeps a reference to the provider so the window
        //      can send commands to it.
        // ES : Conserva una referencia al proveedor para que la ventana
        //      pueda enviarle comandos.
        // DE : Speichert eine Referenz auf den Provider, damit das Fenster
        //      Befehle an ihn senden kann.
        _provider = provider;
        TrafficSourceTextBlock.Text = string.IsNullOrWhiteSpace(trafficSource) ? "(not configured)" : trafficSource;
    }

    private void OnClearTrafficClick(object? sender, RoutedEventArgs e)
    {
        _provider?.ClearTraffic();
    }
}