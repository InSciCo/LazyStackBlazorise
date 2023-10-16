#region Using directives
#endregion

namespace Blazorise.AnnotatedImage;

public interface ICanvasInfo
{
    double Order { get; set; }
    double Scale { get; set; }
    double Width { get; set; }
    double Height { get; set; }
    double X { get; set; }
    double Y { get; set; }
    bool Selected { get; set; }
}

public interface IImageAnnotationData
{
    string Id { get; set; }
    string Name { get; set; }
    string Note { get; set; }
    string Source { get; set; }
    ICanvasInfo? CanvasInfo { get; set; } 
}
