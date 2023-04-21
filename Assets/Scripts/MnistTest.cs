using System.Linq;
using Unity.Barracuda;
using UnityEngine;


public class MnistTest : MonoBehaviour
{
    public NNModel model;
    public Texture2D image;
    public PredictionPlot predictionPlot;

    private Model runtimeModel;
    private IWorker engine;

    private float[] predicted;
    private bool isProcessing;

    private void Start()
    {
        runtimeModel = ModelLoader.Load(model);
        engine = WorkerFactory.CreateWorker(runtimeModel);
        
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

    private void OnDestroy()
    {
        engine?.Dispose();
    }
}
