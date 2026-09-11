using System;
using System.Collections.Generic;
using Stride.Core.Assets.Editor.Services;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core.Diagnostics;

namespace Xoderony.Numerics.Stride.Editor;

public sealed class Q16EditorPlugin : AssetsPlugin {
    private readonly Q16NodeUpdater _updater = new();

    public override void InitializePlugin(ILogger logger) {
    }

    public override void InitializeSession(SessionViewModel session) {
        session.AssetViewProperties.RegisterNodePresenterUpdater(_updater);
    }

    public override void RegisterPrimitiveTypes(ICollection<Type> primitiveTypes) {
    }

    protected override void SessionDisposed(SessionViewModel session) {
        session.AssetViewProperties.UnregisterNodePresenterUpdater(_updater);
    }
}
