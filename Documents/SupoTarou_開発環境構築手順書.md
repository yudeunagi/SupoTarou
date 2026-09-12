# スポ太郎 開発環境構築手順書

## 1. 概要

本書では、画面色スポイトアプリ「スポ太郎」を開発するための環境構築手順を説明する。

本アプリは以下の環境で開発する。

| 項目 | 採用技術 |
|---|---|
| OS | Windows 10 / 11 |
| IDE | Visual Studio Code |
| プログラミング言語 | C# |
| UIフレームワーク | WPF |
| .NET | .NET 10 LTS |
| ビルド | `dotnet build` |
| 実行 | `dotnet run` |
| デバッグ | Visual Studio Code + C# Dev Kit |
| パッケージ管理 | NuGet |
| バージョン管理 | Git（推奨） |

---

# 2. 開発環境の構成

最終的な開発環境は以下とする。

```text
Windows
│
├─ Visual Studio Code
│    │
│    └─ C# Dev Kit
│
├─ .NET 10 SDK
│
└─ Git（推奨）
     │
     └─ ColorSpotter
```

Visual Studio Codeを使用するため、Visual Studio本体は必須ではない。

---

# 3. .NET 10 SDKのインストール

## 3.1 SDKのインストール

.NETの公式サイトから.NET 10 SDKをインストールする。

対象：

```text
.NET 10 SDK
Windows
x64
```

一般的なIntel/AMD CPUを搭載したWindows PCではx64版を使用する。

公式サイト：

https://dotnet.microsoft.com/download/dotnet/10.0

## 3.2 SDKとRuntimeの違い

開発には「Runtime」ではなく「SDK」をインストールする。

SDKには.NETアプリケーションを開発・ビルドするために必要な機能が含まれている。

SDKをインストールした場合、通常は対応するRuntimeを別途インストールする必要はない。

---

# 4. .NET SDKのインストール確認

PowerShellまたはコマンドプロンプトを起動する。

以下を実行する。

```powershell
dotnet --version
```

例えば、

```text
10.0.xxx
```

のように表示されればインストール成功。

さらに詳細を確認する場合は、

```powershell
dotnet --info
```

を実行する。

---

# 5. Visual Studio Codeのインストール

## 5.1 インストール

Visual Studio Code公式サイトからWindows版をインストールする。

公式サイト：

https://code.visualstudio.com/

基本的にはインストーラーの標準設定で問題ない。

---

# 6. C# Dev Kitのインストール

Visual Studio Codeを起動する。

左側の「Extensions」を開き、

```text
C# Dev Kit
```

を検索する。

Microsoftが提供しているC# Dev Kitをインストールする。

C# Dev Kitにより、Visual Studio Code上で以下の機能を利用できる。

- C#コード補完
- Solution Explorer
- プロジェクト管理
- ビルド
- デバッグ
- テスト
- .NETプロジェクト操作

C# Dev KitはC#拡張機能と連携して動作する。

---

# 7. プロジェクト作成

## 7.1 プロジェクト保存場所

プロジェクトを保存する場所を決定する。

例：

```text
E:\Projects\
```

PowerShellを起動し、

```powershell
cd E:\Projects
```

とする。

## 7.2 WPFプロジェクト作成

以下のコマンドを実行する。

```powershell
dotnet new wpf -n ColorSpotter
```

これにより、WPFプロジェクトが作成される。

作成後は概ね以下のような構成になる。

```text
E:\Projects\
└─ ColorSpotter\
    ├─ App.xaml
    ├─ App.xaml.cs
    ├─ MainWindow.xaml
    ├─ MainWindow.xaml.cs
    ├─ ColorSpotter.csproj
    └─ ...
```

---

# 8. Visual Studio Codeでプロジェクトを開く

作成したプロジェクトへ移動する。

```powershell
cd ColorSpotter
```

その後、

```powershell
code .
```

を実行する。

Visual Studio Codeが起動し、ColorSpotterプロジェクトが開く。

`code`コマンドが使用できない場合は、Visual Studio Codeの「ファイル → フォルダーを開く」から以下のフォルダーを開く。

```text
E:\Projects\ColorSpotter
```

---

# 9. プロジェクト構成

開発初期は以下の構成とする。

```text
ColorSpotter
│
├─ App.xaml
├─ App.xaml.cs
├─ MainWindow.xaml
├─ MainWindow.xaml.cs
│
├─ ColorSpotter.csproj
│
└─ obj/
```

開発が進んだ段階では、仕様書に合わせて以下のように整理する。

```text
ColorSpotter
│
├─ App.xaml
├─ App.xaml.cs
│
├─ MainWindow.xaml
├─ MainWindow.xaml.cs
│
├─ Controls/
│   ├─ HueRingControl.cs
│   └─ SVBoxControl.cs
│
├─ Models/
│   └─ ColorData.cs
│
├─ Services/
│   ├─ ScreenColorSampler.cs
│   └─ ColorConverter.cs
│
├─ Input/
│   └─ GlobalKeyboardHook.cs
│
└─ ColorSpotter.csproj
```

---

# 10. `.csproj`の確認

プロジェクトの、

```text
ColorSpotter.csproj
```

を確認する。

.NET 10を使用する場合、概ね以下のような設定になる。

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UseWPF>true</UseWPF>
  </PropertyGroup>

