DirSizeChecker
===============
Last Update 01 Jun 2014  
Copyright &copy; 2014 CeresSoft  
Distributed under the MIT License


概要 (Introduction)
------
Unityを使っているとスプライト画像をまとめる必要が出てきます。
今まではフリーソフトの「ShoeBox」を便利に使わせて頂いてましたが、
ParticleSystemでパーティクルをアニメーションさせようとすると
スプライト画像の並び順が重要になるのでShoeBoxでスプライト画像の並び順を
指定する方法を探したのですが結局分かりませんでした。
仕方なく他ツールを探してみたものの、これまた良い物が見つけられなかったので
作ってしまいました。


環境 (Environment)
------
#### 開発環境  
Visual Studio 2013

#### 確認環境  
Windows 7 / 8.1 + .NET Framework 4



使い方 (Usage)
------
1. 起動した後にパック画像横幅 / パック画像縦幅 / パックファイル名を入力してください。
これはスプライト画像をパックした画像サイズと画像ファイル名となります。
パック画像横幅とパック画像縦幅は2〜40962の間で自由な値を指定できますが、
Unityで使う場合は2の累乗(2 / 4 / 8・・・256 / 512・・・4096)で指定してください。

2. パックするPNG画像をエクスプローラーからリストボックスにDrag&Dropしてください。
ここに登録した順番(=リストの上から下に向かっての順)にパック後画像の
左上から右に向かって順番にパックされます。

3. PACKボタン
設定内容に従ってスプライトをパックします。

4. SAVEボタン
設定内容をファイルに保存します。
ファイルの拡張子は「.spack」になります

5. LOADボタン
SAVEボタンで保存した設定内容を読み込みます。


*パックした画像の使い方はWikiを参照してください。*



関連情報 (Links)
--------
#### 参考ページ
* [ShoeBox]
   (http://renderhjs.net/shoebox/
    "ShoeBox")



改版履歴 (Change Log)
----------
#### Version 1.0.0.0 (01 Jun 2014)
新規作成
