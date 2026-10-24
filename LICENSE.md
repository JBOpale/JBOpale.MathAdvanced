# Licence / License

**MathAdvanced** — Copyright (C) 2026 Jérôme FRANCOIS / JB-Opale

SPDX-License-Identifier: `LGPL-3.0-or-later`

## Français

Cette bibliothèque est distribuée selon les termes de la **GNU Lesser General
Public License** version 3, ou (à votre choix) toute version ultérieure.
Les textes complets figurent dans [`COPYING.LESSER`](COPYING.LESSER) (LGPL v3)
et [`COPYING`](COPYING) (GPL v3, que la LGPL v3 incorpore par référence).

### Utilisation dans une application commerciale ou propriétaire

Vous pouvez utiliser MathAdvanced dans une application **commerciale ou
propriétaire**, sans publier le code source de votre application, à condition
de respecter la LGPL v3 pour la bibliothèque elle-même :

1. **Lier dynamiquement** : référencer le paquet NuGet / la DLL
   `JBOPaleAPI.MathAdvanced.dll` telle quelle (référence de projet ou de
   paquet). Ne pas fusionner la DLL dans votre exécutable (ILMerge, Fody
   Costura, NativeAOT, publication « single-file » avec compilation statique)
   sans fournir de quoi permettre à l'utilisateur de la remplacer (section 4
   de la LGPL v3).
2. **Conserver les mentions** de copyright et de licence, et fournir une copie
   de la LGPL v3 (et de la GPL v3) avec votre produit, par exemple dans un
   fichier « Licences tierces ».
3. **Permettre le remplacement** : l'utilisateur doit pouvoir remplacer la
   version de MathAdvanced par une version modifiée de la bibliothèque, et le
   rétro-ingénierie (« reverse engineering ») nécessaire au débogage de cette
   modification ne doit pas être interdite par votre contrat de licence.
4. **Publier vos modifications** de MathAdvanced elle-même (pas celles de votre
   application) sous LGPL v3.
5. Si votre produit est un **appareil verrouillé** (« tivoïsation »), fournir
   les « Informations d'installation » prévues par la section 6 de la LGPL v3.

Ceci est un résumé informatif et non un avis juridique : seul le texte de la
licence fait foi.

## English

This library is licensed under the **GNU Lesser General Public License**,
version 3 or (at your option) any later version. Full texts are in
[`COPYING.LESSER`](COPYING.LESSER) and [`COPYING`](COPYING).

You may use MathAdvanced in **commercial / proprietary applications** without
disclosing your application's source code, provided you comply with the LGPL
v3 for the library itself: link dynamically (reference the NuGet package or
DLL unmodified; do not statically merge it into your executable without
allowing relinking), keep the copyright and license notices and ship a copy of
the license, let users replace the library with a modified version (without
forbidding reverse engineering for debugging), release your changes to
MathAdvanced itself under the LGPL v3, and provide Installation Information
for locked-down devices. This summary is informational, not legal advice.
