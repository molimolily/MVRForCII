# MVR For CII
UnityでCIIのための多視点レンダリングを行います。  

## サンプルシーンを動かすまで
Render Pipeline Asset の変更
Render Pipeline Asset を MV Render Pipeline Asset に設定  
Project Settings -> Quality -> Rendering -> Render Pipeline Asset
<div align="center">
<img src="https://github.com/molimolily/MVRForCII/assets/65477859/966bd30f-1efb-4fe3-843b-8bccdd4cbd34" width="600">
</div>
  
Scriptable Render Pipeline Settings も変更  
Project Settings -> Graphics -> Scriptable Render Pipline Settings  
<div align="center">
<img src="https://github.com/molimolily/MVRForCII/assets/65477859/6351446f-2670-4973-a051-2f00456c8e65" width="600">
</div>

SampleScene(Assets/CII/Demo/Scene) が実行できるようになる  
<div align="center">
<img src="https://github.com/molimolily/MVRForCII/assets/65477859/b95132c4-b1f4-46d1-a35b-1d0da9590571" width="600">
</div>

## 使用方法
1. Unlitシェーダーのマテリアルを持つオブジェクトをシーン内に配置する
2. CullingCameraコンポーネントを付けたカメラをオブジェクト全体が映るようにシーン内に配置する
