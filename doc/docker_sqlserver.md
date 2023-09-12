# 概要  
コンテナを使用したASP.net MVC coreの場合、LocalDBデータベースファイルを使用したデバッグは対応していないため  
Linux用SQLserverをインストールしたコンテナを利用してデバッグを実行します。  

## windowsの場合（Linuxの場合は割愛します。）  
1.powershellを起動する。  

2.コンテナー イメージをプルします。  
 docker pull mcr.microsoft.com/mssql/server:2017-latest  

3.Docker でコンテナー イメージを実行する。  
 docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=<YourStrong!Passw0rd>' \
   -p 1433:1433 --name sql1 \
   -d mcr.microsoft.com/mssql/server:2017-latest

4.コンテナへアタッチする。（sql1は起動時につけた名前）  
 docker exec -it sql1 "bash"

5.SQLserverへ接続する。  
 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P '<YourNewStrong!Passw0rd>'

6.DB作成（TestDBは接続するDB名にする。。。接続文字列に含める）  
 CREATE DATABASE TestDB;
 Go

7.sqlcmdのセッションを切断する。  
 QUIT

これでデバッグ用のSQLserverの準備完了。  

## 接続文字列について  
基本的にはSQLserverにつなく場合と同じ  
"DefaultConnection": "Server=tcp:172.17.0.2,1433;Initial Catalog=webappsmvc001;Persist Security Info=False;User ID=sa;Password=*******;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Connection Timeout=1;"

### 注意点としては  
1."Encrypt=False"としてください。（Azure上のDBとかの場合は、Encrypt=ture）  
2."tcp:172.17.0.2"のIPアドレスはコンテナの内部アドレスを指定して下さい。  
 上記の手順で構築したコンテナの場合、ifconfigコマンドは使用できないので"hostname -i"等でIPアドレスを取得してください。  
 Dockerのコンテナ同士が名前解決している場合は名前でもよい。
  ※おそらくIPアドレスは172.17.0.Xとなるはず。  
