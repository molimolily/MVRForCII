# MVR For CII
UnityでCIIのためのシンプルな多視点レンダリングを行います。  
#### 動作環境 (参考)
- Core i7 9700F
- RTX 2070 Super
- Unity 2021.3.22.f1
  
400視点のレンダリングで最大200fpsほど出ています。
<div align="center">
<img src="https://github.com/molimolily/MVRForCII/assets/65477859/10e2b296-7770-4b89-9529-ea683b02c5f3" width="600">
</div>

## サンプルシーンを動かすまで
### 1. プロジェクトの作成
URPのマクロを利用してるシェーダーがあります(今後修正するかも)。URPでプロジェクトを作成してください。

### 2. Render Pipeline の設定
サンプルシーンを動作させるためにはRender Pipeline を MV Render Pipeline に変更する必要があります。  
#### Render Pipeline Asset の変更
Render Pipeline Asset を MV Render Pipeline Asset に設定  
Project Settings -> Quality -> Rendering -> Render Pipeline Asset
<div align="center">
<img src="https://github.com/molimolily/MVRForCII/assets/65477859/966bd30f-1efb-4fe3-843b-8bccdd4cbd34" width="600">
</div>
  
#### Scriptable Render Pipeline Settings も変更  
Project Settings -> Graphics -> Scriptable Render Pipline Settings  
<div align="center">
<img src="https://github.com/molimolily/MVRForCII/assets/65477859/6351446f-2670-4973-a051-2f00456c8e65" width="600">
</div>

### サンプルシーンの実行
Assets/CII/Demo/Scenes にVirtual(虚像型)とReal(実像型)のシーンがあります。こちらを実行してください。

## 使用方法
### 1. Render Pipeline Asset の設定
![image](https://github.com/molimolily/MVRForCII/assets/65477859/6a630bad-b08a-4ad3-ab66-bb07aa27e7bd)  

- Enable SRP Batcher : SRP Batcher を有効にする
- Draw Skybox : スカイボックスの描画を行う
- Draw Transparent : 半透明オブジェクトの描画を行う

目的に合わせて設定してください。
SRP Batcher を有効にし、スカイボックスと半透明オブジェクトの描画を行わないようにすると最も軽量になります。

### 2. CullingCameraコンポーネントを付けたカメラを配置する
MV Render Pipeline ではCullingCameraコンポーネントを付けたカメラのCullingResultを全てのカメラで共有しています。
そのため、CullingCameraコンポーネントを付けたカメラで描画したい範囲を映すようにしてください。

### 3. Unlitシェーダーのマテリアルを持つオブジェクトをシーン内に配置する
MV Render Pipeline では ShaderTagId が SRPDefaultUnlit にのみ対応しています。
そのため、LightMode が記述されないシェーダーのみ描画が行われます。
基本的にはUnlitシェーダー、またはMVRシェーダーを使用してください。  
なお、MVRシェーダーはSRPBatcher に対応しています。


## License
© Unity Technologies Japan/UCL