</Project>
```

特に以下の2項目が重要。

```xml
<TargetFramework>net10.0-windows</TargetFramework>
```

```xml
<UseWPF>true</UseWPF>
```

これにより、Windows向けWPFアプリとしてビルドされる。

---

# 11. 初回ビルド

Visual Studio Codeのターミナルから以下を実行する。

```powershell
dotnet build
```

以下のように表示されれば成功。

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

# 12. 初回起動

以下を実行する。

```powershell
dotnet run
```

WPFのデフォルトウィンドウが表示されれば、基本的な開発環境の構築は完了。

---

# 13. Visual Studio Codeからのデバッグ

C# Dev Kitをインストールすると、Visual Studio Codeからデバッグ実行できる。

Visual Studio Code左側の「Run and Debug」からデバッグを開始する。

コード上にブレークポイントを設定することで、実行中の変数や処理を確認できる。

---

# 14. NuGet

必要に応じてNuGetパッケージを追加する。

ただし、初期段階では不要なパッケージを大量に追加しない。

今回のアプリでは、

- カーソル座標取得
- 画面ピクセル取得
- RGB/HSV変換
- Hリング描画
- SV四角形描画
- グローバルキーボードフック

などを、可能な限り.NETおよびWindows APIを利用して実装する。

外部ライブラリが必要になった場合に、その都度NuGetパッケージを追加する。

---

# 15. Gitの導入

Gitによるバージョン管理を推奨する。

## 15.1 リポジトリ作成

プロジェクトフォルダーで以下を実行する。

```powershell
git init
```

## 15.2 `.gitignore`

.NETプロジェクトでは、ビルドによって生成されるファイルなどをGit管理対象から除外する。

最低限、以下のようなディレクトリを除外する。

```text
bin/
obj/
.vs/
```

Visual Studio Code自体のGit機能や、既存のGitクライアントを利用してもよい。

---

# 16. 開発開始時点の完成状態

環境構築が完了した時点では、以下の状態になっていることを確認する。

```text
Windows
│
├─ Visual Studio Code
│   │
│   └─ C# Dev Kit
│
├─ .NET 10 SDK
│
└─ Git
     │
     └─ ColorSpotter
          │
          ├─ App.xaml
          ├─ App.xaml.cs
          ├─ MainWindow.xaml
          ├─ MainWindow.xaml.cs
          └─ ColorSpotter.csproj
```

以下のコマンドが正常に実行できることを確認する。

### SDK確認

```powershell
dotnet --version
```

### ビルド

```powershell
dotnet build
```

### 起動

```powershell
dotnet run
```

---

# 17. 開発開始後の推奨実装順序

環境構築後は、仕様書に従って以下の順番で実装する。

```text
STEP 1
WPFプロジェクト作成
        ↓
STEP 2
マウスカーソル座標取得
        ↓
STEP 3
画面ピクセルからRGB取得
        ↓
STEP 4
RGB → HSV変換
        ↓
STEP 5
HSV数値表示
        ↓
STEP 6
Hリング描画
        ↓
STEP 7
SV四角形描画
        ↓
STEP 8
H/S/Vマーカー追加
        ↓
STEP 9
リアルタイム更新
        ↓
STEP 10
非アクティブ時の動作確認
        ↓
STEP 11
グローバルSpace検出
        ↓
STEP 12
色固定・解除
        ↓
STEP 13
マルチモニター/DPI検証
```

特に最初は、

```text
カーソル座標
    ↓
画面ピクセル
    ↓
RGB
    ↓
HSV
    ↓
数値表示
```

までを完成させることを最初のマイルストーンとする。

---

# 18. 開発環境に関する注意事項

## 18.1 Visual Studio本体は必須ではない

今回の開発ではVisual Studio Codeを使用するため、Visual Studio本体をインストールする必要はない。

基本構成は、

```text
Visual Studio Code
+
C# Dev Kit
+
.NET 10 SDK
```

とする。

## 18.2 Windows APIを使用する

画面上のピクセル取得やグローバルキーボードフックなど、Windows固有の機能を使用する。

そのため、本アプリはWindows専用アプリケーションとして開発する。

## 18.3 DPI対応を初期段階から考慮する

スポイトアプリでは、

- 100%
- 125%
- 150%
- 200%

などのWindows表示倍率によって、マウス座標と画面ピクセル座標の扱いが変わる可能性がある。

また、複数モニターで異なるDPIが設定されている環境も考慮する。

そのため、**Per-Monitor DPI対応を前提として設計・実装する。**

---

# 19. 開発環境構築チェックリスト

- [ ] Windows環境を準備
- [ ] .NET 10 SDKをインストール
- [ ] `dotnet --version`でSDKを確認
- [ ] Visual Studio Codeをインストール
- [ ] C# Dev Kitをインストール
- [ ] WPFプロジェクトを作成
- [ ] Visual Studio Codeでプロジェクトを開く
- [ ] `ColorSpotter.csproj`を確認
- [ ] `dotnet build`が成功することを確認
- [ ] `dotnet run`でアプリが起動することを確認
- [ ] Gitを設定する（推奨）

---

# 20. 次の開発ステップ

開発環境の構築が完了したら、最初の実装として以下を行う。

1. マウスカーソルの画面座標を取得する。
2. 取得した座標から画面上の1ピクセルのRGB値を取得する。
3. RGB値をHSVへ変換する。
4. WPF画面にHSV値を表示する。
5. マウス移動に合わせてリアルタイムに更新する。

この段階では、まだHリングやSV四角形は実装せず、**画面上の色を正しく取得できることを最優先で確認する。**

その後、Hリング・SV四角形・Spaceキーによる固定機能を順番に実装する。
