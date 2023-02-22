# aws-sam-apigw-limit-apikey

- hoge0220restrictapi
  - API キーで保護されたサンプル API
- hoge0220createkey
  - API キーのリスト・作成などを行う API
  - 作成した API の情報を DynamoDB へ登録する
  - IAM オーソライザーで保護されている
- hoge0220removekey
  - hoge0220createkey で作成した DynamoDB Streams で発生するイベント処理
  - DynamoDB 上の API キー項目が TTL を迎えたらこちらでイベントをフックして hoge0220restrictapi の API キーの実体を削除する
