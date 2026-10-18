# PhytoGuard

PhytoGuard est une application développée avec .NET MAUI pour accompagner les utilisateurs dans le suivi de leurs cultures. Elle permet de renseigner une parcelle, de noter des symptômes et de préparer une demande de conseil auprès d’un spécialiste.

> **Important :** les informations affichées par l’application ne remplacent pas le diagnostic d’un professionnel agricole. Le modèle d’intelligence artificielle de reconnaissance des maladies n’est pas encore intégré à l’application.

## Sommaire

- [Objectifs](#objectifs)
- [Fonctionnalités](#fonctionnalités)
- [Parcours utilisateur](#parcours-utilisateur)
- [Technologies](#technologies)
- [Prérequis](#prérequis)
- [Installation et lancement](#installation-et-lancement)
- [GPS et cartes](#gps-et-cartes)
- [Données enregistrées](#données-enregistrées)
- [Limites connues](#limites-connues)
- [Sécurité et confidentialité](#sécurité-et-confidentialité)
- [Améliorations envisagées](#améliorations-envisagées)
- [Captures d’écran](#captures-décran)
- [Auteur et contexte](#auteur-et-contexte)

## Objectifs

PhytoGuard vise à faciliter le suivi quotidien des cultures en regroupant dans une même application :

- les informations d’une parcelle ;
- les photos de plantes ;
- les symptômes observés ;
- l’historique des observations ;
- des conseils agricoles ;
- une possibilité de contacter ou consulter un spécialiste.

## Fonctionnalités

### Parcelles

- Saisie du nom d’une parcelle.
- Saisie de la culture cultivée.
- Récupération de la position GPS de l’appareil, selon les autorisations accordées.
- Affichage des informations enregistrées dans les pages de l’application.

### Diagnostic guidé

- Prise d’une photo avec la caméra ou sélection d’une image dans la galerie.
- Affichage de la photo sélectionnée.
- Sélection de symptômes, par exemple les taches jaunes, les feuilles sèches ou la poudre blanche.
- Saisie de la période depuis laquelle les symptômes sont observés.
- Présentation d’une page récapitulative.

La photo et les symptômes servent actuellement à documenter une observation. Ils ne produisent pas encore une identification automatique fiable de la maladie.

### Navigation

L’application comprend des pages consacrées à l’accueil, aux parcelles, à l’historique, aux conseils, au diagnostic et aux spécialistes. La navigation entre ces pages dépend de la configuration des routes dans le projet.

### Paiement et rendez-vous

Les écrans de rendez-vous et de paiement sont prévus pour présenter le parcours de réservation. Si une simulation de paiement est utilisée, elle ne débite pas d’argent et ne constitue pas une intégration à un prestataire de paiement réel.

## Parcours utilisateur

Un parcours typique dans l’application est le suivant :

1. Enregistrer une parcelle et sa culture.
2. Consulter les informations de la parcelle.
3. Commencer un diagnostic.
4. Prendre ou choisir une photo.
5. Indiquer les symptômes et leur durée.
6. Consulter le récapitulatif et, si nécessaire, demander conseil à un spécialiste.

## Technologies

- **C#**
- **.NET MAUI**
- **XAML**
- **Preferences de .NET MAUI** pour certaines données locales
- **MediaPicker** pour prendre ou sélectionner une photo
- **Geolocation** pour récupérer la position de l’appareil
- **Shell** pour la navigation entre les pages

Les versions exactes du SDK et des dépendances sont indiquées dans les fichiers du projet, notamment le fichier `.csproj`.

## Prérequis

Pour ouvrir et lancer le projet, il faut installer :

- le SDK .NET compatible avec le fichier `.csproj` ;
- Visual Studio avec la charge de travail **Développement d’applications .NET Multi-platform App UI** pour MAUI, ou l’environnement MAUI configuré utilisé par le projet ;
- les outils correspondant à la plateforme ciblée, par exemple Windows ou Android.

La compilation pour Android peut aussi nécessiter Android SDK, les outils de compilation Android et un émulateur ou un appareil physique.

## Installation et lancement

### Cloner le dépôt

Remplace `URL_DU_DEPOT` par l’adresse de ton dépôt GitHub :

```bash
git clone URL_DU_DEPOT
cd PhytoGuard