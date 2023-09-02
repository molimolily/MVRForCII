# MVR For CII
UnityでCIIのためのシンプルな多視点レンダリングを行います。  
#### 動作環境 (参考)
- Core i7 9700F
- RTX 2070 Super
- Unity 2021.3.22.f1
  
400視点のレンダリングで最大200fpsほど出ています。
<div align="center">
<img src="https://github.com/molimolily/MVRForCII/assets/65477859/729ce801-8a19-4693-8d57-eaeb26c673da" width="600">
</div>

## サンプルシーンを動かすまで

### Render Pipeline の設定
サンプルシーンを動作させるためにはRender Pipeline を MV Render Pipeline に変更する必要があります。  
#### Render Pipeline Asset の変更
Render Pipeline Asset を MV Render Pipeline Asset に設定  
Project Settings -> Quality -> Rendering -> Render Pipeline Asset
<div align="center">
<img src="https://github.com/molimolily/MVRForCII/assets/65477859/bcfdf518-5efa-4a4e-b8df-ed40998ecd68" width="600">
</div>

#### Scriptable Render Pipeline Settings も変更  
Project Settings -> Graphics -> Scriptable Render Pipline Settings  
<div align="center">
<img src="https://github.com/molimolily/MVRForCII/assets/65477859/bba91dbc-bc4e-4645-bdd8-f89ac0ee6c6c" width="600">
</div>


### サンプルシーンの実行
Assets/CII/Demo/Scenes にVirtual(虚像型)とReal(実像型)のシーンがあります。こちらを実行してください。

## 使用方法
### 1. Render Pipeline Asset の設定
![262572055-6a630bad-b08a-4ad3-ab66-bb07aa27e7bd](https://github.com/molimolily/MVRForCII/assets/65477859/56a64b04-8e42-4944-8d20-cf3ebda61d30)

- Enable SRP Batcher : SRP Batcher を有効にする
- Draw Skybox : スカイボックスの描画を行う
- Draw Transparent : 半透明オブジェクトの描画を行う

目的に合わせて設定してください。
SRP Batcher を有効にし、スカイボックスと半透明オブジェクトの描画を行わないようにすると最も軽量になります。

### 2. CullingCameraコンポーネントを付けたカメラを配置する
MV Render Pipeline ではCullingCameraコンポーネントを付けたカメラのCullingResultを全てのカメラで共有しています。
CullingCameraコンポーネントを付けたカメラで描画したい範囲を映すようにしてください。


## License
© Unity Technologies Japan/UCL
