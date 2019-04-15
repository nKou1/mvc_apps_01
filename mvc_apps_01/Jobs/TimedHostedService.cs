using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using mvc_apps_01.Controllers;
using mvc_apps_01.Data;
using mvc_apps_01.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace mvc_apps_01.Jobs
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private IConfiguration _configuration { get; }

        public TimedHostedService(ILogger<TimedHostedService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(86400));

            return Task.CompletedTask;
        }

        private async void DoWorkAsync(object state)
        {
            _logger.LogInformation("バックグラウンド処理：開始");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            using (var context = new ApplicationDbContext(optionsBuilder.Options))
            {
                try
                {
                    // データベースにテーブルがない場合は作成する。※ただし、マイグレーションファイルは使用されない
                    await context.Database.EnsureCreatedAsync();



                    // ランキング取得
                    await GetRankingAsync(context);

                    // 近日公開取得
                    await GetComingSoonAsync(context);

                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    _logger.LogError(e.StackTrace);
                }

            }
            _logger.LogInformation("バックグラウンド処理：終了");
        }

        private async Task GetRankingAsync(ApplicationDbContext context)
        {            
            int ranking = 0;    // ランキングカウンター
            // ランキングを初期化する。
            var tmDbTrendingList = context.TmDbTrendings.ToList();
            foreach (var tmDbTrendingListItem in tmDbTrendingList)
            {
                context.TmDbTrendings.Remove(tmDbTrendingListItem);
                await context.SaveChangesAsync();
            }
            // ランキングを10ページ分取得
            for (int i = 1; i <= 10; i++)
            {
                _logger.LogInformation("バックグラウンド処理：ランキング取得：ページ" + i);
                // ランキングを取得
                StreamReader reader = GetWebAPI(
                    string.Format(
                        "https://api.themoviedb.org/3/discover/movie?api_key={0}&language=ja-JP&sort_by=popularity.desc&include_adult=false&page={1}",
                        _configuration["TMDbAPIkey"],
                        i
                        ));

                var obj_from_json = JObject.Parse(reader.ReadToEnd());

                var search_result = from foo in obj_from_json["results"] select foo;  //全部表示
                foreach (var r in search_result)
                {
                    ranking++;
                    // ランキングに存在しない場合、映画情報を追加
                    await context.TmDbTrendings.AddRangeAsync(
                        new TmDbTrending
                        {
                            Ranking = ranking,
                            MovieId = int.Parse(r["id"].ToString()),
                            Title = r["title"].ToString(),
                            ReleaseDate = DateTime.Parse(r["release_date"].ToString()),
                            Popularity = decimal.Parse(r["popularity"].ToString()),
                            BackdropPath = (r["backdrop_path"] is null || r["backdrop_path"].ToString().Equals("")) ?
                                "" : "https://image.tmdb.org/t/p/w533_and_h300_bestv2" + r["backdrop_path"].ToString(),
                            PosterPath = (r["poster_path"] is null || r["poster_path"].ToString().Equals("")) ?
                                "" : "https://image.tmdb.org/t/p/w533_and_h300_bestv2" + r["poster_path"].ToString(),
                            UpdateDate = DateTime.Now
                        });
                    await context.SaveChangesAsync();
                }

            }
        }
        private async Task GetComingSoonAsync(ApplicationDbContext context)
        {
            // 近日公開を削除する。
            var tmDbComingSoonList = context.TmDbComingSoon.ToList();
            foreach (var tmDbComingSoonListItem in tmDbComingSoonList)
            {
                context.TmDbComingSoon.Remove(tmDbComingSoonListItem);
                await context.SaveChangesAsync();
            }
            // 近日公開を10ページ分取得
            for (int i = 1; i <= 10; i++)
            {
                _logger.LogInformation("バックグラウンド処理：近日公開取得：ページ" + i);
                // 近日公開を取得
                StreamReader reader = GetWebAPI(
                    string.Format(
                        "https://api.themoviedb.org/3/discover/movie?api_key={0}&language=ja-JP&sort_by=release_date.asc&include_adult=false&include_video=false&page={1}&primary_release_date.gte={2}&primary_release_date.lte={3}",
                        _configuration["TMDbAPIkey"],
                        i,
                        DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                        DateTime.Parse(DateTime.Now.AddMonths(1).ToString("yyyy-MM-01")).AddDays(-1).ToString("yyyy-MM-dd")
                        ));

                var obj_from_json = JObject.Parse(reader.ReadToEnd());

                var search_result = from foo in obj_from_json["results"] select foo;  //全部表示

                foreach (var r in search_result)
                {
                    // 映画情報を追加
                    await context.TmDbComingSoon.AddRangeAsync(
                        new TmDbComingSoon
                        {
                            MovieId = int.Parse(r["id"].ToString()),
                            Title = r["title"].ToString(),
                            ReleaseDate = DateTime.Parse(r["release_date"].ToString()),
                            Popularity = decimal.Parse(r["popularity"].ToString()),
                            BackdropPath = (r["backdrop_path"] is null || r["backdrop_path"].ToString().Equals("")) ? 
                                "" : "https://image.tmdb.org/t/p/w533_and_h300_bestv2" + r["backdrop_path"].ToString(),
                            PosterPath = (r["poster_path"] is null || r["poster_path"].ToString().Equals("")) ? 
                                "" : "https://image.tmdb.org/t/p/w533_and_h300_bestv2" + r["poster_path"].ToString(),
                            UpdateDate = DateTime.Now
                        });
                    await context.SaveChangesAsync();
                }
            }
        }

        private StreamReader GetWebAPI(string url)
        {
            WebRequest request = WebRequest.Create(url);
            Stream response_stream = request.GetResponse().GetResponseStream();
            StreamReader reader = new StreamReader(response_stream);

            return reader;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
