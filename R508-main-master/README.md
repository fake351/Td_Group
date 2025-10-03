Projet API .NET 8 + Blazor


    Ce projet contient une API développée en .NET 8 avec Entity Framework Core et PostgreSQL, ainsi qu’une application Blazor servant d’interface utilisateur. Il inclut également des tests unitaires et des tests end-to-end pour garantir la fiabilité de l’ensemble.
    Prérequis
    Avant de pouvoir exécuter le projet, il est nécessaire d’installer :
    Le SDK .NET 8.0.100
    PostgreSQL (version 15 ou supérieure)
    Un IDE comme Visual Studio 2022 ou Visual Studio Code
Configuration

    Créez une base de données PostgreSQL nommée qualidb sur votre serveur local.
    Dans le fichier de configuration appsettings.Development.json du projet API, définissez la chaîne de connexion à PostgreSQL avec vos paramètres (nom de base, utilisateur, mot de passe).
    Le projet est configuré pour utiliser cette chaîne de connexion grâce à l’injection de dépendances dans la configuration de l’application (fichier Program.cs).
    Le DbContext de l’application (classe AppDbContext) utilise cette connexion pour accéder à la base de données et définir les relations entre les entités Produit, Marque et TypeProduit.
Installation et exécution

    Restaurez les dépendances du projet avec la commande adaptée à .NET.
    Appliquez les migrations Entity Framework afin de créer les tables de la base de données.
    Lancez l’API depuis le projet nommé App. L’API sera accessible en local via les ports HTTPS (5001) et HTTP (5000).
    Lancez ensuite l’application Blazor depuis le projet BlazorApp1. L’interface sera disponible via un port configuré automatiquement (par exemple 7143).
Tests

    Les tests unitaires liés à l’API se trouvent dans le projet Tests.
    Les tests end-to-end de l’application Blazor se trouvent dans le projet BlazorApp.E2ETests.
    Ces tests peuvent être exécutés séparément avec la commande de test de .NET.
Structure du projet

    Le projet est organisé de la manière suivante :
    Un dossier App contenant l’API et la logique métier.
    Un dossier BlazorApp1 contenant le frontend en Blazor.
    Un dossier Tests regroupant les tests unitaires.
    Un dossier BlazorApp.E2ETests regroupant les tests end-to-end.
    Un fichier fa17c929.sln représentant la solution Visual Studio.
    Un fichier global.json qui définit la version du SDK .NET utilisée.
Gestion de la base de données

    Les migrations Entity Framework permettent de créer et de mettre à jour la base de données en fonction du modèle défini dans le code.
    Lorsqu’une modification est apportée au modèle de données, une migration doit être générée puis appliquée à la base.
    La base de données PostgreSQL est ainsi synchronisée automatiquement avec le code de l’application.
    Variables d’environnement
    La chaîne de connexion peut être définie soit dans le fichier appsettings.json, soit à l’aide d’une variable d’environnement. Cela permet de ne pas exposer directement des informations sensibles comme le mot de passe de la base de données dans le code.
Bonnes pratiques

    Évitez de stocker la chaîne de connexion directement dans le code de la classe DbContext. Utilisez plutôt les fichiers de configuration et les variables d’environnement.
    Pour un déploiement en production, configurez correctement la sécurité (HTTPS, certificats SSL, stratégies CORS, identifiants sécurisés pour PostgreSQL).
    Vérifiez que la base de données est toujours accessible avant d’appliquer les migrations.
Auteur : Megias Victor 
