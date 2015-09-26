# linux-du
linuxのduコマンドで取得したディスクの使用量をwindows上で再現。

` du -m  --max-depth 4 | sort -nr`の様なコマンドを叩く
```
128522  .
77128   ./var
65776   ./var/lib
32790   ./var/lib/pgsql
32575   ./var/lib/mongo
```
と表示される。windows上にテキストファイルとして保存する。exeにテキストファイルをドラッグ＆ドロップする。カレントディレクトリの下にテキストファイルから取得した階層を再現する。適切なソフトでビジュアル化する
