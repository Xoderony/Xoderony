using System.Windows;
using System.Windows.Input;

namespace Xoderony.Localization.Editor;

public partial class TranslationEditDialog : Window {

    public string Translation => TranslationTextBox.Text;

    public TranslationEditDialog(string title, string keyText, string localeText, string shortcutText, string translation, string confirmText, string cancelText) {
        InitializeComponent();
        Title = title;
        KeyText.Text = keyText;
        LocaleText.Text = localeText;
        ShortcutText.Text = shortcutText;
        TranslationTextBox.Text = translation;
        ConfirmButton.Content = confirmText;
        CancelButton.Content = cancelText;
    }

    private void OnLoaded(object sender, RoutedEventArgs e) {
        TranslationTextBox.Focus();
        TranslationTextBox.CaretIndex = TranslationTextBox.Text.Length;
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) != 0) {
            DialogResult = true;
            e.Handled = true;
        }
    }

    private void Confirm(object sender, RoutedEventArgs e) {
        DialogResult = true;
    }
}
