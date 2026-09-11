using Stride.Core.Assets.Editor.Quantum.NodePresenters;
using Stride.Core.Assets.Editor.Quantum.NodePresenters.Keys;
using Stride.Core.Diagnostics;
using System;

namespace Xoderony.Numerics.Stride.Editor;

internal sealed class Q16NodeUpdater : AssetNodePresenterUpdaterBase {
    private const double Minimum = (double)int.MinValue / Q16.Scale;
    private const double Maximum = (double)int.MaxValue / Q16.Scale;
    private const double Quantum = 1.0 / Q16.Scale;

    protected override void UpdateNode(IAssetNodePresenter node) {
        if (node.Type != typeof(Q16) || node.Parent is null || !node.IsVisible) {
            return;
        }

        var name = node.Name + "__Q16Decimal";
        if (node.Parent.TryGetChild(name) is not null) {
            node.IsVisible = false;
            return;
        }

        // double 能精确表示所有 Q16 刻度，避免 float 在较大数值处丢失低位。
        var editor = node.Factory.CreateVirtualNodePresenter(
            node.Parent,
            name,
            typeof(double),
            node.Order,
            () => ((Q16)node.Value).RawValue / (double)Q16.Scale,
            value => SetValue(node, (double)value),
            () => node.HasBase,
            () => node.IsInherited,
            () => node.IsOverridden,
            node.ResetOverride
        );
        editor.DisplayName = node.DisplayName;
        editor.IsReadOnly = node.IsReadOnly;
        editor.AttachedProperties.Set(NumericData.MinimumKey, Minimum);
        editor.AttachedProperties.Set(NumericData.MaximumKey, Maximum);
        editor.AttachedProperties.Set(NumericData.DecimalPlacesKey, 16);
        editor.AttachedProperties.Set(NumericData.SmallStepKey, Quantum);
        editor.AttachedProperties.Set(NumericData.LargeStepKey, 1.0);
        editor.AddDependency(node, true);
        node.IsVisible = false;
    }

    private static void SetValue(IAssetNodePresenter node, double value) {
        if (!double.IsFinite(value) || value < Minimum || value > Maximum) {
            GlobalLogger.GetLogger("Xoderony.Numerics.Stride.Editor").Warning("Q16 value is outside the representable range.");
            return;
        }
        var rawValue = (int)Math.Truncate(value * Q16.Scale);
        if (((Q16)node.Value).RawValue == rawValue) {
            return;
        }
        node.UpdateValue(new Q16 { RawValue = rawValue });
    }
}
