#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.AnnotatedImage;


public class JSAnnotatedImageModule : BaseJSModule
{
    #region Constructors
    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    public JSAnnotatedImageModule(IJSRuntime jsRuntime, IVersionProvider versionProvider) : base(jsRuntime, versionProvider)
    {
    }
    #endregion

    #region Methods
    public virtual ValueTask Initialize()
        => InvokeSafeVoidAsync("initialize");
    public virtual ValueTask SetPointerCapture(ElementReference elementRef, long pointerId)
        => InvokeSafeVoidAsync("setPointerCapture",elementRef,pointerId);

    public virtual ValueTask<string> GetImageAnnotationDataURL(ElementReference elementRef)
        => InvokeSafeAsync<string>("getImageAnnotationDataURL", elementRef);

    public virtual ValueTask<string> GetImageAnnotationDataURLById(string id)
        => InvokeSafeAsync<string>("getImageAnnotationDataURLById", id);


    public virtual ValueTask<string> CanvasElementToDataURL(ElementReference elementRef)
        => InvokeSafeAsync<string>("canvasElementToDataURL", elementRef);

    public virtual ValueTask<BoundingClientRect> GetBoundingClientRect(ElementReference elementRef)
        => InvokeSafeAsync<BoundingClientRect>("getBoundingClientRect", elementRef);

    public virtual ValueTask<string> GetBase64Image(ElementReference img)
        => InvokeSafeAsync<string>("getBase64Image", img);

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.AnnotatedImage/blazorise.annotatedimage.js?v={VersionProvider.Version}";

    #endregion

}
