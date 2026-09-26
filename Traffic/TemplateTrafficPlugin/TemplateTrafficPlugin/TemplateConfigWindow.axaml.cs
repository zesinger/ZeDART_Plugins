// ============================================================================
// FR : LOGIQUE DE LA FENÊTRE DE CONFIGURATION — OPTIONNELLE
//      Ce fichier est le code associé à TemplateConfigWindow.axaml.
//      Il récupère les valeurs saisies par l'utilisateur et indique au plugin
//      si la configuration a été validée ou annulée. Adaptez les propriétés
//      et les événements aux paramètres nécessaires à votre propre plugin.
//
// EN : CONFIGURATION WINDOW LOGIC — OPTIONAL
//      This file is the code behind TemplateConfigWindow.axaml.
//      It retrieves values entered by the user and tells the plugin whether
//      configuration was confirmed or cancelled. Adapt its properties and
//      events to the settings required by your own plugin.
//
// ES : LÓGICA DE LA VENTANA DE CONFIGURACIÓN — OPCIONAL
//      Este archivo contiene el código asociado a TemplateConfigWindow.axaml.
//      Recupera los valores introducidos por el usuario e indica al plugin si
//      la configuración fue confirmada o cancelada. Adapte sus propiedades y
//      eventos a los parámetros necesarios para su propio plugin.
//
// DE : LOGIK DES KONFIGURATIONSFENSTERS — OPTIONAL
//      Diese Datei enthält den Code zu TemplateConfigWindow.axaml.
//      Sie übernimmt die vom Benutzer eingegebenen Werte und teilt dem Plugin
//      mit, ob die Konfiguration bestätigt oder abgebrochen wurde. Passen Sie
//      Eigenschaften und Ereignisse an die Anforderungen Ihres Plugins an.
// ============================================================================

using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TemplateTrafficPlugin;

public partial class TemplateConfigWindow : Window
{
    // FR : Résultat de la fenêtre : true si l'utilisateur valide,
    //      false s'il annule.
    // EN : Window result: true if the user confirms,
    //      false if the user cancels.
    // ES : Resultado de la ventana: true si el usuario confirma,
    //      false si cancela.
    // DE : Ergebnis des Fensters: true bei Bestätigung,
    //      false bei Abbruch.
    public bool Result { get; private set; }

    // FR : Valeur saisie dans la fenêtre de configuration.
    // EN : Value entered in the configuration window.
    // ES : Valor introducido en la ventana de configuración.
    // DE : Im Konfigurationsfenster eingegebener Wert.
    public string TrafficSource { get; private set; } = "";

    public TemplateConfigWindow()
    {
        InitializeComponent();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        TrafficSource = TrafficSourceTextBox.Text?.Trim() ?? "";
        Result = true;
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Result = false;
        Close();
    }
}
