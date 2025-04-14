# PoseSynth
PoseSynthはアバタの関節構造や動作方式の実装を効率化するツールキットです．<br>
ご使用の際はReleasesより `PoseSynth.unitypackage` とVRシステム別のパッケージをUnityプロジェクトにインポートしてください．<br>

2024/10/25現在，ディレクトリ構造の改修のためリポジトリの作り直し作業を行っています．
しばらくの間ご不便をおかけしますが，ご理解の程よろしくお願いいたします．<br>
→ 2024/10/30 完了しました．

## Features
- 融合身体制御

従来手法では，2 人が操作する各エンドエフェクター（手・足など）の目標位置をまず 1 つに統合し，その後にアバター全体へ逆運動学（IK）を適用していました．
PoseSynth では順序を反転させ，各操作者の骨格に個別に IK を解いたあと，得られた 2 つの全身ポーズをリアルタイムで融合します．
これによりトラッキングの数を増やした場合に腕や足などの位置が破綻せず操作することが出来ます。

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

## For Researchers
PoseSynthを利用した研究の成果を公開する場合は，ソフトウェアおよび関連論文の引用をお願いします．<br>
If you publish the results of your research using PoseSynth, please cite the software and related papers.<br>

ご使用に際しトラブルなどございましたら連絡先 yamaama@cyber.t.u-tokyo.ac.jp までお気軽にご連絡ください．
