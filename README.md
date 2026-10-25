# PhytoGuard — application mobile .NET MAUI (C#)

Assistant agricole : identifier une maladie des cultures à partir d'une photo et de symptômes guidés, puis agir rapidement — **même sans réseau**.

## Fonctionnalités (cahier des charges)
| Fonctionnalité | Où dans le code |
|---|---|
| Prise de photo (caméra / galerie) | `Pages/DiagPhotoPage`, `Services/DeviceServices.cs` (PhotoService, MediaPicker) |
| Symptômes guidés | `Pages/DiagSymptomsPage`, `DiagnosisService.Symptoms` |
| Fiche de maladie | `Pages/DiseaseDetailPage`, données dans `Resources/Raw/diseases.json` |
| Conseils de traitement | Fiche de maladie + onglet **Conseils** |
| Historique de parcelle | Onglet **Historique**, tables SQLite `Parcel` / `DiagnosticRecord` |
| Mode hors ligne | SQLite locale, diagnostic par règles embarqué, GPS hors ligne, `SyncService` (envoi au retour du réseau) |
| Extras (écrans fournis) | Inscription / connexion / mot de passe oublié, parcelle GPS, rendez-vous spécialiste, paiement simulé |

## Parcours
`Bienvenue → Connexion / Inscription → Accueil (onglets Accueil · Parcelles · Historique · Conseils)`
Diagnostic : `Culture → Photo → Symptômes → Résultat → Fiche de maladie → Rendez-vous → Paiement (simulation)`

## Architecture
MVVM avec CommunityToolkit.Mvvm, injection de dépendances (`MauiProgram.cs`), navigation Shell (`AppShell.xaml`), SQLite (sqlite-net-pcl).
```
Models/      entités SQLite + modèles (Disease, DiagnosisResult…)
Services/    DatabaseService, AuthService, DiagnosisService, SyncService, Location/Photo
ViewModels/  un ViewModel par écran
Pages/       XAML des écrans   |  Controls/  LabeledEntry, StepBar
```

## Lancer le projet
Prérequis : Visual Studio 2022 17.8+ avec la charge de travail **.NET MAUI** (.NET 8).
1. Ouvrir `PhytoGuard.csproj` (ou le dossier) dans Visual Studio.
2. Choisir la cible **Android Emulator** (ou *Windows Machine*), puis F5.
3. Ligne de commande : `dotnet build -f net8.0-android` puis `dotnet build -t:Run -f net8.0-android`.

## Mode simulation (à dire dans la vidéo / le PDF)
- Le **SMS de vérification** n'est pas réellement envoyé : le code s'affiche dans une alerte (`AuthService.SendResetCodeAsync`). Brancher une passerelle SMS pour la production.
- Le **paiement** est une simulation (tarif de démonstration 3 000 FCFA, aucun débit).
- La **synchronisation** marque les données comme envoyées (`SyncService.SyncAsync`) : remplacer le `TODO` par un appel API REST.

## IA / ONNX (optionnel, en cours)
Le diagnostic fonctionne déjà par règles (symptômes → maladies). Pour ajouter un modèle d'image :
1. Entraîner et exporter avec `ml/train_export_onnx.py` (Google Colab) → `phytoguard.onnx`.
2. Copier le fichier dans `Resources/Raw/`.
3. Dans `PhytoGuard.csproj`, décommenter `USE_ONNX` et ajouter `Microsoft.ML.OnnxRuntime` + `SixLabors.ImageSharp`.
4. Vérifier que `Labels` dans `Services/OnnxImageClassifier.cs` suit l'ordre des classes d'entraînement.
Le score final combine alors la probabilité du modèle et les symptômes cochés.

## Dépôt GitHub (un commit par phase)
```
git init
git add Models Services ViewModels/AuthViewModels.cs Pages/Welcome* Pages/Login* Pages/Register* Pages/Forgot* Controls App.xaml App.xaml.cs AppShell.xaml AppShell.xaml.cs MauiProgram.cs PhytoGuard.csproj Platforms Resources .gitignore
git commit -m "Phase A : interfaces principales, connexion et inscription"
# puis Phase B : navigation, accueil, parcelles ; Phase C : diagnostic ; puis historique/conseils, rendez-vous, paiement, GPS
```
