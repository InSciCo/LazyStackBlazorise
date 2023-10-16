#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Camera;

/// <summary>
/// Contracts for the Camera JS module.
/// </summary>
public class JSCameraModule : BaseJSModule
{
    private DotNetObjectReference<JSCameraModule> moduleInstance;

    #region Constructors
    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    public JSCameraModule(IJSRuntime jsRuntime, IVersionProvider versionProvider) : base(jsRuntime, versionProvider)
	{
		moduleInstance = DotNetObjectReference.Create(this);
    }
	#endregion

	#region Methods

	/// <summary>
	/// Initialize Camera with specified options
	/// </summary>
	/// <param name="videoRef">video element reference</param>
	/// <param name="mirrorImage"></param>
	/// <param name="facingMode">Must be one of: "user" | "environment"</param>
	/// <returns></returns>
	public virtual ValueTask<bool> Initialize(ElementReference videoRef, bool mirrorImage, string facingMode)
		=> InvokeSafeAsync<bool>("initialize", videoRef, mirrorImage, facingMode, moduleInstance);

	/// <summary>
	/// Take picture and return base64 encoded string.
	/// </summary>
	/// <returns>A task that represents the asynchronous operation.</returns>
	public virtual ValueTask<string> TakePicture()
		=> InvokeSafeAsync<string>("takepicture");

	public virtual async ValueTask<(int, int)> GetWidthAndHeight()
	{
		var resultArray = await InvokeAsync<int[]>("getWidthAndHeight");
		return (resultArray[0], resultArray[1]);
	}

	[JSInvokable]
	public void OnCameraInitialized()
	{
		CameraInitializedHandlerEvent?.Invoke();

    }
	public delegate void CameraInitializedHandler();
	public event CameraInitializedHandler CameraInitializedHandlerEvent;

    [JSInvokable]
    public void OnCameraUnavailable()
    {
        CameraUnavailableHandlerEvent?.Invoke();
    }
    public delegate void CameraUnavailabledHandler();
    public event CameraUnavailabledHandler CameraUnavailableHandlerEvent;

    protected override async ValueTask DisposeAsync(bool disposing)
	{
		if(disposing)
		{
			try
			{
				await InvokeVoidAsync("releaseCamera");
			} catch
			{
				; // ingore error
			}
		}
	}
    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Camera/blazorise.camera.js?v={VersionProvider.Version}";

	#endregion
}
