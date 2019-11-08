namespace Avalonia.Controls
{
    public enum WindowRegion
    {
        //Please don't reorder stuff here, I was lazy to write proper conversion code
        //so the order of values is matching one from GTK
        TopLeft = 0,
        Top,
        TopRight,
        Left,
        Right,
        BottomLeft,
        Bottom,
        BottomRight,
        Title,
        Client
    }
}
