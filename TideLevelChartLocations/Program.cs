using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace TideLevelChartLocations
{
    /// <summary>
    /// 気象庁の潮位表掲載地点一覧表をjsonファイルに変換する
    /// </summary>
    // ToDo: 潮位表掲載地点一覧表（2021年）http://www.data.jma.go.jp/kaiyou/db/tide/suisan/station2021.php をスクレイピングし、データを取得する
    // ToDo: 取得したデータをリスト化する
    // ToDo: リスト化したデータをjsonファイルに変換する

    class Program
    {
        static async Task Main(string[] args)
        {
            var uri = "http://www.data.jma.go.jp/kaiyou/db/tide/suisan/station2021.php";
            var fileName = "./TideLevelChartLocations.json";

            Console.WriteLine("Start App");
            // 処理時間を計測
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            // 1列に1つの拠点の詳細がすべて入っているリストを拠点ごとに分ける
            // 拠点ごとの情報をオブジェクトにする
            var tideLevelChartLocations = await GetTideLevelChartLocationsInfoAsync(uri);
            var locationDitailList = new List<string>();
            var tideLevelChartLocation1 = new TideLevelChartLocationsProperty();

            // Json option
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };

            foreach (var tideLevelChartLocation in tideLevelChartLocations)
            {
                foreach (var locationDitail in tideLevelChartLocation.Split("\n"))
                {
                    locationDitailList.Add(locationDitail);
                }
                tideLevelChartLocation1.Number = int.Parse(locationDitailList[0]);
                tideLevelChartLocation1.LocationSymbol = locationDitailList[1].Substring(0, 2);
                tideLevelChartLocation1.LocationName = locationDitailList[1].Substring(2, 2);
            }

            // Serialize
            var json = JsonSerializer.Serialize(tideLevelChartLocation1, options);

            // jsonファイルを生成する
            using (FileStream fs = File.Create(fileName))
            {
                await JsonSerializer.SerializeAsync(fs, json);
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}:{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Runtime: " + elapsedTime);
        }

        public static async Task<List<string>> GetTideLevelChartLocationsInfoAsync(string uri)
        {
            var doc = default(IHtmlDocument);

            // サイト全体をパース
            using (var client = new HttpClient())
            using (var stream = await client.GetStreamAsync(new Uri(uri)))
            {
                var parser = new HtmlParser();
                doc = await parser.ParseDocumentAsync(stream);
            }

            // 潮位表掲載地点一覧表を含む要素をリスト化
            var itemList = new List<string>();
            foreach (var items in doc.GetElementsByClassName("mtx"))
            {
                // Console.WriteLine(items.OuterHtml);
                itemList.Add(items.TextContent);
            }

            // 不要な要素を削除
            itemList.RemoveRange(0, 2);
            itemList.RemoveAt(239);

            return itemList;
        }
    }

    class TideLevelChartLocationsProperty
    {
        public int Number { get; set; }
        public string LocationSymbol { get; set; }
        public string LocationName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double MSL { get; set; }
        public double MSLElevation { get; set; }
        public double ElevationoftheTideTableReferencePlane { get; set; }
        public MajorQuarterTide majorQuarterTide { get; set; }
        public string SeparationTideList { get; set; }
        public string Note { get; set; }
    }

    class MajorQuarterTide
    {
        public double M2Amplitude { get; set; }
        public double M2SlowRolling { get; set; }
        public double S2Amplitude { get; set; }
        public double S2SlowRolling { get; set; }
        public double K1Amplitude { get; set; }
        public double K1SlowRolling { get; set; }
        public double O1Amplitude { get; set; }
        public double O1SlowRolling { get; set; }
    }
}
