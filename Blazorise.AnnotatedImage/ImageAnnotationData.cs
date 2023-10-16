#region Using directives
#endregion

namespace Blazorise.AnnotatedImage;

public class CanvasInfo  : ICanvasInfo
{
    public double Order { get; set; }
    public double Scale { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public bool Selected { get; set; }
    public BoundingClientRect CanvasFloorRect { get; set; } = new();
}

public class ImageAnnotationData : IImageAnnotationData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public ICanvasInfo? CanvasInfo { get; set; } 
}
