#region Using directives
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.AnnotatedImage;

public enum PointerState { None, Single, Double }
public partial class ImageAnnotation<TItem> : BaseComponent, IAsyncDisposable
    where TItem : IImageAnnotationData
{
    #region Members
    private string containerPos => $"top:{y}px; left:{x}px; width:{imageWidth}px; height:{imageHeight}px; {borderStyle} z-index:{ImageAnnotationData!.CanvasInfo!.Order + 100}";
    private string borderStyle => ImageAnnotationData!.CanvasInfo!.Selected ? "border: solid; border-color: yellow;" : "" ;
    private double pageX;
    private double pageY;
    private bool pointerDown;
    private bool isMoved;
    private bool pendingUnselect;

    private double imageHeight => ImageAnnotationData!.CanvasInfo!.Scale * ImageAnnotationData.CanvasInfo.Height;
    private double imageWidth => ImageAnnotationData!.CanvasInfo!.Scale * ImageAnnotationData.CanvasInfo.Width;
    private double x => ImageAnnotationData!.CanvasInfo!.X - imageWidth / 2.0;
    private double y => ImageAnnotationData!.CanvasInfo!.Y - imageHeight / 2.0;
    private double xCenterOffset;
    private double yCenterOffset;   
    #endregion

    #region Methods
    protected override void BuildClasses(ClassBuilder builder)
    {
        builder.Append("imageannotation");
        base.BuildClasses(builder);
    }
    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        JSModule ??= new JSAnnotatedImageModule(JSRuntime!, VersionProvider!);
        return base.OnInitializedAsync();
    }
    /// <inheritdoc />
    protected override async Task OnFirstAfterRenderAsync()
    {
        await JSModule!.Initialize();
    }
    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing && Rendered)
            await JSModule.SafeDisposeAsync();
        await base.DisposeAsync(disposing);
    }
    private async void PointerDown(PointerEventArgs args)
    {
        if (pointerDown || ImageAnnotationData?.CanvasInfo is null)
            return;

        await JSModule!.SetPointerCapture(ElementRef, args.PointerId);
        pageX = args.PageX;
        pageY = args.PageY;
        CalculateCenterOffset(args.PageX, args.PageY);
        pointerDown = true;
        isMoved = false;
        pendingUnselect = ImageAnnotationData.CanvasInfo.Selected;
        ImageAnnotationData.CanvasInfo.Selected = true;
        await OnImageAnnotationSelected.InvokeAsync(ImageAnnotationData.Id);
    }
    private async Task PointerMove(PointerEventArgs args)
    {
        if (!pointerDown || ImageAnnotationData is null )
            return;
        await CalculateMovement(args.PageX, args.PageY);
    }
    private async Task PointerUp(PointerEventArgs args)
    {
        if (!pointerDown || ImageAnnotationData?.CanvasInfo is null)
            return;

        await CalculateMovement(args.PageX, args.PageY); 

        if (isMoved)
            await OnImageAnnotationEndMove.InvokeAsync(ImageAnnotationData.Id);

        if (!isMoved && pendingUnselect)
        {
            ImageAnnotationData.CanvasInfo.Selected = false;
            await OnImageAnnotationUnselected.InvokeAsync(ImageAnnotationData.Id);
        }
        pointerDown = false;
        return;
    }

    private async Task CalculateMovement(double x, double y)
    {
        if (!pointerDown || ImageAnnotationData?.CanvasInfo is null)
            return;

        if(x + xCenterOffset < AnnotatedImageClientRect.Left) x= AnnotatedImageClientRect.Left + xCenterOffset;
        if(x + xCenterOffset > AnnotatedImageClientRect.Right) x= AnnotatedImageClientRect.Right + xCenterOffset;     
        if(y + yCenterOffset < AnnotatedImageClientRect.Top) y= AnnotatedImageClientRect.Top + yCenterOffset;
        if(y + yCenterOffset > AnnotatedImageClientRect.Bottom) y= AnnotatedImageClientRect.Bottom + yCenterOffset; 

        ImageAnnotationData.CanvasInfo.X += x - pageX;
        ImageAnnotationData.CanvasInfo.Y += y - pageY;

        var previouslyMoved = isMoved;

        // Fire moved only if we moved more than a pixel
        if ((x - pageX == 0) && (y - pageY == 0))
            return;

        pageX = x;
        pageY = y;
        isMoved = true;

        if(!previouslyMoved)
            await OnImageAnnotationStartMove.InvokeAsync(ImageAnnotationData.Id);

        await OnImageAnnotationMoved.InvokeAsync(ImageAnnotationData.Id);

        //Console.WriteLine($"Movement: {x - pageX} {y - pageY}");


    }
    private async void CalculateCenterOffset(double x, double y)
    {
        var imgRect = await JSModule!.GetBoundingClientRect(ElementRef);
        xCenterOffset = x - imgRect.Left - (imgRect.Width / 2) ;
        yCenterOffset = y - imgRect.Top  - (imgRect.Height / 2) ;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the JSCameraModule instance.
    /// </summary>
    protected JSAnnotatedImageModule? JSModule { get; private set; }
    /// <summary>
    /// Gets or sets the JS runtime.
    /// </summary>
    // public ElementReference ImgRef { get; set; }    
    [Inject] private IJSRuntime? JSRuntime { get; set; }
    /// <summary>
    /// Gets or sets the version provider.
    /// </summary>
    [Inject] private IVersionProvider? VersionProvider { get; set; }
    /// <summary>
    /// The absolute or relative URL of the image.
    /// </summary>
    [Parameter] public string Source { get; set; } = string.Empty;
    /// <summary>
    /// Alternate text for an image.
    /// </summary>
    [Parameter] public string Text { get; set; } = string.Empty;
    /// <summary>
    /// Forces an image to take up the whole width.
    /// </summary>
    [Parameter] public bool Fluid { get; set; }

    [Parameter] public TItem? ImageAnnotationData { get; set; } 
    [Parameter] public EventCallback<string> OnImageAnnotationSelected { get; set; }
    [Parameter] public EventCallback<string> OnImageAnnotationStartMove { get; set; }
    [Parameter] public EventCallback<string> OnImageAnnotationMoved { get; set; }
    [Parameter] public EventCallback<string> OnImageAnnotationEndMove { get; set; }
    [Parameter] public EventCallback<string> OnImageAnnotationUnselected { get; set; }
    [Parameter] public BoundingClientRect AnnotatedImageClientRect { get; set; } = new();

    #endregion

}
