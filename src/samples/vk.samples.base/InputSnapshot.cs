using System.Collections.Generic;
using System.Numerics;

namespace Veldrid.Platform
{
    public interface InputSnapshot
    {
        IReadOnlyList<Veldrid.Platform.KeyEvent> KeyEvents { get; }
        IReadOnlyList<MouseButtonEvent> MouseEvents { get; }
        IReadOnlyList<char> KeyCharPresses { get; }
        bool IsMouseDown(MouseButton button);
        Vector2 MousePosition { get; }
        float WheelDelta { get; }
    }
}