# PoseSynth
PoseSynthはアバタの関節構造や動作方式の実装を効率化するツールキットです．<br>
ご使用の際はReleasesより `PoseSynth.unitypackage` とVRシステム別のパッケージをUnityプロジェクトにインポートしてください．<br>

2024/10/25現在，ディレクトリ構造の改修のためリポジトリの作り直し作業を行っています．
しばらくの間ご不便をおかけしますが，ご理解の程よろしくお願いいたします．<br>
→ 2024/10/30 完了しました．

## Developpers
- Amane Yamaguchi
- Kenta Hashiura

## Preparation
- Unity 2022.3.6f1

以下のパッケージは必須です．
- [Final IK](https://assetstore.unity.com/packages/tools/animation/final-ik-14290?locale=ja-JP&srsltid=AfmBOorUUYiQTAoEkpdjBnK3XMrzI5K-kmmlXV9W8mSsx6QrRtlcSQRm)<br>
  - ヒューマノイドアバタの動作生成のため
- [NetCode for GameObjects](https://unity.com/ja/products/netcode)<br>
  - ネットワーク通信のため

## How to Install
PoseSynthは基本パッケージ `PoseSynth_x_x_x.unitypackage` とVRシステムごとの追加パッケージ `ExtensionFor{VR_System}_x_x_x.unitypackage` に分割されています．<br>
バージョン `x_x_x` は最新版を使用することを推奨します．<br>
バージョンは基本パッケージと追加パッケージで同じものをインポートしてください．<br>
1. 必須パッケージをインポートしたUnityプロジェクトに `PoseSynth_x_x_x.unitypackage` をインポートする
2. VRシステム別の追加パッケージをインポートする
    - Oculus XR Plugin: `ExtensionForOVR_x_x_x.unitypackage`
※他のVRシステムへの追加パッケージは現在開発中です．<br>

## Documents
wikiをご覧ください（現在執筆中）．

ご使用に際しトラブルなどございましたら連絡先 yamaama@cyber.t.u-tokyo.ac.jp までお気軽にご連絡ください．
