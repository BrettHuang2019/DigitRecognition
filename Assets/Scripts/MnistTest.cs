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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int channel = 1;
            Tensor input = new Tensor(image, channel);
            Tensor output = engine.Execute(input).PeekOutput();
            input.Dispose();

            predicted = output.AsFloats().SoftMax().ToArray();
            Debug.Log(predicted);
        }
    }

    public void OnDrawTexture(RenderTexture texture)
    {
        if (!isProcessing)
        {
            DrawInference(texture);
        }
    }

    private void DrawInference(RenderTexture texture)
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