using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace TideLevelChartLocations
{
    /// <summary>
    /// 気象庁の潮位表掲載地点一覧表をjsonファイルに変換する
    /// </summary>
    class Program
    {
        static async Task Main()
        {
            var uri = "http://www.data.jma.go.jp/kaiyou/db/tide/suisan/station2021.php";
            var fileName = "../../../outputfile/TideLevelChartLocations.json";

            Console.WriteLine("Start App");
            // 処理時間を計測
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            // 1列に1つの拠点の詳細がすべて入っているリストを拠点ごとに分ける
            // 拠点ごとの情報をオブジェクトにする
            var tideLevelChartLocationsSource = await GetTideLevelChartLocationsInfoAsync(uri);
            var locationDitailList = new List<string>();
            string jsonString;

            // Json option
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            var tideLevelChartLocations = new List<TideLevelChartLocations>();

            using (var stream = new MemoryStream())
            {
                foreach (var tideLevelChartLocation in tideLevelChartLocationsSource)
                {
                    foreach (var locationDitail in tideLevelChartLocation.Split("\n"))
                    {
                        locationDitailList.Add(locationDitail);
                    }
                }
                // オブジェクトを生成
                for (int i = 0; i < tideLevelChartLocationsSource.Count; i++)
                {
                    tideLevelChartLocations.Add(new TideLevelChartLocations
                    {
                        Id = locationDitailList[i * 18],
                        LocationSymbol = locationDitailList[i * 18 + 1].Substring(0, 2),
                        LocationName = locationDitailList[i * 18 + 1].Substring(2),
                        Latitude = locationDitailList[i * 18 + 2],
                        Longitude = locationDitailList[i * 18 + 3],
                        MSL = locationDitailList[i * 18 + 4],
                        MSLElevation = locationDitailList[i * 18 + 5],
                        ElevationoftheTideTableReferencePlane = locationDitailList[i * 18 + 6],
                        majorQuarterTide = new MajorQuarterTide
                        {
                            M2Amplitude = locationDitailList[i * 18 + 7],
                            M2SlowRolling = locationDitailList[i * 18 + 8],
                            S2Amplitude = locationDitailList[i * 18 + 9],
                            S2SlowRolling = locationDitailList[i * 18 + 10],
                            K1Amplitude = locationDitailList[i * 18 + 11],
                            K1SlowRolling = locationDitailList[i * 18 + 12],
                            O1Amplitude = locationDitailList[i * 18 + 13],
                            O1SlowRolling = locationDitailList[i * 18 + 14]
                        },
                        SeparationTideList = locationDitailList[i * 18 + 16],
                        Note = locationDitailList[i * 18 + 17] == "\u00A0" ? "" : locationDitailList[i * 18 + 17]
                    });
                }

                jsonString = JsonSerializer.Serialize(tideLevelChartLocations, options);
                // Console.WriteLine(jsonString);
                File.WriteAllText(fileName, jsonString);
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

    class TideLevelChartLocations
    {
        public string Id { get; set; }
        public string LocationSymbol { get; set; }
        public string LocationName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string MSL { get; set; }
        public string MSLElevation { get; set; }
        public string ElevationoftheTideTableReferencePlane { get; set; }
        public MajorQuarterTide majorQuarterTide { get; set; }
        public string SeparationTideList { get; set; }
        public string Note { get; set; }
    }

    class MajorQuarterTide
    {
        public string M2Amplitude { get; set; }
        public string M2SlowRolling { get; set; }
        public string S2Amplitude { get; set; }
        public string S2SlowRolling { get; set; }
        public string K1Amplitude { get; set; }
        public string K1SlowRolling { get; set; }
        public string O1Amplitude { get; set; }
        public string O1SlowRolling { get; set; }
    }
}
