"""Google Colab : entraîner un classifieur de maladies (MobileNetV2) et l'exporter en ONNX.
Dossier de données attendu : data/train/<id_maladie>/*.jpg  (les noms de dossiers = ids de diseases.json)
!pip install torch torchvision
"""
import torch, torch.nn as nn
from torchvision import datasets, transforms, models

tf = transforms.Compose([
    transforms.Resize((224, 224)), transforms.ToTensor(),
    transforms.Normalize([0.485, 0.456, 0.406], [0.229, 0.224, 0.225])])
ds = datasets.ImageFolder("data/train", transform=tf)
print("Classes (à recopier dans OnnxImageClassifier.Labels) :", ds.classes)
dl = torch.utils.data.DataLoader(ds, batch_size=32, shuffle=True)

model = models.mobilenet_v2(weights=models.MobileNet_V2_Weights.DEFAULT)
model.classifier[1] = nn.Linear(model.last_channel, len(ds.classes))
opt = torch.optim.Adam(model.parameters(), lr=1e-4)
loss_fn = nn.CrossEntropyLoss()

for epoch in range(5):
    model.train()
    for x, y in dl:
        opt.zero_grad(); loss = loss_fn(model(x), y); loss.backward(); opt.step()
    print("epoch", epoch, "loss", float(loss))

model.eval()
torch.onnx.export(model, torch.randn(1, 3, 224, 224), "phytoguard.onnx",
                  input_names=["input"], output_names=["logits"], opset_version=13)
print("OK -> copier phytoguard.onnx dans Resources/Raw/")
