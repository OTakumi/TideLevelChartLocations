# TideLevelChartLocations

### Overview
- 気象庁のサイトから潮位表掲載地点一覧表を取得し、Jsonファイルに出力する
- [気象庁潮位表掲載地点一覧表ページ](http://www.data.jma.go.jp/kaiyou/db/tide/suisan/station2021.php)

### 仕様
- 気象庁潮位表掲載地点一覧表ページから、HTMLをパースし、潮位表掲載地点一覧表データを取得する
- URLは固定
- 出力ファイル：Json形式

### 使用ライブラリ
- AngleSharp

### データ項目一覧
- 番号：Id 連番
- 地点記号：LocationSymbol
- 掲載地点名：LocationName
- 緯度(北緯)：Latitude
- 経度(東経)：Longitude
- MSL-潮位表基準面(cm)：MSL
- MSLの標高(cm)：MSLElevation
- 潮位表基準面の標高(cm)：ElevationoftheTideTableReferencePlane
- 主要4分潮：majorQuarterTide<br>
  {
  - M2振幅(cm)：M2Amplitude
  - M2遅角(°)：M2SlowRolling
  - S2振幅(cm)：S2Amplitude
  - S2遅角(°)：S2SlowRolling
  - K1振幅(cm)：K1Amplitude
  - K1遅角(°)：K1SlowRolling
  - O1振幅(cm)：O1Amplitude
  - O1遅角(°)：O1SlowRolling<br>
  }
- 分潮一覧表：SeparationTideList
- 備考：Note
