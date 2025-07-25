using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;

public class QRCodeRecenter : MonoBehaviour
{
    [SerializeField]
    private ARSession session;
    [SerializeField]
    private ARSessionOrigin sessionOrigin;
    [SerializeField]
    private ARCameraManager cameraManager;
    [SerializeField]
    private List<Target> navigationTargetObjects = new List<Target>();

    private Texture2D cameraImagetexture;
    private IBarcodeReader reader = new BarcodeReader();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            SetQrcodeRecenterTarget("Accounts");
        }
    }

    private void OnEnable()
    {
        cameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnDisable()
    {
        cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if(!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            return;
        }

        var conversionParams = new XRCpuImage.ConversionParams
        {
            //get the entire image
            inputRect = new RectInt(0, 0, image.width, image.height),

            //Downsample by 2
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

            //choose RGBA format
            outputFormat = TextureFormat.RGBA32,

            //Flip across the vertical axis (mirror image)
            transformation = XRCpuImage.Transformation.MirrorY
        };

        //See how many bytes you need to store the final image
        int Size = image.GetConvertedDataSize(conversionParams);

        //Allocate a buffer to store the image
        var buffer = new NativeArray<byte>(Size, Allocator.Temp);

        //Extract the image data
        image.Convert(conversionParams, buffer);

        // The image was converted to RGBA32 format and written into the provided buffer
        // so you can dispose of the XRCpuImage. You must do this or it will leak resources.

        image.Dispose();

        // At this point, you can process the image, pass it to a computer vision algorithm, etc.
        // In this example, you apply it to a texture to visualize it.

        // You've got the data; let's put it into a texture so you can visualize it.
        cameraImagetexture = new Texture2D(
            conversionParams.outputDimensions.x,
            conversionParams.outputDimensions.y,
            conversionParams.outputFormat,
            false);

        cameraImagetexture.LoadRawTextureData(buffer);
        cameraImagetexture.Apply();

        // Done with your temporary data, so you can dispose it.
        buffer.Dispose();

        // Detect and decode the barcode inside the bitmap
        var result = reader.Decode(cameraImagetexture.GetPixels32(), cameraImagetexture.width, cameraImagetexture.height);

        //Do something with the result
        if (result != null)
        {
            SetQrcodeRecenterTarget(result.Text);
        }
    }

    private void SetQrcodeRecenterTarget(string targetText)
    {
        Target currentTarget = navigationTargetObjects.Find(x => x.Name.ToLower().Equals(targetText.ToLower()));
        if(currentTarget != null) { 
            //Reset position and rotation of arsession
            session.Reset();

            //Add offset for recentering
            sessionOrigin.transform.position = currentTarget.PositionObject.transform.position;
            sessionOrigin.transform.rotation = currentTarget.PositionObject.transform.rotation;
        }
    }
}
