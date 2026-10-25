#if USE_ONNX
// Activer : ajouter dans le .csproj
//   <DefineConstants>$(DefineConstants);USE_ONNX</DefineConstants>
//   <PackageReference Include="Microsoft.ML.OnnxRuntime" Version="1.18.1" />
//   <PackageReference Include="SixLabors.ImageSharp" Version="3.1.5" />
// Placer le modèle exporté (ml/train_export_onnx.py) dans Resources/Raw/phytoguard.onnx
// puis, dans MauiProgram.cs, remplacer NullImageClassifier par OnnxImageClassifier.
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PhytoGuard.Services;

public class OnnxImageClassifier : IImageClassifier
{
    // Ordre IDENTIQUE à celui des classes lors de l'entraînement (voir ml/train_export_onnx.py).
    private static readonly string[] Labels =
        { "mildiou_tomate", "alternariose_tomate", "oidium_tomate", "fletrissement_tomate", "rouille_mais" };
    private InferenceSession? _session;
    public bool IsAvailable => true;

    private async Task<InferenceSession> GetSessionAsync()
    {
        if (_session != null) return _session;
        using var s = await FileSystem.OpenAppPackageFileAsync("phytoguard.onnx");
        using var ms = new MemoryStream();
        await s.CopyToAsync(ms);
        return _session = new InferenceSession(ms.ToArray());
    }

    public async Task<IReadOnlyDictionary<string, float>> ClassifyAsync(string photoPath)
    {
        var session = await GetSessionAsync();
        using var img = await Image.LoadAsync<Rgb24>(photoPath);
        img.Mutate(x => x.Resize(224, 224));
        var mean = new[] { 0.485f, 0.456f, 0.406f };
        var std = new[] { 0.229f, 0.224f, 0.225f };
        var tensor = new DenseTensor<float>(new[] { 1, 3, 224, 224 });
        for (int y = 0; y < 224; y++)
            for (int x = 0; x < 224; x++)
            {
                var p = img[x, y];
                tensor[0, 0, y, x] = (p.R / 255f - mean[0]) / std[0];
                tensor[0, 1, y, x] = (p.G / 255f - mean[1]) / std[1];
                tensor[0, 2, y, x] = (p.B / 255f - mean[2]) / std[2];
            }
        var inputName = session.InputMetadata.Keys.First();
        using var results = session.Run(new[] { NamedOnnxValue.CreateFromTensor(inputName, tensor) });
        var logits = results.First().AsEnumerable<float>().ToArray();
        var max = logits.Max();
        var exps = logits.Select(l => MathF.Exp(l - max)).ToArray();
        var sum = exps.Sum();
        var dict = new Dictionary<string, float>();
        for (int i = 0; i < Labels.Length && i < exps.Length; i++) dict[Labels[i]] = exps[i] / sum;
        return dict;
    }
}
#endif
