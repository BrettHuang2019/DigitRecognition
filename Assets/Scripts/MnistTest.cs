using System.Linq;
using Unity.Barracuda;
using UnityEngine;


public class MnistTest : MonoBehaviour
{
    public NNModel model;
    public Texture2D image;
    public PredictionPlot predictionPlot;
    public PreviewManager previewManager;

    private Model runtimeModel;
    private IWorker engine;

    private float[] predicted;
    private bool isProcessing;

    private void Start()
    {
        runtimeModel = ModelLoader.Load(model);
        engine = WorkerFactory.CreateWorker(runtimeModel);
        
        // Instant Inference
        Tensor input = new Tensor(image, 1);
        Tensor output = engine.Execute(input).PeekOutput();
        input.Dispose();
        predicted = output.AsFloats().SoftMax().ToArray();
        
        //Print prediction
        string result = "";
        foreach (float predict in predicted)
        {
            result += predict + ",";
        }

        Debug.Log(result);
    }
    
    public void OnDrawTexture(Texture texture)
    {
        if (!isProcessing)
        {
            DrawInference(texture);
        }
    }

    private void DrawInference(Texture texture)
    {
        isProcessing = true;
        int channel = 1;
        Tensor input = new Tensor(texture, channel);
        Tensor output = engine.Execute(input).PeekOutput();
        input.Dispose();
        predicted = output.AsFloats().SoftMax().ToArray();
        predictionPlot.UpdatePlot(predicted);
        isProcessing = false;
    }

    public void DrawInferentFromPreview()
    {
        if (!previewManager.ScaledTexture) return;
        
        isProcessing = true;
        int channel = 1;
        Tensor input = new Tensor(previewManager.ScaledTexture, channel);
        Tensor output = engine.Execute(input).PeekOutput();
        input.Dispose();
        predicted = output.AsFloats().SoftMax().ToArray();
        predictionPlot.UpdatePlot(predicted);
        isProcessing = false;
    }

    private void OnDestroy()
    {
        engine?.Dispose();
    }
}
