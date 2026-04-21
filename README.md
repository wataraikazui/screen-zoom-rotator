# Screen Zoom Rotator

Windowsの画面倍率（DPIスケーリング）を **100% → 150% → 200%** でワンクリック切替する、タスクトレイ常駐アプリです。

- **左クリック** → 次の倍率にローテーション
- **右クリック** → メニューから倍率を直接選択／終了

## 動作要件

- Windows 10 / 11
- .NET 6 ランタイム（ビルド時は .NET 6 SDK）

## ビルド方法

### 方法A：Visual Studio（推奨）

1. [Visual Studio Community 2022](https://visualstudio.microsoft.com/ja/vs/community/) をインストール
   - ワークロード：**「.NET デスクトップ開発」** にチェック
2. `ScreenZoomRotator.sln` をダブルクリックで開く
3. 画面上部のメニュー **ビルド → ソリューションのビルド**（Ctrl+Shift+B）
4. 画面上部の **▶ 開始** ボタンで実行

### 方法B：コマンドライン（.NET SDK）

```powershell
cd ScreenZoomRotator
dotnet build -c Release
dotnet run -c Release
```

### 単体exeの作成（配布用）

```powershell
cd ScreenZoomRotator
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

出力先：`ScreenZoomRotator\bin\Release\net6.0-windows\win-x64\publish\ScreenZoomRotator.exe`

このexeファイル1つをデスクトップやスタートアップフォルダに置けば、どこでも使えます。

## 使い方

1. `ScreenZoomRotator.exe` をダブルクリックで起動
2. タスクトレイ（画面右下）に **現在の倍率が書かれた緑の丸いアイコン**が表示される
3. **左クリック** で倍率を順番に切り替え（100 → 150 → 200 → 100...）
4. **右クリック** でメニューが開き、倍率を直接選択したり、終了できる

## スタートアップ登録（自動起動）

1. `Win + R` を押して `shell:startup` と入力 → Enter
2. 開いたフォルダに `ScreenZoomRotator.exe` のショートカットを置く

次回のサインイン時から自動で常駐します。

## 注意事項

- 倍率変更はレジストリ `HKCU\Control Panel\Desktop\LogPixels` を書き換えて反映します。
- **多くの最新アプリは即時反映されます**が、一部の古いアプリやシステムUIはサインアウト／サインインで完全反映されます。
- 管理者権限は不要です（ユーザー単位の設定のため）。

## 実装メモ

| ファイル | 役割 |
|---|---|
| `Program.cs` | エントリポイント・多重起動防止 |
| `TrayAppContext.cs` | タスクトレイUIとクリック処理 |
| `DpiManager.cs` | レジストリ書き換えとDPI変更通知 |
| `NativeMethods.cs` | Win32 API 宣言 |
| `app.manifest` | Per-Monitor DPI 対応の宣言 |

## ライセンス

© 荻野 尚志 / says@o-h.co.jp
